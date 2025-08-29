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

	public GameObject getPlayerModelPrefab(int spriteIndex) {
		return PlayerModelPrefabs[spriteIndex];
	}

	public GameObject getActivePlayerModelPrefab() {
		return getPlayerModelPrefab(GameManager.Instance.PlayerSpriteIndex);
	}

	public GameObject getColorPalettePrefab(int paletteIndex) {
		return ColorPalettePrefabs[paletteIndex];
	}

	public GameObject getActiveColorPalettePrefab() {
		return getColorPalettePrefab(GameManager.Instance.ColorPaletteIndex);
	}

}
