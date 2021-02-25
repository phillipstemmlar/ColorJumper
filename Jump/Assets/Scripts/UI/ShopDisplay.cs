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

	private void Awake() {
		SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>();

		foreach (SpriteRenderer rend in rends) {
			if (rend.gameObject.tag == "Block") {
				background = rend;
				break;
			}
		}

	}

	private void OnMouseDown() {
		ShopMenuScene.Instance.ShopDisplayClicked(this);
	}

	public void indexColorUpdate() {
		selected = (GameManager.Instance.PlayerSpriteIndex == index);
		background.color = selected ? selectedColor : normalColor;
	}
}
