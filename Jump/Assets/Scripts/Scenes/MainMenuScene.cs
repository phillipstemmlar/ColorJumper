using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuScene : MonoBehaviour
{
	public Text highScoreText;
	public Button btnStart;

	void Start() {
		btnStart.onClick.AddListener(onStartClicked);
	}


	void Update() {

	}

	void onStartClicked() {
		GameManager.Instance.MainMenuStartClicked();
	}

}
