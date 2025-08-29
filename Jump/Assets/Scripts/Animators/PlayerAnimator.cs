using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	const string isGroundedKey = "isGrounded";
	const string isRunningKey = "isRunning";

	const string vertSpeedKey = "vertSpeed";
	const string horzSpeedKey = "horzSpeed";


	public Animator animatorBody;
	public Animator animatorFace;

	public bool isGrounded { get; private set; }
	public bool isRunning { get; private set; }

	public float VerticalVelocity { get; private set; }
	public float HorizontalVelocity { get; private set; }

	private void Awake() {
		init();
	}

	void Start() {
	}
	void Update() { }

	public void init() {
		animatorBody = GetComponentInChildren<Animator>();
		if (animatorBody != null) {
			Transform spriteTransform = animatorBody.transform.Find("sprite");
			if (spriteTransform != null) {
				animatorFace = spriteTransform.gameObject.GetComponentInChildren<Animator>();
			}
		}
	}
	public void SetGrounded(bool isGrounded) {
		this.isGrounded = isGrounded;
		SetBool(isGroundedKey, this.isGrounded);
	}

	public void SetRunning(bool isRunning) {
		this.isRunning = isRunning;
		SetBool(isRunningKey, this.isRunning);
	}

	public void SetVerticalVelocity(float velocity) {
		this.VerticalVelocity = velocity;
		SetFloat(vertSpeedKey, this.VerticalVelocity);
	}
	public void SetHorizontalVelocity(float velocity) {
		this.HorizontalVelocity = velocity;
		SetFloat(horzSpeedKey, this.HorizontalVelocity);
	}

	// ========== Utilities ==========

	private void SetBool(string name, bool value) {
		if (animatorBody != null) animatorBody.SetBool(name, value);
		if (animatorFace != null) animatorFace.SetBool(name, value);
	}
	private void SetFloat(string name, float value) {
		if (animatorBody != null) animatorBody.SetFloat(name, value);
		if (animatorFace != null) animatorFace.SetFloat(name, value);
	}
}
