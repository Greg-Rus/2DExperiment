using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//public enum TileType {Green, Blue, Yellow, Red, Purple}
public enum TileType {Finance, Science, Industry, Military, Social}

public class TileBehaviour : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

	public int x;
	public int y;
	public TileType tileType;
	public bool falling;
	
	
	public float selectedScale = 0.6f;
	private const float defaultFallTimeInSec = 0.4f;
	private float bounceHeight = 0.1f;
	private float bounceTime = 0.1f;
	private Transform myTransform;
	private IEnumerator tileMovement;
	
	// Use this for initialization
	void Awake () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	public void  OnPointerClick (PointerEventData eventData) {
		//Debug.Log ("Works");
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		//Debug.Log(this.tag);
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		BoardController.instance.selectedTile(this);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		BoardController.instance.unselectedTile(this);
	}
	
	public void isSelected()
	{
		ReactOnSelected();
	}
	public void isDeselected(){
		ReactOnDeselected();
	}
	
	private void ReactOnSelected()
	{
		transform.localScale = new Vector3 (selectedScale, selectedScale, selectedScale);
	}
	private void ReactOnDeselected(){
		transform.localScale = new Vector3 (1f, 1f, 1f);
	}
	
	public void fallOneRow()
	{
		
		Vector3 position = transform.position;
//		StartCoroutine(cMoveTile(position, position.y - 1f, defaultFallTimeInSec));
		
	}
	public void fallToRow(int distance)
	{
		if(tileMovement == null)
		{
			//Debug.Log("Falling is " + falling);
			tileMovement = cMoveTile(myTransform.position.x, distance, defaultFallTimeInSec, true);
			StartCoroutine(tileMovement);
		}
		else
		{
			//Debug.Log ("Stopping move coroutine");
			StopCoroutine(tileMovement);
			tileMovement = cMoveTile(myTransform.position.x, distance, defaultFallTimeInSec, true);
			StartCoroutine(tileMovement);
		}
		
		StartCoroutine(cMoveTile(myTransform.position.x, distance, defaultFallTimeInSec, true));
		
	}
	public void moveTo(int x, int y, float time = defaultFallTimeInSec)
	{
		if(tileMovement == null)
		{
			//Debug.Log("Falling is " + falling);
			tileMovement = cMoveTile(x, y, time);
			StartCoroutine(tileMovement);
		}
		else
		{
			//Debug.Log ("Stopping move coroutine");
			StopCoroutine(tileMovement);
			tileMovement = cMoveTile(x, y, time);
			StartCoroutine(tileMovement);
		}

	}
	
	public IEnumerator cMoveTile(float x, float y, float time, bool shouldBounce = false)
	{
		falling = true;
		float elapsedTime = 0;
		Vector3 currentPosition = myTransform.position;
		
		while (elapsedTime < time)
		{
			
			myTransform.position = new Vector3(Mathf.Lerp(currentPosition.x, x, (elapsedTime / time)),
			                                   Mathf.Lerp(currentPosition.y, y, (elapsedTime / time)),
			                                   currentPosition.z);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
			
		}
		if (shouldBounce)
		{
			StartCoroutine(bounce(myTransform.position, myTransform.position.y + bounceHeight, bounceTime));
		}
		myTransform.position = new Vector3(x, y, myTransform.position.z);
		falling = false;
		tileMovement=null;
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
}
