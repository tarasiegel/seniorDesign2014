﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureObject {

	int finger = 0;
	DiscreteGesture gesture1 = null;
	DiscreteGesture gesture2 = null;
	FingerGestures.SwipeDirection gestureDirection1;
	FingerGestures.SwipeDirection gestureDirection2;
	char letter;
	
	
	public GestureObject( int finger, DiscreteGesture gesture1, DiscreteGesture gesture2) {
		this.finger = finger;
		this.gesture1 = gesture1;
		this.gesture2 = gesture2;
	}

	public GestureObject( int finger, DiscreteGesture gesture1, DiscreteGesture gesture2, FingerGestures.SwipeDirection dir) {
		this.finger = finger;
		this.gesture1 = gesture1;
		this.gesture2 = gesture2;
		if (gesture1.ToString() == "SwipeGesture") {
			this.Direction1 = dir;
		} 
		if (gesture2.ToString() == "SwipeGesture") {
			this.Direction2 = dir;
		}
	}
	
	public GestureObject(int finger) {
		this.finger = finger;
	}
	
	public GestureObject(int finger, DiscreteGesture gesture1) {
		this.finger = finger;
		this.gesture1 = gesture1;
		this.gesture2 = null;
	}

	public GestureObject(int finger, DiscreteGesture gesture1, FingerGestures.SwipeDirection dir) {
		this.finger = finger;
		this.gesture1 = gesture1;
		this.gesture2 = null;
		this.Direction1 = dir;
	}
	
	/// <summary>
	/// First Gesture seen
	/// </summary>
	public DiscreteGesture Gesture1 {
		get { return gesture1; }
		set { gesture1 = value; }
	}

	/// <summary>
	/// Second Gesture seen
	/// </summary>
	public DiscreteGesture Gesture2 {
		get { return gesture2; }
		set { gesture2 = value; }
	}

	/// <summary>
	/// Finger position 
	/// </summary>
	public int Finger {
		get { return finger; }
		set { finger = value; }
	}

	/// <summary>
	/// Letter associated with the overall gesture. Set after gesture is finished
	/// </summary>
	public char Letter {
		get { return letter; }
		set { letter = value; }
	}

	/// <summary>
	/// Direction of first gesture if it was a swipe
	/// </summary>
	public FingerGestures.SwipeDirection Direction1 {
		get { return gestureDirection1; }
		internal set { gestureDirection1 = value; }
	}

	/// <summary>
	/// Direction of second gesture if it was a swipe
	/// </summary>
	public FingerGestures.SwipeDirection Direction2 {
		get { return gestureDirection2; }
		internal set { gestureDirection2 = value; }
	}
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}