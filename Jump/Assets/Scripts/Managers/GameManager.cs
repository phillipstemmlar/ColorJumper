using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance = null;

	public GameObject UnlimitedPlatformGeneratorPrefab;
	public GameObject PlayerPrefab;

	[HideInInspector] public bool isPaused;

	[HideInInspector] public PlatformGenerator platformGenerator;
	[HideInInspector] public Player player;
	[HideInInspector] public int PlayerSpriteIndex = Player.DefaultSpriteIndex;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		if (Instance == null) {
			Instance = this;
		} else {
			Object.Destroy(gameObject);
		}
	}

	void Start() {
		LoadGameData();
		MainMenuScene.Instance.initScore();
	}

	void Update() {

	}

	public void StartEndlessLevel() {
		initPlayer();
		initPlatformGenerator();

		ScoreManager.Instance.init(player);
		SaveGameData();
		LoadGameData();

		if (EndlessLevelScene.Instance != null) EndlessLevelScene.Instance.hideDeathScreen();
		RestartLevel();
	}

	void initPlayer() {
		GameObject playerGO = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
		player = playerGO.GetComponent<Player>();
		player.GetComponent<PlayerController2D>().player = player;
		player.manager = this;
		player.SpriteIndex = PlayerSpriteIndex;
		ScoreManager.Instance.player = player;
	}

	public void playerSpriteIndexChanged(int newIndex) {
		PlayerSpriteIndex = newIndex;

		SaveManager.Instance.state.PlayerSpriteIndex = PlayerSpriteIndex;

	}

	void initPlatformGenerator() {
		GameObject platformGen = Instantiate(UnlimitedPlatformGeneratorPrefab, Vector3.zero, Quaternion.identity);
		platformGenerator = platformGen.GetComponent<UnlimitedPlatformGenerator>();
		platformGenerator.player = player;

	}

	public void RestartLevel() {
		ScoreManager.Instance.RestartLevel();
		ResumeLevel();
		if (EndlessLevelScene.Instance != null) EndlessLevelScene.Instance.init();
		platformGenerator.restartGeneration();
	}

	public void ContinueLevel() {
		platformGenerator.continueGeneration();
		ScoreManager.Instance.ContinueLevel();
		ResumeLevel();
		if (EndlessLevelScene.Instance != null) EndlessLevelScene.Instance.init();
	}



	public void PlayerDied(bool bottom) {
		PauseLevel();
		player.score.finalize();
		EndlessLevelScene.Instance.PlayerDied();
		SaveGameData();
		platformGenerator.onPlayerOutOfBounds(bottom);
		if (EndlessLevelScene.Instance != null) EndlessLevelScene.Instance.showDeathScreen();
	}

	public void PauseLevel() {
		isPaused = true;
		Time.timeScale = 0;
	}

	public void ResumeLevel() {
		isPaused = false;
		Time.timeScale = 1;
	}

	public void MainMenuStartClicked() {
		//Debug.Log("Loading EndlessLevel");
		SceneManager.LoadScene(sceneName: "EndlessLevel");
		//Debug.Log("Loading EndlessLevel - done");
	}

	public void MainMenuShopClicked() {
		//Debug.Log("Loading ShopMenu");
		SceneManager.LoadScene(sceneName: "ShopMenu");
		//Debug.Log("Loading ShopMenu - done");
	}

	public void GotToHome() {
		//Debug.Log("Loading MainMenu");
		SceneManager.LoadScene(sceneName: "MainMenu");
		//Debug.Log("Loading MainMenu - done");
	}

	void SaveGameData() {
		SaveManager.Instance.state = ScoreManager.Instance.highScore.getState();
		SaveManager.Instance.state.PlayerSpriteIndex = PlayerSpriteIndex;
		SaveManager.Instance.Save();
	}

	void LoadGameData() {
		SaveManager.Instance.Load();
		ScoreManager.Instance.LoadHighScore(SaveManager.Instance.state);
		PlayerSpriteIndex = SaveManager.Instance.state.PlayerSpriteIndex;
	}
}
