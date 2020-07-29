using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ?
public class PlatformController : RaycastController
{
	public LayerMask passengerMask;
	public Vector3 move;

	List<PassengerMovement> passengerMovements;
	Dictionary<Transform, PlayerController2D> passengerDictionary = new Dictionary<Transform, PlayerController2D>();

	public override void Start() {
		base.Start();
		passengerMovements = null;
	}


	void Update() {
		UpdateRaycastOrigins();
		Vector3 velocity = move * Time.deltaTime;

		MovePassengers(true);
		CalculatePassengerMovement(velocity);
		MovePassengers(false);

		transform.Translate(velocity);
	}

	void MovePassengers(bool beforeMovePlatform) {
		if (passengerMovements != null) {
			foreach (PassengerMovement passenger in passengerMovements) {
				if (!passengerDictionary.ContainsKey(passenger.transform)) {
					passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<PlayerController2D>());
				}
				if (passenger.moveBeforePlatform == beforeMovePlatform) {
					passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
				}
			}
		}
	}

	void CalculatePassengerMovement(Vector3 velocity) {

		HashSet<Transform> movedPassengers = new HashSet<Transform>();
		passengerMovements = new List<PassengerMovement>();

		float directionX = Mathf.Sign(velocity.x);
		float directionY = Mathf.Sign(velocity.y);

		if (velocity.y != 0) { //vertical movement
			float rayLength = Mathf.Abs(velocity.y) + skinWidth;

			for (int i = 0; i < verticalRayCount; ++i) {
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				if (hit && hit.distance != 0) {

					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);

						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
					}
				}
			}
		}

		if (velocity.x != 0) { //horizontal movement
			float rayLength = Mathf.Abs(velocity.x) + skinWidth;

			for (int i = 0; i < horizontalRayCount; ++i) {
				Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);

						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

						passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
					}
				}

			}
		}

		if (directionY == -1 || velocity.y == 0 && velocity.x != 0) { //on top of horizontally or downward moving platform
			float rayLength = skinWidth * 2;

			for (int i = 0; i < verticalRayCount; ++i) {
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);

						float pushX = velocity.x;
						float pushY = velocity.y;

						passengerMovements.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
					}
				}
			}
		}

	}

	struct PassengerMovement
	{
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform, moveBeforePlatform;

		public PassengerMovement(Transform t, Vector3 v, bool standing, bool moveBefore) {
			transform = t;
			velocity = v;
			standingOnPlatform = standing;
			moveBeforePlatform = moveBefore;
		}

	}

}
