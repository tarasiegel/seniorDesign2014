using UnityEngine;
using System.Collections;

public class typerLearningUI : MonoBehaviour {
	
		public GUISkin skin;
		GUIStyle textStyle;
		GUIStyle statusStyle;
		Rect promptTextRect = new Rect( 30, 156, 540, 100 );
		Rect statusTextRect = new Rect( 30, 336, 540, 100 );
		Rect backButtonRect = new Rect( 5, 2, 60, 35 );
		string statusText = "";
		string currentLetter = "";
		private FormattedLabel _formattedLabelText = null;
		int width = 8;
		Color color = Color.gray;
		
		public string StatusText {
			get { return statusText; }
			set { statusText = value; }
		}
		
		public string CurrentLetter {
			get { return currentLetter; }
			set { currentLetter = value;}
		}
		
		public bool showStatusText = true;
		
		void Awake()
		{
			textStyle = new GUIStyle( skin.label );
			textStyle.normal.textColor = Color.red;
			textStyle.alignment = TextAnchor.UpperCenter;
			textStyle.fontSize = 80;
			
			statusStyle = new GUIStyle( skin.label );
			statusStyle.alignment = TextAnchor.UpperCenter;
			statusStyle.fontSize = 30;
			
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
			GUI.depth = 1;
			if( GUI.Button( backButtonRect, "Back" ) )
				Application.LoadLevel( 0 );
			
			if (showStatusText) {
				GUI.TextArea(promptTextRect, currentLetter, textStyle);
			}
			GUI.Label(statusTextRect, statusText, statusStyle);
			
			Vector2 pointA = new Vector2(0, VirtualScreenHeight/2);
			Vector2 pointB = new Vector2(VirtualScreenWidth, VirtualScreenHeight/2);
			Vector2 pointC = new Vector2 (0, 0);
			Vector2 pointD = new Vector2 (0, VirtualScreenHeight*4);
			Drawing.DrawLine(pointA, pointB, color, width);
			Drawing.DrawLine (pointD, pointC, color, width);
			
		}
	}
