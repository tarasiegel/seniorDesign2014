using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FingerMapObject {

	char l = ' ';
	char r = ' ';
	char lp = ' ';
	char rp = ' ';
	char pl = ' ';
	char pr = ' ';
	char t = ' ';
	
	public FingerMapObject() {

	}

	public FingerMapObject(	char leftSwipe, char rightSwipe, char leftSwipePress, char rightSwipePress, char pressLeftSwipe, char pressRightSwipe, char tap) {
		this.l = leftSwipe;
		this.r = rightSwipe;
		this.lp = leftSwipePress;
		this.rp = rightSwipePress;
		this.pl = pressLeftSwipe; 
		this.pr = pressRightSwipe;
		this.t = tap;
	}

	/// <summary>
	/// Left Swipe Gesture
	/// </summary>
	public char Left {
		get { return l; }
		set { l = value; }
	}

	/// <summary>
	/// Right Swipe Gesture
	/// </summary>
	public char Right {
		get { return r; }
		set { r = value; }
	}

	/// <summary>
	/// Left Swipe Gesture followed by a LongPress
	/// </summary>
	public char LeftPress {
		get { return lp; }
		set { lp = value; }
	}

	/// <summary>
	/// Right Swipe Gesture followed by a LongPress
	/// </summary>
	public char RightPress {
		get { return rp; }
		set { rp = value; }
	}

	/// <summary>
	/// LongPress Gesture followed by Left Swipe Gesture
	/// </summary>
	public char PressLeft {
		get { return pl; }
		set { pl = value; }
	}

	/// <summary>
	/// LongPress Gesture followed by Right Swipe Gesture
	/// </summary>
	public char PressRight {
		get { return pr; }
		set { pr = value; }
	}

	/// <summary>
	/// Tap Gesture
	/// </summary>
	public char Tap {
		get { return t; }
		set { t = value; }
	}
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
