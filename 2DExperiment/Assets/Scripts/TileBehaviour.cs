using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public enum TileType {Green, Blue, Yellow, Red, Purple}

public class TileBehaviour : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

	public int x;
	public int y;
	public TileType tileType;
	public bool falling;
	
	
	public float selectedScale = 0.6f;
	private float defaultFallTimeInSec = 0.4f;
	private float bounceHeight = 0.1f;
	private float bounceTime = 0.1f;
	private Transform myTransform;
	
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
		Debug.Log ("Tiles selection called");
		ReactOnSelected();
	}
	public void isDeselected(){
		ReactOnDeselected();
	}
	
	private void ReactOnSelected(){
		Debug.Log ("Scaling should happen here");
		transform.localScale = new Vector3 (selectedScale, selectedScale, selectedScale);
	}
	private void ReactOnDeselected(){
		transform.localScale = new Vector3 (1f, 1f, 1f);
	}
	
	public void fallOneRow()
	{
		
		Vector3 position = transform.position;
		StartCoroutine(cFall(position, position.y - 1f, defaultFallTimeInSec));
		
	}
	public void fallDistance(int distance)
	{
		Vector3 position = transform.position;
		StartCoroutine(cFall(position, position.y - distance, defaultFallTimeInSec));
	}
	
	private IEnumerator cFall(Vector3 position, float fallEnd, float time)
	{
		falling = true;
		float elapsedTime = 0;
		
		while (elapsedTime < time)
		{
			
			myTransform.position = new Vector3( position.x, Mathf.Lerp(position.y, fallEnd, (elapsedTime / time)), position.z);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
			
		}
		myTransform.position = new Vector3(position.x, fallEnd, position.z);
		falling = false;
		StartCoroutine(bounce(myTransform.position, myTransform.position.y + bounceHeight, bounceTime));
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
		//Debug.Log (myTransform.position.y);
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
