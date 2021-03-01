﻿using System.Collections;
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
	public Button btnSettings;
	public Button btnLeaderboard;

	public Canvas canvas;

	GameObject playerSprite = null;

	private void Awake() {
		Instance = this;
	}

	void Start() {
		btnStart.onClick.AddListener(onStartClicked);
		btnShop.onClick.AddListener(onShopClicked);
		//Debug.Log("Player Sprite Index: " + GameManager.Instance.PlayerSpriteIndex);
		inintPlayerModel();
		//initScore();
	}


	void Update() {

	}

	public void initScore() {
		highScoreText.text = "High Score:\n" + ScoreManager.Instance.highScore.ToString();
	}

	public void inintPlayerModel() {
		if (playerSprite != null) Destroy(playerSprite);

		Vector3 pos = Camera.main.ScreenToWorldPoint(btnStart.transform.position);
		pos.z = 0;

		playerSprite = Instantiate(SpriteModelManager.Instance.getPlayerModelPrefab(), pos, Quaternion.identity);
		playerSprite.transform.localScale = new Vector3(7f, 7f, 1f);

	}

	void onStartClicked() {
		GameManager.Instance.MainMenuStartClicked();
	}
	void onShopClicked() {
		Debug.Log("Player Sprite Index - " + GameManager.Instance.PlayerSpriteIndex);
		GameManager.Instance.MainMenuShopClicked();
	}
}
