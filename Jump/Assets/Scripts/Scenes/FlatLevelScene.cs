using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlatLevelScene : MonoBehaviour
{
	public static FlatLevelScene Instance = null;

	public Button btnPause, btnRestart, btnContinue, btnHome;

	public GameObject pnlDeathScreen;
	Text btnPauseText;

	public Text textBoxScore, textBoxHighScore, ScoreText, OldTotalText, addTotalText, HighSoreText;

	public Vector3 playerScoreOffset;
	public Vector3 highscoreTextOffset;


	Player player = null;

	const string plus_sign = "+";

	private void Awake() {
		Instance = this;
	}

	void Start() {
		btnPauseText = btnPause.GetComponentInChildren<Text>();
		btnPause.onClick.AddListener(onPauseClicked);

		btnRestart.onClick.AddListener(onRestartClicked);
		btnContinue.onClick.AddListener(onContinueClicked);
		btnHome.onClick.AddListener(onHomeClicked);

		init();
		GameManager.Instance.StartFlatLevel();
		player = GameManager.Instance.player;
	}

	void Update() {
	}

	public void onHighScoreText(Transform highScoreFlag) {
		//HighScoreFlagTransform = highScoreFlag;
	}

	public void onHighScorePassed() {
		//HighScoreFlagTransform = null;
		//textBoxHighScore.gameObject.SetActive(false);
	}

	public void init() {
		textBoxScore.transform.parent.gameObject.SetActive(true);
		btnPause.gameObject.SetActive(true);

		hideDeathScreen();
	}

	public void PlayerDied() {
		textBoxScore.transform.parent.gameObject.SetActive(false);
		btnPause.gameObject.SetActive(false);
	}

	void onPauseClicked() {
		if (GameManager.Instance.isPaused) GameManager.Instance.ResumeLevel();
		else GameManager.Instance.PauseLevel();

		//btnPauseText.text = "Resume";
		//btnPauseText.text = "Pause";
		noFocus();
	}

	void onRestartClicked() {
		GameManager.Instance.RestartFlatLevel();
		noFocus();
	}

	void onContinueClicked() {
		//must watch advertisement first
		GameManager.Instance.ContinueFlatLevel();
		noFocus();
	}


	void onHomeClicked() {
		GameManager.Instance.GotToHome();
		noFocus();
	}

	public void showDeathScreen() => pnlDeathScreen.SetActive(true);
	public void hideDeathScreen() => pnlDeathScreen.SetActive(false);
	void noFocus() => EventSystem.current.SetSelectedGameObject(null);
}
