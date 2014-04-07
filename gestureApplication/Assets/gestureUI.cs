using UnityEngine;
using System.Collections;

public class gestureUI : MonoBehaviour {
		public GUISkin skin;
		GUIStyle statusStyle;
		Rect statusTextRect = new Rect( 30, 336, 540, 80 );
		string statusText = "";//"status text goes here";
		//GUIStyle textStyle;
		//Rect sententceTextRect = new Rect (30, 400, 300, 60);
		//string sentenceText = "";
		public string StatusText
		{
			get { return statusText; }
			set { statusText = value; }
		}
		//public string SentenceText
		//{
		//	get { return sentenceText; }
		//	set { sentenceText = value; }
		//}
		
		public bool showStatusText = true;
		//public bool showSentenceText = true;
		
		void Awake()
		{
			statusStyle = new GUIStyle( skin.label );
			statusStyle.alignment = TextAnchor.UpperCenter;
			statusStyle.fontSize = 18;

			//textStyle = new GUIStyle (skin.label);
			//textStyle.alignment = TextAnchor.UpperCenter;
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
			//if( showSentenceText )
			//	GUI.Label(sententceTextRect, sentenceText, textStyle);
		}
	}