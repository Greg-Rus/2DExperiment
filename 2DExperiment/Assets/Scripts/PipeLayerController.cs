using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PipeDirection {Right, Down, Left, Up, RightUp, RightDown, LeftDown, LeftUp}

public class PipeLayerController : MonoBehaviour {

	private static PipeDirection[] possibleDirections = new PipeDirection[]{PipeDirection.Right, PipeDirection.Down, PipeDirection.Left, PipeDirection.Up, PipeDirection.RightUp, PipeDirection.RightDown, PipeDirection.LeftDown, PipeDirection.LeftUp};
	private static int[] pipeProbabilityWeights = new int[]{5, //0
														1,//1
														1,//2
														1,//3
														1,//4
														1,//5
														1,//6
														1};//7
	private int[] pipeProbabilities;
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
	
	public GameObject pipeSegment;
	private PipeManager[,] pipeSegments;
	private Transform myTransform;
	public PrefabManager myPrefabs;
	
	
	// Use this for initialization
	void Start()
	{
		calculatePipeProbabilities();
		myTransform = transform;
	}
	public void setUpPipeSegments()
	{
		
		initialPipeLayerSetup();
	}
	void initialPipeLayerSetup()
	{
		pipeSegments = new PipeManager[boardWidth,boardHeight];
		
		for (int i = 0; i< boardWidth; i++)
		{
			for (int j = 0; j < boardHeight; j++)
			{
				PipeManager newPipeSegment = spawnRandomPipeSegment(i,j);
				newPipeSegment.x = i;
				newPipeSegment.y = j;
				pipeSegments[i,j] = newPipeSegment;
			}
		} 	
	}
	void calculatePipeProbabilities()
	{
		int sum =0;
		foreach (int i in pipeProbabilityWeights)
		{
			sum += i;
		}
		pipeProbabilities = new int[pipeProbabilityWeights.Length ];
		int lastProbability =0;
		for(int i =0; i<pipeProbabilityWeights.Length -1; i++)
		{
			pipeProbabilities[i] = Mathf.RoundToInt ((pipeProbabilityWeights[i] *100) / (float)sum) + lastProbability;
			lastProbability = pipeProbabilities[i];
		}
		pipeProbabilities[pipeProbabilityWeights.Length -1] = 100;
		
	}
	int selectRandomPipeTileIndex()
	{
		int randomNumber = Random.Range(0,100);
		int lastProbabilty =0;
		int pipeIndex = 0;
		for (int i=0; i< pipeProbabilities.Length; i++)
		{
			if (randomNumber > lastProbabilty && randomNumber < pipeProbabilities[i])
			{
				pipeIndex = i;
				break;
			}
			lastProbabilty = pipeProbabilities[i];
		}
		return pipeIndex;
	}
	
	PipeManager spawnRandomPipeSegment(int x,int y)
	{
		int randomPipeTileIndex = selectRandomPipeTileIndex();
		GameObject newTile = Instantiate(myPrefabs.pipeSegment, new Vector3(x,y,-1), Quaternion.identity) as GameObject;
		PipeManager newPipeManager = newTile.GetComponent<PipeManager>();
		newPipeManager.x = x;
		newPipeManager.y = y;
		newPipeManager.pipeTile = instantiatePipeTile(randomPipeTileIndex, x, y);


		newPipeManager.pipeTile.transform.parent = newTile.transform;
		newTile.transform.parent = transform;

		return newPipeManager;
	}
	GameObject instantiatePipeTile(int pipeTileIndex, int x, int y)
	{
		GameObject newTile = Instantiate(myPrefabs.pipeTiles[pipeTileIndex], new Vector3(x,y,-1), Quaternion.identity) as GameObject;
		return newTile;
	}
	
	public void cleanUpOldPipes()
	{
		if (pipeSegments != null)
		{
			;
		}
	}
	
}


//Legacy. Old idea was to spawn a random number of arrow pointing in one of 8 directions. This made the tile look very cluttered.
//Code uses two lists to draw random directions for arrows. Firs list contains all directions. A number is selected at random from range 0 to list max index.
//The element at this index is removed and placed in a different list. If necessary the operation is repeated on the now reduced list of directions.
//This way random directions are drawn from a shirinking list without duplicates.
/*	
	public void setUpPipeSegments()
	{
		initialPipeLayerSetup();
	}
	
	void initialPipeLayerSetup()
	{
		pipeSegments = new PipeSegmentManager[boardWidth,boardHeight];
		
		for (int i = 0; i< boardWidth; i++)
		{
			for (int j = 0; j < boardHeight; j++)
			{
				PipeSegmentManager newPipeSegment = spawnRandomPipeSegment(i,j);
				newPipeSegment.x = i;
				newPipeSegment.y = j;
				pipeSegments[i,j] = newPipeSegment;
			}
		} 	
	}
	PipeSegmentManager spawnRandomPipeSegment(int x,int y)
	{
		GameObject newPipeSegment = Instantiate(pipeSegment, new Vector3(x,y, myTransform.position.z), Quaternion.identity) as GameObject;
		newPipeSegment.transform.parent = myTransform;
		PipeSegmentManager newPipeSegmentManager = newPipeSegment.GetComponent<PipeSegmentManager>() as PipeSegmentManager;
		Debug.Log(GameControll.instance.prefabManager.HV_Arrow);
		newPipeSegmentManager.initializePipeSegmentManager ();
		
		//RadomizeSegmentConfig
		int numberOfPipes = Random.Range(0,8);
		if(numberOfPipes > 0)
		{
			List<PipeDirection> availableDirections = new List<PipeDirection>{PipeDirection.Right, PipeDirection.Down, PipeDirection.Left, PipeDirection.Up, PipeDirection.RightUp, PipeDirection.RightDown, PipeDirection.LeftDown, PipeDirection.LeftUp};
			Debug.Log (availableDirections.Count);
			int availableDirectionMaxIndex = 7;
			PipeDirection[] directions = new PipeDirection[numberOfPipes];
			
			for (int i=0; i< numberOfPipes; i++)
			{
				int directionIndex = Random.Range(0,availableDirectionMaxIndex);
				directions[i] = availableDirections[directionIndex];
				availableDirections.RemoveAt(directionIndex);
				availableDirectionMaxIndex--;
			}
			newPipeSegmentManager.spawnPipes(directions);
		}
	
		return newPipeSegmentManager;
	}
	
	public void cleanUpOldPipes()
	{
		if (pipeSegments != null)
		{
			
		}
	}
	
	
	
}
*/