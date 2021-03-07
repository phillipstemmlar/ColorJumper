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

	public Vector3 playerScoreOffset;
	public Vector3 highscoreTextOffset;


	Player player = null;
	Transform HighScoreFlagTransform = null;


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
		GameManager.Instance.StartEndlessLevel();
		player = GameManager.Instance.player;
	}

	void Update() {

		if (HighScoreFlagTransform != null) {
			textBoxHighScore.gameObject.SetActive(true);
			textBoxHighScore.transform.position = HighScoreFlagTransform.position + highscoreTextOffset;
			//textBoxHighScore.text = Mathf.RoundToInt(ScoreManager.Instance.highScore.distance).ToString();
		}

		if (textBoxHighScore.transform.position.x <= GameManager.Instance.platformGenerator.LeftOfScreen.x) {
			textBoxHighScore.gameObject.SetActive(false);
			textBoxHighScore = null;
		}

		textBoxScore.text = Mathf.RoundToInt(ScoreManager.Instance.playerScore.distance).ToString();
	}

	public void onHighScoreText(Transform highScoreFlag) {
		HighScoreFlagTransform = highScoreFlag;
	}

	public void onHighScorePassed() {
		HighScoreFlagTransform = null;
		textBoxHighScore.gameObject.SetActive(false);
	}

	public void init() {
		textBoxScore.transform.parent.gameObject.SetActive(true);
		btnPause.gameObject.SetActive(true);

		textBoxScorePanel.text = "";
		textBoxHighScorePanel.text = "";
		hideDeathScreen();
		HighScoreFlagTransform = null;
		textBoxHighScore.gameObject.SetActive(false);
	}

	public void PlayerDied() {
		textBoxScore.transform.parent.gameObject.SetActive(false);
		btnPause.gameObject.SetActive(false);

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
