using UnityEngine;
using System.Collections;

public class gestureUI : MonoBehaviour {
		public GUISkin skin;
		GUIStyle statusStyle;
		Rect statusTextRect = new Rect( 30, 336, 540, 80 );
		Rect backButtonRect = new Rect( 5, 2, 60, 35 );
		string statusText = "";
		
		public string StatusText
		{
			get { return statusText; }
			set { statusText = value; }
		}

		public bool showStatusText = true;
		
		void Awake()
		{
			statusStyle = new GUIStyle( skin.label );
			statusStyle.alignment = TextAnchor.UpperCenter;
			statusStyle.fontSize = 18;

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
			if( GUI.Button( backButtonRect, "Back" ) )
				Application.LoadLevel( 0 );
			
			if( showStatusText )
				GUI.Label(statusTextRect, statusText, statusStyle);
		}
	}