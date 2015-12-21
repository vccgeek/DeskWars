using UnityEngine;
using System.Collections;

public class TurretAI : unitAI {
	
	#region [ Private variables ]
	
	private float _hitpoints;
	//private float _speed;
	private float _cooldown;
	private float _damage;
	//private float _turnDelay;
	private float _turnSpeed;
	
	private bool  removing;
	private bool  destroyed;

	private float targetBearing;
	private float targetElevation;


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
			playerCollider.GetComponent<PlayerScript>().addPoints(2);
		}
		
	}
	
	public override void remove() {
		
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

	public IEnumerator doAim() {

		while (!removing && !destroyed) {

			targetBearing = getAngle(playerCollider, frame);
			targetElevation = getElevation(playerCollider, yoke);

			//Debug.Log("elevation == " + targetElevation);

			Quaternion fromBearing = yoke.rotation;
			Quaternion toBearing = Quaternion.Euler(0,targetBearing,0);
			Quaternion fromElevation = frame.localRotation;
			Quaternion toElevation = Quaternion.Euler(targetElevation,0,0);

			yoke.rotation = Quaternion.RotateTowards(fromBearing, toBearing, _turnSpeed * Time.deltaTime);
			frame.localRotation = Quaternion.RotateTowards(fromElevation, toElevation, _turnSpeed * Time.deltaTime);

			yield return new WaitForSeconds(Time.deltaTime);

		}

	}

	public float getAngle(Transform b, Transform a) {

		return getAngle(a.position.x, a.position.z, b.position.x, b.position.z);

	}

	public float getElevation(Transform a, Transform b) {

		float adj = b.position.y - a.position.y;
		float hyp = Vector3.Distance(a.position, b.position);

		//Debug.Log("adj = " + adj.ToString() + " hyp = " + hyp.ToString());

		return (Mathf.Asin(adj/hyp) * (180f / Mathf.PI));

	}

	public float getAngle(float X1, float Y1, float X2, float Y2) {
		
		// take care of special cases - if the angle
		// is along any axis, it will return NaN,
		// or Not A Number.  This is a Very Bad Thing(tm).
		if (Y2 == Y1) {
			return (X1 > X2) ? 180 : 0;
		}
		if (X2 == X1) {
			return (Y2 > Y1) ? 90 : 270;
		}
		
		float tangent = (X2 - X1) / (Y2 - Y1);
		// convert from radians to degrees
		double ang = (float) Mathf.Atan(tangent) * 57.2958;
		// the arctangent function is non-deterministic,
		// which means that there are two possible answers
		// for any given input.  We decide which one here.
		if (Y2-Y1 < 0) ang -= 180;
		
		
		// NOTE that this does NOT need to be normalised.  Arctangent
		// always returns an angle that is within the 0-360 range.
		
		
		// barf it back to the calling function
		return (float) ang;
		
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
	public Transform yoke;
	public Transform frame;
	
	#endregion
	
	#region [ Unity events ]
	
	private void Start() {
		
		_hitpoints = Starting_Hitpoints;
		//_speed = Movement_Speed;
		_cooldown = Fire_Cooldown;
		_damage = Weapon_Damage;
		//_turnDelay = Turn_Delay;
		_turnSpeed = Turn_Speed;
		
		if (playerCollider == null) {
			playerCollider = GameObject.FindWithTag("Player").transform;
		}
		
		StartCoroutine(doFire());
		StartCoroutine(doAim());
		
	}
	
	#endregion
	
}
