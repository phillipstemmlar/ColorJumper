using UnityEngine;

// ?

public class PlayerController2D : RaycastController
{
	public float maxClimpAngle = 80;
	public float maxDecendAngle = 75;

	public Player player;

	public CollisionInfo collisions;

	void Awake() {
		player = GetComponent<Player>();
	}
	public override void Start() {
		base.Start();
	}
	public void Move(Vector3 velocity, bool standingOnPlatform = false) {
		UpdateRaycastOrigins();
		collisions.Reset();
		collisions.velocityOld = velocity;

		if (velocity.y < 0) DecendSlope(ref velocity);
		if (velocity.x != 0) HorizontalCollisions(ref velocity);
		if (velocity.y != 0) VerticalCollisions(ref velocity);

		HorizontalTriggers(velocity);
		VerticalTriggers(velocity);

		transform.Translate(velocity);

		if (standingOnPlatform) collisions.below = true;
	}

	void HorizontalCollisions(ref Vector3 velocity) {

		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; ++i) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (drawHorizontalRays) {
				Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
			}

			if (hit) {
				if (hit.distance == 0) continue;
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (i == 0 && slopeAngle <= maxClimpAngle) {

					if (collisions.decendingSlope) {
						collisions.decendingSlope = false;
						velocity = collisions.velocityOld;
					}

					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimpSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimpAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}


					collisions.left = (directionX == -1);
					collisions.right = (directionX == 1);

				}
			}
		}

	}

	void VerticalCollisions(ref Vector3 velocity) {

		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; ++i) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			if (drawVerticalRays) {
				Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
			}

			if (hit) {

				if (hit.collider.tag == "platform" && (directionY == 1 || hit.distance == 0)) continue;

				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				collisions.below = (directionY == -1);
				collisions.above = (directionY == 1);

			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;

			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimpSlope(ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs(velocity.x);
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DecendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign(velocity.x);

		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {

			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDecendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float decendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= decendVelocityY;

						collisions.below = true;
						collisions.decendingSlope = true;
						collisions.slopeAngle = slopeAngle;
					}
				}
			}
		}


	}

	void HorizontalTriggers(Vector3 velocity) {
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;
		for (int i = 0; i < horizontalRayCount; ++i) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, triggerMask);

			if (hit) onTrigger(hit.collider);
		}
	}

	void VerticalTriggers(Vector3 velocity) {
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; ++i) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			if (hit) onTrigger(hit.collider);
		}
	}
	void onTrigger(Collider2D other) {
		if (player == null) player = GetComponent<Player>();
		player.onTrigger(other);
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope, decendingSlope;
		public float slopeAngle, slopeAngleOld;

		public Vector3 velocityOld;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = decendingSlope = false;
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
