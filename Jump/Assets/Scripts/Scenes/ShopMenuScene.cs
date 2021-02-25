using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuScene : MonoBehaviour
{
	public static ShopMenuScene Instance = null;

	public Button btnBack;
	public GameObject ShopDisplayPrefab;
	public Vector3 DisplayPlayerModelPosition;
	public Vector3 DisplayPlayerModelScale;

	//public Transform PlayerModelDisplayStart;

	public float displayOffsetX = 0;
	public float displayOffsetY = 0;
	public float displayIntervalDistance = 210;

	const string PlayerModelDisplayType = "PlayerModel";

	List<GameObject> playerModelDisplays;

	private void Awake() {
		Instance = this;
	}

	void Start() {
		btnBack.onClick.AddListener(onBackClicked);

		//SpriteModelManager.SpritePlayer[] playerSprites = SpriteModelManager.Instance.getSprites();
		GameObject[] playerSprites = SpriteModelManager.Instance.PlayerModelPrefabs;
		playerModelDisplays = new List<GameObject>();

		Vector3 displayPosition = new Vector3(displayOffsetX, displayOffsetY, 0);
		Vector3 interval = new Vector3(displayIntervalDistance, 0, 0);

		for (int i = 0; i < playerSprites.Length; ++i) {

			//if (!playerSprites[i].isNull()) {
			GameObject display = Instantiate(ShopDisplayPrefab, displayPosition, Quaternion.identity);
			display.GetComponent<ShopDisplay>().index = i;
			display.GetComponent<ShopDisplay>().type = PlayerModelDisplayType;

			//display.GetComponent<Image>().sprite = playerSprites[i].body;

			GameObject sprite = Instantiate(playerSprites[i], new Vector3(), Quaternion.identity);
			sprite.transform.parent = display.transform;
			sprite.transform.localPosition = new Vector3();
			sprite.transform.localScale = DisplayPlayerModelScale;

			displayPosition = displayPosition + interval;
			//}

			playerModelDisplays.Add(display);
		}
		UpdateSelectedPlayerModel();

	}

	void Update() {

	}

	public void init() {
	}

	public void ShopDisplayClicked(ShopDisplay shopDisplay) {
		GameManager.Instance.playerSpriteIndexChanged(shopDisplay.index);
		Debug.Log("Player Sprite Index - " + GameManager.Instance.PlayerSpriteIndex);
		UpdateSelectedPlayerModel();
	}

	void onBackClicked() {
		GameManager.Instance.GotToHome();
	}

	void UpdateSelectedPlayerModel() {
		foreach (GameObject go in playerModelDisplays) go.GetComponent<ShopDisplay>().indexColorUpdate();
	}

}
