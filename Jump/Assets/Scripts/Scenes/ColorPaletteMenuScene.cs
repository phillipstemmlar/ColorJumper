using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPaletteMenuScene : MonoBehaviour
{
	public static ColorPaletteMenuScene Instance = null;

	public Button btnBack;
	public GameObject colorPaletteList;

	public float displayOffsetX = 0;
	public float displayOffsetY = 0;
	public float displayIntervalDistance = 210;

	public static string ColorPaletteDisplayType = "colorpalette";

	List<GameObject> colorPaletteDisplays;

	private void Awake() {
		Instance = this;
	}

	void Start() {
		btnBack.onClick.AddListener(onBackClicked);

		ShopDisplay[] displays = colorPaletteList.GetComponentsInChildren<ShopDisplay>();

		colorPaletteDisplays = new List<GameObject>();
		foreach (ShopDisplay disp in displays) colorPaletteDisplays.Add(disp.gameObject);

		UpdateSelectedColorPalette();

	}

	void Update() {

	}

	public void init() {
	}

	public void ColorPaletteDisplayClicked(ShopDisplay shopDisplay) {
		GameManager.Instance.colorPaletteIndexChanged(shopDisplay.index);
		Debug.Log("color Palette Index - " + GameManager.Instance.ColorPaletteIndex);
		UpdateSelectedColorPalette();
	}

	void onBackClicked() {
		GameManager.Instance.GotToHome();
	}

	void UpdateSelectedColorPalette() {
		Debug.Log("color Palette Index - " + GameManager.Instance.ColorPaletteIndex);
		foreach (GameObject go in colorPaletteDisplays) go.GetComponent<ShopDisplay>().indexUpdate(GameManager.Instance.ColorPaletteIndex);
	}

}
