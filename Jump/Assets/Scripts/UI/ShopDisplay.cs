using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDisplay : MonoBehaviour
{
	public int index = -1;
	public string type = "none";
	public bool selected = false;

	SpriteRenderer background;

	public Color normalColor;
	public Color selectedColor;

	public Texture texture;

	BoxCollider2D collider;
	Box box;

	[HideInInspector] public Platform.Rect rect;

	private void Awake() {
		collider = GetComponentInParent<BoxCollider2D>();
		box = GetComponentInChildren<Box>();

		SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>();

		foreach (SpriteRenderer rend in rends) {
			if (rend.gameObject.tag == "Block") {
				background = rend;
				break;
			}
		}

		Vector2 sizeWorld = collider.size;//transform.parent.TransformVector(collider.size);
		rect = new Platform.Rect(transform.position, sizeWorld.x, sizeWorld.y);
	}

	private void Update() {
		rect.middle = transform.position;
		box.setRect(rect);
		box.setBorderColor(selectedColor);

		box.gameObject.SetActive(selected);
	}

	private void OnMouseDown() {
		if (type == CharacterMenuScene.PlayerModelDisplayType) CharacterMenuScene.Instance.characterDisplayClicked(this);
		if (type == ColorPaletteMenuScene.ColorPaletteDisplayType) ColorPaletteMenuScene.Instance.ColorPaletteDisplayClicked(this);
	}

	public void indexUpdate(int selectedIndex) {
		selected = (selectedIndex == index);
	}

	void DrawQuad(Rect position, Color color) {
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, color);
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(position, GUIContent.none);
	}

	private void OnDrawGizmos() {
		//rect.draw(Color.green);
	}

}
