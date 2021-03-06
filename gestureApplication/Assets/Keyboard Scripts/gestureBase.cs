﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all sample scripts
/// </summary>
[RequireComponent( typeof( gestureUI ) )]
public class gestureBase : MonoBehaviour
{
	protected virtual string GetHelpText()
	{
		return "";
	}
	
	// reference to the shared sample UI script
	gestureUI ui;
	public gestureUI UI
	{
		get { return ui; }
	}
	
	protected virtual void Awake()
	{
		ui = GetComponent<gestureUI>();
	}
	
	protected virtual void Start()
	{
	}
	
	#region Utils
	
	// Convert from screen-space coordinates to world-space coordinates on the Z = 0 plane
	public static Vector3 GetWorldPos( Vector2 screenPos )
	{
		Ray ray = Camera.main.ScreenPointToRay( screenPos );
		
		// we solve for intersection with z = 0 plane
		float t = -ray.origin.z / ray.direction.z;
		
		return ray.GetPoint( t );
	}
	
	#endregion
}
