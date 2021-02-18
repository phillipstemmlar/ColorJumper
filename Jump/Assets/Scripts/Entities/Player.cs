using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ?

[RequireComponent(typeof(PlayerController2D))]
public class Player : MonoBehaviour
{

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

	public bool pauseMovement = false;

	[HideInInspector] public int ColorIndex = -1;
	Color playerColor = Color.white;

	PlatformGenerator platformGenerator = null;
	SpriteRenderer spriteRenderer;

	[HideInInspector] public GameManager manager;

	[HideInInspector] public Vector3 velocity;
	PlayerController2D controller;
	[HideInInspector]
	public ScoreManager.Score score;

	bool willBeJumping, jumpKeyUp;

	public float hangTime = 0.2f;
	float hangCounter;

	public float verticalOffset {
		get { return (platformGenerator == null) ? 2f : platformGenerator.VerticalOffset; }
		set { if (platformGenerator != null) platformGenerator.VerticalOffset = value; }
	}

	private void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		controller = GetComponent<PlayerController2D>();
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

		if (willBeJumping) onJump(velocity.y, jumpKeyUp);

		float targetVelocityX = input.x * moveSpeed;
		float accTime = (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocitySmoothingX, accTime);
		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);
		checkOutofBounds();

		score.travel(platformGenerator.platformSpeed * Time.deltaTime);
	}

	public void Reset() {
		velocity = new Vector3();
	}

	void onJump(float velocity_Y, bool keyUp) {
		GameObject trail = Instantiate(PlayerTrailPrefab, new Vector3(), Quaternion.identity);
		trail.transform.parent = transform;
		trail.transform.localPosition = trailSpawnPoint;

		PlayerTrail playerTrail = trail.GetComponent<PlayerTrail>();
		playerTrail.player = this;

		score.jump();
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
		if (other.gameObject.tag == "ColorChanger") {
			ColorChanger colorChanger = other.gameObject.GetComponent<ColorChanger>();
			colorChanger.setPlatformGenerator(platformGenerator);
			colorChanger.change();
			colorChanger.trigger();
			score.change();
		}
	}

}
