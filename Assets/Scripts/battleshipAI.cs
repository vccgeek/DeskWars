#define _DEBUG

using UnityEngine;
using System.Collections;

public class battleshipAI : unitAI {

	#region [ private variables ]
	private float _startingHitpoints;
	private float _speed;
	private float _fireCooldown;
	private float _damage;
	private float _hitpoints;
	private float _turnDelay;
	private float _turnSpeed;
	private bool removing;
	private bool sinking;

	private Animator anim;
	private ParticleSystem explosion;
	#endregion

	#region [ private methods ]
	
	private IEnumerator sink() {

		sinking = true;
		anim.SetBool("sunk", true);
		yield return new WaitForSeconds(1.83f);
		explosion.Emit(50);
		yield return new WaitForSeconds(2.83f);
		//explosion.emit = false;
		Destroy(gameObject);

	}

	private IEnumerator doMove() {

		float delay = _turnDelay;
		float heading = Random.Range(0f, 360f);
		while (_hitpoints > 0) {

			// turn
			if (delay < 0) {
				heading = Random.Range(0f, 360f);
				delay = _turnDelay;
			}
			if (Mathf.Abs(heading - transform.eulerAngles.y) > 1f) {
				if (sinking || removing) yield break;
				float newHeading = 
					Mathf.MoveTowardsAngle(transform.eulerAngles.y, heading, turnSpeed * Time.deltaTime);
				transform.eulerAngles = new Vector3(0, newHeading, 0);
			}

			//move
			if (sinking || removing) yield break;
			Vector3 dir = transform.TransformDirection(Vector3.forward);
			transform.Translate(dir * speed * Time.deltaTime, Space.World);
			delay -= Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);

		}

	}

	private IEnumerator doFire() {

		//Debug.Log("You may fire when ready.");

		while (_hitpoints > 0) {
			if (sinking || removing) yield break;

			//Debug.Log("Please stand by!");

			float delay = _fireCooldown;
			while (delay > 0) {

				delay -= Time.deltaTime;
				if (removing) yield break;
				yield return new WaitForSeconds(Time.deltaTime);

			}

			//Debug.Log("It's away!");
			GameObject missile = 
				((Transform)Instantiate(missilePrefab, launcherMount.position, launcherMount.rotation)).gameObject;

			missile.GetComponent<missileScript>().setTarget(playerCollider, _damage);

		}

		yield break;

	}

	private IEnumerator doRemove() {

		removing = true;
		yield return new WaitForSeconds(1.0f);
		Destroy(gameObject);

	}
	
	#endregion

	#region [ Unity events ]
	private void Start() {

		anim = GetComponent<Animator>();
		explosion = this.GetComponentInChildren<ParticleSystem>();

		if (playerCollider == null) {
			playerCollider = GameObject.FindWithTag("Player").transform;
		}

		// do the kludge for wiring up some public fields to their properties because
		// freaking Unity can't expose a property in the inspector for some sick, 
		// twisted reason
		_startingHitpoints = Starting_HP;
		_speed = Move_Speed;
		_fireCooldown = Fire_Cooldown;
		_damage = Weapon_Damage;
		_turnDelay = Turn_Delay;
		_turnSpeed = Turn_Speed;
		_hitpoints = _startingHitpoints;

		StartCoroutine(doMove());
		StartCoroutine(doFire());
	
	}

	private void Update() {
		
		
		#if _DEBUG
		CurrentHitpoints = _hitpoints;
		#endif
	}

	#endregion

	#region [ public methods ]
	public override void doDamage(float amount) {

		//Debug.Log("Doing " + amount.ToString() + " damage!");

		if (amount <= 0) return;
		_hitpoints -= amount;
		if (_hitpoints < 0)  {
			StartCoroutine(sink());
			playerCollider.GetComponent<PlayerScript>().addPoints(0);
		}

	}

	public override void remove() {

		removing = true;
		StartCoroutine(doRemove());

	}

	#endregion

	#region [ public properties ]

	public override float startingHitpoints {
		get { return _startingHitpoints; }
		set { 
			_startingHitpoints = Mathf.Max(0, value);
		}
	}

	public override float speed {
		get { return _speed; }
		set {
			_speed = Mathf.Max(0, value);
		}
	}

	public override float fireCooldown {
		get { return _fireCooldown; }
		set {
			_fireCooldown = Mathf.Max(0, value);
		}
	}

	public override float damage {
		get { return _damage; }
		set {
			_damage = Mathf.Max(0, value);
		}
	}

	public override float turnDelay {
		get { return _turnDelay; }
		set {
			_turnDelay = Mathf.Max(0, value);
		}
	}

	public override float turnSpeed {
		get { return _turnSpeed; }
		set {
			_turnSpeed = Mathf.Max(0, value);
		}
	}

	#endregion

	#region [ public field kludge because unity can't expose a stinking property ]

	public float Starting_HP;
	public float Move_Speed;
	public float Fire_Cooldown;
	public float Weapon_Damage;
	public float Turn_Delay;
	public float Turn_Speed;

	#endregion

	#region [ public fields ]

	public Transform missilePrefab;
	public Transform launcherMount;
	public Transform playerCollider;

	#if _DEBUG
	public float CurrentHitpoints;
	#endif

	#endregion

}
