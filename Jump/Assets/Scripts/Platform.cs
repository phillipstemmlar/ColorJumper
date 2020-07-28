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

	public bool isDrawingBoundingBox = false;

	public float blockWidth {
		get { return (platformGenerator == null) ? 10f : platformGenerator.blockWidth; }
		set { if (platformGenerator != null) platformGenerator.blockWidth = value; }
	}
	public float blockHeight {
		get { return (platformGenerator == null) ? 3.33f : platformGenerator.blockHeight; }
		set { if (platformGenerator != null) platformGenerator.blockHeight = value; }
	}
	public float scaleMultiplier {
		get { return (platformGenerator == null) ? 100f : platformGenerator.scaleMultiplier; }
		set { if (platformGenerator != null) platformGenerator.scaleMultiplier = value; }
	}

	private void Awake() {
		platformController = GetComponent<PlatformController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start() {
		transform.localScale = new Vector3(blockWidth * scaleMultiplier, blockHeight * scaleMultiplier, 1);
		platformGenerator.registerBlock(this);
		setColor(platformGenerator.randomColor());
	}

	void Update() {
		if (isOutOfBounds()) onOutOfBounds();
		if (isDrawingBoundingBox) drawBoundingBox();
	}

	protected virtual bool isOutOfBounds() {
		return (transform.position.x <= platformGenerator.LeftOfScreen.x - blockWidth / 2);
	}
	protected virtual void onOutOfBounds() {
		Die();
	}

	public void setColor(Color col) {
		spriteRenderer.color = col;
	}
	public void setSpeedX(float speed) {
		platformController.move = new Vector3(-speed, platformController.move.y, 0);
	}

	void drawBoundingBox() {
		PlatformGenerator.Rect rect = new PlatformGenerator.Rect(transform.position, blockWidth, blockHeight);
		rect.draw(Color.yellow);
	}

	public void Die() {
		print("I am Dead now!");

		platformGenerator.unregisterBlock(this);
		Destroy(gameObject);
	}
}
