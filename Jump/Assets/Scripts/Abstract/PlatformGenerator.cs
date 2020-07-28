using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlatformGenerator : MonoBehaviour
{
	public Player player;
	public GameObject PlatformPrefab;
	public HashSet<Platform> platforms;

	[HideInInspector]
	public Color[] colors;

	public float alphaNotSelected = 0.1f;
	public float alphaSelected = 1f;

	public bool isDrawingScreenBorder = false;
	public bool isDrawingPlatformRect = false;

	[HideInInspector]
	public float CameraHeight, CameraWidth;

	[HideInInspector]
	public Vector3 RightOfScreen { get { return Camera.main.transform.position + new Vector3(CameraWidth / 2 + HorizontalOffest, 0, 0); } }
	[HideInInspector]
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

	protected bool generating { get; private set; }
	bool finishedGenerating = false;
	float time;

	[HideInInspector]
	public int platformIndex;

	protected abstract void onGenerationInterval();
	protected abstract void onGeneratingFinished();
	protected abstract void generateNextPlatform();
	protected abstract Color[] createColorsArray();

	protected virtual void Start() {
		calculateCameraDimentions();
		transform.position = RightOfScreen;
		colors = createColorsArray();
		calculateInterval();
		startGeneration();
		platformIndex = 0;
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

	public void startGeneration() {
		generating = true;
		finishedGenerating = false;
		time = generationInterval;

		if (platforms != null) {
			Platform[] plats = platforms.ToArray<Platform>();
			for (int i = 0; i < plats.Length; ++i) if (plats[i] != null) plats[i].Die();
		}
		platforms = new HashSet<Platform>();


		CreateInitialPlatform();
	}
	public void restartGeneration() => startGeneration();
	public void stopGenerating() => generating = false;

	void CreatePlatform(Vector3 pos, float width, bool initial, int ColorIndex) {
		GameObject plat = Instantiate(PlatformPrefab, pos, Quaternion.identity);
		Platform platform = plat.GetComponent<Platform>();
		platform.platformGenerator = this;
		platform.player = player;
		platform.isDrawingBoundingBox = isDrawingPlatformRect;
		platform.isInitial = initial;
		platform.Width = width;
		platform.Height = blockHeight;
		platform.setSpeed(platformSpeed);
		platform.changeColor(ColorIndex);
	}
	protected void CreatePlatform(Vector3 topLeft) {
		CreatePlatform(new Vector3(topLeft.x + blockWidth / 2, topLeft.y - blockHeight / 2), blockWidth, false, randomColorIndex());
	}
	void CreateInitialPlatform() {
		int colIndex = randomColorIndex();
		player.changeColor(colIndex);
		CreatePlatform(new Vector3(0, -blockHeight / 2), CameraWidth, true, player.ColorIndex);
	}

	public void updatePlatformColors() {
		foreach (Platform plat in platforms) if (plat != null) plat.changeColor(plat.ColorIndex);
	}

	public Color randomColor() => colors[Random.Range(0, colors.Length)];
	public int randomColorIndex() => Random.Range(0, colors.Length);
	public Color randomColorRange() => new Color(Random.value, Random.value, Random.value);

	public void registerBlock(Platform platform) => platforms.Add(platform);
	public void unregisterBlock(Platform platform) => platforms.Remove(platform);

	void calculateCameraDimentions() {
		CameraHeight = 2 * Camera.main.orthographicSize;
		CameraWidth = CameraHeight * Camera.main.aspect;
	}
	protected virtual void calculateInterval() {
		generationInterval = (blockWidth + intervalDistance) / (platformSpeed);
		//print("v: " + platformSpeed + " t: " + generationInterval + " L: " + blockWidth + " d: " + intervalDistance);
	}

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
	}

	void drawTempRect(Vector3 middle, float width, float height) {
		Rect tempRect = new Rect(middle, width, height);
		tempRect.draw(Color.red);
	}

	void drawScreenBorder() {
		Rect screen = new Rect(Camera.main.transform.position, CameraWidth, CameraHeight);
		screen.draw(Color.blue);
	}

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
