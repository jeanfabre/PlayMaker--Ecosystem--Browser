using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using Net.FabreJean.UnityEditor.MarkdownSharp;

namespace Net.FabreJean.UnityEditor
{
	[CustomEditor(typeof(ChangeLog))]
	public class ChangeLogInspector : Editor{

		Dictionary<ChangeLogEntry,MarkdownGUI> EntryMarkDownGuiLUT = new Dictionary<ChangeLogEntry, MarkdownGUI>();

		ChangeLogEntry _currentLogEntry = new ChangeLogEntry();

		public override void OnInspectorGUI ()
		{
		
			ChangeLog _target = target as ChangeLog;


			EditorGUILayout.Separator();

			if (_target.RequestEditingFlag)
			{
				EditorGUILayout.LabelField("Are you sure?");
				Repaint();
			}
			//EditorGUILayout.LabelField("Current Version",_target.CurrentVersion .ToString());

			if (_currentLogEntry.Text==null)
			{
				_currentLogEntry.Version = new VersionInfo();
				_currentLogEntry.Text = "";
			}

			_currentLogEntry.Text = GUILayout.TextField(_currentLogEntry.Text,500,GUILayout.Height(200));

			if (GUILayout.Button("Save"))
			{
				if (ArrayUtility.Contains<ChangeLogEntry>(_target.Log,_currentLogEntry))
				{
					EntryMarkDownGuiLUT[_currentLogEntry].ProcessSource(_currentLogEntry.Text);
					Repaint();
					return;
					//ArrayUtility. <ChangeLogEntry>(ref _target.Log,_currentLogEntry);
				}else{
					ArrayUtility.Insert<ChangeLogEntry>(ref _target.Log,0,_currentLogEntry);
				}
				EditorUtility.SetDirty(_target);
			}


			foreach(ChangeLogEntry entry in _target.Log)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Version "+entry.Version.ToShortString());
				GUILayout.FlexibleSpace();

				if(GUILayout.Button("Edit"))
				{
					_currentLogEntry = entry;
					Repaint();
				}

				if (GUILayout.Button("Delete"))
				{
					ArrayUtility.Remove<ChangeLogEntry>(ref _target.Log,entry);
					EditorUtility.SetDirty(_target);
					return;
				}

				GUILayout.EndHorizontal();

				if (!EntryMarkDownGuiLUT.ContainsKey(entry))
				{
					MarkdownGUI _mdGui = new MarkdownGUI();
					_mdGui.ProcessSource(entry.Text);

					EntryMarkDownGuiLUT.Add(entry,_mdGui);
				}

				EntryMarkDownGuiLUT[entry].OnGUILayout_MardkDownTextArea();
			}


			//EditorGUILayout.BeginHorizontal();
			//GUILayout.FlexibleSpace();
			/*
			if (!_markdownGui.HasContent)
			{
				_target.processedContent =	_markdownGui.ProcessSource(_target.content);
			//	Debug.Log(_target.processedContent);
			}

			_markdownGui.OnGUILayout_MardkDownTextArea();

			//EditorGUILayout.EndHorizontal();
*/


		}
	}
}

