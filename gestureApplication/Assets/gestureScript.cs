using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof( FingerDownDetector ) )]
[RequireComponent( typeof( FingerMotionDetector ) )]
[RequireComponent( typeof( FingerUpDetector ) )]
[RequireComponent( typeof( ScreenRaycaster ) )]

public class gestureScript : gestureBase {

	List<TapGesture> listOfTapGestures = new List<TapGesture>();
	List<SwipeGesture> listOfSwipeGestures = new List<SwipeGesture>();
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
		int finger;
		string typeOfGesture;

		public GestureObject( int finger, string type) {
			this.finger = finger;
			this.typeOfGesture = type;
		}
	}

	class GestureTracker {
		List<TapGesture> listOfTapGestures = new List<TapGesture>();
		List<SwipeGesture> listOfSwipeGestures = new List<SwipeGesture>();
		List<PathRenderer> listOfPaths = new List<PathRenderer>();
		List<string> sequentialGestures = new List<string>();
		List<GestureObject> gestureList = new List<GestureObject>();

		public GestureTracker() {
			//UpdateLines();
		}

		//T: tap
		//TD: tap, swipe down
		//TU: tap, swipe up
		//DT: swipe down, tap
		//UT: swipe up, tap
		//U: swipe up
		//D: swipe down
		public void updateList(TapGesture tap) {
			if (sequentialGestures.Count >= 1) {
				string lastGesture = sequentialGestures [sequentialGestures.Count - 1];
				TapGesture lastTap; SwipeGesture lastSwipe;
				if (listOfTapGestures.Count > 1 && lastGesture == "tap") {
					lastTap = listOfTapGestures [listOfTapGestures.Count - 1];
						//just tap
					addGesture(1, "T"); //this is adding the previous tap to the gesture object
					Debug.Log ("added tap gesture with finger: " + lastTap.Fingers [0].Index);
				}
				if (listOfSwipeGestures.Count > 1 && lastGesture == "swipe") {
						lastSwipe = listOfSwipeGestures [listOfSwipeGestures.Count - 1];
						float time = lastSwipe.StartTime + lastSwipe.ElapsedTime;
						Debug.Log("last time: " + time + "start: " + tap.StartTime);
						if (Mathf.Abs (time - tap.StartTime) <= 0.3f) {
							if (lastSwipe.Direction == FingerGestures.SwipeDirection.Down) {
								//tap, down
								addGesture(1, "DT");
								Debug.Log ("added down-tap gesture");
							} 
							else if (lastSwipe.Direction == FingerGestures.SwipeDirection.Up) {
								//tap, up
								addGesture(1, "UT");
								Debug.Log ("added up-tap gesture");
							}
					}
				}
			}
			listOfTapGestures.Add (tap);
			sequentialGestures.Add ("tap");
			Debug.Log ("added tap");
		}

		public void updateList(PathRenderer path) {
			listOfPaths.Add (path);
		}

		public void updateList(SwipeGesture swipe) {
			if (sequentialGestures.Count >= 1) {
				string lastGesture = sequentialGestures [sequentialGestures.Count - 1];
				TapGesture lastTap; SwipeGesture lastSwipe;
				if (listOfTapGestures.Count > 1 && lastGesture == "swipe") {
					lastSwipe = listOfSwipeGestures [listOfSwipeGestures.Count - 1];
					//this is adding the previous swipe to the gesture object
					if (lastSwipe.Direction == FingerGestures.SwipeDirection.Down) {
						addGesture(1, "D"); //this is adding the previous tap to the gesture object
						Debug.Log ("added down gesture with finger: " + lastSwipe.Fingers [0].Index);
					}
					else if (lastSwipe.Direction == FingerGestures.SwipeDirection.Up){
						addGesture(1, "U"); //this is adding the previous tap to the gesture object
						Debug.Log ("added up gesture with finger: " + lastSwipe.Fingers [0].Index );
					}
				} 
				if (listOfTapGestures.Count > 1 && lastGesture == "tap") {
					lastTap = listOfTapGestures [listOfTapGestures.Count - 1];
					float time = lastTap.StartTime + lastTap.ElapsedTime;
					Debug.Log("last time: " + time + "start: " + swipe.StartTime);
					if (Mathf.Abs (time - swipe.StartTime) <= 0.3f) {
						if (swipe.Direction == FingerGestures.SwipeDirection.Down) {
							//tap, down
							addGesture(1, "TD");
							Debug.Log ("added tap-down gesture");
						} 
						else if (swipe.Direction == FingerGestures.SwipeDirection.Up) {
							//tap, up
							addGesture(1, "TU");
							//swipe.Fingers [0].Index
							Debug.Log ("added tap-up gesture");
						}
					}
				}
			}
			listOfSwipeGestures.Add (swipe);
			sequentialGestures.Add ("swipe");
		}

		public void addGesture(int finger, string type) {
			GestureObject gesture = new GestureObject (finger, type);
			gestureList.Add (gesture);
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
		Debug.Log( "Tap gesture detected at " + gesture.Position + 
		          ". It was sent by " + gesture.Recognizer.name );
		Debug.Log (listOfTapGestures);
		gestureTracker.updateList (gesture);
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

		Debug.Log (UI.StatusText);
		gestureTracker.updateList (gesture);
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
