using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class unitAI : MonoBehaviour {

	public abstract void doDamage(float amount);

	public abstract void remove();
	
	public abstract float startingHitpoints {
		get;
		set;
	}
	public abstract float speed {
		get;
		set;
	}
	public abstract float fireCooldown {
		get;
		set;
	}
	public abstract float damage {
		get;
		set;
	}
	public abstract float turnDelay {
		get;
		set;
	}
	public abstract float turnSpeed {
		get;
		set;
	}

}


