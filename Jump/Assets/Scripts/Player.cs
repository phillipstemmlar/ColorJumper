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

	Vector3 velocity;
	PlayerController2D controller;

	void Start() {
		controller = GetComponent<PlayerController2D>();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
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
	}
}
