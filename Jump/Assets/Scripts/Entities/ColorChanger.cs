﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ColorChanger : MonoBehaviour
{
	BoxCollider2D collider;
	PlatformGenerator platformGenerator;
	SpriteRenderer spriteRenderer;

	[HideInInspector]
	public Platform parent;

	public int ColorIndex = 0;

	[HideInInspector]
	public bool triggered = false;

	SpriteRenderer[] spriteRenderes;

	void Awake() {
		collider = GetComponent<BoxCollider2D>();

		spriteRenderes = GetComponentsInChildren<SpriteRenderer>();
	}

	public void setColor(int newColorIndex) {
		ColorIndex = newColorIndex;
		Color color = platformGenerator.colors[ColorIndex];

		foreach (SpriteRenderer rend in spriteRenderes) {
			if (rend.gameObject.tag == "ColorChanger") rend.color = color;
		}

	}

	public void change() {
		if (platformGenerator != null) platformGenerator.changeBackgroundColor(ColorIndex);
	}
	public void setPlatformGenerator(PlatformGenerator platgen) {
		platformGenerator = platgen;
	}
	public void trigger() {
		collider.enabled = false;
		Die();
	}

	public void Die() {
		Destroy(gameObject);
	}

}
