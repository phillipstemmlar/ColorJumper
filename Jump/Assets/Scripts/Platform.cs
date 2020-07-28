using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlatformController))]
public class Platform : MonoBehaviour
{
	const string Q = "?";

	public PlatformGenerator platformGenerator;
	PlatformController platformController;
	SpriteRenderer spriteRenderer;

	[HideInInspector]
	public Player player;

	[HideInInspector]
	public int ColorIndex = -1;
	Color platformColor = Color.white;

	public bool isInitial = false;
	public bool isDrawingBoundingBox = false;

	public float Width = 4f;
	public float Height = 1.333f;
	public float scaleMultiplier {
		get { return (platformGenerator == null) ? 100f : platformGenerator.scaleMultiplier; }
		set { if (platformGenerator != null) platformGenerator.scaleMultiplier = value; }
	}

	private void Awake() {
		platformController = GetComponent<PlatformController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start() {
		transform.localScale = new Vector3(Width * scaleMultiplier, Height * scaleMultiplier, 1);
		platformGenerator.registerBlock(this);
		//setColor(platformGenerator.randomColor());    //TEMP
	}

	void Update() {
		if (isOutOfBounds()) onOutOfBounds();
		if (isDrawingBoundingBox) drawBoundingBox();
	}

	protected virtual bool isOutOfBounds() {
		return (transform.position.x <= platformGenerator.LeftOfScreen.x - Width / 2);
	}
	protected virtual void onOutOfBounds() {
		Die();
	}

	public void changeColor(int colIndex) {
		if (platformGenerator == null || platformGenerator.colors == null
			|| colIndex < 0 || colIndex >= platformGenerator.colors.Length) {
			colIndex = -1;
		}
		ColorIndex = colIndex;
		updateColor();
	}

	void updateColor() {
		if (ColorIndex < 0) platformColor = Color.white;
		else platformColor = platformGenerator.colors[ColorIndex];

		if (ColorIndex != -1 && player != null && ColorIndex == player.ColorIndex) platformColor.a = platformGenerator.alphaSelected;
		else platformColor.a = platformGenerator.alphaNotSelected;

		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.color = platformColor;
	}

	public void setSpeed(float speed) {
		platformController.move = new Vector3(-speed, platformController.move.y, 0);
	}

	void drawBoundingBox() {
		PlatformGenerator.Rect rect = new PlatformGenerator.Rect(transform.position, Width, Height);
		rect.draw(Color.yellow);
	}

	public void Die() {
		print("I am Dead now!");

		platformGenerator.unregisterBlock(this);
		Destroy(gameObject);
	}
}
