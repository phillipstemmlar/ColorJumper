using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ?

[RequireComponent(typeof(PlayerController2D))]
public class Player : MonoBehaviour
{

	[HideInInspector] public GameObject PlayerSpritePrefab;
	public GameObject PlayerTrailPrefab;
	public Vector2 trailSpawnPoint;

	public float moveSpeed = 6;
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = 0.4f;

	float accelerationTimeAirborne = 0.2f;
	float accelerationTimeGrounded = 0.1f;

	float velocitySmoothingX;
	float maxJumpVelocity;
	float minJumpVelocity;
	float gravity;

	public bool isAlive { get; set; }
	public int SpriteIndex {
		get { return SpriteModelIndex; }
		set { SpriteIndexChange(value); }
	}
	int SpriteModelIndex = 0;
	public static int DefaultSpriteIndex = 0;

	public bool pauseMovement = false;

	[HideInInspector] public int ColorIndex = -1;
	Color playerColor = Color.white;

	PlatformGenerator platformGenerator = null;
	SpriteRenderer spriteRenderer;
	GameObject playerSprite = null;

	[HideInInspector] public GameManager manager;

	[HideInInspector] public Vector3 velocity;
	PlayerController2D controller;
	[HideInInspector]
	public ScoreManager.Score score;

	bool willBeJumping, jumpKeyUp;

	public float hangTime = 0.2f;
	float hangCounter;

	bool scoreStarted;

	public float verticalOffset {
		get { return (platformGenerator == null) ? 2f : platformGenerator.VerticalOffset; }
		set { if (platformGenerator != null) platformGenerator.VerticalOffset = value; }
	}

	private void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		controller = GetComponent<PlayerController2D>();
		controller.collider = GetComponent<BoxCollider2D>();

		SpriteIndexChange(DefaultSpriteIndex);
	}

	void Start() {
		calculatePhysicsValues();
		Reset();
	}

	void Update() {
		willBeJumping = false;
		jumpKeyUp = false;

		if (pauseMovement) return;

		if (controller.collisions.above || controller.collisions.below) velocity.y = 0;

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		input.x = 0f;

		if (controller.collisions.below) hangCounter = hangTime;
		else hangCounter -= Time.deltaTime;

		//Touch Controls
		if (Input.touchCount > 0) {
			if (Input.touches[0].phase == TouchPhase.Began && controller.collisions.below) {
				Debug.Log("Touch Pressed");
				velocity.y = maxJumpVelocity;
				willBeJumping = true;
			}

			if (Input.touches[0].phase == TouchPhase.Began && hangCounter > 0) {
				Debug.Log("Touch Pressed");
				hangCounter = -1f;
				velocity.y = maxJumpVelocity;
				willBeJumping = true;
			}

			if (Input.touches[0].phase == TouchPhase.Ended && velocity.y > minJumpVelocity) {
				Debug.Log("Touch Lifted/Released");
				velocity.y = minJumpVelocity;
				willBeJumping = true;
				jumpKeyUp = true;
			}
		}

		//Keyboard controls
		if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
			velocity.y = maxJumpVelocity;
			willBeJumping = true;
		}

		if (Input.GetKeyDown(KeyCode.Space) && hangCounter > 0) {
			hangCounter = -1f;
			velocity.y = maxJumpVelocity;
			willBeJumping = true;
		}
		if (Input.GetKeyUp(KeyCode.Space) && velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
			willBeJumping = true;
			jumpKeyUp = true;
		}

		//Calculations
		if (willBeJumping) onJump(velocity.y, jumpKeyUp);

		float targetVelocityX = input.x * moveSpeed;
		float accTime = (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocitySmoothingX, accTime);
		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);
		checkOutofBounds();

		scoreTravel(platformGenerator.platformSpeed * Time.deltaTime);
	}

	void scoreTravel(float distance) {
		if (scoreStarted) score.travel(distance);
	}

	void scoreJumpo() {
		if (scoreStarted) score.jump();
	}

	void scoreColorChange() {
		if (scoreStarted) score.change();
	}

	private void OnDrawGizmos() {
		float Width = 0.55f;
		float Height = 0.9f;
		Vector3 offset = new Vector3(0.01f, 0.02f, 0f);

		Platform.Rect rect = new Platform.Rect(transform.position + offset, Width, Height);
		rect.draw(Color.yellow);
	}

	public void Reset() {
		velocity = new Vector3();
		scoreStarted = false;
		isAlive = true;
	}

	void onJump(float velocity_Y, bool keyUp) {
		GameObject trail = Instantiate(PlayerTrailPrefab, new Vector3(), Quaternion.identity);
		trail.transform.parent = transform;
		trail.transform.localPosition = trailSpawnPoint;

		PlayerTrail playerTrail = trail.GetComponent<PlayerTrail>();
		playerTrail.player = this;

		scoreJumpo();
	}

	void onColorChange(Collider2D other) {
		ColorChanger colorChanger = other.gameObject.GetComponent<ColorChanger>();
		colorChanger.setPlatformGenerator(platformGenerator);
		colorChanger.change();
		colorChanger.trigger();
		scoreColorChange();
	}

	void onScoreStarted(Collider2D other) {
		Debug.Log("Score Started");
		scoreStarted = true;

		other.enabled = false;
	}

	void onHighScorePassed(Collider2D other) {
		Debug.Log("HighScore Passed");

		other.enabled = false;
	}


	void checkOutofBounds() {
		if (isOutOfBoundsBottom()) onOutOfBounds(true);
		else if (isOutOfBoundsTop()) onOutOfBounds(false);
	}

	bool isOutOfBoundsBottom() => (transform.position.y < -(platformGenerator.CameraHeight / 2 + verticalOffset));
	bool isOutOfBoundsTop() => false && (transform.position.y > platformGenerator.CameraHeight / 2 + verticalOffset);


	void onOutOfBounds(bool bottom) {
		manager.PlayerDied(bottom);
	}

	public void setX(float x) {
		Vector3 pos = transform.position;
		pos.x = x;
		transform.position = pos;
	}
	public void setY(float y) {
		Vector3 pos = transform.position;
		pos.y = y;
		transform.position = pos;
	}
	public void setXY(float x, float y) {
		Vector3 pos = transform.position;
		pos.x = x;
		pos.y = y;
		transform.position = pos;
	}
	public void setXY(Vector2 newPos) {
		Vector3 pos = transform.position;
		pos.x = newPos.x;
		pos.y = newPos.y;
		transform.position = pos;
	}

	void calculatePhysicsValues() {
		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}
	public void setPlatformGenerator(PlatformGenerator platgen) {
		//print("plat.type: " + platgen.type);
		platformGenerator = platgen;
	}

	public void onTrigger(Collider2D other) {
		if (other.gameObject.tag == "ColorChanger") onColorChange(other);
		else if (other.gameObject.tag == "StartFlag") onScoreStarted(other);
		else if (other.gameObject.tag == "HighScoreFlag") onHighScorePassed(other);
	}


	void SpriteIndexChange(int index) {
		if (index < 0) index = 0;
		if (index >= SpriteModelManager.Instance.PlayerModelPrefabs.Length) index = SpriteModelManager.Instance.PlayerModelPrefabs.Length - 1;

		SpriteModelIndex = index;
		PlayerSpritePrefab = SpriteModelManager.Instance.PlayerModelPrefabs[SpriteModelIndex];

		initSpriteModel();
	}

	void initSpriteModel() {
		if (playerSprite != null) Destroy(playerSprite);

		playerSprite = Instantiate(PlayerSpritePrefab, transform.position, Quaternion.identity);
		playerSprite.transform.parent = transform;
		//playerSprite.transform.localPosition = new Vector3();
	}

}
