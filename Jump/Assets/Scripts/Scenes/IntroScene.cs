using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroScene : MonoBehaviour
{

	public VideoPlayer videoPlayer;

	public float FadeOutSeconds = 3; //seconds

	void Start() {
		StartCoroutine(WaitForVideo());
	}

	IEnumerator WaitForVideo() {
		yield return new WaitForSeconds((float)videoPlayer.clip.length - FadeOutSeconds);

		GameManager.Instance.GotToHome();
	}


}
