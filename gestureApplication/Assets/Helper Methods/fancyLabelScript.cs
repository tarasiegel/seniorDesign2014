//**************************************************************************

//

// Project Name: FancyLabel2

//

// Purpose: To allow Unity users to create GUI elements that can change

//          style partway through. Modified from another script to

//          include word wrap.

//

// URL of original program: [url]http://forum.unity3d.com/viewtopic.php?t=10175&start=0&postdays=0&postorder=asc&highlight=[/url]

//

// Notes: There is a variable called avgPpC (AVErage Pixels Per Character)

//        that needs to be changed to suit your font size/style.

//

// Author: Nathan Andre

//

// Email: na203602@ohio.edu

//

//**************************************************************************





using UnityEngine;

using System.Collections;



public class atextscript : MonoBehaviour {
	
	
	
	public GUISkin skin;
	
	
	
	// Use this for initialization
	
	void Start()
		
	{
		
		
		
	}
	
	
	
	// Update is called once per frame
	
	void Update()
		
	{
		
		
		
	}
	
	
	
	int HexStringToInt(string hexString)
		
	{ 
		
		int value = 0; 
		
		int digitValue = 1; 
		
		hexString = hexString.ToUpper(); 
		
		char[] hexDigits = hexString.ToCharArray(0, hexString.Length); 
		
		
		
		for(int i = hexString.Length - 1; i >= 0; i--)
			
		{ 
			
			int digit = 0; 
			
			if (hexDigits[i] >= '0' && hexDigits[i] <= '9')
				
			{ 
				
				digit = hexDigits[i] - '0'; 
				
			}
			
			else if(hexDigits[i] >= 'A' && hexDigits[i] <= 'F')
				
			{ 
				
				digit = hexDigits[i] - 'A' + 10;             
				
			}
			
			else
				
			{ 
				
				// Not a hex string 
				
				return -1; 
				
			}
			
			
			
			value += digit * digitValue; 
			
			digitValue *= 16; 
			
		} 
		
		
		
		return value; 
		
	} 
	
	
	
	
	
	
	
	public void FancyLabel2(Rect rect, string text, Font normalFont, Font boldFont, Font italicFont, TextAlignment alignment)
		
	{
		
		//bool   leftSpace = false, rightSpace = false, topSpace = false, bottomSpace = false; 
		
		
		
		Color    textColor = GUI.skin.GetStyle("Label").normal.textColor; 
		
		Font    defaultFont = GUI.skin.font;
		
		Font   newFont = null; 
		
		
		
		//GUIStyle fontStyle = new GUIStyle(testStyle); 
		
		GUIStyle fontStyle = new GUIStyle();
		
		fontStyle.normal.textColor = textColor;
		
		
		
		// Start with normal font 
		
		if(normalFont != null)
			
		{ 
			
			fontStyle.font = normalFont; 
			
		}
		
		else
			
		{ 
			
			fontStyle.font = defaultFont;          
			
		}
		
		
		
		// NOTE: Lowering this padding reduces the line spacing 
		
		// May need to adjust per font 
		
		fontStyle.padding.bottom = -5; 
		
		
		
		GUILayout.BeginArea(rect); 
		
		GUILayout.BeginVertical(GUILayout.ExpandHeight(true), 
		                        
		                        GUILayout.Width(rect.height), 
		                        
		                        GUILayout.MinWidth(rect.height)); 
		
		GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), 
		                          
		                          GUILayout.Width(rect.width), 
		                          
		                          GUILayout.MinWidth(rect.width));
		
		
		
		// Insert flexible space on the left if Center or Right aligned 
		
		if(alignment == TextAlignment.Right || alignment == TextAlignment.Center)
			
		{ 
			
			GUILayout.FlexibleSpace(); 
			
		}
		
		
		
		int newline = 0;
		
		bool clearBuffer = false;
		
		double pixelsPerLine = 0;
		
		double avgPpC = 8.5;
		
		
		
		text = text.Replace("\n", "");
		
		text = text.Replace("\r", "");
		
		
		
		string[] toks = text.Split(' ');
		
		string output = "";
		
		
		
		for(int i=0; i<toks.Length; ++i)
			
		{
			
			//Add a leading space if you need one
			
			if(i != 0)
				
			{
				
				output += " ";
				
				pixelsPerLine += avgPpC;
				
			}
			
			
			
			int index = 0;
			
			while(index < toks[i].Length)
				
			{
				
				if(toks[i][index] == '\\')
					
				{
					
					//Must be an escape character
					
					if(toks[i][index+1] == 'n')
						
					{
						
						++newline;
						
					}
					
					else if(toks[i][index+1] == '#')
						
					{
						
						output += "#";
						
					}
					
					
					
					index += 2;
					
				}
				
				else if(toks[i][index] == '#')
					
				{
					
					//Must be a style sequence
					
					//The style will probably change, so might as well start a new label
					
					clearBuffer = true;
					
					
					
					if(toks[i][index+1] == '!')
						
					{
						
						//Original Color
						
						textColor = GUI.skin.GetStyle("Label").normal.textColor;
						
					}
					
					else if(toks[i][index+1] == 'n')
						
					{
						
						//Normal Font
						
						if (normalFont != null)
							
							newFont = normalFont;
						
					}
					
					else if(toks[i][index+1] == 'x')
						
					{
						
						//Bold Font
						
						if (boldFont != null)
							
							newFont = boldFont;
						
					}
					
					else if(toks[i][index+1] == 'i')
						
					{
						
						//Italic Font
						
						if (italicFont != null)
							
							newFont = italicFont;
						
					}
					
					else
						
					{
						
						//Must be a color change
						
						string rText = toks[i].Substring(index + 1, 2);
						
						string gText = toks[i].Substring(index + 3, 2);
						
						string bText = toks[i].Substring(index + 5, 2);
						
						string aText = toks[i].Substring(index + 7, 2);
						
						
						
						float r = HexStringToInt(rText) / 255.0f; 
						
						float g = HexStringToInt(gText) / 255.0f; 
						
						float b = HexStringToInt(bText) / 255.0f; 
						
						float a = HexStringToInt(aText) / 255.0f; 
						
						
						
						if(r < 0 || g < 0 || b < 0 || a < 0)
							
						{ 
							
							Debug.Log("Invalid color sequence");
							
							return;
							
						} 
						
						
						
						textColor = new Color(r, g, b, a);
						
						index += 7;
						
					}
					
					
					
					index += 2;
					
				}
				
				else
					
				{
					
					//Must just be a regular string
					
					//Check to see if a new line is needed, then go ahead and add the text to the string
					
					int index2, firstFormat, firstEscape = 0;
					
					firstFormat = toks[i].IndexOf("#", index);
					
					firstEscape = toks[i].IndexOf("\\", index);
					
					//if(firstFormat != -1 && (firstFormat < firstEscape && firstEscape != -1))
					
					if(firstFormat != -1 && (firstEscape!=-1?firstFormat<firstEscape:true))
						
					{
						
						index2 = firstFormat;
						
					}
					
					else if(firstEscape != -1)
						
					{
						
						index2 = firstEscape;
						
					}
					
					else
						
					{
						
						index2 = toks[i].Length;
						
					}
					
					
					
					//Check to see if the words need to wrap
					
					if((pixelsPerLine + (index2 - index)*avgPpC) >= rect.width)
						
					{
						
						if(newline == 0) newline = 1;
						
					}
					
					
					
					//Check to see if you need to make a label
					
					if(clearBuffer || newline > 0)
						
					{
						
						//Clear the buffer if the style changes or there is a newline
						
						GUILayout.Label(output, fontStyle);
						
						
						
						//Add in trailing spaces
						
						int spaces = output.Length - output.TrimEnd(' ').Length;
						
						//Might have to adjust this constant
						
						GUILayout.Space(spaces * 5.0f);
						
						//And also count that space in the pixel size of the buffer...
						
						pixelsPerLine += spaces*avgPpC;
						
						
						
						//Clear the buffer and cleanup
						
						output = "";
						
						clearBuffer = false;
						
						fontStyle.normal.textColor = textColor;
						
						if(newFont != null)
							
						{ 
							
							fontStyle.font = newFont; 
							
							newFont = null; 
							
						}
						
					}
					
					
					
					//You might have multiple newlines since the last label was created
					
					//ie if you do multiple newlines in a row
					
					while(newline > 0)
						
					{
						
						//Create a new line by ending the horizontal layout 
						
						if(alignment == TextAlignment.Left || alignment == TextAlignment.Center)
							
						{ 
							
							GUILayout.FlexibleSpace(); 
							
						} 
						
						GUILayout.EndHorizontal(); 
						
						GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), 
						                          
						                          GUILayout.Width(rect.width), 
						                          
						                          GUILayout.MinWidth(rect.width));          
						
						if(alignment == TextAlignment.Right || alignment == TextAlignment.Center)
							
						{ 
							
							GUILayout.FlexibleSpace(); 
							
						}
						
						
						
						//You have to include this label, otherwise the newline will be
						
						//at the same place as the last one.
						
						if(newline > 1) GUILayout.Label(" ", fontStyle);
						
						
						
						--newline; 
						
						pixelsPerLine = 0;
						
					}
					
					
					
					//Write the new stuff to the buffer
					
					output += toks[i].Substring(index, index2-index);
					
					pixelsPerLine += (index2-index)*avgPpC;
					
					index += index2-index;
					
				}
				
			}
			
		}
		
		
		
		
		
		//Clear the buffer one last time
		
		GUILayout.Label(output, fontStyle );
		
		
		
		if(alignment == TextAlignment.Left || alignment == TextAlignment.Center)
			
		{ 
			
			GUILayout.FlexibleSpace(); 
			
		} 
		
		GUILayout.EndHorizontal(); 
		
		GUILayout.FlexibleSpace(); 
		
		GUILayout.EndVertical(); 
		
		GUILayout.EndArea();   
		
	}
	
	
	
}