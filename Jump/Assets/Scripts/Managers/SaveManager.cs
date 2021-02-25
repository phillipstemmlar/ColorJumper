using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class SaveManager : MonoBehaviour
{
	public static SaveManager Instance = null;
	public SaveState state;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		if (Instance == null) {
			Instance = this;
		} else {
			Object.Destroy(gameObject);
		}
	}

	public void Save() {
		Debug.Log(SaveHelper.Serialize<SaveState>(state));
		PlayerPrefs.SetString("save", SaveHelper.Serialize<SaveState>(state));
	}

	public void Load() {
		if (PlayerPrefs.HasKey("save")) state = SaveHelper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
		else {
			state = new SaveState();
			Save();
		}
		Debug.Log(SaveHelper.Serialize<SaveState>(state));
	}


	public class SaveState
	{
		public float distance = 0;
		public int jumps = 0;
		public int colorChagnes = 0;
		public int PlayerSpriteIndex = 0;
	}
}

public static class SaveHelper
{
	public static string Serialize<T>(this T toSerialize) {
		XmlSerializer xml = new XmlSerializer(typeof(T));
		StringWriter writer = new StringWriter();
		xml.Serialize(writer, toSerialize);
		return writer.ToString();
	}
	public static T Deserialize<T>(this string toDeserialize) {
		XmlSerializer xml = new XmlSerializer(typeof(T));
		StringReader reader = new StringReader(toDeserialize);
		return (T)xml.Deserialize(reader);
	}
}
