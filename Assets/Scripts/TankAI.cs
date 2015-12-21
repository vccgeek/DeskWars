using UnityEngine;
using System.Collections;

public class TankAI : unitAI {

	#region [ Private variables ]

	private float _hitpoints;
	private float _speed;
	private float _cooldown;
	private float _damage;
	private float _turnDelay;
	private float _turnSpeed;

	private bool  removing;
	private bool  destroyed;

	#endregion

	#region [ Public methods ]

	public override void doDamage(float amount) {

		if (destroyed || removing) return;

		if (amount < 0) {
			Debug.LogWarning("doDamage(): Negative damage amount");
			return;
		}

		_hitpoints -= amount;

		if (_hitpoints < 0) {
			StartCoroutine(die());
			playerCollider.GetComponent<PlayerScript>().addPoints(1);
		}

	}
	
	public override void remove() {

		if (destroyed) return;
		StartCoroutine(doRemove());

	}

    #endregion

	#region [ Private methods ]

	private IEnumerator die() {

		destroyed = true;

		Instantiate(explosionPrefab, explosionMount.position, explosionMount.rotation);
		yield return new WaitForSeconds(1.0f);
		Destroy(gameObject);

	}

	private IEnumerator doRemove() {

		removing = true;
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);

	}

	private IEnumerator doFire() {

		while (!removing && !destroyed) {

			Transform m = 
				(Transform)Instantiate(missilePrefab, missileMount.position, missileMount.rotation);

			m.GetComponent<missileScript>().setTarget(playerCollider, _damage);

			yield return new WaitForSeconds(_cooldown);

		}

	}

	private IEnumerator doMove() {

		while (!destroyed && !removing) {

			// turn
			float heading = Random.Range(0f, 360f);
			//Debug.Log("Turning to " + heading.ToString() + " from " + transform.eulerAngles.y.ToString());
			while (Mathf.Abs(heading - transform.eulerAngles.y) > 1f) {
				if (destroyed || removing) yield break;
				float newHeading = 
					Mathf.MoveTowardsAngle(transform.eulerAngles.y, heading, _turnSpeed * Time.deltaTime);
				transform.eulerAngles = new Vector3(0, newHeading, 0);
				yield return new WaitForSeconds(Time.deltaTime);
			}
			
			//move
			float delay = _turnDelay;
			//Debug.Log("Moving");
			while (delay > 0) {
				if (destroyed || removing) yield break;
				Vector3 dir = transform.TransformDirection(Vector3.forward);
				transform.Translate(dir * _speed * Time.deltaTime, Space.World);
				delay -= Time.deltaTime;
				yield return new WaitForSeconds(Time.deltaTime);
			}
			
			yield return new WaitForSeconds(Time.deltaTime);

		}

	}

	#endregion

	#region [ Public property kludge ]

	public float Starting_Hitpoints;
	public float Movement_Speed;
	public float Fire_Cooldown;
	public float Weapon_Damage;
	public float Turn_Delay;
	public float Turn_Speed;

	#endregion

	#region [ Public properties ]

	public override float startingHitpoints {
		get { return Starting_Hitpoints; }
		set { Starting_Hitpoints = Mathf.Max(0, value); }
	}
	public override float speed {
		get { return Movement_Speed; }
		set { Movement_Speed = Mathf.Max(0, value); }
	}
	public override float fireCooldown {
		get { return Fire_Cooldown; }
		set { Fire_Cooldown = Mathf.Max(0, value); }
	}
	public override float damage {
		get { return Weapon_Damage; }
		set { Weapon_Damage = Mathf.Max(0, value); }
	}
	public override float turnDelay {
		get { return Turn_Delay; }
		set { Turn_Delay = Mathf.Max(0, value); }
	}
	public override float turnSpeed {
		get { return Turn_Speed; }
		set { Turn_Speed = Mathf.Max(0, value); }
	}

	#endregion

	#region [ Public fields ]

	public Transform explosionMount;
	public Transform missileMount;
	public Transform explosionPrefab;
	public Transform missilePrefab;
	public Transform playerCollider;

	#endregion

	#region [ Unity events ]

	private void Start() {

		_hitpoints = Starting_Hitpoints;
		_speed = Movement_Speed;
		_cooldown = Fire_Cooldown;
		_damage = Weapon_Damage;
		_turnDelay = Turn_Delay;
		_turnSpeed = Turn_Speed;

		if (playerCollider == null) {
			playerCollider = GameObject.FindWithTag("Player").transform;
		}

		StartCoroutine(doMove());
		StartCoroutine(doFire());

	}

	#endregion

}
