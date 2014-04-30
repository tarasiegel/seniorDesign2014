using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

[RequireComponent( typeof( FingerDownDetector ) )]
[RequireComponent( typeof( FingerMotionDetector ) )]
[RequireComponent( typeof( FingerUpDetector ) )]
[RequireComponent( typeof( ScreenRaycaster ) )]

public class typerLearningScript : typerLearningBase {

		public GameObject swipeLeftPressObject;
		public GameObject swipeRightPressObject;
		public GameObject pressSwipeLeftObject;
		public GameObject pressSwipeRightObject;
		public GameObject swipeRightObject;
		public GameObject swipeLeftObject;
		public GameObject pressObject;
		public List<GameObject> objectList;
		
		
		
		class GestureTracker {
			
			GestureObject[] gestureQueue = new GestureObject[5];
			List<GestureObject> gestureSequence = new List<GestureObject>();
			characterMap charMap;
			int lastFinger = 0;
			int lastLastFinger = 0;
			bool correctGest;
			char currentLetter = '-';
			
			public characterMap CharMap{
				get { return charMap; }
			}	
			public bool CorrectGesture{
				get { return correctGest; }
				set { correctGest = value; }
			}	

			public char CurrentLetter{
				get { return currentLetter; }
				set { currentLetter = value; }
			}	
				
			public GestureTracker() {
				//FingerMapObject sam = new FingerMapObject('L', 'R', 'LP', 'RP', 'PL', 'PR', 'T')
				FingerMapObject one = new FingerMapObject('t', 'd', 'v', 'o', 'f', 'm', 's');
				FingerMapObject two = new FingerMapObject('y', 'n', 'w', 'j', 'i', 'c', 'r');
				FingerMapObject three = new FingerMapObject('h', 'a', 'q', 'b', 'l', 'u', 'e');
				FingerMapObject four = new FingerMapObject('g', '<', 'z', 'x', 'p', 'k', ' ');
				charMap = new characterMap(one, two, three, four);
				//prompt = new promptScript();
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
					} 
				}
			}

			
			private void addToGestureSequence(GestureObject g){
				char letter = charMap.getCharacter (g);
				if (letter == '<') {
					if (gestureSequence.Count == 1){
						gestureSequence.Clear();
					}
					else if (gestureSequence.Count != 0){
						gestureSequence.RemoveAt (gestureSequence.Count - 1);
					}
					else {
					}
					
				}
				else if (!g.HasBeenAddedYet){
					g.Letter = letter;
					gestureSequence.Add (g);
					if (letter == currentLetter || (letter == ' ' && currentLetter == '_')) {
						correctGest = true;
					}
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
							GestureObject newg = new GestureObject(finger, type, l, 1);
							newg.Direction1 = dir;
							newg.PreviousFinger = lastFinger;
							//gestureQueue[finger] = null;
							checkOtherFingerGestures(newg);
							addToGestureSequence(newg);
						}
						//else override it as the gesture for that 
						else {
							Debug.Log("adding 2nd gesture, direction= " + dir);
							GestureObject newg2 = new GestureObject(finger, type, 0);
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
							GestureObject newg = new GestureObject(finger, type, 0);
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
							addGestureWithDirection1(finger, type, dir);
							//checkOtherFingerGestures(g);
						}
						else {
							addGestureWithDirection1(finger, type, dir);
						}
					}
					else {
						addGestureTap(finger, type);
						//checkOtherFingerGestures(g);
					}
				}
				//return gestureQueue;
			}
			
			
			void addGestureWithDirection1(int finger, DiscreteGesture type, FingerGestures.SwipeDirection dir = FingerGestures.SwipeDirection.Left){
			GestureObject gesture = new GestureObject (finger, type, 0);
				gesture.Direction1 = dir;
				gesture.PreviousFinger = lastFinger;
				gestureQueue[finger] = gesture;
				checkOtherFingerGestures(gesture);
			}
			void addGestureTap(int finger, DiscreteGesture type){
				GestureObject gesture = new GestureObject (finger, type, 0);
				gesture.PreviousFinger = lastFinger;
				gestureQueue[finger] = gesture;
				checkOtherFingerGestures(gesture);
			}
			
		}
		GestureTracker gestureTracker;
		
		Vector3 finger1Location = new Vector3(-6,5,0);
		Vector3 finger2Location = new Vector3(-6,-5,0);
		Vector3 finger3Location = new Vector3(6,5,0);
		Vector3 finger4Location = new Vector3(6,-5,0);
		Vector3 dirPos; GameObject dirObj; GameObject currObj; GameObject currentArrow;

//		public class TypeObjectTracker{
//
//		List<AlphabetObject> alphabetList;
//
//			public TypeObjectTracker(){
//				alphabetList = new List<AlphabetObject>();
//				populateAlphaList();
//			}
//			private void populateAlphaList(){
//				for (char c = 'a'; c <= 'z'; c++){
//					
//
//				AlphabetObject letter = new AlphabetObject();
//
//
//				} 
//				
//			}
//
//
//
//
//		}
//
//		private class AlphabetObject {
//			GameObject prefab;
//			char letter;
//			AlphabetObject(GameObject p, char l){
//				prefab = p;
//				letter = l;
//			}
//		}


		// Use this for initialization
		void Start () {
			objectList.Add(swipeLeftObject);
			objectList.Add(swipeRightObject);
			objectList.Add(swipeLeftPressObject);
			objectList.Add(swipeRightPressObject);
			objectList.Add(pressSwipeLeftObject);
			objectList.Add(pressSwipeRightObject);
			objectList.Add(pressObject);
			gestureTracker = new GestureTracker();
			updateStateOfGame ();
			
		}
		
		
		// Update is called once per frame
		void Update () {
			base.Start();
		}
		

	//  FingerMapObject sam = new FingerMapObject('L', 'R', 'LP', 'RP', 'PL', 'PR', 'T')
	//	FingerMapObject one = new FingerMapObject('t', 'd', 'v', 'o', 'f', 'm', 's');
	//	FingerMapObject two = new FingerMapObject('y', 'n', 'w', 'j', 'i', 'c', 'r');
	//	FingerMapObject three = new FingerMapObject('h', 'a', 'q', 'b', 'l', 'u', 'e');
	//	FingerMapObject four = new FingerMapObject('g', '<', 'z', 'x', 'p', 'k', ' ');
		void updateStateOfGame(){

			if (gestureTracker.CurrentLetter == '-') {
				gestureTracker.CurrentLetter = 'a';
				dirPos = determineLocation(gestureTracker.CurrentLetter);
				dirObj = determineObj(gestureTracker.CurrentLetter);
				currentArrow = Instantiate(dirObj, dirPos, Quaternion.identity) as GameObject;
				UI.CurrentLetter = gestureTracker.CurrentLetter.ToString();
				gestureTracker.CorrectGesture = false;
			}
			else if (gestureTracker.CurrentLetter <= 'z' && gestureTracker.CorrectGesture) {
				Destroy(currentArrow.gameObject);
				gestureTracker.CurrentLetter++;
				dirPos = determineLocation(gestureTracker.CurrentLetter);
				dirObj = determineObj(gestureTracker.CurrentLetter);
				currentArrow = Instantiate(dirObj, dirPos, Quaternion.identity) as GameObject;
				UI.CurrentLetter = gestureTracker.CurrentLetter.ToString();
				gestureTracker.CorrectGesture = false;
			} 
			else {
				endGame();
			}
		}
		
		Vector3 determineLocation(char l){
			int finger = gestureTracker.CharMap.getFinger (l);
			if (finger == 1) {
					return finger1Location;
			} else if (finger == 2) {
					return finger2Location;
			} else if (finger == 3) {
					return finger3Location;
			} else if (finger == 4) {
					return finger4Location;
			} else {
					return new Vector3 (0.0f, 0.0f);
			}
		}
		
		GameObject determineObj(char l){
			int objNumber = gestureTracker.CharMap.getGesture (l);
			return objectList [objNumber - 1];
		}


		void endGame(){
			UI.StatusText = "DONE WITH TEST";
		}
		
		void OnSwipe( SwipeGesture gesture ) {
			gestureTracker.updateList (gesture);
			updateStateOfGame ();
			UI.StatusText = gestureTracker.printOutCurrentPath();	
		}
		
		void OnLongPress(LongPressGesture gesture) {
			gestureTracker.updateList (gesture);
			updateStateOfGame ();
			UI.StatusText = gestureTracker.printOutCurrentPath();
		}
		
		void OnTap( TapGesture gesture ) {
			gestureTracker.updateList (gesture);
			updateStateOfGame ();
			UI.StatusText = gestureTracker.printOutCurrentPath();
		}
		
	}
