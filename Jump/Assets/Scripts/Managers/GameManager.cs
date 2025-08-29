using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance = null;

	public GameObject UnlimitedPlatformGeneratorPrefab;
	public GameObject FlatPlatformGeneratorPrefab;
	public GameObject PlayerPrefab;

	[HideInInspector] public bool isPaused;

	[HideInInspector] public PlatformGenerator platformGenerator;
	[HideInInspector] public Player player;
	[HideInInspector] public int PlayerSpriteIndex = Player.DefaultSpriteIndex;
	[HideInInspector] public int ColorPaletteIndex = 0; //TODO: ColorManager.DefaultColorPaletteIndex;

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

		if (MainMenuScene.Instance != null) {
			MainMenuScene.Instance.initScore();
		}
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
		SaveGameData();
	}

	public void colorPaletteIndexChanged(int newIndex) {
		ColorPaletteIndex = newIndex;

		SaveManager.Instance.state.ColorPaletteIndex = ColorPaletteIndex;
		SaveGameData();
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
		if (player.isAlive) {
			player.isAlive = false;
			PauseLevel();
			player.score.finalize();
			if (EndlessLevelScene.Instance != null) EndlessLevelScene.Instance.PlayerDied();
			SaveGameData();
			platformGenerator.onPlayerOutOfBounds(bottom);
			if (EndlessLevelScene.Instance != null) EndlessLevelScene.Instance.showDeathScreen();
		}
	}

	public void PauseLevel() {
		isPaused = true;
		Time.timeScale = 0;
	}

	public void ResumeLevel() {
		player.Reset();
		isPaused = false;
		Time.timeScale = 1;
	}

	public void MainMenuStartClicked() {
		//Debug.Log("Loading EndlessLevel");
		//SceneManager.LoadScene(sceneName: "EndlessLevel");
		SceneManager.LoadScene(sceneName: "FlatLevel");
		//Debug.Log("Loading EndlessLevel - done");
		MainMenuScene.Instance.inintPlayerModel();
	}

	public void MainMenuCharacterClicked() {
		//Debug.Log("Loading ShopMenu");
		SceneManager.LoadScene(sceneName: "CharacterMenu");
		//Debug.Log("Loading ShopMenu - done");
	}

	public void MainMenuColorPaletteClicked() {
		//Debug.Log("Loading ShopMenu");
		SceneManager.LoadScene(sceneName: "ColorPaletteMenu");
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
		SaveManager.Instance.state.ColorPaletteIndex = ColorPaletteIndex;
		SaveManager.Instance.Save();
	}

	void LoadGameData() {
		SaveManager.Instance.Load();
		ScoreManager.Instance.LoadHighScore(SaveManager.Instance.state);
		PlayerSpriteIndex = SaveManager.Instance.state.PlayerSpriteIndex;
		ColorPaletteIndex = SaveManager.Instance.state.ColorPaletteIndex;

		if (MainMenuScene.Instance != null) MainMenuScene.Instance.initAll();
	}

	public void ResetPlayerData() {
		SaveManager.Instance.ResetData();
		LoadGameData();
	}


	// ======================= Flat Level

	public void StartFlatLevel() {
		initPlayer();
		initFlatPlatformGenerator();

		ScoreManager.Instance.init(player);
		SaveGameData();
		LoadGameData();

		if (FlatLevelScene.Instance != null) FlatLevelScene.Instance.hideDeathScreen();
		RestartFlatLevel();
	}

	void initFlatPlatformGenerator() {
		GameObject platformGen = Instantiate(FlatPlatformGeneratorPrefab, Vector3.zero, Quaternion.identity);
		platformGenerator = platformGen.GetComponent<FlatPlatformGenerator>();
		platformGenerator.player = player;
	}
	public void RestartFlatLevel() {
		ResumeLevel();
		if (FlatLevelScene.Instance != null) FlatLevelScene.Instance.init();
		platformGenerator.restartGeneration();
	}

	public void ContinueFlatLevel() {
		platformGenerator.continueGeneration();
		ScoreManager.Instance.ContinueLevel();
		ResumeLevel();
		if (FlatLevelScene.Instance != null) FlatLevelScene.Instance.init();
	}
}
