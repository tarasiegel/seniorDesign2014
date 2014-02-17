using UnityEngine;
using System.Collections;

public class gestureScript : gestureBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTap( TapGesture gesture ) 
	{
		UI.StatusText = "Tapped with finger " + gesture.Fingers[0] + " at: " + gesture.Position;
		Debug.Log( "Tap gesture detected at " + gesture.Position + 
		          ". It was sent by " + gesture.Recognizer.name );
	}


	void OnSwipe( SwipeGesture gesture ) 
	{
		// Total swipe vector (from start to end position)
		Vector2 move = gesture.Move;
		
		// Instant gesture velocity in screen units per second
		float velocity = gesture.Velocity;
		
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;

		UI.StatusText = "Swiped " + direction + " with finger " + gesture.Fingers[0] +
			" (velocity:" + velocity + ", distance: " + gesture.Move.magnitude + " )";

		Debug.Log (UI.StatusText);
	}
}
