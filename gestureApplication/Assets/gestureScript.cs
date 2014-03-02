using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( FingerDownDetector ) )]
[RequireComponent( typeof( FingerMotionDetector ) )]
[RequireComponent( typeof( FingerUpDetector ) )]
[RequireComponent( typeof( ScreenRaycaster ) )]

public class gestureScript : gestureBase {

	public LineRenderer lineRendererPrefab;
	public GameObject fingerDownMarkerPrefab;
	public GameObject fingerMoveBeginMarkerPrefab;
	public GameObject fingerMoveEndMarkerPrefab;
	public GameObject fingerUpMarkerPrefab;



	class PathRenderer {
		LineRenderer lineRenderer;
		
		// passage points
		List<Vector3> points = new List<Vector3>();
		
		// list of marker objects currently instantiated
		List<GameObject> markers = new List<GameObject>();
		
		public PathRenderer( int index, LineRenderer lineRendererPrefab ) {
			lineRenderer = Instantiate( lineRendererPrefab ) as LineRenderer;
			lineRenderer.name = lineRendererPrefab.name + index;
			lineRenderer.enabled = true;	
			UpdateLines();
		}

		public void Reset() {
			points.Clear();
			UpdateLines();
			foreach (GameObject marker in markers) {
				Destroy (marker);
			}
			markers.Clear();
		}
		
		public void AddPoint( Vector2 screenPos ) {
			AddPoint( screenPos, null );
		}
		
		public void AddPoint( Vector2 screenPos, GameObject markerPrefab ) {
			Vector3 pos = gestureBase.GetWorldPos (screenPos);

			if( markerPrefab )
				AddMarker( pos, markerPrefab );

			points.Add( pos );
			UpdateLines();
		}
		
		GameObject AddMarker( Vector2 pos, GameObject prefab ) {
			GameObject instance = Instantiate( prefab, pos, Quaternion.identity ) as GameObject;
			instance.name = prefab.name + "(" + markers.Count + ")";
			markers.Add( instance );
			return instance;
		}
		
		void UpdateLines() {
			lineRenderer.SetVertexCount( points.Count );
			for( int i = 0; i < points.Count; ++i )
				lineRenderer.SetPosition( i, points[i] );
		}
	}

	PathRenderer[] paths;
	
	public class GestureObject {
		int finger = 0;
		DiscreteGesture gesture1 = null;
		DiscreteGesture gesture2 = null;


		public GestureObject( int finger, DiscreteGesture gesture1, DiscreteGesture gesture2) {
			this.finger = finger;
			this.gesture1 = gesture1;
			this.gesture2 = gesture2;
		}

		public GestureObject(int finger) {
			this.finger = finger;
		}

		public GestureObject(int finger, DiscreteGesture gesture1) {
			this.finger = finger;
			this.gesture1 = gesture1;
			this.gesture2 = null;
		}


		public void setGesture1(DiscreteGesture gesture1) {
			this.gesture1 = gesture1;
		}
		public void setGesture2(DiscreteGesture gesture2) {
			this.gesture2 = gesture2;
		}
		public DiscreteGesture getGesture1() {
			return gesture1;
		}
		public DiscreteGesture getGesture2() {
			return gesture2;
		}
		public int getFinger() {
			return finger;
		}

	}

	class GestureTracker {
		List<TapGesture> listOfTapGestures = new List<TapGesture>();
		List<SwipeGesture> listOfSwipeGestures = new List<SwipeGesture>();
		List<LongPressGesture> listOfPressGestures = new List<LongPressGesture>();
		List<PathRenderer> listOfPaths = new List<PathRenderer>();
		//List<string> sequentialGestures = new List<string>();
		List<GestureObject> gestureQueue = new List<GestureObject>();
		List<GestureObject> gestureSequence = new List<GestureObject>();

		public GestureTracker() {

		}

		public void printOutCurrentPath() {
			Debug.Log ("Start of Path");
			foreach (GestureObject gest in gestureSequence) {
				int finger = gest.getFinger();
				GestureRecognizer g1 = gest.getGesture1().Recognizer;
				if (gest.getGesture2() == null){
					Debug.Log (" Finger: " + finger + ", Gesture 1: " + g1);
				} 
				else {
					GestureRecognizer g2 = gest.getGesture2().Recognizer;
					Debug.Log (" Finger: " + finger + ", Gesture 1: " + g1 + ", Gesture 2: " + g2);
				}
			}
		}

		//T: tap
		//TD: tap, swipe down
		//TU: tap, swipe up
		//DT: swipe down, tap
		//UT: swipe up, tap
		//U: swipe up
		//D: swipe down

		//vibrations after long press Handheld.Vibrate();
		//then long press + swipe up or down COMPOUND GESTURES
		//modify existing code or creating modified existing code combining two together 
		//reservation queue if multiple fingers
		public void updateList(TapGesture tap) {
			addGesture(tap.Fingers[0].Index, tap); //this is adding the previous tap to the gesture object
			listOfTapGestures.Add (tap);

		}

		public void updateList(PathRenderer path) {
			listOfPaths.Add (path);
		}

		public void updateList(LongPressGesture press) {
			addGesture (press.Fingers [0].Index, press);
			listOfPressGestures.Add (press);
		}

		public void updateList(SwipeGesture swipe) {
			addGesture (swipe.Fingers [0].Index, swipe);
			listOfSwipeGestures.Add (swipe);
		}

		public void addGesture(int finger, DiscreteGesture type) {
			bool gestureStarted = false;
			int gestIndex = 0;
			foreach (GestureObject gest in gestureQueue) {
				if (gest.getFinger () == finger) {
					gestIndex = gestureQueue.IndexOf(gest);
					gestureStarted = true;
				}
			}
			if (gestureStarted == true){     
				DiscreteGesture firstGesture = gestureQueue[gestIndex].getGesture1 ();
				TapGesture t = new TapGesture();
				if ((firstGesture.Recognizer == type.Recognizer || type.ToString() == "TapGesture" || firstGesture.ToString() == "TapGesture")) {
					GestureObject g = gestureQueue[gestIndex];
					gestureSequence.Add (g);
					gestureQueue.Remove(g);
					GestureObject newg = new GestureObject(finger, type);
					gestureQueue.Add(newg);
				} 
				else {

					float time = firstGesture.StartTime - firstGesture.ElapsedTime;
					float distance;
					if (firstGesture.ToString() == "SwipeGesture"){
						distance = Mathf.Sqrt (Mathf.Pow (firstGesture.Position.x - type.Position.x, 2) + Mathf.Pow (firstGesture.Position.y - type.Position.y, 2));
					}
					else {
						distance = Mathf.Sqrt (Mathf.Pow (firstGesture.Position.x - type.StartPosition.x, 2) + Mathf.Pow (firstGesture.Position.y - type.StartPosition.y, 2));
					}
					if (Mathf.Abs (time - type.StartTime) <= 10.0f && distance <= 3.0f) {
						gestureQueue[gestIndex].setGesture2 (type);
					} 
					else {
						GestureObject g = gestureQueue[gestIndex];
						gestureSequence.Add (g);
						gestureQueue.Remove(g);
						GestureObject newg = new GestureObject(finger, type);
						gestureQueue.Add(newg);
						}
					}
				}
			else {
				GestureObject gesture = new GestureObject (finger, type);
				gestureQueue.Add(gesture);
			}
			//return gestureQueue;
		}
	
	}
	GestureTracker gestureTracker = new GestureTracker();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		base.Start();
		paths = new PathRenderer[FingerGestures.Instance.MaxFingers];
		for (int i = 0; i < paths.Length; ++i) {
			paths [i] = new PathRenderer (i, lineRendererPrefab);
		}
	}

	void OnTap( TapGesture gesture ) {
		UI.StatusText = "Tapped with finger " + gesture.Fingers[0] + " at: " + gesture.Position;
		//Debug.Log( "Tap gesture detected at " + gesture.Position + 
		         // ". It was sent by " + gesture.Recognizer.name );
		gestureTracker.updateList (gesture);
		gestureTracker.printOutCurrentPath();
	}

	//The limits are the one imposed by the platform/device you run the app on.  
	//Swipes can be detected "per-finger" via the OnFingerSwipe event (or TBSwipe 
	//toolbox script). This allows you to detect multiple swipes at once, using the
	//"fingerIndex" value to identify which finger performed the gesture.
	void OnSwipe( SwipeGesture gesture ) {
		// Total swipe vector (from start to end position)
		Vector2 move = gesture.Move;
		// Instant gesture velocity in screen units per second
		float velocity = gesture.Velocity;
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;
		UI.StatusText = "Swiped " + direction + " with finger " + gesture.Fingers[0] +
			" (velocity:" + velocity + ", distance: " + gesture.Move.magnitude + " )";

		//Debug.Log (UI.StatusText);
		gestureTracker.updateList (gesture);
		gestureTracker.printOutCurrentPath();

	}


	void OnLongPress(LongPressGesture gesture) {
		gestureTracker.updateList (gesture);
		gestureTracker.printOutCurrentPath();
	}



	void OnFingerDown( FingerDownEvent e ) {
		PathRenderer path = paths[e.Finger.Index];
		path.Reset();
		path.AddPoint( e.Finger.Position, fingerDownMarkerPrefab );
	}
	
	/*void OnFingerMove( FingerMotionEvent e ) {
		PathRenderer path = paths[e.Finger.Index];
		
		if( e.Phase == FingerMotionPhase.Started )
		{
			UI.StatusText = "Started moving " + e.Finger;
			path.AddPoint( e.Position, fingerMoveBeginMarkerPrefab );
		}
		else if( e.Phase == FingerMotionPhase.Updated )
		{
			path.AddPoint( e.Position );
		}
		else
		{
			UI.StatusText = "Stopped moving " + e.Finger;
			path.AddPoint( e.Position, fingerMoveEndMarkerPrefab );
		}
	}*/
	
	void OnFingerUp( FingerUpEvent e ) {
		PathRenderer path = paths[e.Finger.Index];
		path.AddPoint( e.Finger.Position, fingerUpMarkerPrefab );
		UI.StatusText = "Finger " + e.Finger + " was held down for " + e.TimeHeldDown.ToString( "N2" ) + " seconds";
		gestureTracker.updateList (path);
	}
}
