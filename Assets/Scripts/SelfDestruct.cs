using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

	public float lifetime;

	private void Start() {

		StartCoroutine(die());

	}

	private IEnumerator die() {

		yield return new WaitForSeconds(lifetime);

		Destroy(gameObject);

	}



}
