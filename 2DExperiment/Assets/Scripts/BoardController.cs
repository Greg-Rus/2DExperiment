using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BoardController : MonoBehaviour {
	public static BoardController instance;
	public GameObject[] tiles;
	private int boardHeight;
	public int BoardHeight{
		get{return boardHeight;}
		set{boardHeight = value;}
	}
	private int boardWidth;
	public int BoardWidth{
		get{return boardWidth;}
		set{boardWidth = value;}
	}
	public Text scoreUI;
	public int limiter =0;
	
	private TileBehaviour[,] boardTiles;
	private GameObject newTile;
	private TileBehaviour newTileBehaviour;
	private List<TileBehaviour> link;
	private bool clicking;
	private TileBehaviour currentSelection;
	private TileBehaviour lastSelection;
	private TileType linkType;
	private float score;
	private bool selectionStart;
	private bool selectionOngoing;
	private bool selectionEnd;
	private LineRenderer myLineRenderer;
	private Transform myTransform;
	public float defaultTileMoveTimeInSec = 0.1f;
	
	
	void Awake(){
		instance = this;
		myLineRenderer = GetComponent<LineRenderer>();
		myTransform = transform;
	}
	
	public void startNewGame () {
		score = 0;
		cleanUpOldGame();
		boardTiles = new TileBehaviour[boardWidth,boardHeight];
		link = new List<TileBehaviour>();	
		initialSetup();
	}
	
	private void cleanUpOldGame()
	{
		scoreUI.text = score.ToString();
		if(link != null)
		{
			link.Clear();
		}
		if (boardTiles != null)
		{
			foreach( TileBehaviour tile in boardTiles)
			{
				Destroy(tile.transform.gameObject);
			}
			boardTiles = null;
		}
		
	}
	
	
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))	handleSelectionStart();
		
	
		if(Input.GetMouseButton(0)) handleSelectionOngoing();
			
		
		if(Input.GetMouseButtonUp(0)) handleSelectionEnd();


	}
	
	public void selectedTile(TileBehaviour tile)
	{
		currentSelection = tile;
	}
	public void unselectedTile(TileBehaviour tile)
	{
		currentSelection = null;
	}
	
	private void handleSelectionStart()
	{
//		Debug.Log (currentSelection);
		if(currentSelection != null)
		{
			link.Add (currentSelection);
			currentSelection.myInputHandler.isSelected();
			linkType = currentSelection.tileType;
		}
	}
	private void handleSelectionOngoing()
	{
		if((currentSelection != null) &&
		   proximityCheck()&&
		   currentSelection.tileType == linkType &&
		   proximityCheck())
		{
			if (!link.Contains(currentSelection))
			{
				link.Add (currentSelection);
				updateLinkLines();
				currentSelection.myInputHandler.isSelected();
			}
			else
			{
				if(link.Count>=2 && currentSelection == link[link.Count-2])
				{
					link[link.Count-1].myInputHandler.isDeselected();
					link.RemoveAt(link.Count-1);
					updateLinkLines();
				}
			}
		}
	}
	private void handleSelectionEnd()
	{
		if (link.Count <=2)
		{
			foreach (TileBehaviour tile in link)
			{
				tile.myInputHandler.isDeselected();
			}
			link.Clear();
			updateLinkLines();
		}
		else 
		{
	
			//need to think about score multiplyers.
			score += link.Count;
			scoreUI.text = score.ToString();
//			foreach (TileBehaviour tile in link)
//			{
//				boardTiles[tile.x, tile.y] = null;
//				Destroy (tile.gameObject);		
//			}
			collectLinkedTiles();
//			link.Clear();
//			updateLinkLines();
		}
//		dropTiles();
	}
	private void collectLinkedTiles()
	{
		for (int i =0; i< link.Count; i++)
		{
			link[i].linkPosition = i;
		}
		for (int linkIndex = 0; linkIndex < link.Count -1; linkIndex++)
		{
			//Debug.Log ("Start set for linkIndex: " + linkIndex);
			StartCoroutine(followLinkToLastTile(linkIndex));
		}
		
		//StartCoroutine(followLinkToLastTile(0));

	}
	private IEnumerator followLinkToLastTile(int tileIndexInLink)
	{
		//It will take the same amount of time for all the tile in the linkt to be collected. 
		//Maybe this should be a fixed speed rather, so that the player can enjoy a long tile slide?
		float moveSpeed = defaultTileMoveTimeInSec;// / (link.Count - (tileIndexInLink +1) ); //<-Proportionate move speed. All tiles will arrive at destination in defaultTileMoveTimeInSec;
		
		//The tile for which the coroutine is called needs to follow a path allong the position of tiles further in the link,
		// to reach the position of the last tile in the link.
		for (int i = tileIndexInLink; i< link.Count -1 ; i++)  //loop over tiles further in the link starting from the selected tile.
		{
			//yield return StartCoroutine(link[tileIndexInLink].cMoveTile(link[i+1].x, link[i+1].y, moveSpeed, false, "link move for link tile at: " + tileIndexInLink+ " Iteration: " + i)); //move to the next tile position
			yield return link[tileIndexInLink].moveTo(link[i+1].x, link[i+1].y, moveSpeed);
			//The tile at index 0 in the link is the last tile in the link. This tile finishes the slide effect.
			//While there are tile mooving along the link the tiles above should not fall. Only after the last tile is moved should they drop.
			if(tileIndexInLink == 0)
			{
				boardTiles[link[i].x, link[i].y] = null; //remove reference from vacated tile spot.

				if(checkLinkForTileAbove(i) == false) //the tiles above should not drop it there are tiles in the link still holding them up.
				{
					dropTilesInColumn(link[i].x);
				}
			}
		}
		if(tileIndexInLink == 0)
		{
			boardTiles[link[link.Count-1].x, link[link.Count-1].y] = null;
			foreach(TileBehaviour tile in link)
			{
				tile.selfDestroy();
			}
			dropTilesInColumn(link[link.Count-1].x);
		
			spawnTilePaticleGenerator();
			link.Clear();
			updateLinkLines();
		}
	}
	
	private void spawnTilePaticleGenerator()
	{
		ParticleSystem particleSystem = Instantiate(GameControll.instance.prefabManager.tileParticleSystems[(int)link[link.Count-1].tileType],
		                                        link[link.Count-1].transform.position,
		                                            Quaternion.identity) as ParticleSystem;
                             
		particleSystem.maxParticles = link.Count;
		particleSystem.Play();
		Destroy(particleSystem.gameObject,2f);                                       
	}
	
	private bool checkLinkForTileAbove(int linkTileIndex)
		//Necessary to prevent calling dropTilesInColumn if the link contains a tile in the same column but higher. This tile should block other tiles from falling.
	{
		bool tileAboveInLink = false;
		for (int j = linkTileIndex+1; j<link.Count ; j++) //start with the next tile in the link
		{
			if(link[j].x == link[linkTileIndex].x && link[j].y > link[linkTileIndex].y) //check if any consecutive tile in link is in the same column but higher.
			{
				tileAboveInLink = true;
			}
		}
		return tileAboveInLink;
	}
	
	void initialSetup()
	{
		for (int i = 0; i< boardWidth; i++)
		{
			for (int j = 0; j < boardHeight; j++)
			{
				TileBehaviour newTile = spawnRandomTile(i,j);
				newTileBehaviour.x = i;
				newTileBehaviour.y = j;
				boardTiles[i,j] = newTileBehaviour;
			}
		} 	
	}
	bool proximityCheck(){
		TileBehaviour lastTileInLink = link[link.Count-1];
		int deltaX = lastTileInLink.x - currentSelection.x;
		deltaX *= deltaX;
		int deltaY = lastTileInLink.y - currentSelection.y;
		deltaY *= deltaY;
		if(deltaX <=1 && deltaY <=1) return true;
		else return false;
	}
	
	//Unused??
	bool hasRoomToFall(int i, int j){
		return boardTiles[i,j-1] == null ? true : false;
	}
	
	TileBehaviour spawnRandomTile(int i, int j)
	{
		GameObject newTileType = tiles[Random.Range(0,tiles.Length - limiter)];
		newTile = Instantiate(newTileType, new Vector3(i,j,myTransform.position.z), Quaternion.identity) as GameObject;
		newTile.transform.parent = myTransform;
		newTileBehaviour = newTile.GetComponent<TileBehaviour>() as TileBehaviour;
		
		return newTileBehaviour;
	}
	TileBehaviour spawnRandomTileAtTopOfColumn(int i)
	{
		GameObject newTileType = tiles[Random.Range(0,tiles.Length-limiter)];
		newTile = Instantiate(newTileType, new Vector3(i,boardHeight,myTransform.position.z), Quaternion.identity) as GameObject;
		newTile.transform.parent = myTransform;
		newTileBehaviour = newTile.GetComponent<TileBehaviour>() as TileBehaviour;
		return newTileBehaviour;
	}
	
	//New - scan columns and make tiles drop an appropreate distance. Spawn new tiles at once.
	//To be depricated because of new link slide effect.
	void dropTiles()
	{
		for (int i = 0; i < boardWidth; i++)
		{
			TileBehaviour[] droppedTiles = new TileBehaviour[boardHeight];
			int droppedTilesIndex =0;
			for (int j = 0; j < boardHeight; j++)
			{
				if(boardTiles[i,j] != null)
				{
					droppedTiles[droppedTilesIndex] = boardTiles[i,j];
					if (droppedTiles[droppedTilesIndex].y != droppedTilesIndex){
						droppedTiles[droppedTilesIndex].fallToRow(droppedTilesIndex);
						droppedTiles[droppedTilesIndex].y = droppedTilesIndex;
					}
					droppedTilesIndex++;
				}
			}
			for (int k =0; k< boardHeight; k++)
			{
				if(droppedTiles[k] != null)
				{
					boardTiles[i,k] = droppedTiles[k];
				}
				else
				{
					boardTiles[i,k] = spawnRandomTileAtTopOfColumn(i);
					
					boardTiles[i,k].fallToRow(k);
					boardTiles[i,k].x = i;
					boardTiles[i,k].y = k;
				}
			}
		}
	}
	void dropTilesInColumn(int column)
	{
		TileBehaviour[] droppedTiles = new TileBehaviour[boardHeight];
		int droppedTilesIndex =0;
		for (int j = 0; j < boardHeight; j++)
		{
			if(boardTiles[column,j] != null)
			{
				droppedTiles[droppedTilesIndex] = boardTiles[column,j];
				if (droppedTiles[droppedTilesIndex].y != droppedTilesIndex){
					droppedTiles[droppedTilesIndex].fallToRow(droppedTilesIndex);
					droppedTiles[droppedTilesIndex].y = droppedTilesIndex;
				}
				droppedTilesIndex++;
			}
		}
		for (int k =0; k< boardHeight; k++)
		{
			if(droppedTiles[k] != null)
			{
				boardTiles[column,k] = droppedTiles[k];
			}
			else
			{
				boardTiles[column,k] = spawnRandomTileAtTopOfColumn(column);
				
				boardTiles[column,k].fallToRow(k);
				boardTiles[column,k].x = column;
				boardTiles[column,k].y = k;
			}
		}
	}
	void updateLinkLines()
	{
		myLineRenderer.SetVertexCount(link.Count);
		//Vector3[] points = new Vector3[link.Count-1];
		for(int i =0; i< link.Count; i++)
		{
			myLineRenderer.SetPosition(i, new Vector3(link[i].x, link[i].y, -2f));
		}
	}
}
