using UnityEngine;
using System.Collections;


public class typingGameUI : MonoBehaviour {
		public GUISkin skin;
		GUIStyle textStyle;
		Rect promptTextRect = new Rect( 30, 136, 540, 100 );
		Rect statusTextRect = new Rect( 30, 336, 540, 100 );
		Rect backButtonRect = new Rect( 5, 2, 60, 35 );
		string statusText = "";
		string textFinished = "";
		string currentLetter = "";
		string textLeft = "";
		private FormattedLabel _formattedLabelText = null;
		

		public string StatusText {
			get { return statusText; }
			set { statusText = value; }
		}

		public string FinishedText {
			get { return textFinished; }
			set { textFinished = value; }
		}

		public string CurrentLetter {
				get { return currentLetter; }
				set { currentLetter = value;}
		}
		public string TextLeft {
				get { return textLeft; }
				set { textLeft = value; }
		}
		
		public bool showStatusText = true;
		
		void Awake()
		{
			textStyle = new GUIStyle( skin.label );
			textStyle.alignment = TextAnchor.UpperCenter;
			textStyle.fontSize = 30;
			textStyle.richText = true;
			
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
				
			if (showStatusText) {
				GUI.TextArea(promptTextRect, "<color=black>"+ textFinished +"</color><color=red>"+ currentLetter +"</color><color=white>"+ textLeft +"</color>" , textStyle);
				
			}
			
			GUI.Label(statusTextRect, statusText, textStyle);

		}
}
