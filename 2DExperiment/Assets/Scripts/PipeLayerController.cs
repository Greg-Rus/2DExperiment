using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PipeDirection {Right, Down, Left, Up, RightUp, RightDown, LeftDown, LeftUp}

public class PipeLayerController : MonoBehaviour {

	private static PipeDirection[] possibleDirections = new PipeDirection[]{PipeDirection.Right, PipeDirection.Down, PipeDirection.Left, PipeDirection.Up, PipeDirection.RightUp, PipeDirection.RightDown, PipeDirection.LeftDown, PipeDirection.LeftUp};
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
	private PipeSegmentManager[,] pipeSegments;
	private Transform myTransform;
	
	
	// Use this for initialization
	void Start()
	{
		myTransform = transform;
	}
	
	
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
