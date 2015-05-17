using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TileController : MonoBehaviour {
	public static TileController instance;
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
	private Transform myTransform;
	private TileBehaviour[,] boardTiles;
	private List<TileBehaviour> link;
	private GameObject newTile;
	private TileBehaviour newTileBehaviour;
	public int numberOfPossibleTiles = 5;
	private TileBehaviour currentSelection;
	private TileBehaviour lastSelection;
	private TileType linkType;
	private LineRenderer myLineRenderer;


	void Awake(){
		instance = this;
		myTransform = transform;
		myLineRenderer = GetComponent<LineRenderer>();
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))	handleSelectionStart();
		if(Input.GetMouseButton(0)) handleSelectionOngoing();
		if(Input.GetMouseButtonUp(0)) handleSelectionEnd();
		
	}
	//Game start and end section
	public void StartNewGame()
	{
		//cleanUpOldGame(); TODO
		boardTiles = new TileBehaviour[boardWidth,boardHeight];
		link = new List<TileBehaviour>();	
		DeployInitialTileSetup();
	}
	private void cleanUpOldGame()
	{
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
	
	void DeployInitialTileSetup()
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
	
	//=== Input handling section ====================================
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
		   currentSelection.tileType == linkType && //Fater to check the tile type first
		   proximityCheck()) //Do proximity calculations last to only when previous checks succeeded
		{
			if (!link.Contains(currentSelection))
			{
				link.Add (currentSelection);
				updateLinkLines();
				currentSelection.myInputHandler.isSelected();
			}
			else
			{
				if(link.Count>=2 && currentSelection == link[link.Count-2]) //Deselect a tile by removing it from the link
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
		if (link.Count <=2) //If link is too short ignore it
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
			//TODO need to think about score multiplyers.
			GameControll.instance.updateScore(link.Count);
			//collectLinkedTiles(); TODO
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
	
	void updateLinkLines()
	{
		myLineRenderer.SetVertexCount(link.Count);
		//Vector3[] points = new Vector3[link.Count-1];
		for(int i =0; i< link.Count; i++)
		{
			myLineRenderer.SetPosition(i, new Vector3(link[i].x, link[i].y, -2f));
		}
	}
	
	//===Tile handling section======================================
	
	TileBehaviour spawnRandomTile(int i, int j) //spawns at specified coordinates
	{
		GameObject newTileType = tiles[Random.Range(0,numberOfPossibleTiles)];
		newTile = Instantiate(newTileType, new Vector3(i,j,myTransform.position.z), Quaternion.identity) as GameObject;
		newTile.transform.parent = myTransform;
		newTileBehaviour = newTile.GetComponent<TileBehaviour>() as TileBehaviour;
		return newTileBehaviour;
	}
	TileBehaviour spawnRandomTile(int i) //spawsn at top of column i
	{
		GameObject newTileType = tiles[Random.Range(0,numberOfPossibleTiles)];
		newTile = Instantiate(newTileType, new Vector3(i,boardHeight,myTransform.position.z), Quaternion.identity) as GameObject;
		newTile.transform.parent = myTransform;
		newTileBehaviour = newTile.GetComponent<TileBehaviour>() as TileBehaviour;
		return newTileBehaviour;
	}
}
