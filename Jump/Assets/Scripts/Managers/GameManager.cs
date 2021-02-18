using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public GameObject UnlimitedPlatformGeneratorPrefab;
	public GameObject PlayerPrefab;

	public GameObject pnlDeathScreen;


	public Button btnPause, btnRestart, btnContinue, btnHome;
	Text btnPauseText;

	bool isPaused;

	[HideInInspector] public PlatformGenerator platformGenerator;
	[HideInInspector] public Player player;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		Instance = this;
	}

	void Start() {
		btnPauseText = btnPause.GetComponentInChildren<Text>();
		btnPause.onClick.AddListener(onPauseClicked);

		btnRestart.onClick.AddListener(onRestartClicked);
		btnContinue.onClick.AddListener(onContinueClicked);
		btnHome.onClick.AddListener(onHomeClicked);


		initPlayer();

		ScoreManager.Instance.init(player);
		LoadGameData();

		initPlatformGenerator();

		hideDeathScreen();
		RestartLevel();
	}

	void Update() {

	}

	void initPlayer() {
		GameObject playerGO = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
		player = playerGO.GetComponent<Player>();
		player.GetComponent<PlayerController2D>().player = player;
		player.manager = this;
	}

	void initPlatformGenerator() {
		GameObject platformGen = Instantiate(UnlimitedPlatformGeneratorPrefab, Vector3.zero, Quaternion.identity);
		platformGenerator = platformGen.GetComponent<UnlimitedPlatformGenerator>();
		platformGenerator.player = player;

	}

	void RestartLevel() {
		ScoreManager.Instance.RestartLevel();
		ResumeLevel();
		hideDeathScreen();
		platformGenerator.restartGeneration();
	}

	void ContinueLevel() {
		platformGenerator.continueGeneration();
		ScoreManager.Instance.ContinueLevel();
		ResumeLevel();
		hideDeathScreen();
	}

	public void PlayerDied(bool bottom) {
		PauseLevel();
		player.score.finalize();
		SaveGameData();
		platformGenerator.onPlayerOutOfBounds(bottom);
		showDeathScreen();
		//ContinueLevel();
	}

	void PauseLevel() {
		isPaused = true;
		Time.timeScale = 0;
		btnPauseText.text = "Resume";
	}

	void ResumeLevel() {
		isPaused = false;
		Time.timeScale = 1;
		btnPauseText.text = "Pause";
	}

	void onPauseClicked() {
		if (isPaused) ResumeLevel();
		else PauseLevel();

		noFocus();
	}

	void onRestartClicked() {
		RestartLevel();
		noFocus();
	}

	void onContinueClicked() {
		//must watch advertisement first
		ContinueLevel();
		noFocus();
	}


	void onHomeClicked() {
		//GotToHome();
		noFocus();
	}

	void SaveGameData() {
		SaveManager.Instance.state = ScoreManager.Instance.highScore.getState();
		SaveManager.Instance.Save();
	}

	void LoadGameData() {
		SaveManager.Instance.Load();
		ScoreManager.Instance.LoadHighScore(SaveManager.Instance.state);
	}

	void noFocus() => EventSystem.current.SetSelectedGameObject(null);

	void showDeathScreen() => pnlDeathScreen.SetActive(true);
	void hideDeathScreen() => pnlDeathScreen.SetActive(false);

}
