using UnityEngine;
using System.Collections;

public class LookOrbit : MonoBehaviour {

	public Transform target;
	private Transform followTarget;
    public float followSpeed = 5;
	public float HeightOffset = 0f;
    public Vector3 offset;
	public float distance= 1.5f;

	public float xSpeed= 175.0f;
	public float ySpeed= 75.0f;
	public float pinchSpeed;
	private float lastDist = 0;
	private float curDist = 0;
	
	public int yMinLimit= 10; //Lowest vertical angle in respect with the target.
	public int yMaxLimit= 80;
	
	public float minDistance= .5f; //Min distance of the camera from the target
	public float maxDistance= 1.5f;
	
    private float x= 0.0f;
	private float y= 0.0f;
	private Touch touch;
	private bool CamFreeze = true;
	private bool ControlsFreeze = false;
	private bool RotationFreeze = false;
    
	
	void  Start (){
		
		//Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
		followTarget = new GameObject().GetComponent<Transform>();
		followTarget.position = new Vector3 (target.position.x, target.position.y + offset.x + offset.y, target.position.z + offset.z);

		transform.parent = followTarget;

		Vector3 angles= transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		// Make the rigid body not change rotation
		
		if (GetComponent<Rigidbody> ()) {
				GetComponent<Rigidbody> ().freezeRotation = true;
			}
		StartCoroutine (UnfreezeCam());
		}
		

	IEnumerator UnfreezeCam () {
				yield return null;
				yield return null;
				yield return null;
				yield return null;
				yield return null;
				yield return null;

				CamFreeze = false;
		}
	public void FreezeControls(){
		ControlsFreeze = true;
		}
	public void UnfreezeControls(){
		ControlsFreeze = false;
	}
	public void FreezeRotation(){
		RotationFreeze = true;
		}
	public void UnfreezeRotation(){
		RotationFreeze = false;
	}
		

	void  Update (){
		if (target && !CamFreeze && !RotationFreeze) {			

			//Zooming with mouse
			if(!ControlsFreeze){
				distance += Input.GetAxis("Mouse ScrollWheel")*-distance;			
				distance = Mathf.Clamp(distance, minDistance, maxDistance);
			}

			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && !ControlsFreeze) {
				
				//One finger touch does orbit
				
				touch = Input.GetTouch(0);
				
				x += touch.deltaPosition.x * xSpeed * 0.01f;
				
				y -= touch.deltaPosition.y * ySpeed * 0.01f;
				
			}
			
			if (Input.touchCount > 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved) && !ControlsFreeze){
				
				//Two finger touch does pinch to zoom
				
				var touch1 = Input.GetTouch(0);
				
				var touch2 = Input.GetTouch(1);
				
				curDist = Vector2.Distance(touch1.position, touch2.position);
				
				if(curDist > lastDist){				
					distance += Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition)*-pinchSpeed/10;
				}
				else{
					distance -= Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition)*-pinchSpeed/10;
				}
				
				
				
				lastDist = curDist;

			}
			
			//Detect mouse drag;
			
			if(!ControlsFreeze && Input.GetMouseButton(0)) {

					x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
					y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            }

            //y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
			
			Vector3 vTemp = new Vector3(0.0f, 0.0f, -distance);
			
			Vector3 position= rotation * vTemp + followTarget.position;

			followTarget.position = Vector3.Slerp (followTarget.position, new Vector3 (target.position.x + offset.x, target.position.y + offset.y, target.position.z + offset.z), followSpeed * Time.deltaTime);

			transform.position = Vector3.Slerp (transform.position, position, 10 * Time.deltaTime);
			transform.rotation = rotation;

			
		}
		transform.LookAt (followTarget);	
	}
	
	static float  ClampAngle ( float angle ,   float min ,   float max  ){
		
		if (angle < -360)
			
			angle += 360;
		
		if (angle > 360)
			
			angle -= 360;

        angle = Mathf.Clamp(angle, min, max);

        return angle;		
	}
}