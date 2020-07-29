﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
	public LayerMask collisionMask;
	public LayerMask triggerMask;

	public const float skinWidth = 0.015f;

	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	public bool drawVerticalRays = false;
	public bool drawHorizontalRays = false;
	public bool drawOriginBox = false;

	[HideInInspector]
	public BoxCollider2D collider;
	[HideInInspector]
	public RaycastOrigins raycastOrigins;

	public virtual void Start() {
		collider = GetComponent<BoxCollider2D>();
		CalculateRaySpacing();
	}

	public void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

		if (drawOriginBox) drawRaycastOriginsBox();
	}

	public void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public void drawRaycastOriginsBox() {
		Color col = Color.green;
		Debug.DrawLine(raycastOrigins.bottomLeft, raycastOrigins.bottomRight, col);
		Debug.DrawLine(raycastOrigins.bottomRight, raycastOrigins.topRight, col);
		Debug.DrawLine(raycastOrigins.topRight, raycastOrigins.topLeft, col);
		Debug.DrawLine(raycastOrigins.topLeft, raycastOrigins.bottomLeft, col);
	}

	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
