using UnityEngine;
using System.Collections;

public class DebugAim : MonoBehaviour {

	public Transform debugCamera;
	public float speedX;
	public float speedY;
	public PlayerScript ps;

	private float pan;
	private float tilt;



	private bool useMouse = true;

	private void Start () {

		pan = debugCamera.eulerAngles.y;
		tilt = debugCamera.eulerAngles.x;

	}

	private void Update () {

		if (Application.platform != RuntimePlatform.OSXEditor) return;

		if (Input.GetKeyDown("x")) useMouse = !useMouse;

		useMouse = (useMouse && ps.sp.spawning);

		if (!useMouse) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			return;
		}

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		pan += Input.GetAxis("Mouse X") * Time.deltaTime * speedX;
		tilt -= Input.GetAxis("Mouse Y") * Time.deltaTime * speedY;

		debugCamera.transform.eulerAngles = new Vector3(tilt, pan, 0);

		if (Input.GetMouseButtonUp(0)) ps.fireLaser();
		if (Input.GetMouseButtonUp(1)) ps.fireMissile();

	}

}
