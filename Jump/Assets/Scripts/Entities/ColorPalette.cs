using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{

	public int PrefabIndex = -1;

	public Color Color1;
	public Color Color2;
	public Color Color3;
	public Color Color4;
	public Color Color5;

	public Color[] colors;

	ColorPaletteItem[] items;

	static int MinItemIndex = 0;
	static int MaxItemIndex = 4;

	void Awake() {
		updateColors();

	}

	private void OnDrawGizmos() {
		updateColors();
	}

	void updateColors() {
		colors = new Color[5];
		colors[0] = Color1;
		colors[1] = Color2;
		colors[2] = Color3;
		colors[3] = Color4;
		colors[4] = Color5;

		items = GetComponentsInChildren<ColorPaletteItem>();

		foreach (ColorPaletteItem item in items) {

			if (item.itemIndex < MinItemIndex) item.itemIndex = MinItemIndex;
			if (item.itemIndex > MaxItemIndex) item.itemIndex = MaxItemIndex;

			item.GetComponent<SpriteRenderer>().color = colors[item.itemIndex];
		}
	}

	void Update() {

	}
}
