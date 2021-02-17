using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

	public GameObject UnlimitedPlatformGeneratorPrefab;
	public GameObject PlayerPrefab;

	public ScoreManager scoreManager;
	public GameObject pnlDeathScreen;


	public Button btnPause, btnRestart, btnContinue, btnHome;
	Text btnPauseText;

	bool isPaused;

	[HideInInspector] public PlatformGenerator platformGenerator;
	[HideInInspector] public Player player;

	void Start() {
		btnPauseText = btnPause.GetComponentInChildren<Text>();
		btnPause.onClick.AddListener(onPauseClicked);

		btnRestart.onClick.AddListener(onRestartClicked);
		btnContinue.onClick.AddListener(onContinueClicked);
		btnHome.onClick.AddListener(onHomeClicked);


		initPlayer();
		scoreManager.init(player);
		initPlatformGenerator();

		hideDeathScreen();
		ResumeLevel();
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
		platformGenerator.restartGeneration();
		scoreManager.RestartLevel();
		ResumeLevel();
		hideDeathScreen();
	}

	void ContinueLevel() {
		platformGenerator.continueGeneration();
		scoreManager.ContinueLevel();
		ResumeLevel();
		hideDeathScreen();
	}

	public void PlayerDied(bool bottom) {
		PauseLevel();
		player.score.finalize();
		platformGenerator.onPlayerOutOfBounds(bottom);
		showDeathScreen();
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


	void noFocus() => EventSystem.current.SetSelectedGameObject(null);

	void showDeathScreen() => pnlDeathScreen.SetActive(true);
	void hideDeathScreen() => pnlDeathScreen.SetActive(false);

}
