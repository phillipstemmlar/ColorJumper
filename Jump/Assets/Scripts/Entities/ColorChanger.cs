using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ColorChanger : MonoBehaviour
{
	BoxCollider2D collider;
	PlatformGenerator platformGenerator;
	public int ColorIndex = 0;

	[HideInInspector]
	public bool triggered = false;

	void Awake() {
		collider = GetComponent<BoxCollider2D>();
	}

	public void change() {
		if (platformGenerator != null) platformGenerator.changeBackgroundColor(ColorIndex);
	}
	public void setPlatformGenerator(PlatformGenerator platgen) {
		platformGenerator = platgen;
	}
	public void trigger() {
		collider.enabled = false;
		Destroy(gameObject);
	}

}
