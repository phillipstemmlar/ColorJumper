using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ?

[RequireComponent(typeof(PlayerController2D))]
public class Player : MonoBehaviour
{
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

	[HideInInspector]
	public int ColorIndex = -1;
	Color playerColor = Color.white;

	public PlatformGenerator platformGenerator;
	SpriteRenderer spriteRenderer;

	Vector3 velocity;
	PlayerController2D controller;

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
		updateColor();
	}

	void Update() {
		if (controller.collisions.above || controller.collisions.below) velocity.y = 0;

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) velocity.y = maxJumpVelocity;
		if (Input.GetKeyUp(KeyCode.Space) && velocity.y > minJumpVelocity) velocity.y = minJumpVelocity;

		float targetVelocityX = input.x * moveSpeed;
		float accTime = (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne;

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocitySmoothingX, accTime);
		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);

		if (transform.position.y < -(platformGenerator.CameraHeight / 2 + verticalOffset)) {
			transform.position = new Vector3(transform.position.x, platformGenerator.CameraHeight / 2 + verticalOffset, transform.position.z);
		}
	}

	void calculatePhysicsValues() {
		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	public void changeColor(int colIndex) {
		if (platformGenerator == null || platformGenerator.colors == null
			|| colIndex < 0 || colIndex >= platformGenerator.colors.Length) {
			colIndex = -1;
		}
		ColorIndex = colIndex;
		updateColor();
	}

	void updateColor() {
		if (ColorIndex < 0) playerColor = Color.white;
		else playerColor = platformGenerator.colors[ColorIndex];

		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.color = playerColor;
	}
}
