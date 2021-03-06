using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Create a series of labels to accomodate the various formatting commands contained
/// within the text.  Commands are enclosed within brackets: [].  The following
/// case-insensitive commands are available:
///   * Background color (BC, BackColor): <RRGGBBAA> or '?' to reset to default
///   * Color (C, Color): <RRGGBBAA> or '?' to reset to default
///   * Font name (F, Font): <font name>
///   * Font attribute (FA, FontAttribute): U (underline on), -U (underline off), S (strikethrough on) or -S (strikethrough off)
///   * Font size (FS, FontSize): <size>
///   * Hyperlink (H, HyperLink): H (hyperlink start) <hyperlink tag>, -H (hyperlink end)
///   * Horizontal alignment (HA, HAlign):  L (left), R (right), or C (center)
///   * Space (S, Space): <pixels>
///   * Vertical alignment (VA, VAlign):  B (bottom) or '?' to reset to Unity default
///   Based on http://forum.unity3d.com/threads/9549-FancyLabel-Multicolor-and-Multifont-label
///   Hexadecimal color picker:  http://www.colorpicker.com/
/// </summary>
class FormattedLabel : IHyperlinkCallback
{
    private const string HYPERLINK_TAG = "Hyperlink_";
    private List<string> _lines;
    private bool _fontUnderline = false;
    private bool _fontStrikethrough = false;
    private Texture2D _backgroundColor;
    private IHyperlinkCallback _hyperlinkCallback;
    private string _lastTooltip = "";
    private string _createHyperlinkId = "";
    private string _hoveredHyperlinkId = "";
    private bool _activatedHyperlink = false;
    private enum VerticalAlignment { Default, Bottom };
    private VerticalAlignment _verticalAlignment = VerticalAlignment.Default;
    private float _lineHeight;
    private Color _defaultColor;
    private Texture2D _defaultBackgroundColor;

    /// <summary>
    /// Internal class to parse a text entry into an easier to process
    /// text (assumedly the parsed text is faster to process).  The
    /// available commands are also standardized to their short forms
    /// and in uppercase.
    /// </summary>
    private class TextFormatter
    {
        private const string _lineHeightCommand = "[LH &]";
        private float _width;
        private List<string> _lines;
        private GUIStyle _guiStyle;
        private StringBuilder _line;
        private float _lineLength;
        private float _lineHeight;

        public TextFormatter(float width, string text)
        {
            _width = width;
            _lines = new List<string>();
            format(text);
        }

        public List<string> getLines()
        {
            return _lines;
        }

        /// <summary>
        /// Format the raw text into an easier (aka faster) format to parse.
        /// * Process \n such that they are removed and 'new lines' created
        /// * Break down the text into lines that fit the requested width
        /// </summary>
        /// <param name="text">The raw text to parse</param>
        private void format(string text)
        {
            //Debug.Log("Formatting: " + text);
            _guiStyle = new GUIStyle();
            int endIndex;
            _line = new StringBuilder();
            addLineHeight(false);
            _lineLength = 0;
            _lineHeight = 0.0f;
            StringBuilder word = new StringBuilder();

            for (int letterIndex = 0; letterIndex < text.Length; letterIndex++)
            {
                if (text[letterIndex] == '\\'
                    && text.Length > letterIndex + 1
                    && text[letterIndex + 1] == '\\')
                {
                    // Escaped '\'
                    word.Append("\\");
                    letterIndex++; // Skip the second '\'
                }
                else if (text[letterIndex] == '\n')
                {
                    // New line
                    addWordToLine(word.ToString());
                    createNewLine();
                    word.Length = 0;
                }
                else if (text[letterIndex] == ' '
                    && word.Length != 0)
                {
                    // Reached a word boundary
                    addWordToLine(word.ToString());
                    word.Length = 0;
                    word.Append(' ');
                }
                else if (text[letterIndex] == '['
                    && text.Length > letterIndex + 1
                    && text[letterIndex + 1] == '[')
                {
                    // Escaped '['
                    word.Append("[[");
                    letterIndex++; // Skip the second '['
                }
                else if (text[letterIndex] == '['
                    && text.Length > letterIndex + 1
                    && (endIndex = text.IndexOf(']', letterIndex)) != -1)
                {
                    // Command
                    addWordToLine(word.ToString());
                    word.Length = 0;
                    string command = text.Substring(letterIndex + 1, endIndex - letterIndex - 1);
                    letterIndex += command.Length + 1;
                    string[] commandList = command.Split(' ');
                    for (int commandIndex = 0; commandIndex < commandList.Length; commandIndex++)
                    {
                        command = commandList[commandIndex].ToUpper();
                        if (command == "BC" || command == "BACKCOLOR")
                        {
                            // Background Color
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                Color color;
                                if (commandList[commandIndex] == "?"
                                    || HexUtil.HexToColor(commandList[commandIndex], out color))
                                {
                                    addCommandToLine("BC " + commandList[commandIndex]);
                                }
                                else
                                {
                                    Debug.LogError("The 'BackColor' command requires a color parameter of RRGGBBAA or '?'.");
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'BackColor' command requires a color parameter of RRGGBBAA or '?'.");
                            }
                        }
                        else if (command == "C" || command == "COLOR")
                        {
                            // Color
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                Color color;
                                if (commandList[commandIndex] == "?"
                                    || HexUtil.HexToColor(commandList[commandIndex], out color))
                                {
                                    addCommandToLine("C " + commandList[commandIndex]);
                                }
                                else
                                {
                                    Debug.LogError("The 'color' command requires a color parameter of RRGGBBAA or '?'.");
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'color' command requires a color parameter of RRGGBBAA:\n" + text);
                            }
                        }
                        else if (command == "F" || command == "FONT")
                        {
                            // Font
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                Font font = (Font)Resources.Load("Fonts/" + commandList[commandIndex]);
                                if (font == null)
                                {
                                    Debug.LogError("The font '" + commandList[commandIndex] + "' does not exist within Assets/Resources/Fonts/");
                                }
                                else
                                {
                                    _guiStyle.font = font; // Update the font to properly measure text
                                    addCommandToLine("F " + commandList[commandIndex]);
                                }
                                if (commandList.Length > commandIndex + 1)
                                {
                                    commandIndex++;
                                    int fontSize;
                                    if (System.Int32.TryParse(commandList[commandIndex], out fontSize))
                                    {
                                        addCommandToLine("FS " + commandList[commandIndex]);
                                        _guiStyle.fontSize = fontSize; // Update the size to properly measure text
                                    }
                                    else
                                    {
                                        Debug.LogError("The font size '" + commandList[commandIndex] + "' is not a valid integer");
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'font' command requires a font name parameter and an optional font size parameter.");
                            }
                        }
                        else if (command == "FA" || command == "FONTATTRIBUTE")
                        {
                            // Font Attribute
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                string attribute = commandList[commandIndex].ToUpper();
                                switch (attribute)
                                {
                                    case "U":
                                    case "UNDERLINE":
                                        attribute = "U";
                                        break;
                                    case "-U":
                                    case "-UNDERLINE":
                                        attribute = "-U";
                                        break;
                                    case "S":
                                    case "STRIKETHROUGH":
                                        attribute = "S";
                                        break;
                                    case "-S":
                                    case "-STRIKETHROUGH":
                                        attribute = "-S";
                                        break;
                                    default:
                                        attribute = "";
                                        Debug.LogError("The 'font attribute' command requires a font parameter of U (underline on), -U (underline off), S (strikethrough on) or -S (strikethrough off).");
                                        break;
                                }
                                if (attribute.Length != 0)
                                {
                                    addCommandToLine("FA " + attribute);
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'font attribute' command requires a font parameter of U (underline on), -U (underline off), S (strikethrough on) or -S (strikethrough off).");
                            }
                        }
                        else if (command == "FS" || command == "FONTSIZE")
                        {
                            // Font Size
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                int fontSize;
                                if (System.Int32.TryParse(commandList[commandIndex], out fontSize))
                                {
                                    addCommandToLine("FS " + commandList[commandIndex]);
                                    _guiStyle.fontSize = fontSize; // Update the size to properly measure text
                                }
                                else
                                {
                                    Debug.LogError("The font size '" + commandList[commandIndex] + "' is not a valid integer");
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'font size' command requires a font size parameter.");
                            }
                        }
                        else if (command == "H" || command == "HYPERLINK")
                        {
                            // Hyperlink on
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                addCommandToLine("H " + commandList[commandIndex]);
                            }
                            else
                            {
                                Debug.LogError("The 'hyperlink' command requires an hyperlink id parameter.");
                            }
                        }
                        else if (command == "-H" || command == "-HYPERLINK")
                        {
                            // Hyperlink off
                            addCommandToLine("-H");
                        }
                        else if (command == "HA" || command == "HALIGN")
                        {
                            // Horizontal line alignment
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                string alignment = commandList[commandIndex].ToUpper();
                                switch (alignment)
                                {
                                    case "L":
                                    case "LEFT":
                                        alignment = "L";
                                        break;
                                    case "R":
                                    case "RIGHT":
                                        alignment = "R";
                                        break;
                                    case "C":
                                    case "CENTER":
                                        alignment = "C";
                                        break;
                                    default:
                                        alignment = "";
                                        Debug.LogError("The 'HAlign' command requires an alignment parameter of L (left), R (right), or C (center).");
                                        break;
                                }
                                if (alignment.Length != 0)
                                {
                                    addCommandToLine("HA " + alignment);
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'HAlign' command requires an alignment parameter of L (left), R (right), or C (center).");
                            }
                        }
                        else if (command == "S" || command == "SPACE")
                        {
                            // Space (pixels)
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                int spaceSize;
                                if (System.Int32.TryParse(commandList[commandIndex], out spaceSize))
                                {
                                    addCommandToLine("S " + commandList[commandIndex]);
                                    _lineLength += spaceSize;
                                }
                                else
                                {
                                    Debug.LogError("The space size '" + commandList[commandIndex] + "' is not a valid integer");
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'space' command requires a pixel count parameter.");
                            }
                        }
                        else if (command == "VA" || command == "VALIGN")
                        {
                            // Vertical alignment
                            if (commandList.Length > commandIndex + 1)
                            {
                                commandIndex++;
                                string alignment = commandList[commandIndex].ToUpper();
                                switch (alignment)
                                {
                                    case "?":
                                        alignment = "?";
                                        break;
                                    case "B":
                                    case "BOTTOM":
                                        alignment = "B";
                                        break;
                                    default:
                                        alignment = "";
                                        Debug.LogError("The 'VAlign' command requires an alignment parameter of ? (default) or B (bottom).");
                                        break;
                                }
                                if (alignment.Length != 0)
                                {
                                    addCommandToLine("VA " + alignment);
                                }
                            }
                            else
                            {
                                Debug.LogError("The 'VAlign' command requires an alignment parameter of ? (default) or B (bottom).");
                            }
                        }
                    }
                }
                else
                {
                    // Add letter to the word
                    word.Append(text[letterIndex]);
                }
            }
            addWordToLine(word.ToString());
            addLineToLines();
        }

        private bool addWordToLine(string word)
        {
            bool createdNewLine = false;
            if (word.Length != 0)
            {
                Vector2 wordSize = _guiStyle.CalcSize(new GUIContent(word));
                float wordLength = (word == " ")
                                 ? getSpaceWidth()
                                 : wordSize.x;
                if (_lineLength + wordLength > _width)
                {
                    // Word does not fit on current line
                    //Debug.Log("--- new line ---");
                    createNewLine();
                    createdNewLine = true;
                    word = word.TrimStart(' '); // Remove leading spaces since we created a new line
                    wordLength = _guiStyle.CalcSize(new GUIContent(word)).x;
                }
                _line.Append(word);
                _lineLength += wordLength;
                _lineHeight = Mathf.Max(_lineHeight, wordSize.y);
                //Debug.Log("Appended '" + word + "', length: " + wordLength + ", line: " + _lineLength + " x " + _lineHeight);
            }
            return createdNewLine;
        }

        private float getSpaceWidth()
        {
            // Apparently we cannot measure a space directly, so let's deduce it
            float starWidth = _guiStyle.CalcSize(new GUIContent("**")).x;
            float wordWidth = _guiStyle.CalcSize(new GUIContent("* *")).x;
            float spaceWidth = wordWidth - starWidth;
            return spaceWidth;
        }

        private void addCommandToLine(string command)
        {
            bool mustPrefixCommand = command.StartsWith("HA ");

            command = "[" + command + "]";
            if (mustPrefixCommand)
            {
                //Debug.Log("Prepended command '" + command + "'");
                _line.Insert(0, command);
            }
            else
            {
                int trailingSpaceCount = _line.Length - _line.ToString().TrimEnd(' ').Length;
                if (trailingSpaceCount != 0)
                {
                    string line = _line.ToString().TrimEnd(' ');
                    _line.Length = 0;
                    _line.Append(line);

                    // Convert the spaces into a 'space' command
                    float spaceWidth = getSpaceWidth() * trailingSpaceCount;
                    command = "[S " + spaceWidth + "]" + command;

                    // Ensure to account for the size of these 'spaces'
                    _lineLength += spaceWidth;

                }
                _line.Append(command);
                //Debug.Log("Appended command '" + command + "'");
            }
        }

        private void addLineToLines()
        {
            if (_line.ToString() == _lineHeightCommand)
            {
                // Empty lines do not properly display; add a space
                _line.Append(" ");
            }
            addLineHeight(true);
            _lines.Add(_line.ToString());
            //Debug.Log("  Parsed line: " + _line.ToString());
        }

        private void createNewLine()
        {
            addLineToLines();
            _line.Length = 0;
            addLineHeight(false);
            _lineLength = 0;
        }

        private void addLineHeight(bool realHeight)
        {
            if (realHeight)
            {
                string realLineHeightCommand = _lineHeightCommand.Replace("&", _lineHeight.ToString());
                _line.Replace(_lineHeightCommand, realLineHeightCommand);
                _lineHeight = 0.0f;
            }
            else
            {
                _line.Append(_lineHeightCommand);
            }
        }
    }

    /// <summary>
    /// Format the commands wihtin the specified text such that they
    /// are wrapped to the specified width.
    /// </summary>
    /// <param name="width">The width at which to wrap text to new lines</param>
    /// <param name="text">The text to parse</param>
    public FormattedLabel(float width, string text)
    {
        TextFormatter textFormatter = new TextFormatter(width, text);
        _lines = textFormatter.getLines();
    }

    /// <summary>
    /// Retrieve the formatted lines of text
    /// </summary>
    /// <returns>The formatted text</returns>
    public List<string> getLines()
    {
        return _lines;
    }

    /// <summary>
    /// Draw the formatted text onto the screen
    /// </summary>
    public void draw()
    {
        int textStart, commandStart, commandEnd;
        string[] commandParts;
        TextAlignment textAlignment = TextAlignment.Left;

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = GUI.skin.GetStyle("Label").normal.textColor;
        guiStyle.font = GUI.skin.font;

        guiStyle.border = new RectOffset(0, 0, 0, 0);
        guiStyle.contentOffset = new Vector2(0.0f, 0.0f);
        guiStyle.margin = new RectOffset(0, 0, 0, 0);
        guiStyle.padding = new RectOffset(0, 0, 0, 0);

        _defaultColor = guiStyle.normal.textColor;
        _defaultBackgroundColor = GUI.skin.GetStyle("Label").normal.background;

        foreach (string line in _lines)
        {
            //Debug.Log("Formatted line: " + line);
            textStart = 0;

            // Special handling for the horizontal line alignment command
            if (line.Length >= 5
                && line.StartsWith("[HA "))
            {
                textStart = 6;
                switch (line[4])
                {
                    case 'L':
                        textAlignment = TextAlignment.Left;
                        break;
                    case 'R':
                        textAlignment = TextAlignment.Right;
                        break;
                    case 'C':
                        textAlignment = TextAlignment.Center;
                        break;
                }
            }
            if (textAlignment == TextAlignment.Right || textAlignment == TextAlignment.Center)
            {
                GUILayout.FlexibleSpace();
            }

            while (textStart < line.Length)
            {
                // Process a command
                commandStart = line.IndexOf('[', textStart);
                if (commandStart == textStart
                    && commandStart + 1 < line.Length
                    && line[commandStart + 1] == '[')
                {
                    // Escaped [
                    drawText(guiStyle, line.Substring(textStart, 1));
                    textStart += 2;
                }
                else if (commandStart == textStart)
                {
                    commandEnd = line.IndexOf(']', commandStart);
                    textStart = commandEnd + 1;
                    commandParts = line.Substring(commandStart + 1, commandEnd - commandStart - 1).Split(' ');
                    switch (commandParts[0])
                    {
                        case "BC": // BackColor
                            if (commandParts[1] == "?")
                            {
                                guiStyle.normal.background = _defaultBackgroundColor;
                            }
                            else
                            {

                                Color color;
                                HexUtil.HexToColor(commandParts[1], out color);
                                _backgroundColor = new Texture2D(1, 1);
                                _backgroundColor.SetPixel(0, 0, color);
                                _backgroundColor.wrapMode = TextureWrapMode.Repeat;
                                _backgroundColor.Apply();
                                guiStyle.normal.background = _backgroundColor;
                                //Debug.Log("Changing back color to: " + commandParts[1]);
                            }
                            break;
                        case "C": // Color
                            if (commandParts[1] == "?")
                            {
                                guiStyle.normal.textColor = _defaultColor;
                            }
                            else
                            {
                                Color color;
                                HexUtil.HexToColor(commandParts[1], out color);
                                guiStyle.normal.textColor = color;
                                //Debug.Log("Changing color to: " + commandParts[1]);
                            }
                            break;
                        case "F": // Font
                            guiStyle.font = (Font)Resources.Load("Fonts/" + commandParts[1]);
                            break;
                        case "FA": // Font Attribute
                            if (commandParts[1] == "U")
                            {
                                _fontUnderline = true;
                            }
                            else if (commandParts[1] == "-U")
                            {
                                _fontUnderline = false;
                            }
                            else if (commandParts[1] == "S")
                            {
                                _fontStrikethrough = true;
                            }
                            else if (commandParts[1] == "-S")
                            {
                                _fontStrikethrough = false;
                            }
                            break;
                        case "FS": // Font Size
                            guiStyle.fontSize = System.Int32.Parse(commandParts[1]);
                            break;
                        case "H": // Hyperlink
                            _createHyperlinkId = HYPERLINK_TAG + commandParts[1];
                            break;
                        case "-H": // Hyperlink
                            _createHyperlinkId = "";
                            break;
                        case "LH": // Line Height (internal command)
                            _lineHeight = float.Parse(commandParts[1]);
                            break;
                        case "S": // Space
                            GUILayout.Space(float.Parse(commandParts[1]));
                            //Debug.Log("Adding a space: " + float.Parse(commandParts[1]));
                            break;
                        case "VA":
                            //_verticalAlignment _verticalAlignmentSpace
                            switch (commandParts[1])
                            {
                                case "?":
                                    _verticalAlignment = VerticalAlignment.Default;
                                    break;
                                case "B":
                                    _verticalAlignment = VerticalAlignment.Bottom;
                                    break;
                            }
                            break;
                    }
                }
                else if (commandStart == -1)
                {
                    // This line does not contain a command
                    string text = line.Substring(textStart);
                    //Debug.Log("text '" + text + "'");
                    drawText(guiStyle, text);
                    textStart = line.Length;
                }
                else
                {
                    // Process the text before a command
                    string text = line.Substring(textStart, commandStart - textStart);
                    //Debug.Log("text '" + text + "'");
                    drawText(guiStyle, text);
                    textStart = commandStart;
                }
            }

            if (textAlignment == TextAlignment.Left || textAlignment == TextAlignment.Center)
            {
                GUILayout.FlexibleSpace();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        handleHyperlink();
    }

    /// <summary>
    /// Tooltips can be used to implement an OnMouseOver / OnMouseOut messaging system.
    /// http://unity3d.com/support/documentation/ScriptReference/GUI-tooltip.html
    /// </summary>
    private void handleHyperlink()
    {
        // Is the mouse over a hyperlink?  Yes if there's a tooltip to display
        if (Event.current.type == EventType.Repaint && GUI.tooltip != _lastTooltip)
        {
            // 1. Handle leaving the hyperlink
            if (_lastTooltip.StartsWith(HYPERLINK_TAG))
            {
                string hyperlinkId = _lastTooltip.Substring(HYPERLINK_TAG.Length);
                onHyperlinkLeave(hyperlinkId);
            }

            // 2. Handle entering the hyperlink
            if (GUI.tooltip.StartsWith(HYPERLINK_TAG))
            {
                string hyperlinkId = GUI.tooltip.Substring(HYPERLINK_TAG.Length);
                onHyperlinkEnter(hyperlinkId);
            }

            // 3. Store the current tooltip, even if not a hyperlink
            _lastTooltip = GUI.tooltip;
        }

        // 4. Handle a left mouse click on the hyperlink
        _activatedHyperlink = false;
        if (Event.current != null
            && Event.current.isMouse
            && Event.current.type == EventType.MouseUp
            && Event.current.button == 0 // Left button
            && isHyperlinkHovered())
        {
            //Debug.Log(_hoveredHyperlinkId);
            onHyperLinkActivated(_hoveredHyperlinkId);
        }
    }

    /// <summary>
    /// Draw a string of text (does not contain any commands).
    /// </summary>
    /// <param name="guiStyle">The current gui style.</param>
    /// <param name="text">The string of text.</param>
    private void drawText(GUIStyle guiStyle, string text)
    {
        Rect lastRect;
        float fillerHeight;
        if (_verticalAlignment == VerticalAlignment.Bottom)
        {
            fillerHeight = _lineHeight
                         - guiStyle.CalcSize(new GUIContent(text)).y
                         + (guiStyle.fontSize - 16) / 4f;
            GUILayout.BeginVertical();
            GUILayout.Label(" ", GUILayout.MinHeight(fillerHeight), GUILayout.MaxHeight(fillerHeight));
            GUILayout.Label(new GUIContent(text, _createHyperlinkId), guiStyle);
            lastRect = GUILayoutUtility.GetLastRect();
            GUILayout.EndVertical();
        }
        else
        {
            fillerHeight = 0.0f;
            GUILayout.Label(new GUIContent(text, _createHyperlinkId), guiStyle);
            lastRect = GUILayoutUtility.GetLastRect();
        }

        if (Event.current.type == EventType.Repaint)
        {
            // GetLastRect() is only valid during a repaint event
            if (_fontUnderline)
            {
                Vector2 from = new Vector2(lastRect.x, lastRect.yMin - fillerHeight + _lineHeight);
                Vector2 to = new Vector2(from.x + lastRect.width, from.y);
                GuiHelper.DrawLine(from, to, guiStyle.normal.textColor);
            }
            if (_fontStrikethrough)
            {
                Vector2 from = new Vector2(lastRect.x,
                                           lastRect.yMin - fillerHeight + _lineHeight - _lineHeight / 2f);
                Vector2 to = new Vector2(from.x + lastRect.width, from.y);
                GuiHelper.DrawLine(from, to, guiStyle.normal.textColor);
            }
        }
    }

    /// <summary>
    /// Specify the class to receive hyperlink callbacks
    /// </summary>
    /// <param name="hyperlinkCallback">The class implementing the IHyperlinkCallback interface</param>
    public void setHyperlinkCallback(IHyperlinkCallback hyperlinkCallback)
    {
        _hyperlinkCallback = hyperlinkCallback;
    }

    /// <summary>
    /// Whether the mouse is currently over a hyperlink
    /// </summary>
    /// <returns>true if the mouse is over a hyperlink, otherwise false</returns>
    public bool isHyperlinkHovered()
    {
        return _hoveredHyperlinkId.Length != 0;
    }

    /// <summary>
    /// Retrieve the hyperlink ID, if the mouse is currently over a hyperlink
    /// </summary>
    /// <returns>The hyperlink ID</returns>
    public string getHoveredHyperlink()
    {
        return _hoveredHyperlinkId;
    }

    /// <summary>
    /// Whether the mouse has clicked on a hyperlink
    /// </summary>
    /// <returns>true if the mouse has clicked on a hyperlink, otherwise false</returns>
    public bool isHyperlinkActivated()
    {
        return _activatedHyperlink;
    }

    /// <summary>
    /// Retrieve the hyperlink ID, if the mouse has clicked on a hyperlink
    /// </summary>
    /// <returns></returns>
    public string getActivatedHyperlink()
    {
        return _activatedHyperlink ? _hoveredHyperlinkId : "";
    }

    #region IHyperlinkCallback Members

    public void onHyperlinkEnter(string hyperlinkId)
    {
        //Debug.Log("onHyperlinkEnter: " + hyperlinkId);
        _hoveredHyperlinkId = hyperlinkId;
        if (_hyperlinkCallback != null)
        {
            _hyperlinkCallback.onHyperlinkEnter(hyperlinkId);
        }
    }

    public void onHyperLinkActivated(string hyperlinkId)
    {
        //Debug.Log("onHyperLinkActivated: " + hyperlinkId);
        _activatedHyperlink = true;
        if (_hyperlinkCallback != null)
        {
            _hyperlinkCallback.onHyperLinkActivated(hyperlinkId);
        }
    }

    public void onHyperlinkLeave(string hyperlinkId)
    {
        //Debug.Log("onHyperlinkLeave: " + hyperlinkId);
        _hoveredHyperlinkId = "";
        if (_hyperlinkCallback != null)
        {
            _hyperlinkCallback.onHyperlinkLeave(hyperlinkId);
        }
    }

    #endregion

    /// <summary>
    /// Define the test texts available
    /// </summary>
    public enum TestText { Demo, Fireball, Hyperlink, SpecialText };

    /// <summary>
    /// Retrieve a test text
    /// </summary>
    /// <param name="testText">The test test to retrieve</param>
    /// <returns></returns>
    public static string GetTestText(TestText testText)
    {
        string text = "FormattedLabel.GetTestText()";
        switch (testText)
        {
            #region Demo
            case TestText.Demo:
                text = "This [c 01F573FF]sentence[C FFFFFFFF] is [c FF6666FF]too[C FFFFFFFF] "
                     + "long so it will be [BC 1B07F5FF]split[BC ?] into multiple lines.\n"
                     + "Normal, [F ArialBold]bold, [font ArialItalic]italic, "
                     + "[F Arial][FA u]underline[FA -u], [FA S]strikethrough[FA -s].\n"
                     + "[F Arial 10]10, [F Arial 16]16, [F Arial 24]24, [F Arial 48]48, "
                     + "[F Arial 72]72[F Arial 16]\n"
                     + "[HA L]Left\n"
                     + "[HA C]Center\n"
                     + "[HA R]Right\n[HA L]"
                     + "20 pixels further:[S 20]*\n"
                     + "Default vertical aligment: [F Arial 10]10, [F Arial 24]24, "
                     + "[F Arial 10]10[FS 16]\n"
                     + "[VA B]Bottom vertical aligment: [F Arial 10]10, [F Arial 24]24, "
                     + "[F Arial 10]10[FS 16][VA ?]\n"
                     + "This is a [FA U][H hyperlink_value]hyperlink[-H][FA -U].";
                break;
            #endregion
            #region Fireball
            case TestText.Fireball:
                text = "[HA Center]" // Alignment center
                     + "[C FA8C8CFF]" // Light red
                     + "[FS 24]" // Font size of 24
                     + "Fireball"
                     + "[FS 16]" // Font size of 16
                     + "[color FFFFFFFF]" // revert to default (white) color
                     + "\n\nHurls a ball of fire that "
                     + "[F ArialBold]" // bold font
                     + "explodes"
                     + "[F Arial]" // normal font
                     + " on "
                     + "[FA U]" // underline on
                     + "contact"
                     + "[FA -U]" // underline off
                     + " and damages all nearby "
                     + "[FA S]" // strikethrough on
                     + "foes "
                     + "[FA -S]" // strikethrough off
                     + "enemies.\n\n"
                     + "[VA B]" // Vertical alignment: bottom
                     + "[C FF6666FF]" // red
                     + "[F ArialBold 18]" // bold font, size 18
                     + "8"
                     + "[FS 16]" // font size 16
                     + "[C FFFFFFFF]" // revert to default (white) color
                     + "[F Arial]" // normal font
                     + " to "
                     + "[C FF6666FF]" // red
                     + "[F ArialBold 18]" // bold font, size 18
                     + "12"
                     + "[F Arial 16]" // normal font
                     + "[C FFFFFFFF]" // revert to default (white) color
                     + "[F ArialItalic]" // italic font
                     + " fire"
                     + "[F Arial]" // normal font
                     + " damage"
                     + "[VA ?]"; // Vertical alignment: Unity default to almost top-aligned;
                break;
            #endregion
            #region Hyperlink
            case TestText.Hyperlink:
                text = "This is a hidden [H hidden]hyperlink[-H].\n"
                     + "This is a visible [FA U][H visible]hyperlink[-H][FA -U].";
                break;
            #endregion
            #region EscapedText
            case TestText.SpecialText:
                text = "Escaped backslash \\\n"
                     + "Escaped bracket [[\n"
                     + "Closing bracket ]\n";
                break;
            #endregion
            default:
                Debug.Log("Invalid index '" + testText.ToString() + "'");
                break;
        }
        return text;
    }
}
