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

	private void Awake() {
		Instance = this;
	}

	void Start() {
		btnStart.onClick.AddListener(onStartClicked);
	}


	void Update() {

	}

	public void init() {

		Debug.Log("MM init");

		highScoreText.text = "High Score:\n" + ScoreManager.Instance.highScore.ToString();
	}

	void onStartClicked() {
		GameManager.Instance.MainMenuStartClicked();
	}

}
