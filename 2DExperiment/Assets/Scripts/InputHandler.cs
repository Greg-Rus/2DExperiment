using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

	public LayerMask clickableMask;
	//private RaycastHit hit;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

//|| UNITY_STANDALONE

	void Update () 
	{
		#if UNITY_EDITOR || UNITY_STANDALONE
		if(Input.GetMouseButton(0))
		{
			
		}
		if(Input.GetMouseButtonDown(0))
		{
			//GameObject clicked = castRay();
			//Debug.Log(clicked);
		}
		if(Input.GetMouseButtonUp(0))
		{
			
		}
		#endif
		#if UNITY_METRO
		//Debug.Log ("Metro!");
		#endif
	}
/*	
	GameObject castRay()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction *100f, Color.green, 100f);
		RaycastHit2D hit;
		if (Physics2D.Raycast(ray, out hit, 100))
		{
			Debug.DrawLine(ray.origin, hit.point, Color.blue, 100f);
			Debug.Log (hit.transform.position);
			return hit.transform.gameObject;
		}
		else return null;
		
	}
*/
}
