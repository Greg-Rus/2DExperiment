using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PipeSegmentManager : MonoBehaviour {

	public int x;
	public int y;
	
	public GameObject HV_Arrow;
	public GameObject X_Arrow;
	private Dictionary<PipeDirection, GameObject> pipes;
	private Transform myTransform;
	// Use this for initialization
	public void initializePipeSegmentManager () {
		myTransform = transform;
		Debug.Log(GameControll.instance.prefabManager.HV_Arrow);
		
		HV_Arrow = GameControll.instance.prefabManager.HV_Arrow;
		X_Arrow = GameControll.instance.prefabManager.X_Arrow;
		
		pipes = new Dictionary<PipeDirection, GameObject>();
	}
	
	// Update is called once per frame
	void addPipe(PipeDirection direction)
	{
		switch(direction)
		{
			case PipeDirection.Right: pipes.Add(direction,placeNewPipeObject(HV_Arrow, 0));	break;
			case PipeDirection.Down: pipes.Add(direction,placeNewPipeObject(HV_Arrow, 90));	break;
			case PipeDirection.Left: pipes.Add(direction,placeNewPipeObject(HV_Arrow, 180)); break;
			case PipeDirection.Up: pipes.Add(direction,placeNewPipeObject(HV_Arrow, 270)); break;
			case PipeDirection.RightUp: pipes.Add(direction,placeNewPipeObject(X_Arrow, 0)); break;
			case PipeDirection.RightDown: pipes.Add(direction,placeNewPipeObject(X_Arrow, 90)); break;
			case PipeDirection.LeftDown: pipes.Add(direction,placeNewPipeObject(X_Arrow, 180)); break;
			case PipeDirection.LeftUp: pipes.Add(direction,placeNewPipeObject(X_Arrow, 270)); break;	
		}
	}
	
	GameObject placeNewPipeObject(GameObject pipeType, int rotation)
	{
		GameObject newPipe = Instantiate(pipeType, myTransform.position, Quaternion.identity) as GameObject;
		newPipe.transform.Rotate( new Vector3(0,0,rotation));
		newPipe.transform.parent = myTransform;
		return newPipe;
		
	}
	public void spawnPipes(PipeDirection[] directions)
	{
		foreach(PipeDirection dir in directions)
		{
			addPipe(dir);
		}
	}
}
