﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class PlatformGenerator : MonoBehaviour
{
	public Player player;
	public Vector2 PlayerPosition = new Vector2(-7f, 3f);

	public GameObject PlatformPrefab;
	public HashSet<Platform> platforms;

	[HideInInspector]
	public Color[] colors;

	public float alphaNotSelected = 1f;
	public float alphaSelected = 0.1f;

	public float additionalSpawnChange = 0.5f;

	public bool isDrawingScreenBorder = false;
	public bool isDrawingPlatformRect = false;

	[HideInInspector]
	public float CameraHeight, CameraWidth;

	public Vector3 RightOfScreen { get { return Camera.main.transform.position + new Vector3(CameraWidth / 2 + HorizontalOffest, 0, 0); } }
	public Vector3 LeftOfScreen { get { return Camera.main.transform.position - new Vector3(CameraWidth / 2 + HorizontalOffest, 0, 0); } }
	public Vector3 TopOfScreen { get { return Camera.main.transform.position + new Vector3(0, CameraHeight / 2 + VerticalOffset, 0); } }
	public Vector3 BottomOfScreen { get { return Camera.main.transform.position - new Vector3(0, CameraHeight / 2 + VerticalOffset, 0); } }

	public float HorizontalOffest = 1f;
	public float VerticalOffset = 2;

	public float blockWidth = 4;
	public float blockHeight = 1.33f;
	public float scaleMultiplier = 100f;

	public float intervalDistance = 0f;
	public float platformSpeed = 5f;
	float generationInterval;

	protected int platformHeightsCount;
	int previousPlatformHeightIndex;

	protected bool generating { get; private set; }
	bool finishedGenerating = false;
	float time;

	public Type type { get; protected set; }

	[HideInInspector]
	public int platformIndex;

	[HideInInspector]
	public int BackgroundColorIndex;
	Color BackgroundColor {
		get { return (BackgroundColorIndex < 0 || BackgroundColorIndex >= colors.Length) ? Color.white : colors[BackgroundColorIndex]; }
	}
	public int colorPatternMax = 2;
	int colorPatternCount = 0;
	int previousColorIndex = -1;

	protected abstract void init();
	protected abstract void onGenerationInterval();
	protected abstract void onGeneratingFinished();
	protected abstract void generateNextPlatform();
	public abstract void onPlayerOutOfBounds(bool bottom);
	protected abstract Color[] createColorsArray();
	protected virtual void calculateInterval() => generationInterval = (blockWidth + intervalDistance) / (platformSpeed);

	void Start() {
		type = Type.none;
		transform.position = RightOfScreen;
		calculateCameraDimentions();
		colors = createColorsArray();
		calculateInterval();
		calculateHeightValues();
		init();
		startGeneration();
	}

	void FixedUpdate() {
		if (!finishedGenerating) {
			time += Time.deltaTime;
			if (time >= generationInterval) {
				time = 0;
				if (generating) onGenerationInterval();
			}
			if (!generating) {
				onGeneratingFinished();
				if (!generating) finishedGenerating = true;
			}
		}
	}

	void configurePlayer() {
		player.setPlatformGenerator(this);
		player.setXY(PlayerPosition);
		player.Reset();
	}

	public void startGeneration() {
		configurePlayer();
		generating = true;
		finishedGenerating = false;
		time = generationInterval;
		platformIndex = 0;
		changeBackgroundColor(randomColorIndex());

		if (platforms != null) {
			Platform[] plats = platforms.ToArray<Platform>();
			for (int i = 0; i < plats.Length; ++i) if (plats[i] != null) plats[i].Die();
		}
		platforms = new HashSet<Platform>();
		CreateInitialPlatform();
	}

	public void restartGeneration() {
		print("Restarting");
		startGeneration();
	}
	public void stopGenerating() => generating = false;

	void CreatePlatform(Vector3 pos, float width, bool initial, int ColorIndex) {
		GameObject plat = Instantiate(PlatformPrefab, pos, Quaternion.identity);
		Platform platform = plat.GetComponent<Platform>();
		platform.platformGenerator = this;
		platform.isDrawingBoundingBox = isDrawingPlatformRect;
		platform.isInitial = initial;
		platform.Width = width;
		platform.Height = blockHeight;
		platform.setSpeed(platformSpeed);
		platform.changeColor(ColorIndex);
		platformIndex++;
	}
	protected void CreatePlatform(Vector3 topLeft) {
		int heightIndex = getHeightIndex(topLeft.y);
		int maxJumpHeightIndex = Mathf.FloorToInt(player.maxJumpHeight / blockHeight) - 1;
		int colorIndex = randomColorIndexPattern();

		if (heightIndex - previousPlatformHeightIndex > maxJumpHeightIndex) {
			if (colorIndex == BackgroundColorIndex) {
				if (colorPatternCount == 1) colorPatternCount++;
				else if (colorPatternCount > 1) {
					previousPlatformHeightIndex = previousPlatformHeightIndex + maxJumpHeightIndex;
					topLeft.y = getHeight(previousPlatformHeightIndex);
					colorIndex = randomColorIndexNot();
				}
			} else {
				previousPlatformHeightIndex = previousPlatformHeightIndex + maxJumpHeightIndex;
				topLeft.y = getHeight(previousPlatformHeightIndex);
			}
		} else if (colorIndex != BackgroundColorIndex) previousPlatformHeightIndex = heightIndex;

		CreatePlatform(new Vector3(topLeft.x + blockWidth / 2, topLeft.y - blockHeight / 2), blockWidth, false, colorIndex);
	}
	void CreateInitialPlatform() {
		float midHeight = getMiddleHeight();
		previousPlatformHeightIndex = getHeightIndex(midHeight);
		colorPatternCount = 1;
		CreatePlatform(new Vector3(0, midHeight - blockHeight / 2), CameraWidth, true, randomColorIndexNot()); ;
	}

	public void changeBackgroundColor(int colIndex) {
		if (colIndex < 0 || colIndex >= colors.Length) {
			colIndex = -1;
		}
		BackgroundColorIndex = colIndex;
		updateBackgroundColor();
	}

	void updateBackgroundColor() {
		Camera.main.backgroundColor = BackgroundColor;
	}

	public void updatePlatformColors() {
		foreach (Platform plat in platforms) if (plat != null) plat.changeColor(plat.ColorIndex);
	}

	public Color randomColor() => colors[Random.Range(0, colors.Length)];
	public int randomColorIndex() => Random.Range(0, colors.Length);
	public int randomColorIndexNot() {
		int index = -1;
		do {
			index = Random.Range(0, colors.Length);
		} while (index == BackgroundColorIndex);
		return index;
	}
	public int randomColorIndexPattern() {
		int index;
		if (previousColorIndex == BackgroundColorIndex && colorPatternCount >= colorPatternMax) {
			do {
				index = Random.Range(0, colors.Length);
			} while (index == BackgroundColorIndex);
		} else {
			index = Random.Range(0, Mathf.FloorToInt(colors.Length * (1f + additionalSpawnChange)));
			if (index >= colors.Length) index = BackgroundColorIndex;
		}
		if (index != previousColorIndex) colorPatternCount = 0;
		colorPatternCount++;
		previousColorIndex = index;
		return index;
	}
	public Color randomColorRange() => new Color(Random.value, Random.value, Random.value);

	public void registerBlock(Platform platform) => platforms.Add(platform);
	public void unregisterBlock(Platform platform) => platforms.Remove(platform);

	void calculateCameraDimentions() {
		CameraHeight = 2 * Camera.main.orthographicSize;
		CameraWidth = CameraHeight * Camera.main.aspect;
	}
	void calculateHeightValues() {
		platformHeightsCount = Mathf.FloorToInt(CameraHeight / blockHeight) - 2;
	}
	protected float getHeight(int heightRowIndex) => (heightRowIndex % platformHeightsCount) * blockHeight - CameraHeight / 2 + blockHeight;
	protected int getHeightIndex(float height) => Mathf.FloorToInt((height + CameraHeight / 2 - blockHeight) / blockHeight);
	protected float getMiddleHeight() => getHeight(Mathf.FloorToInt((platformHeightsCount - 1) / 2));


	private void OnDrawGizmos() {
		calculateCameraDimentions();
		if (isDrawingScreenBorder) drawScreenBorder();

		//Debug.DrawLine(Camera.main.transform.position + new Vector3(0, 2, 0), new Vector3(CameraWidth / 2 + screenOffest, 2, 0), Color.magenta);
		//Debug.DrawLine(Camera.main.transform.position + new Vector3(0, 1, 0), new Vector3(CameraWidth / 2, 1, 0), Color.magenta);

		Debug.DrawLine(RightOfScreen + new Vector3(0, CameraHeight / 2, 0), RightOfScreen + new Vector3(0, -CameraHeight / 2, 0), Color.magenta);
		Debug.DrawLine(LeftOfScreen + new Vector3(0, CameraHeight / 2, 0), LeftOfScreen + new Vector3(0, -CameraHeight / 2, 0), Color.magenta);

		Debug.DrawLine(TopOfScreen + new Vector3(CameraWidth / 2, 0, 0), TopOfScreen + new Vector3(-CameraWidth / 2, 0, 0), Color.magenta);
		Debug.DrawLine(BottomOfScreen + new Vector3(CameraWidth / 2, 0, 0), BottomOfScreen + new Vector3(-CameraWidth / 2, 0, 0), Color.magenta);

		//print("Camera W: " + CameraWidth + " H: " + CameraHeight);
		//print("Platform W: " + blockWidth + " H: " + blockHeight);
		//print("Scaled W: " + (blockWidth * scaleMultiplier) + " H: " + (blockHeight * scaleMultiplier));

		if (!EditorApplication.isPlaying && player != null) player.setXY(PlayerPosition);
	}

	void drawScreenBorder() {
		Rect screen = new Rect(Camera.main.transform.position, CameraWidth, CameraHeight);
		screen.draw(Color.blue);
	}

	public enum Type
	{
		none, unlimited, indexed, count, custom
	};

	public struct Rect
	{
		public Vector2 middle;
		public float width, height;
		public Rect(Vector2 mid, float w, float h) {
			middle = mid;
			width = w;
			height = h;
		}
		public Vector2 topLeft { get { return new Vector3(middle.x - width / 2, middle.y + height / 2); } }
		public Vector2 topRight { get { return new Vector3(middle.x + width / 2, middle.y + height / 2); } }
		public Vector2 bottomLeft { get { return new Vector3(middle.x - width / 2, middle.y - height / 2); } }
		public Vector2 bottomRight { get { return new Vector3(middle.x + width / 2, middle.y - height / 2); } }
		public void draw(Color col) {
			Debug.DrawLine(topLeft, topRight, col);
			Debug.DrawLine(topRight, bottomRight, col);
			Debug.DrawLine(bottomRight, bottomLeft, col);
			Debug.DrawLine(bottomLeft, topLeft, col);
		}
	}



}