using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
	[HideInInspector] public Player player;

	TrailRenderer trailRenderer;

	float timer;
	float lifetime;

	// Start is called before the first frame update
	void Start() {
		trailRenderer = GetComponent<TrailRenderer>();
		timer = 0;
		lifetime = trailRenderer.time;
		Debug.Log("Lifetime: " + lifetime);
	}

	// Update is called once per frame
	void Update() {
		timer += Time.deltaTime;
		if (timer >= lifetime || player.velocity.y <= 0) {
			Destroy(gameObject);
			Debug.Log("Trail Destroyed");
		}
	}
}
