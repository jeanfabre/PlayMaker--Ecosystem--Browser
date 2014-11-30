
using System;
using UnityEditor;
using UnityEngine;
using Net.FabreJean.UnityEditor.MarkdownSharp;



namespace Net.FabreJean.UnityEditor
{
	/*
Will become a heading
==============

Will become a sub heading
--------------

*This will be Italic*

**This will be Bold**

- This will be a list item
- This will be a list item

Numbered list:

1. apples
2. oranges
3. pears



*/
	[CustomEditor(typeof(ChangeLog))]
	public class ChangeLogInspector : Editor{

		GUIContent processedContent = GUIContent.none;

		GUISkin UnityDarkPLus;

	
		Markdown _md;

		bool isMouseDown;

		public override void OnInspectorGUI ()
		{

			if (UnityDarkPLus==null)
			{
				UnityDarkPLus = Utils.GetGuiSkin("UnityDarkPlus");
			}

			ChangeLog _target = target as ChangeLog;
			DrawDefaultInspector();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Version","1.3.1b123");
			//EditorGUILayout.HelpBox("You can directly run this plan from here.", MessageType.Warning);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Run this plan")) {

				_md = new Markdown();
				_target.processedContent = _md.Transform(_target.content);
				//dryText = _md.Transform(_target.content,true);
			
				processedContent = new GUIContent(_target.processedContent);


			}
			EditorGUILayout.EndHorizontal();

			if(Event.current.type == EventType.MouseDown && Event.current.button == 0) {

				isMouseDown = true;

			}

			//_target.processedContent = GUI.TextArea(new Rect(8, 8, 200, 200), _target.processedContent);
			GUI.skin = UnityDarkPLus;
			_target.processedContent = GUILayout.TextArea(_target.processedContent,"MarkDownTextArea");
			GUI.skin = null;
			TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
			
			GUILayout.Label(string.Format("Selected text: {0}\nPos: {1}\nSelect pos: {2}",
			                                                    editor.SelectedText,
			                                                    editor.pos,
			                                                    editor.selectPos));

			if (_md!=null && isMouseDown)
			{
				Debug.Log("MouseDown "+editor.pos+ editor.selectPos);
				isMouseDown = false;
				if (editor.pos == editor.selectPos )
				{
					bool isDownOnLink;
					HyperTextLUT _link = _md.TryGetHyperLinkAt(editor.pos,out isDownOnLink);
					if (isDownOnLink)
					{
						editor.pos = 0;
						editor.selectPos = 0;
						editor.SelectNone();
						Application.OpenURL(_link.url);
					}
				}

				Repaint();
			}else{
				if (!string.IsNullOrEmpty(editor.SelectedText))
				{
					editor.SelectNone();
				}
			}


			if (_md!=null && _md.hyperTextList!=null)
			{
				GUILayout.Label(""+_md.hyperTextList.Count);
			}
			//if (GUILayout.Button( "Insert Tab"))
			//	_target.processedContent = _target.processedContent.Insert(editor.pos, "\t");

		//	GUI.FocusControl("asd");

		

			//GUILayout.TextArea(_target.processedContent,style);
			//GUI.SetNextControlName("test");
			//GUILayout.TextArea(_target.processedContent,style);
			//GUI.FocusControl("test");
			//Rect _lastpos = GUILayoutUtility.GetLastRect();
			//int controlID = EditorGUIUtility.GetControlID(_lastpos.GetHashCode(), FocusType.Keyboard);          
			//TextEditor editor = (TextEditor)EditorGUIUtility.GetStateObject(typeof(TextEditor), controlID -1 );
		
			
			//int stringIndex = editor.pos;//style.GetCursorStringIndex(_lastpos,processedContent,Event.current.mousePosition);
		//	GUILayout.Label("String Index "+editor.content.text);
			//EditorGUILayout.LabelField("String Index ",stringIndex.ToString());
		}
	}
}

