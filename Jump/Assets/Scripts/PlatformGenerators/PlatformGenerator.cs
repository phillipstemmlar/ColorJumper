using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class PlatformGenerator : MonoBehaviour
{
	public Player player;
	public Vector2 PlayerPosition = new Vector2(-7f, 3f);

	public GameObject PlatformPrefab;
	public GameObject ColorChangerPrefab;
	public HashSet<Platform> platforms;


	HashSet<ColorChanger> colorChangers;

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

	public float ColorChangerChance = 0.1f;
	public float ColorChangerOffset = -1f;

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


	public GameObject StartFlagPrefab;
	public float StartFlagEdgeOffset = 1f;
	public Vector2 FlagSize;
	public GameObject HighScoreFlagPrefab;
	protected bool HighScoreFlagSpawned = false;

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
		init();
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

	public void startGeneration(int colInd = -1) {
		calculateCameraDimentions();
		colors = createColorsArray();
		calculateInterval();
		calculateHeightValues();
		transform.position = RightOfScreen;
		configurePlayer();
		generating = true;
		finishedGenerating = false;
		time = generationInterval;
		platformIndex = 0;
		if (colInd == -1) changeBackgroundColor(randomColorIndex());
		else changeBackgroundColor(colInd);


		HighScoreFlagSpawned = (ScoreManager.Instance.highScore.distance - ScoreManager.MinDistance <= 1f);
		//Debug.Log(HighScoreFlagSpawned + " :HighDist: " + ScoreManager.Instance.highScore.distance);

		if (platforms != null) {
			Platform[] plats = platforms.ToArray<Platform>();
			for (int i = 0; i < plats.Length; ++i) if (plats[i] != null) plats[i].Die();
		}
		platforms = new HashSet<Platform>();
		colorChangers = new HashSet<ColorChanger>();
		CreateInitialPlatform();
	}

	public void restartGeneration(int colInd = -1) {
		startGeneration(colInd);
	}
	public void stopGenerating() => generating = false;

	public void continueGeneration() {
		restartGeneration(BackgroundColorIndex);
	}

	void CreatePlatform(Vector3 pos, float width, bool initial, bool changer, bool isHighScore, int ColorIndex) {
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

		if (changer) {
			GameObject colChanger = Instantiate(ColorChangerPrefab, pos + new Vector3(0, ColorChangerOffset, 0), Quaternion.identity);
			ColorChanger ColChang = colChanger.GetComponent<ColorChanger>();
			ColChang.setPlatformGenerator(this);

			platform.setColorChanger(ColChang);
			ColChang.setColor(randomColorIndexNotColor(BackgroundColorIndex, ColorIndex));

			colorChangers.Add(ColChang);
		}

		if (initial) CreateStartFlag(platform, width, blockHeight);
		else if (isHighScore) CreateHighScoreFlag(platform, width, blockHeight);

	}
	protected void CreatePlatform(Vector3 topLeft, bool canSpawnColorChanger = true) {
		int heightIndex = getHeightIndex(topLeft.y);
		int maxJumpHeightIndex = Mathf.FloorToInt(player.maxJumpHeight / blockHeight) - 1;
		int colorIndex = randomColorIndexPattern();

		if (heightIndex - previousPlatformHeightIndex > maxJumpHeightIndex) {
			if (colorIndex == BackgroundColorIndex) {
				if (colorPatternCount == 1) colorPatternCount++;
				else if (colorPatternCount > 1) {
					previousPlatformHeightIndex += maxJumpHeightIndex;
					topLeft.y = getHeight(previousPlatformHeightIndex);
					colorIndex = randomColorIndexNot();
				}
			} else {
				previousPlatformHeightIndex += maxJumpHeightIndex;
				topLeft.y = getHeight(previousPlatformHeightIndex);
			}
		} else if (colorIndex != BackgroundColorIndex) previousPlatformHeightIndex = heightIndex;

		bool isHighScore = false;
		if (!HighScoreFlagSpawned) {

			float currentDist = player.score.distance;
			float highScoreDist = 50f;// ScoreManager.Instance.highScore.distance;

			float playerDist = Mathf.Abs(transform.position.x - player.transform.position.x);
			float delta_Dist = Mathf.Abs(highScoreDist - currentDist);
			float alpha_dist = blockWidth;

			isHighScore = Mathf.Abs(playerDist - delta_Dist) <= alpha_dist;

			if (colorIndex == BackgroundColorIndex) colorIndex = randomColorIndexNot();
			if (isHighScore) HighScoreFlagSpawned = true;
		}


		bool changer = canSpawnColorChanger && !isHighScore && (colorIndex != BackgroundColorIndex) && (Random.value <= ColorChangerChance);

		CreatePlatform(new Vector3(topLeft.x + blockWidth / 2, topLeft.y - blockHeight / 2), blockWidth, false, changer, isHighScore, colorIndex);

	}
	void CreateInitialPlatform() {
		float midHeight = getMiddleHeight();
		previousPlatformHeightIndex = getHeightIndex(midHeight);
		colorPatternCount = 1;

		float wid = CameraWidth + HorizontalOffest * 2;
		Vector3 pos = new Vector3(0, midHeight - blockHeight / 2);

		CreatePlatform(pos, wid, true, false, false, randomColorIndexNot());
	}

	void CreateStartFlag(Platform initPlatform, float initPlatformWidth, float initPlatformHeight) {
		Vector3 positionOffset = new Vector3(initPlatformWidth / 2, initPlatformHeight / 2) + new Vector3(-StartFlagEdgeOffset, 0f);
		CreateFlag(StartFlagPrefab, initPlatform, positionOffset, true);
	}

	void CreateHighScoreFlag(Platform platform, float platformWidth, float platformHeight) {
		Vector3 positionOffset = new Vector3(0f, platformHeight / 2);
		CreateFlag(HighScoreFlagPrefab, platform, positionOffset, false);
	}

	void CreateFlag(GameObject flagPrefab, Platform platform, Vector3 positionOffset, bool start = false) {
		GameObject flagStart = Instantiate(flagPrefab, platform.transform);

		flagStart.transform.localScale = FlagSize;
		flagStart.transform.localPosition = positionOffset;

		if (!start) EndlessLevelScene.Instance.onHighScoreText(flagStart.transform);
	}


	public void changeBackgroundColor(int colIndex) {
		if (colIndex < 0 || colIndex >= colors.Length) {
			colIndex = -1;
		}
		BackgroundColorIndex = colIndex;
		updateBackgroundColor();
		removeInvalidColoChangers();
	}

	void updateBackgroundColor() {
		Camera.main.backgroundColor = BackgroundColor;
	}

	void removeInvalidColoChangers() {
		if (colorChangers != null) {
			ColorChanger[] cols = colorChangers.ToArray<ColorChanger>();
			for (int i = 0; i < cols.Length; ++i) if (cols[i] != null && cols[i].ColorIndex == BackgroundColorIndex) cols[i].Die();
		}
	}

	public void updatePlatformColors() {
		foreach (Platform plat in platforms) if (plat != null) plat.changeColor(plat.ColorIndex);
	}

	public Color randomColor() => colors[Random.Range(0, colors.Length)];
	public int randomColorIndex() => Random.Range(0, colors.Length);
	public int randomColorIndexNot() {
		return randomColorIndexNotColor(BackgroundColorIndex);
	}

	public int randomColorIndexNotColor(int NotIndex) {
		int index = -1;
		do {
			index = Random.Range(0, colors.Length);
		} while (index == NotIndex);
		return index;
	}

	public int randomColorIndexNotColor(int NotIndex, int NotIndex2) {
		int index = -1;
		do {
			index = Random.Range(0, colors.Length);
		} while (index == NotIndex || index == NotIndex2);
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
		return;
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

		//if (!EditorApplication.isPlaying && player != null) player.setXY(PlayerPosition);
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
