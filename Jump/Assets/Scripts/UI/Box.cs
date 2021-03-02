using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{

	public Color BorderColor;
	public float BorderThickness = 1;
	public Vector2 size;
	public float PixelsPerUnit = 100;

	Transform Left = null;
	Transform Right = null;
	Transform Top = null;
	Transform Bottom = null;

	SpriteRenderer[] walls = null;

	Platform.Rect rect;

	float MaxBorderThickness = 4;
	float MinBorderThickness = 0;

	void init() {
		rect = new Platform.Rect(transform.position, size.x, size.y);

		Transform[] children = GetComponentsInChildren<Transform>();
		walls = GetComponentsInChildren<SpriteRenderer>();

		foreach (Transform child in children) {
			if (child.tag == "left") Left = child;
			if (child.tag == "right") Right = child;
			if (child.tag == "top") Top = child;
			if (child.tag == "bottom") Bottom = child;
		}
	}

	void Awake() {
		init();
	}

	void Update() {
		updateRect();
		updateChildren();
		updateChildrenColor();
	}

	void updateRect() {
		rect.middle = transform.position;
		rect.width = size.x;
		rect.height = size.y;
	}

	void updateChildren() {

		if (BorderThickness < MinBorderThickness) BorderThickness = MinBorderThickness;
		if (BorderThickness > MaxBorderThickness) BorderThickness = MaxBorderThickness;

		float thicknessOffset = BorderThickness / PixelsPerUnit / 2;

		Top.localScale = new Vector3(rect.width * PixelsPerUnit, BorderThickness, 1f);
		Bottom.localScale = new Vector3(rect.width * PixelsPerUnit, BorderThickness, 1f);

		Left.localScale = new Vector3(BorderThickness, rect.height * PixelsPerUnit, 1f);
		Right.localScale = new Vector3(BorderThickness, rect.height * PixelsPerUnit, 1f);

		Top.localPosition = new Vector3(0, rect.height / 2 - thicknessOffset, 0);
		Bottom.localPosition = new Vector3(0, -rect.height / 2 + thicknessOffset, 0);

		Left.localPosition = new Vector3(-rect.width / 2 + thicknessOffset, 0, 0);
		Right.localPosition = new Vector3(rect.width / 2 - thicknessOffset, 0, 0);
	}

	void updateChildrenColor() {
		if (walls != null) {
			foreach (SpriteRenderer wall in walls) wall.color = BorderColor;
		}
	}

	public void setRect(Platform.Rect newRect) {
		transform.position = newRect.middle;
		size = new Vector2(newRect.width, newRect.height);
	}

	public void setBorderColor(Color color) {
		BorderColor = color;
	}

}
