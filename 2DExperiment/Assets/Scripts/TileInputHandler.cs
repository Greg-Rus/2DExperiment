using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TileInputHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler  {
	
	public float selectedScale = 0.6f;
	public TileBehaviour myTileBehaviour;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void  OnPointerClick (PointerEventData eventData) {
		//Debug.Log ("Works");
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		//Debug.Log(this.tag);
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		BoardController.instance.selectedTile(myTileBehaviour);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		BoardController.instance.unselectedTile(myTileBehaviour);
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
}
