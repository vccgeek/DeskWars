//#define _DEBUG

using UnityEngine;
using System.Collections;



public class missileScript : MonoBehaviour {

	public Transform	 	engine;
	public ParticleSystem 	explosion;
	public Transform		gyro;
	public float 			speed;
	public float			acceleration;
	public float 			turnRate;
	public float			damage;
	public GameObject		graphics;
	public Transform		engineMount;
	public Transform		enginePrefab;
	public bool				debugMe;
	public AudioSource		speaker;
	public AudioClip []		explosionSounds;

	public float 			lifetime;

	private	bool			destroyed = false;
	private Transform		target;
	private float			currentSpeed;
	private float 			_lifeLeft;
	
	public void setTarget(Transform t, float d) {

		target = t;
		damage = d;
		currentSpeed = 0;

	}

	public void destroy() {

		StartCoroutine(doDestroy());
	}

	private IEnumerator doDestroy() {

		destroyed = true;
		if (engine != null)
			Destroy(engine.gameObject);
		explosion.Emit(50);
		int explosionPick = Random.Range(0, explosionSounds.Length);
		speaker.PlayOneShot(explosionSounds[explosionPick]);
		graphics.SetActive(false);
		yield return new WaitForSeconds(2.0f);
		Destroy(gameObject);

	}

	private void Update() {

		if (destroyed) return;
		
		if (target != null) {
			
			gyro.LookAt(target);
			/**
			float x = Mathf.MoveTowardsAngle(transform.eulerAngles.x, gyro.eulerAngles.x, 
			                                 turnRate * Time.deltaTime);
			float y = Mathf.MoveTowardsAngle(transform.eulerAngles.y, gyro.eulerAngles.y, 
			                                 turnRate * Time.deltaTime);
			float z = Mathf.MoveTowardsAngle(transform.eulerAngles.z, gyro.eulerAngles.z, 
			                                 turnRate * Time.deltaTime);
			
			transform.eulerAngles = new Vector3(x,y,z);
			/**/

			transform.rotation = 
				Quaternion.RotateTowards(transform.rotation, gyro.rotation, turnRate * currentSpeed);

		}

		if (engine != null)
			engine.transform.position = engineMount.transform.position;

		if (currentSpeed < speed) currentSpeed += acceleration;

		//Vector3 angle = transform.TransformDirection(Vector3.forward);

		//Vector3 dir = angle * currentSpeed * Time.deltaTime;

		transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

		_lifeLeft -= Time.deltaTime;
		//Debug.Log()
		if (_lifeLeft < 0) destroy();

	}
	
	private void Start() {

		engine = Instantiate(enginePrefab);
		_lifeLeft = lifetime;
		#if _DEBUG
		setTarget(GameObject.Find("battleship").transform, 2000);
		#endif
	}
	
	public void OnCollisionEnter(Collision collision) {

		if (lifetime - _lifeLeft < 0.5f || _lifeLeft == 0) return;

		//Debug.Log("lifetime = " + lifetime.ToString() + " _lifeLeft = " + _lifeLeft.ToString());

		unitAI other = collision.collider.transform.root.gameObject.GetComponent<unitAI>();
		if (other == null)
			other = collision.collider.gameObject.GetComponent<unitAI>();
		//Debug.Log("Other == " + collision.collider.gameObject.name);
		if (other == null) return;
		other.doDamage(damage);
		destroy();

	}

}
