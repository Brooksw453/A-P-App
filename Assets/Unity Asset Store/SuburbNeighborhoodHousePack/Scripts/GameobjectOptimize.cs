using UnityEngine;
using System.Collections;

public class GameobjectOptimize : MonoBehaviour {

	public GameObject GameobjectToDisable;
	public GameObject DistanceFromObject;
	public float VisibilityDistance; // Distance to MainCamera
	private float Distance;
	private GameObject MainCamera;
	///------------------------------
	void Start(){

		

		if (DistanceFromObject == null) // If no additional game object is assigned, use this game object the script is attached to.
        {
			DistanceFromObject = gameObject;
        }

		if (GameObject.FindGameObjectWithTag ("MainCamera") != null) {
			MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		} else {
			Destroy(GetComponent("GameobjectOptimize"));
		}

	}

	// Update is called once per frame
	void Update () {

		
			Distance = Vector3.Distance(MainCamera.transform.position, DistanceFromObject.transform.position);

			if (Distance < VisibilityDistance)
			{
				GameobjectToDisable.SetActive(true);
			}
			if (Distance > VisibilityDistance)
			{
				GameobjectToDisable.SetActive(false);
		}
		
	}

}