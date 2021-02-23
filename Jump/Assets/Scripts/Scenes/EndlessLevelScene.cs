using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndlessLevelScene : MonoBehaviour
{
	public static EndlessLevelScene Instance = null;

	public Button btnPause, btnRestart, btnContinue, btnHome;

	public GameObject pnlDeathScreen;
	Text btnPauseText;

	public Text textBoxScore, textBoxHighScore, textBoxScorePanel, textBoxHighScorePanel;

	private void Awake() {
		Instance = this;
	}

	// Start is called before the first frame update
	void Start() {
		btnPauseText = btnPause.GetComponentInChildren<Text>();
		btnPause.onClick.AddListener(onPauseClicked);

		btnRestart.onClick.AddListener(onRestartClicked);
		btnContinue.onClick.AddListener(onContinueClicked);
		btnHome.onClick.AddListener(onHomeClicked);

		init();
		GameManager.Instance.StartEndlessLevel();
	}

	// Update is called once per frame
	void Update() {
		textBoxScore.text = ScoreManager.Instance.playerScore.ToString();
		textBoxHighScore.text = "High Score:\n" + ScoreManager.Instance.highScore.ToString();
	}

	public void init() {
		textBoxScore.enabled = true;
		textBoxHighScore.enabled = true;
		textBoxScorePanel.text = "";
		textBoxHighScorePanel.text = "";
		hideDeathScreen();
	}

	public void PlayerDied() {
		textBoxScore.enabled = false;
		textBoxHighScore.enabled = false;

		textBoxHighScorePanel.text = "High Score:\n" + ScoreManager.Instance.highScore.ToString();
		textBoxScorePanel.text = "Player Score:\n" + ScoreManager.Instance.playerScore.ToString();
	}

	void onPauseClicked() {
		if (GameManager.Instance.isPaused) GameManager.Instance.ResumeLevel();
		else GameManager.Instance.PauseLevel();

		//btnPauseText.text = "Resume";
		//btnPauseText.text = "Pause";
		noFocus();
	}

	void onRestartClicked() {
		GameManager.Instance.RestartLevel();
		noFocus();
	}

	void onContinueClicked() {
		//must watch advertisement first
		GameManager.Instance.ContinueLevel();
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
