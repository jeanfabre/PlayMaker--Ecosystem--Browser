
using UnityEngine;
using UnityEditor;

/// <summary>
/// Markdown GU.
/// </summary>
namespace Net.FabreJean.UnityEditor.MarkdownSharp
{
	#if FALSE
	// Attempt, maybe I could make a viewer version, but I guess it's much work and will be confusing for publishers

	public static class ExtensionMethods
	{
		/// <summary>
		/// Markdowns the text area. 
		/// </summary>
		/// <returns>The text area.</returns>
		/// <param name="guiLayout">GUI layout.</param>
		/// <param name="content">Content.</param>
		public static string MarkdownTextArea(this GUILayout guiLayout,MarkDownGuiContent content)
		{

		}
	}

	public struct HyperLinkDefinition
	{
		public string text;
		public string url;
		public int startPos;
		public int EndPos;
	}

	public class MarkDownGuiContent
	{
		public MarkDownGuiContent(string text)
		{
			_text = text;
		}
		string _text;

		public string Text
		{
			get{
				return _text;
			}
		}
	}

	#endif

	/// <summary>
	/// Markdown GUI.
	/// </summary>
	public class MarkdownGUI{

		/// <summary>
		/// The name of the GUI skin.
		/// </summary>
		const string __guiSkinName__ = "MarkdownSharpGuiSkin";

		/// <summary>
		/// The markdown skin.
		/// </summary>
		static GUISkin _markdownSkin;

		/// <summary>
		/// The markdown parser.
		/// </summary>
		MarkdownParser _markdownParser;

		/// <summary>
		/// The processed text ready to be injecting in a RichText enabled GUI element
		/// </summary>
		string _processedText;

		/// <summary>
		/// Flag to pass the mouse down state over to the check routine for hypertext clicks
		/// </summary>
		bool _isMouseDown;

		bool _hasContent;

		/// <summary>
		/// flag to know if this instance has content to show.
		/// </summary>
		public bool HasContent
		{
			get{return _hasContent;}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Net.FabreJean.UnityEditor.MarkdownSharp.MarkdownGUI"/> class.
		/// </summary>
		public MarkdownGUI()
		{
		
		}

		/// <summary>
		/// Processes markdown syntax from a source 
		/// </summary>
		/// <returns>The processed source as Unity rich text</returns>
		/// <param name="source">Source text featuring markdown syntax</param>

		public string ProcessSource(string source)
		{

			_markdownParser = new MarkdownParser();
			_processedText = _markdownParser.Transform(source);
			_hasContent = true;
			return _processedText;
		}

		/// <summary>
		/// Display a Text Area GUILayout element with the processed source
		/// </summary>
		public bool OnGUILayout_MardkDownTextArea()
		{
			if (_markdownSkin==null) 
			{
				_markdownSkin = Utils.GetGuiSkin(__guiSkinName__);
			}

			if(Event.current.type == EventType.MouseDown && Event.current.button == 0) {					
				_isMouseDown = true;
			}

			GUI.skin = _markdownSkin;
			string style = "MarkdownTextArea" + (EditorGUIUtility.isProSkin?"Dark":"Light");
			GUILayout.TextArea(_processedText,style);
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.skin = null;
			TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

			/*
			GUILayout.Label(string.Format("Selected text: {0}\nPos: {1}\nSelect pos: {2}",
			                                                    editor.SelectedText,
			                                                    editor.pos,
			                                                    editor.selectPos));
			 */


			if (_markdownParser!=null && _isMouseDown)
			{
				if (Event.current.type == EventType.Repaint && rect.Contains(Event.current.mousePosition))
				{
					Debug.Log("MouseDown "+Event.current.mousePosition+" on"+rect);
					_isMouseDown = false;
					if (editor.pos == editor.selectPos )
					{
						bool isDownOnLink;
						HyperTextLUT _link = _markdownParser.TryGetHyperLinkAt(editor.pos,out isDownOnLink);
						if (isDownOnLink)
						{


							Application.OpenURL(_link.url);

						}
					}



				}

				//editor.pos = 0;
				//editor.selectPos = 0;
				editor.SelectNone();

				return true;

			}else{
				if (!string.IsNullOrEmpty(editor.SelectedText))
				{
					//editor.pos = 0;
					//editor.selectPos = 0;
					editor.SelectNone();
				}
			}

			return false;
		}
	}
}
