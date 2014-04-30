using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class characterMap {

	protected static Dictionary<GestureObject,char> charMap = new Dictionary<GestureObject, char >();
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
		charMap.Add (new GestureObject (1, t, 7), one.Tap);
		charMap.Add (new GestureObject (1, sl, FingerGestures.SwipeDirection.Left, 1), one.Left );
		charMap.Add (new GestureObject (1, sr, FingerGestures.SwipeDirection.Right, 2), one.Right);
		charMap.Add (new GestureObject (1, sl, lp, FingerGestures.SwipeDirection.Left, 3), one.LeftPress);
		charMap.Add (new GestureObject (1, sr, lp, FingerGestures.SwipeDirection.Right, 4), one.RightPress);
		charMap.Add (new GestureObject (1, lp, sl, FingerGestures.SwipeDirection.Left, 5), one.PressLeft);
		charMap.Add (new GestureObject (1, lp, sr, FingerGestures.SwipeDirection.Right, 6), one.PressRight);
		charMap.Add (new GestureObject (2, t, 7), two.Tap);
		charMap.Add (new GestureObject (2, sl, FingerGestures.SwipeDirection.Left, 1), two.Left);
		charMap.Add (new GestureObject (2, sr, FingerGestures.SwipeDirection.Right, 2), two.Right);
		charMap.Add (new GestureObject (2, sl, lp, FingerGestures.SwipeDirection.Left, 3), two.LeftPress);
		charMap.Add (new GestureObject (2, sr, lp, FingerGestures.SwipeDirection.Right, 4), two.RightPress);
		charMap.Add (new GestureObject (2, lp, sl, FingerGestures.SwipeDirection.Left, 5), two.PressLeft);
		charMap.Add (new GestureObject (2, lp, sr, FingerGestures.SwipeDirection.Right, 6), two.PressRight);
		charMap.Add (new GestureObject (3, t, 7), three.Tap);
		charMap.Add (new GestureObject (3, sl, FingerGestures.SwipeDirection.Left, 1), three.Left);
		charMap.Add (new GestureObject (3, sr, FingerGestures.SwipeDirection.Right, 2), three.Right);
		charMap.Add (new GestureObject (3, sl, lp, FingerGestures.SwipeDirection.Left, 3), three.LeftPress);
		charMap.Add (new GestureObject (3, sr, lp, FingerGestures.SwipeDirection.Right, 4), three.RightPress);
		charMap.Add (new GestureObject (3, lp, sl, FingerGestures.SwipeDirection.Left, 5), three.PressLeft);
		charMap.Add (new GestureObject (3, lp, sr, FingerGestures.SwipeDirection.Right, 6), three.PressRight);
		charMap.Add (new GestureObject (4, t, 7), four.Tap);
		charMap.Add (new GestureObject (4, sl, FingerGestures.SwipeDirection.Left, 1), four.Left);
		charMap.Add (new GestureObject (4, sr, FingerGestures.SwipeDirection.Right, 2), four.Right);
		charMap.Add (new GestureObject (4, sl, lp, FingerGestures.SwipeDirection.Left, 3), four.LeftPress);
		charMap.Add (new GestureObject (4, sr, lp, FingerGestures.SwipeDirection.Right, 4), four.RightPress);
		charMap.Add (new GestureObject (4, lp, sl, FingerGestures.SwipeDirection.Left, 5), four.PressLeft);
		charMap.Add (new GestureObject (4, lp, sr, FingerGestures.SwipeDirection.Right, 6), four.PressRight);
		
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

	public int getFinger(char c){
		foreach (var <GestureObject,char> k in charMap) {
			if(k.Value == c){
				GestureObject g = k.Key;
				return g.Finger;
			}
		}
		return 0;
	}

	public int getGesture(char c){
		foreach (var <GestureObject,char> k in charMap) {
			if(k.Value == c){
				GestureObject g = k.Key;
				return g.Type;
			}
		}
		return 0;
	}



	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	

}
