using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class characterMap {

	private static Dictionary<GestureObject,char> charMap = new Dictionary<GestureObject, char >();
	// Use this for initialization

	public characterMap(FingerMapObject one, FingerMapObject two, FingerMapObject three, FingerMapObject four){
		populateCharMapping(one, two, three, four);
	}


	private void populateCharMapping(FingerMapObject one, FingerMapObject two, FingerMapObject three, FingerMapObject four){
		LongPressGesture lp = new LongPressGesture ();
		TapGesture t = new TapGesture ();
		SwipeGesture sl = new SwipeGesture ();
		sl.Direction = FingerGestures.SwipeDirection.Left;
		SwipeGesture sr = new SwipeGesture ();
		sr.Direction = FingerGestures.SwipeDirection.Right;
		charMap.Add (new GestureObject (1, t), one.Tap);
		charMap.Add (new GestureObject (1, sl, FingerGestures.SwipeDirection.Left), one.Left );
		charMap.Add (new GestureObject (1, sr, FingerGestures.SwipeDirection.Right), one.Right);
		charMap.Add (new GestureObject (1, sl, lp, FingerGestures.SwipeDirection.Left), one.LeftPress);
		charMap.Add (new GestureObject (1, sr, lp, FingerGestures.SwipeDirection.Right), one.RightPress);
		charMap.Add (new GestureObject (1, lp, sl, FingerGestures.SwipeDirection.Left), one.PressLeft);
		charMap.Add (new GestureObject (1, lp, sr, FingerGestures.SwipeDirection.Right), one.PressRight);
		charMap.Add (new GestureObject (2, t), two.Tap);
		charMap.Add (new GestureObject (2, sl, FingerGestures.SwipeDirection.Left), two.Left);
		charMap.Add (new GestureObject (2, sr, FingerGestures.SwipeDirection.Right), two.Right);
		charMap.Add (new GestureObject (2, sl, lp, FingerGestures.SwipeDirection.Left), two.LeftPress);
		charMap.Add (new GestureObject (2, sr, lp, FingerGestures.SwipeDirection.Right), two.RightPress);
		charMap.Add (new GestureObject (2, lp, sl, FingerGestures.SwipeDirection.Left), two.PressLeft);
		charMap.Add (new GestureObject (2, lp, sr, FingerGestures.SwipeDirection.Right), two.PressRight);
		charMap.Add (new GestureObject (3, t), three.Tap);
		charMap.Add (new GestureObject (3, sl, FingerGestures.SwipeDirection.Left), three.Left);
		charMap.Add (new GestureObject (3, sr, FingerGestures.SwipeDirection.Right), three.Right);
		charMap.Add (new GestureObject (3, sl, lp, FingerGestures.SwipeDirection.Left), three.LeftPress);
		charMap.Add (new GestureObject (3, sr, lp, FingerGestures.SwipeDirection.Right), three.RightPress);
		charMap.Add (new GestureObject (3, lp, sl, FingerGestures.SwipeDirection.Left), three.PressLeft);
		charMap.Add (new GestureObject (3, lp, sr, FingerGestures.SwipeDirection.Right), three.PressRight);
		charMap.Add (new GestureObject (4, t), four.Tap);
		charMap.Add (new GestureObject (4, sl, FingerGestures.SwipeDirection.Left), four.Left);
		charMap.Add (new GestureObject (4, sr, FingerGestures.SwipeDirection.Right), four.Right);
		charMap.Add (new GestureObject (4, sl, lp, FingerGestures.SwipeDirection.Left), four.LeftPress);
		charMap.Add (new GestureObject (4, sr, lp, FingerGestures.SwipeDirection.Right), four.RightPress);
		charMap.Add (new GestureObject (4, lp, sl, FingerGestures.SwipeDirection.Left), four.PressLeft);
		charMap.Add (new GestureObject (4, lp, sr, FingerGestures.SwipeDirection.Right), four.PressRight);
		
	}
	
	public char getCharacter(GestureObject gest) {
		foreach (GestureObject currGest in charMap.Keys) {
			if (currGest.Finger == gest.Finger) {
				if ((currGest.Gesture1.ToString() == gest.Gesture1.ToString()) && ((currGest.Direction1.ToString() == gest.Direction1.ToString()) || gest.Direction1.ToString() == null)) {
					if (gest.Gesture2 != null) {
						if (currGest.Gesture2 != null){
							if ((currGest.Gesture2.ToString() == gest.Gesture2.ToString()) && ((currGest.Direction2.ToString() == gest.Direction2.ToString()) || gest.Direction2.ToString() == null)) {
								return charMap [currGest];
							}
						}
					} 
					else {
						return charMap [currGest];
					}
				}
			}
		}
		return ' ';
	}



	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	

}
