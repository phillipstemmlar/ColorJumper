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
	BoxCollider2D collider;

	SpriteRenderer spriteRenderer_Left;
	SpriteRenderer spriteRenderer_Middel;
	SpriteRenderer spriteRenderer_Right;

	ColorChanger colorChanger = null;

	[HideInInspector]
	public int ColorIndex = -1;
	Color platformColor = Color.white;
	int previousBackgroundColorIndex = -1;

	public bool isInitial = false;
	public bool isDrawingBoundingBox = false;
	public float Width = 2f;
	public float Height = 1f;
	public float scaleMultiplier {
		get { return (platformGenerator == null) ? 1f : platformGenerator.scaleMultiplier; }
		set { if (platformGenerator != null) platformGenerator.scaleMultiplier = value; }
	}

	private void Awake() {
		platformController = GetComponent<PlatformController>();
		collider = GetComponent<BoxCollider2D>();

		SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();

		foreach (SpriteRenderer rend in children) {
			if (rend.gameObject.tag == "platform_left") spriteRenderer_Left = rend;
			if (rend.gameObject.tag == "platform_middel") spriteRenderer_Middel = rend;
			if (rend.gameObject.tag == "platform_right") spriteRenderer_Right = rend;
		}

	}

	void Start() {
		changeSize(Width, Height);
		platformGenerator.registerBlock(this);
		previousBackgroundColorIndex = platformGenerator.BackgroundColorIndex;
	}

	void Update() {
		if (isOutOfBounds()) onOutOfBounds();
		if (checkBackgroundColorChanged()) onBackgroundColorChanged();
	}

	private void OnDrawGizmos() {
		//changeSize(Width, Height);
		if (isDrawingBoundingBox) drawBoundingBox();
	}

	protected virtual bool isOutOfBounds() {
		return (transform.position.x <= platformGenerator.LeftOfScreen.x - Width / 2);
	}
	protected virtual void onOutOfBounds() {
		Die();
	}

	bool checkBackgroundColorChanged() => previousBackgroundColorIndex != platformGenerator.BackgroundColorIndex;

	void onBackgroundColorChanged() {
		previousBackgroundColorIndex = platformGenerator.BackgroundColorIndex;
		updateColor();
	}

	public void changeSize(float w, float h) {
		float width_side = spriteRenderer_Left.bounds.size.x;
		float h_offset = Width / 2 - width_side / 2;

		spriteRenderer_Middel.transform.localScale = new Vector3(h_offset * scaleMultiplier, Height * scaleMultiplier, 1);
		spriteRenderer_Right.transform.localScale = new Vector3(width_side * scaleMultiplier, Height * scaleMultiplier);
		spriteRenderer_Left.transform.localScale = new Vector3(width_side * scaleMultiplier, Height * scaleMultiplier);

		spriteRenderer_Middel.transform.localPosition = new Vector3(0, 0, 0);
		spriteRenderer_Right.transform.localPosition = new Vector3(h_offset * scaleMultiplier, 0, 0);
		spriteRenderer_Left.transform.localPosition = new Vector3(-h_offset * scaleMultiplier, 0, 0);

		collider.offset = new Vector2(0, 0);
		collider.size = new Vector2(Width * scaleMultiplier, Height * scaleMultiplier);
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

		if (ColorIndex != -1 && ColorIndex == platformGenerator.BackgroundColorIndex) {
			platformColor.a = platformGenerator.alphaSelected;
			Disable();
		} else {
			platformColor.a = platformGenerator.alphaNotSelected;
			Enable();
		}

		if (spriteRenderer_Left != null) spriteRenderer_Left.color = platformColor;
		if (spriteRenderer_Middel != null) spriteRenderer_Middel.color = platformColor;
		if (spriteRenderer_Right != null) spriteRenderer_Right.color = platformColor;
	}

	public void setSpeed(float speed) {
		platformController.move = new Vector3(-speed, platformController.move.y, 0);
	}

	public void setColorChanger(ColorChanger colChanger) {
		colorChanger = colChanger;
		colorChanger.transform.parent = transform;
		colorChanger.transform.SetAsFirstSibling();
	}

	void drawBoundingBox() {
		Rect rect = new Rect(transform.position, Width, Height);
		rect.draw(Color.yellow);
	}

	public void Enable() {
		collider.enabled = true;
		spriteRenderer_Left.enabled = true;
		spriteRenderer_Middel.enabled = true;
		spriteRenderer_Right.enabled = true;
	}

	public void Disable() {
		if (colorChanger != null) colorChanger.Die();
		colorChanger = null;
		collider.enabled = false;
		spriteRenderer_Left.enabled = false;
		spriteRenderer_Middel.enabled = false;
		spriteRenderer_Right.enabled = false;
	}

	public void Die() {
		platformGenerator.unregisterBlock(this);
		Destroy(gameObject);
	}

	public struct Rect
	{
		public Vector2 middle;
		public float width, height;
		public Rect(Vector2 mid, float w, float h) {
			middle = mid;
			width = w;
			height = h;
		}
		public Vector2 topLeft { get { return new Vector3(middle.x - width / 2, middle.y + height / 2); } }
		public Vector2 topRight { get { return new Vector3(middle.x + width / 2, middle.y + height / 2); } }
		public Vector2 bottomLeft { get { return new Vector3(middle.x - width / 2, middle.y - height / 2); } }
		public Vector2 bottomRight { get { return new Vector3(middle.x + width / 2, middle.y - height / 2); } }
		public void draw(Color col) {
			Debug.DrawLine(topLeft, topRight, col);
			Debug.DrawLine(topRight, bottomRight, col);
			Debug.DrawLine(bottomRight, bottomLeft, col);
			Debug.DrawLine(bottomLeft, topLeft, col);
		}
		public UnityEngine.Rect toRect() {
			Vector2 mid = middle;// Camera.main.WorldToScreenPoint(middle);

			return new UnityEngine.Rect(mid.x, mid.y, width, height);
		}
	}
}
