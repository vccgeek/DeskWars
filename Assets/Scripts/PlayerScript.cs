using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TouchScript;

public class PlayerScript : unitAI {

	public float 		StartHealth;
	public float 		MissileCooldown;
	public float 		MissileDamage;
	public float 		LaserCooldown;
	public float 		LaserDamage;
	public float 		laserShotDuration;
	public LayerMask 	shotHitLayer;
	public Transform	gameOver;
	public Transform	startGame;
	public AudioSource	speaker;

	public AudioClip []	musicTracks;
	public AudioSource 	musicSpeaker;
	private int			currentTrack;

	public AudioClip	laserSound;
	public AudioClip	deathSound;
	public AudioClip	missileSound;

	public AudioClip	unitEnter;

	public int []		scoreAmounts;

	public Transform	smallExplosion;

	public Transform []	lasers;
	
	public Transform 	missileMount;
	public Transform 	missilePrefab;
	public RectTransform healthBar;
	public RectTransform crosshair;

	public Text 		scoreLabel;

	public SpawnPoint 	sp;

	private float 		_health;
	private int 		score;

	private float 		missileCooldownRemaining;
	private float 		laserCooldownRemaining;

	public  Button		missilesButton;
	public  Button		gatlingButton;

	public RectTransform	missileCD;
	public RectTransform 	gatlingCD;

	private int 		currentBeam;

	private void Start() {

		_health = StartHealth;
		
		
		StartCoroutine(doMusic());

	}

	public void addPoints(int type) {

		if (type < 0 || type > 2) return;

		score += scoreAmounts[type];

	}

	public void newUnitAlert() {

		speaker.PlayOneShot(unitEnter);

	}

	private void Update() {

		// update cooldowns
		if (missileCooldownRemaining > 0) missileCooldownRemaining -= Time.deltaTime;
		missileCD.localScale = new Vector3(1, 1 - (missileCooldownRemaining/MissileCooldown), 1);
		if (laserCooldownRemaining > 0) laserCooldownRemaining -= Time.deltaTime;
		gatlingCD.localScale = new Vector3(1, 1 - (laserCooldownRemaining/LaserCooldown), 1);

		// update score
		scoreLabel.text = score.ToString();


	}

	private IEnumerator doMusic() {

		while (true) {
			// play music
			musicSpeaker.PlayOneShot(musicTracks[currentTrack]);
			float delay = musicTracks[currentTrack].length + 3f;
			currentTrack++;
			if (currentTrack > musicTracks.Length) currentTrack = 0;
			yield return new WaitForSeconds(delay);
		}

	}

	public void fireLaser() {

		if (laserCooldownRemaining > 0) return;

		Vector3 dir = transform.TransformDirection(Vector3.up);

		Ray r = new Ray(transform.root.position, dir);
		RaycastHit rch = new RaycastHit();

		/**
		Debug.DrawRay(r.origin, r.direction * 60);
		Debug.Break();
		/**/
		Physics.Raycast(r, out rch, 99999f, shotHitLayer);

		bool bCollider = (rch.collider != null);
		bool bTransform = false;
		if (bCollider) bTransform = (rch.collider.transform != null);
		//Debug.Log("Collider: " + bCollider.ToString() + " Transform: " + bTransform.ToString());

		Transform target;
		if (rch.collider == null) {
			target = null;
		} else {
			target = rch.collider.transform;
		}

		if (target != null) {
			unitAI u = target.root.GetComponent<unitAI>();
			if (u != null) u.doDamage(LaserDamage);
			missileScript ms = target.root.GetComponent<missileScript>();
			if (ms != null) ms.destroy();
			Instantiate(smallExplosion, rch.point, Quaternion.identity);
		}

		currentBeam++;
		if (currentBeam == 3) currentBeam = 0;
		StartCoroutine(laserBeam(currentBeam));
		laserCooldownRemaining = LaserCooldown;

	}

	private IEnumerator laserBeam(int beam) {

		speaker.PlayOneShot(laserSound);
		lasers[beam].gameObject.SetActive(true);
		yield return new WaitForSeconds(laserShotDuration);
		lasers[beam].gameObject.SetActive(false);

	}

	public void fireMissile() {

		if (missileCooldownRemaining > 0) return;

		Transform m = (Transform) Instantiate(missilePrefab, missileMount.position, missileMount.rotation);

		Vector3 dir = transform.TransformDirection(Vector3.up);
		
		Ray r = new Ray(transform.root.position, dir);
		RaycastHit rch = new RaycastHit();
		Physics.Raycast(r, out rch, 99999f, shotHitLayer);

		speaker.PlayOneShot(missileSound);

		Transform target;
		if (rch.collider == null) {
			target = null;
		} else {
			target = rch.collider.transform;
		}

		m.GetComponent<missileScript>().setTarget(target, MissileDamage);

		missileCooldownRemaining = MissileCooldown;

	}

	public override void doDamage(float amount) {

		//Debug.Log("Losing " + amount.ToString() + " health!");
		_health -= amount;
		float size = Mathf.Max(0, _health/StartHealth);
		//Debug.Log("Setting health bar size to " + size.ToString());
		healthBar.localScale = new Vector3(size, 1);

		if (_health < 0) doGameOver();

	}

	public void restartGame() {

		sp.begin();
		score = 0;
		_health = StartHealth;
		gameOver.gameObject.SetActive(false);
		startGame.gameObject.SetActive(false);

	}

	private void doGameOver() {

		sp.stop();
		sp.RemoveAllUnits();

		gameOver.gameObject.SetActive(true);
		speaker.PlayOneShot(deathSound);

	}

	// These are here just to satisfy the unitAI overrides. We don't actually use them for
	// the player.
	public override float startingHitpoints {
		get { return StartHealth; }
		set {}
	}
	public override float speed {
		get { return StartHealth; }
		set {}
	}
	public override float fireCooldown {
		get { return StartHealth; }
		set {}
	}
	public override float damage {
		get { return StartHealth; }
		set {}
	}
	public override float turnDelay {
		get { return StartHealth; }
		set {}
	}
	public override float turnSpeed {
		get { return StartHealth; }
		set {}
	}

	public override void remove() {

	}


}
