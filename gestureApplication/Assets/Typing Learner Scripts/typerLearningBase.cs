using UnityEngine;
using System.Collections;


[RequireComponent( typeof( typerLearningUI ) )]

public class typerLearningBase : MonoBehaviour {

	protected virtual string GetHelpText()
	{
		return "";
	}
	
	// reference to the shared UI script
	typerLearningUI ui;
	public typerLearningUI UI
	{
		get { return ui; }
	}
	
	protected virtual void Awake()
	{
		ui = GetComponent<typerLearningUI>();
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