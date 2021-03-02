using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuScene : MonoBehaviour
{
	public static CharacterMenuScene Instance = null;

	public Button btnBack;
	public GameObject playerModelList;

	public float displayOffsetX = 0;
	public float displayOffsetY = 0;
	public float displayIntervalDistance = 210;

	public static string PlayerModelDisplayType = "character";

	List<GameObject> playerModelDisplays;

	private void Awake() {
		Instance = this;
	}

	void Start() {
		btnBack.onClick.AddListener(onBackClicked);

		ShopDisplay[] displays = playerModelList.GetComponentsInChildren<ShopDisplay>();

		playerModelDisplays = new List<GameObject>();
		foreach (ShopDisplay disp in displays) playerModelDisplays.Add(disp.gameObject);

		UpdateSelectedPlayerModel();

	}

	void Update() {

	}

	public void init() {
	}

	public void characterDisplayClicked(ShopDisplay shopDisplay) {
		GameManager.Instance.playerSpriteIndexChanged(shopDisplay.index);
		Debug.Log("Player Sprite Index - " + GameManager.Instance.PlayerSpriteIndex);
		UpdateSelectedPlayerModel();
	}

	void onBackClicked() {
		GameManager.Instance.GotToHome();
	}

	void UpdateSelectedPlayerModel() {
		foreach (GameObject go in playerModelDisplays) go.GetComponent<ShopDisplay>().indexUpdate(GameManager.Instance.PlayerSpriteIndex);
	}

}
