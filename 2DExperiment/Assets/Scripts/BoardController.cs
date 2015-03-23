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
	
	
	void Awake(){
		instance = this;
		myLineRenderer = GetComponent<LineRenderer>();
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
			currentSelection.isSelected();
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
				currentSelection.isSelected();
			}
			else
			{
				if(link.Count>=2 && currentSelection == link[link.Count-2])
				{
					link[link.Count-1].isDeselected();
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
				tile.isDeselected();
			}
			link.Clear();
			updateLinkLines();
		}
		else 
		{
	
			//need to think about score multiplyers.
			score += link.Count;
			scoreUI.text = score.ToString();
			foreach (TileBehaviour tile in link)
			{
				boardTiles[tile.x, tile.y] = null;
				Destroy (tile.gameObject);		
			}
			link.Clear();
			updateLinkLines();
		}
		dropTiles();
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
	
	bool hasRoomToFall(int i, int j){
		return boardTiles[i,j-1] == null ? true : false;
	}
	
	TileBehaviour spawnRandomTile(int i, int j)
	{
		GameObject newTileType = tiles[Random.Range(0,tiles.Length)];
		newTile = Instantiate(newTileType, new Vector3(i,j,-1f), Quaternion.identity) as GameObject;
		newTile.transform.parent = transform;
		newTileBehaviour = newTile.GetComponent<TileBehaviour>() as TileBehaviour;
		return newTileBehaviour;
	}
	TileBehaviour spawnRandomTileAtTopOfColumn(int i)
	{
		GameObject newTileType = tiles[Random.Range(0,tiles.Length)];
		newTile = Instantiate(newTileType, new Vector3(i,boardHeight,-1f), Quaternion.identity) as GameObject;
		newTile.transform.parent = transform;
		newTileBehaviour = newTile.GetComponent<TileBehaviour>() as TileBehaviour;
		return newTileBehaviour;
	}
	
	//New - scan columns and make tiles drop an appropreate distance. Spawn new tiles at once.
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
						droppedTiles[droppedTilesIndex].fallDistance(droppedTiles[droppedTilesIndex].y - droppedTilesIndex);
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
					
					boardTiles[i,k].fallDistance(boardHeight - k);
					boardTiles[i,k].x = i;
					boardTiles[i,k].y = k;
				}
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
