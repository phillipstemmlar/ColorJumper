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
	public Button btnCharacter;
	public Button btnColorPalette;
	public Button btnSettings;
	public Button btnLeaderboard;

	public Button btnReset;

	public Canvas canvas;

	GameObject playerSprite = null;

	private void Awake() {
		Instance = this;
	}

	void Start() {
		btnStart.onClick.AddListener(onStartClicked);
		btnCharacter.onClick.AddListener(onCharacterClicked);
		btnColorPalette.onClick.AddListener(onColorPaletteClicked);
		btnReset.onClick.AddListener(onResetClicked);

		initAll();
	}


	void Update() {

	}

	public void initAll() {
		inintPlayerModel();
		initScore();
	}

	public void initScore() {
		if (ScoreManager.Instance != null) highScoreText.text = "High Score:\n" + ScoreManager.Instance.highScore.ToString();
	}

	public void inintPlayerModel() {
		if (playerSprite != null) Destroy(playerSprite);

		Debug.Log("player: " + GameManager.Instance.PlayerSpriteIndex);

		Vector3 pos = Camera.main.ScreenToWorldPoint(btnStart.transform.position);
		pos.z = 0;

		playerSprite = Instantiate(SpriteModelManager.Instance.getPlayerModelPrefab(), pos, Quaternion.identity);
		playerSprite.transform.localScale = new Vector3(7f, 7f, 1f);

	}

	void onStartClicked() {
		GameManager.Instance.MainMenuStartClicked();
	}
	void onCharacterClicked() {
		//Debug.Log("Player Sprite Index - " + GameManager.Instance.PlayerSpriteIndex);
		GameManager.Instance.MainMenuCharacterClicked();
	}

	void onColorPaletteClicked() {
		//Debug.Log("Player Sprite Index - " + GameManager.Instance.PlayerSpriteIndex);
		GameManager.Instance.MainMenuColorPaletteClicked();
	}

	void onResetClicked() {
		GameManager.Instance.ResetPlayerData();
	}

}
