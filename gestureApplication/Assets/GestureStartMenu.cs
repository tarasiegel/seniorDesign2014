using UnityEngine;
using System.Collections;

public class GestureStartMenu : MonoBehaviour {

		public GUIStyle titleStyle;
		public GUIStyle buttonStyle;
		
		public float buttonHeight = 150;
		
		public Transform itemsTree;
		
		Transform currentMenuRoot;
		public Transform CurrentMenuRoot
		{
			get { return currentMenuRoot; }
			set { currentMenuRoot = value; }
		}
		
		// Use this for initialization
		void Start()
		{
		GUI.backgroundColor = Color.gray;
			CurrentMenuRoot = itemsTree;
			//buttonStyle = new GUIStyle (GUI.skin.button);
//			buttonStyle.fontSize = 18;
//			buttonStyle.normal.textColor = Color.white;
//			buttonStyle.onHover.textColor = Color.cyan;
//			buttonStyle.margin.bottom = 20;
//			buttonStyle.alignment = TextAnchor.MiddleCenter;

		}
		
		Rect screenRect = new Rect( 0, 0, SampleUI.VirtualScreenWidth, SampleUI.VirtualScreenHeight );
		public float menuWidth = 150;
		
		public float sideBorder = 200;
		
		void OnGUI()
		{
			SampleUI.ApplyVirtualScreen();
			
			GUILayout.BeginArea( screenRect );
			GUILayout.BeginHorizontal();
			
			GUILayout.Space( sideBorder );
			
			if( CurrentMenuRoot )
			{
				GUILayout.BeginVertical();
				
				GUILayout.Space( 5 );
				GUILayout.Label( CurrentMenuRoot.name, titleStyle );
				
				for( int i = 0; i < CurrentMenuRoot.childCount; ++i )
				{
					Transform item = CurrentMenuRoot.GetChild( i );
					
					if( GUILayout.Button( item.name, buttonStyle, GUILayout.Height( buttonHeight ) ) )
					{
						MenuNode menuNode = item.GetComponent<MenuNode>();
						if( menuNode && menuNode.sceneName != null && menuNode.sceneName.Length > 0 )
							Application.LoadLevel( menuNode.sceneName );
						else if( item.childCount > 0 )
							CurrentMenuRoot = item;
					}
					
					GUILayout.Space( 5 );
				}            
				
				GUILayout.FlexibleSpace();
				
				
				
				GUILayout.EndVertical();
			}
			
			GUILayout.Space( sideBorder );
			GUILayout.EndHorizontal();        
			GUILayout.EndArea();
		}
	}
