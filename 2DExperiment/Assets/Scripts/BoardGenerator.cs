using UnityEngine;
using System.Collections;

public class BoardGenerator : MonoBehaviour {

	public GameObject blackTile;
	public GameObject whiteTile;
	
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
	
	GameObject newTile;
	GameObject[] backgroudTiles;
	
	// Update is called once per frame
	public void setUpBoard () {
		cleanUpOldTiles();
		backgroudTiles = new GameObject[boardWidth * boardHeight];
		for (int i=0; i< boardWidth; i++){
			for (int j=0; j< boardHeight; j++)
			{
				if( (i+j) % 2 == 0)
				{
					newTile = Instantiate(whiteTile, new Vector3(i,j,0f), Quaternion.identity) as GameObject;
				}
				else
				{
					newTile = Instantiate(blackTile, new Vector3(i,j,0f), Quaternion.identity) as GameObject;
				}
				newTile.transform.parent = transform;
				backgroudTiles[i * boardHeight + j] = newTile;
			}
			
		}
	}
	public void cleanUpOldTiles()
	{
		if(backgroudTiles != null)
		{
			
			foreach (GameObject tile in backgroudTiles)
			{
				Destroy(tile);
			}
			backgroudTiles = null;
		}
	}
}
