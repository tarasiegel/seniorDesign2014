using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;



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

	class GestureTracker {

		GestureObject[] gestureQueue = new GestureObject[5];
		List<GestureObject> gestureSequence = new List<GestureObject>();
		characterMap charMap;
		int lastFinger = 0;
		int lastLastFinger = 0;
		bool justSawBackspace = false;


		public GestureTracker() {
	     //FingerMapObject sam = new FingerMapObject('L', 'R', 'LP', 'RP', 'PL', 'PR', 'T')
			FingerMapObject one = new FingerMapObject('t', 'd', 'v', 'o', 'f', 'm', 's');
			FingerMapObject two = new FingerMapObject('y', 'n', 'w', 'j', 'i', 'c', 'r');
			FingerMapObject three = new FingerMapObject('h', 'a', 'q', 'b', 'l', 'u', 'e');
			FingerMapObject four = new FingerMapObject('g', '<', 'z', 'x', 'p', 'k', ' ');
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
			lastLastFinger = lastFinger;
			lastFinger = determineFinger (tap);
			Debug.Log (lastFinger);
		}

		public void updateList(LongPressGesture press) {
			addGesture (press);
			lastLastFinger = lastFinger;
			lastFinger = determineFinger (press);
			Debug.Log (lastFinger);
		}

		public void updateList(SwipeGesture swipe) {
			addGesture (swipe, swipe.Direction);
			lastLastFinger = lastFinger;
			lastFinger = determineFinger (swipe);
			Debug.Log (lastFinger);
		}


		private int determineFinger(DiscreteGesture type) {
			Vector2 currPos = type.StartPosition;
			float w = Screen.width;
			float h = Screen.height;
			/*Debug.Log (justSawBackspace);
			if (justSawBackspace) {
				if (gestureSequence.Count == 0) {
					return 0;
				}
				else {
					return lastFinger;
				}
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
			GestureObject lastG;
			//float lastTime = g.Gesture1.StartTime;
			if (gestureSequence.Count == 0) {
				Debug.Log ("all are deleted");
				addToGestureSequence (g);
			} 
			else {
				Debug.Log ("last finger: " + lastFinger + ", current finger: " + g.Finger);
				if (g.Finger != lastFinger && lastFinger != 0) {
					lastG = gestureQueue [lastFinger];
					if (lastG != null) {
						float lastT = 0;
						float currT = 0;
						if (!lastG.hasSecondGesture ()) {
							lastT = lastG.Gesture1.StartTime + lastG.Gesture1.ElapsedTime;
						} 
						else {
							lastT = lastG.Gesture2.StartTime + lastG.Gesture2.ElapsedTime;
						}

						if (!g.hasSecondGesture ()) {
							currT = g.Gesture1.StartTime;
						} 
						else {
							currT = g.Gesture2.StartTime;
						}

						if (Mathf.Abs (currT - lastT) > 0.05f) {
							//gestureQueue [lastFinger] = null;
							addToGestureSequence (lastG);
						}
					}
				} //else if (lastFinger == 0) {
					//	addToGestureSequence (g);
				//}
						}
		}


				//if (currG.Gesture2 == null && t < gTime) {
					//gestureQueue[lastFinger] = null;
					//addToGestureSequence (currG);
				//}
				
			//}
//			for (int i = 0; i < gestureQueue.Length; i++) {
//				if (g.Finger != i && gestureQueue[i] != null) {
//					currG = gestureQueue[i];
//					float t = currG.Gesture1.StartTime + currG.Gesture1.ElapsedTime;
//					if (currG.Gesture2 == null && t < gTime) {
//						gestureQueue[i] = null;
//						addToGestureSequence (currG);
//					}
//				}
//			}
//		}
		
		private void addToGestureSequence(GestureObject g){
			char letter = charMap.getCharacter (g);
			if (letter == '<') {
				if (gestureSequence.Count == 1){
					gestureSequence.Clear();
					justSawBackspace = true;
				}
				else if (gestureSequence.Count != 0){
					gestureSequence.RemoveAt (gestureSequence.Count - 1);
					justSawBackspace = true;
				}
				else {
				}

			}
			else if (!g.HasBeenAddedYet){
				justSawBackspace = false;
				g.Letter = letter;
				gestureSequence.Add (g);
				gestureQueue[g.Finger].HasBeenAddedYet = true;
			}
		}

		private void addGesture(DiscreteGesture type, FingerGestures.SwipeDirection dir = FingerGestures.SwipeDirection.Left) {
			bool gestureStarted = false;
			int finger = determineFinger (type);
			Debug.Log ("Adding Gesture, finger: " + finger);
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
					Debug.Log ("saw tap or same gesture right before");
					//seen same gesture as before
					GestureObject g = gestureQueue[finger];
					checkOtherFingerGestures(g);
					addToGestureSequence(g);
					//just in case it is a swipe --> press
					if (type.ToString() == "SwipeGesture" && type.LongPressAfterSwipe == true){
						Debug.Log ("getting into longpress + swipe");
						LongPressGesture l = new LongPressGesture();
						l.Position = type.Position;
						GestureObject newg = new GestureObject(finger, type, l);
						newg.Direction1 = dir;
						newg.PreviousFinger = lastFinger;
						//gestureQueue[finger] = null;
						checkOtherFingerGestures(newg);
						addToGestureSequence(newg);
					}
					//else override it as the gesture for that 
					else {
						Debug.Log("adding 2nd gesture, direction= " + dir);
						GestureObject newg2 = new GestureObject(finger, type);
						newg2.PreviousFinger = lastFinger;
						if (type.ToString() == "SwipeGesture"){
							Debug.Log(dir);
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
						Debug.Log("swipe + longpress");
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
						newg.PreviousFinger = lastFinger;
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
				Debug.Log("first gesture for finger");
				if (type.ToString() == "SwipeGesture") {
					if (type.LongPressAfterSwipe == true){
						LongPressGesture l = new LongPressGesture();
						l.Position = type.Position;
						GestureObject g = addGestureWithDirection1(finger, type, dir);
						//checkOtherFingerGestures(g);
					}
					else {
						GestureObject g = addGestureWithDirection1(finger, type, dir);
					}
				}
				else {
					GestureObject g = addGestureTap(finger, type);
					//checkOtherFingerGestures(g);
				}
			}
			//return gestureQueue;
		}


		GestureObject addGestureWithDirection1(int finger, DiscreteGesture type, FingerGestures.SwipeDirection dir = FingerGestures.SwipeDirection.Left){
			GestureObject gesture = new GestureObject (finger, type);
			gesture.Direction1 = dir;
			gesture.PreviousFinger = lastFinger;
			gestureQueue[finger] = gesture;
			checkOtherFingerGestures(gesture);
			return gesture;
		}
		GestureObject addGestureTap(int finger, DiscreteGesture type){
			GestureObject gesture = new GestureObject (finger, type);
			gesture.PreviousFinger = lastFinger;
			gestureQueue[finger] = gesture;
			checkOtherFingerGestures(gesture);
			return gesture;
		}
	
	}
	GestureTracker gestureTracker = new GestureTracker();
	
	// Use this for initialization
	//void Start () {
	//}


	// Update is called once per frame
	void Update () {
		base.Start();

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
		UI.StatusText = gestureTracker.printOutCurrentPath();
	}

	void OnTap( TapGesture gesture ) {
		//UI.StatusText = "Tapped with finger " + gesture.Fingers[0] + " at: " + gesture.Position;
		gestureTracker.updateList (gesture);
		UI.StatusText = gestureTracker.printOutCurrentPath();
	}

}
