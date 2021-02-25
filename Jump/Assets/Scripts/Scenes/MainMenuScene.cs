using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuScene : MonoBehaviour
{
	public static MainMenuScene Instance = null;

	public Text highScoreText;
	public Button btnStart;
	public Button btnShop;

	private void Awake() {
		Instance = this;
	}

	void Start() {
		btnStart.onClick.AddListener(onStartClicked);
		btnShop.onClick.AddListener(onShopClicked);
		Debug.Log("Player Sprite Index - " + GameManager.Instance.PlayerSpriteIndex);
	}


	void Update() {

	}

	public void init() {
		highScoreText.text = "High Score:\n" + ScoreManager.Instance.highScore.ToString();
	}

	void onStartClicked() {
		GameManager.Instance.MainMenuStartClicked();
	}
	void onShopClicked() {
		Debug.Log("Player Sprite Index - " + GameManager.Instance.PlayerSpriteIndex);
		GameManager.Instance.MainMenuShopClicked();
	}
}
