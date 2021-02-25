using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteModelManager : MonoBehaviour
{
	public static SpriteModelManager Instance = null;
	public GameObject[] PlayerModelPrefabs;
	public Sprite[] PlayerModelHeads;
	public Sprite[] PlayerModelBodies;
	//public SpritePlayer test;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		if (Instance == null) {
			Instance = this;
		} else {
			Object.Destroy(gameObject);
		}
	}

	public SpritePlayer[] getSprites() {
		List<SpritePlayer> sprites = new List<SpritePlayer>();

		for (int i = 0; i < PlayerModelBodies.Length; ++i) {
			Sprite body = (i < PlayerModelBodies.Length) ? PlayerModelBodies[i] : null;
			Sprite head = (i < PlayerModelHeads.Length) ? PlayerModelHeads[i] : null;

			sprites.Add(new SpritePlayer(body, head));
		}


		return sprites.ToArray();
	}

	public class SpritePlayer
	{
		public Sprite body = null;
		public Sprite head = null;

		public SpritePlayer(Sprite _body, Sprite _head = null) {
			body = _body;
			head = _head;
		}

		public bool isNull() => body == null && head == null;

	}

}
