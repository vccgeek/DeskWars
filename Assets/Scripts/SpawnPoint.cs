using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour {

	public Transform battleshipPrefab;
	public Transform turretPrefab;
	public Transform tankPrefab;
	public float spawnDelay;
	public float range;

	private List<unitAI> units;

	public bool spawning;

	public void stop() {
		spawning = false;
	}

	public void begin() {

		spawning = true;
		StartCoroutine(spawn());

	}

	private void Start() {

		//spawning = true;
		units = new List<unitAI>();
		//StartCoroutine(spawn());

	}

	private IEnumerator spawn() {

		while (spawning) {
			int pick = Random.Range(0, 100);

			if (pick < 40) {
				spawnTank();
			} else if (pick < 80) {
				spawnTurret();
			} else {
				spawnBattleship();
			}

			yield return new WaitForSeconds(spawnDelay);

		}

	}

	private void spawnTank() {

		float xPick = Random.Range(-1 * range, range);
		float yPick = Random.Range(-1 * range, range);

		Transform t = (Transform) Instantiate(tankPrefab, new Vector3(xPick, 0, yPick), Quaternion.identity);
		if (t == null) Debug.LogError("The object was not instantiated.");
		unitAI uai = t.gameObject.GetComponent<unitAI>();
		if (uai == null) Debug.LogError("unitAI object not found.");
		units.Add(uai);

	}

	private void spawnTurret() {
		
		float xPick = Random.Range(-1 * range, range);
		float yPick = Random.Range(-1 * range, range);
		
		Instantiate(turretPrefab, new Vector3(xPick, 0, yPick), Quaternion.identity);
		
	}

	private void spawnBattleship() {
		
		float xPick = Random.Range(-1 * range, range);
		float yPick = Random.Range(-1 * range, range);
		
		Instantiate(battleshipPrefab, new Vector3(xPick, 0, yPick), Quaternion.identity);
		
	}

	public void RemoveAllUnits() {

		foreach (unitAI u in units) {
			if (u != null)
				u.remove();
		}

		for (int L = 0; L < units.Count; L++) units.RemoveAt(0);

	}

}
