using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteModelManager : MonoBehaviour
{
	public static SpriteModelManager Instance = null;

	public GameObject[] PlayerModelPrefabs;
	public GameObject[] ColorPalettePrefabs;

	//public SpritePlayer test;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		if (Instance == null) {
			Instance = this;
		} else {
			Object.Destroy(gameObject);
		}
	}

	public GameObject getPlayerModelPrefab() {
		return PlayerModelPrefabs[GameManager.Instance.PlayerSpriteIndex];
	}

	public GameObject getColorPalettePrefab() {
		return ColorPalettePrefabs[GameManager.Instance.ColorPaletteIndex];
	}

}
