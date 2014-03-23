using UnityEngine;
using System.Collections;

public class gestureUI : MonoBehaviour {
		public GUISkin skin;
		GUIStyle statusStyle;
		Rect statusTextRect = new Rect( 30, 336, 540, 60 );
		string statusText = "";//"status text goes here";
		public string StatusText
		{
			get { return statusText; }
			set { statusText = value; }
		}
		
		public bool showStatusText = true;
		
		void Awake()
		{
			statusStyle = new GUIStyle( skin.label );
			statusStyle.alignment = TextAnchor.MiddleCenter;
		}
		
		#region Virtual Screen for automatic UI resolution scaling
		
		public static readonly float VirtualScreenWidth = 600;
		public static readonly float VirtualScreenHeight = 400;
		
		public static void ApplyVirtualScreen()
		{
			// resolution scaling matrix
			GUI.matrix = Matrix4x4.Scale( new Vector3( Screen.width / VirtualScreenWidth, Screen.height / VirtualScreenHeight, 1 ) );
		}
		
		#endregion
		
		protected virtual void OnGUI()
		{
			if( skin != null )
				GUI.skin = skin;
			
			ApplyVirtualScreen();
			
			if( showStatusText )
				GUI.Label(statusTextRect, statusText, statusStyle);
		}
	}