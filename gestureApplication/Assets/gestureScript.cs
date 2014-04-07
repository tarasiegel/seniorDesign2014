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
	public GameObject emitter;
	public string currentString;

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

	class GestureTracker {
		//List<TapGesture> listOfTapGestures = new List<TapGesture>();
		//List<SwipeGesture> listOfSwipeGestures = new List<SwipeGesture>();
		//List<LongPressGesture> listOfPressGestures = new List<LongPressGesture>();
		//List<PathRenderer> listOfPaths = new List<PathRenderer>();
		//List<string> sequentialGestures = new List<string>();
		GestureObject[] gestureQueue = new GestureObject[5];
		List<GestureObject> gestureSequence = new List<GestureObject>();
		characterMap charMap;
		int lastFinger = 0;


		public GestureTracker() {
			FingerMapObject one = new FingerMapObject('t', 'd', ' ', 'o', 'f', 'm', 's');
			FingerMapObject two = new FingerMapObject('y', 'n', 'w', 'j', 'i', 'c', 'r');
			FingerMapObject three = new FingerMapObject('h', 'a', 'q', 'b', 'l', 'u', 'e');
			FingerMapObject four = new FingerMapObject('<', 'g', 'z', 'x', 'p', 'k', 'v');
			charMap = new characterMap(one, two, three, four);
		}
		//dont let them go farther until get the key correct but log the amount of times get wrong


		public string printOutCurrentPath() {
			Debug.Log ("Start of Path");
			string updatedString = "";
			foreach (GestureObject gest in gestureSequence) {
				char letter = gest.Letter;
				int finger = gest.Finger;
				string g1 = gest.Gesture1.ToString();
				string dir1 = gest.Direction1.ToString();
				updatedString += letter;
				if (gest.Gesture2 == null){
					Debug.Log (" Finger: " + finger + ", Gesture 1: " + g1 + ", Direction 1: " + dir1 + ", Letter: " + letter);
				} 
				else {
					string g2 = gest.Gesture2.ToString();
					string dir2 = gest.Direction2.ToString();
					Debug.Log (" Finger: " + finger + ", Gesture 1: " + g1 + ", Gesture 2: " + g2 + ", Direction 1: " + dir1 + ", Direction 2: " + dir2 +", Letter: " + letter);
				}
			}
			return updatedString;

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
			addGesture(tap); //this is adding the previous tap to the gesture object
			//listOfTapGestures.Add (tap);
			lastFinger = determineFinger (tap);
		}

		/*public void updateList(PathRenderer path) {
			listOfPaths.Add (path);
		}*/

		public void updateList(LongPressGesture press) {
			addGesture (press);
			//listOfPressGestures.Add (press);
			lastFinger = determineFinger (press);

		}

		public void updateList(SwipeGesture swipe) {
			addGesture (swipe, swipe.Direction);
			lastFinger = determineFinger (swipe);
			//listOfSwipeGestures.Add (swipe);

		}

		private int determineFinger(DiscreteGesture type) {
			Vector2 currPos = type.StartPosition;
			float w = Screen.width;
			float h = Screen.height;
			/*if (currPos.y <= h / 2.0f) {
								return 2;
			}*/
			if (currPos.x <= w / 2.0f && currPos.y <= h / 2.0f) {
					return  2;
			} else if (currPos.x <= w / 2.0f && currPos.y >= h / 2.0f) {
					return 1;
			} else if (currPos.x >= w / 2.0f && currPos.y <= h / 2.0f) {
					return 4;
			} else if (currPos.x >= w / 2.0f && currPos.y >= h / 2.0f) {
					return 3;
			} else {
					return 0;
			}
		}

		private void checkOtherFingerGestures(GestureObject g){
			GestureObject currG;
			float gTime = g.Gesture1.StartTime;
			/*if (g.Finger != lastFinger && lastFinger != 0) {
				currG = gestureQueue[lastFinger];
				float t = currG.Gesture1.StartTime + currG.Gesture1.ElapsedTime;*/
				//if (currG.Gesture2 == null && t < gTime) {
					//gestureQueue[lastFinger] = null;
					//addToGestureSequence (currG);
				//}
				
			//}
			for (int i = 0; i < gestureQueue.Length; i++) {
				if (g.Finger != i && gestureQueue[i] != null) {
					currG = gestureQueue[i];
					float t = currG.Gesture1.StartTime + currG.Gesture1.ElapsedTime;
					if (currG.Gesture2 == null && t < gTime) {
						gestureQueue[i] = null;
						addToGestureSequence (currG);
					}
				}
			}
		}
		
		private void addToGestureSequence(GestureObject g){
			char letter = charMap.getCharacter (g);
			if (letter == '<') {
				gestureSequence.RemoveAt (gestureSequence.Count - 1);
			} 
			else {
				g.Letter = letter;
				gestureSequence.Add (g);
			}
		}

		private void addGesture(DiscreteGesture type, FingerGestures.SwipeDirection dir = FingerGestures.SwipeDirection.Left) {
			bool gestureStarted = false;
			int finger = determineFinger (type);
			for (int i = 0; i < gestureQueue.Length; i++) {
				if (gestureQueue[i] != null){
					if(gestureQueue[i].Finger == finger) {
						gestureStarted = true;
					}
				}
			}
			if (gestureStarted == true){     
				DiscreteGesture firstGesture = gestureQueue[finger].Gesture1;
				if ((firstGesture.Recognizer == type.Recognizer || type.ToString() == "TapGesture" || firstGesture.ToString() == "TapGesture")) {
					//seen same gesture as before
					GestureObject g = gestureQueue[finger];
					checkOtherFingerGestures(g);
					addToGestureSequence(g);
					//just in case it is a swipe --> press
					if (type.ToString() == "SwipeGesture" && type.LongPressAfterSwipe == true){
						LongPressGesture l = new LongPressGesture();
						l.Position = type.Position;
						GestureObject newg = new GestureObject(finger, type, l);
						newg.Direction1 = dir;
						gestureQueue[finger] = null;
						checkOtherFingerGestures(newg);
						addToGestureSequence(newg);
					}
					//else override it as the gesture for that 
					else {
						GestureObject newg2 = new GestureObject(finger, type);
						if (type.ToString() == "SwipeGesture"){
							newg2.Direction1 = dir;
						}
						gestureQueue[finger] = newg2;
						checkOtherFingerGestures(newg2);
					}
				} 
				else {
					float time = firstGesture.StartTime + firstGesture.ElapsedTime;
					float distance;
					if (firstGesture.ToString() == "SwipeGesture"){
						distance = Mathf.Sqrt (Mathf.Pow (firstGesture.Position.x - type.Position.x, 2) + Mathf.Pow (firstGesture.Position.y - type.Position.y, 2));
					}
					else {
						//long press --> swipe
						distance = Mathf.Sqrt (Mathf.Pow (firstGesture.Position.x - type.StartPosition.x, 2) + Mathf.Pow (firstGesture.Position.y - type.StartPosition.y, 2));
					}

					if (Mathf.Abs (time - type.StartTime) <= 10.0f && distance <= 3.0f) {
						//recognized long press --> swipe, adding swipe as the second part of gesture
						gestureQueue[finger].Gesture2 = type;
						gestureQueue[finger].Direction2 = dir;
						checkOtherFingerGestures(gestureQueue[finger]);
						addToGestureSequence(gestureQueue[finger]);
					}

					else {
						//create a new gesture
						GestureObject g = gestureQueue[finger];
						addToGestureSequence(g);
						GestureObject newg = new GestureObject(finger, type);
						if (type.ToString() == "SwipeGesture") {
							newg.Direction1 = dir;
						}
						gestureQueue[finger] = newg;
						checkOtherFingerGestures(newg);
						}
					}
				}
			//first gesture of that finger
			else {
				if (type.ToString() == "SwipeGesture") {
					if (type.LongPressAfterSwipe == true){
						LongPressGesture l = new LongPressGesture();
						l.Position = type.Position;
						GestureObject newg = new GestureObject(finger, type, l);
						newg.Direction1 = dir;
						//gestureQueue[finger] = newg;
						checkOtherFingerGestures(newg);
						addToGestureSequence(newg);
					}
					else {
						GestureObject gesture = new GestureObject (finger, type);
						gesture.Direction1 = dir;
						gestureQueue[finger] = gesture;
						checkOtherFingerGestures(gesture);
					}
				}
				else {
					GestureObject gesture = new GestureObject (finger, type);
					gestureQueue[finger] = gesture;
					checkOtherFingerGestures(gesture);
				}
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


	//The limits are the one imposed by the platform/device you run the app on.  
	//Swipes can be detected "per-finger" via the OnFingerSwipe event (or TBSwipe 
	//toolbox script). This allows you to detect multiple swipes at once, using the
	//"fingerIndex" value to identify which finger performed the gesture.
	void OnSwipe( SwipeGesture gesture ) {
		//FingerGestures.SwipeDirection direction = gesture.Direction;
		//UI.StatusText = "Swiped " + direction + " with finger " + gesture.Fingers[0];
		gestureTracker.updateList (gesture);
		UI.StatusText = gestureTracker.printOutCurrentPath();
		
	}
	
	void OnLongPress(LongPressGesture gesture) {
		//UI.StatusText = "Long Press with finger " + gesture.Fingers[0] + " at: " + gesture.Position;
		//SpawnParticles (gesture);
		gestureTracker.updateList (gesture);
		UI.StatusText = gestureTracker.printOutCurrentPath();}




//NOT CURRENTLY BEING USED METHODS

	void OnTap( TapGesture gesture ) {
		//UI.StatusText = "Tapped with finger " + gesture.Fingers[0] + " at: " + gesture.Position;
		gestureTracker.updateList (gesture);
		UI.StatusText = gestureTracker.printOutCurrentPath();}

	/*void SpawnParticles(LongPressGesture obj) {
		//ParticleSystem.Particle particle = new ParticleSystem.Particle ();
		//emitter.transform.position = obj.Position;
		//particle.position = obj.Position;
		//emitter.Emit (particle);
		//emitter.particleSystem.Play ();
		Vector3 pos = new Vector3 (obj.Position.x, obj.Position.y);
		GameObject emit = Instantiate (emitter, pos, Quaternion.identity) as GameObject;
		ParticleSystem p = emit.GetComponent<ParticleSystem> ();
		//p.Play ();
	}*/
	
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
		//gestureTracker.updateList (path);
	}
}
