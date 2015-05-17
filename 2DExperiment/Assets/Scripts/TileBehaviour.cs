using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//public enum TileType {Green, Blue, Yellow, Red, Purple}
public enum TileType {Finance, Science, Industry, Military, Social}
public enum State {Idle, Moving, Bouncing}

public class TileBehaviour : MonoBehaviour{

	public int x;
	public int y;
	public TileType tileType;
	public bool moving;
	public int linkPosition = -1;
	public string destroyerMessage;
	public TileInputHandler myInputHandler;
	public State currentState = State.Idle;
	
	//
	public float moveSpeed;
	
	
	//
	public float selectedScale = 0.6f;
	private const float defaultFallTimeInSec = 0.4f;
	private float bounceHeight = 0.1f;
	private float bounceTime = 0.1f;
	private Transform myTransform;
	private IEnumerator tileMovement;
	private Coroutine tileMovementCoroutine;
	
	// Use this for initialization
	void Awake () {
		myTransform = transform;
	}
	
	
	public void fallToRow(int distance)
	{
		if(tileMovement == null)
		{
			tileMovement = cMoveTile(myTransform.position.x, distance, defaultFallTimeInSec, true);
			StartCoroutine(tileMovement);
		}
		else
		{
			StopCoroutine(tileMovement);
			tileMovement = cMoveTile(myTransform.position.x, distance, defaultFallTimeInSec, true);
			StartCoroutine(tileMovement);
		}
		
		//StartCoroutine(cMoveTile(myTransform.position.x, distance, defaultFallTimeInSec, true));
		
	}
	
	public void moveTileToPosition(int x, int y, float time = defaultFallTimeInSec)
	{
		
	}
	
	private void updateTileMovement()
	{
		
	}
	
	public Coroutine moveTo(int x, int y, float time = defaultFallTimeInSec)
	{
		if(tileMovement == null)
		{
			tileMovement = cMoveTile(x, y, time);
			tileMovementCoroutine = StartCoroutine(tileMovement);
		}
		else
		{
			StopCoroutine(tileMovement);
			tileMovement = cMoveTile(x, y, time);
			tileMovementCoroutine = StartCoroutine(tileMovement);
		}
		return tileMovementCoroutine;

	}
	
	public IEnumerator cMoveTile(float x, float y, float time, bool shouldBounce = false, string context = null)
	{
		moving = true;
		float elapsedTime = 0;
		Vector3 currentPosition = myTransform.position;
		string executionContext = context; 

		while (elapsedTime < time)
		{
			myTransform.position = new Vector3(Mathf.Lerp(currentPosition.x, x, (elapsedTime/time)), //(elapsedTime/time)
			                                   Mathf.Lerp(currentPosition.y, y, (elapsedTime/time)), //(elapsedTime/time)
			                                   currentPosition.z);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
			
		}
		myTransform.position = new Vector3(x, y, myTransform.position.z);
		moving = false;
		tileMovement=null;
		
		if (shouldBounce)
		{
			StartCoroutine(bounce(myTransform.position, myTransform.position.y + bounceHeight, bounceTime));
		}

	}
	private IEnumerator bounce(Vector3 position, float height, float time)
	{
		float elapsedTime = 0;
		while (elapsedTime < time )
		{
			
			myTransform.position = new Vector3( position.x, Mathf.Lerp(position.y, height, (elapsedTime / time)), position.z);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
			
		}
		myTransform.position = new Vector3(position.x, height, position.z);
		
		elapsedTime = 0;
		while (elapsedTime < time)
		{
			
			myTransform.position = new Vector3( position.x, Mathf.Lerp(height, position.y, (elapsedTime / time)), position.z);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
			
		}
		myTransform.position = new Vector3(position.x, y, position.z);
	}
	public void selfDestroy()
	{
		if(tileMovement != null)
		{
			Debug.Log ("Should destroy but busy");
			StopCoroutine(tileMovement);
			Destroy (transform.gameObject);
		}
		else
		{
			Destroy (transform.gameObject);
		}
	}
}
