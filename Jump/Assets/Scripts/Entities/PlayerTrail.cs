using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
	[HideInInspector] public Player player;

	TrailRenderer trailRenderer;

	public float dyingTime = 0.1f;
	public float dyingVelocity = 0.5f;
	float initAlpha;
	Color ColorCopy;

	float timer;
	float lifetime;

	bool isDead;

	// Start is called before the first frame update
	void Start() {
		trailRenderer = GetComponent<TrailRenderer>();
		timer = 0;
		isDead = false;
		lifetime = trailRenderer.time;
		initAlpha = trailRenderer.startColor.a;

		ColorCopy = trailRenderer.startColor;
	}

	// Update is called once per frame
	void Update() {
		timer += Time.deltaTime;

		if (isDead) {

			ColorCopy.a = Mathf.Lerp(initAlpha, 0f, timer);
			trailRenderer.startColor = ColorCopy;

			if (timer >= dyingTime) Destroy(gameObject);
		} else if (timer >= lifetime || player.velocity.y <= dyingVelocity) {
			isDead = true;
		}

	}


}
