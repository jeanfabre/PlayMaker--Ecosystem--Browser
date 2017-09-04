//	(c) Jean Fabre, 2011-2013 All rights reserved.
//	http://www.fabrejean.net
//  contact: http://www.fabrejean.net/contact.htm
//

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using MyUtils = Net.FabreJean.UnityEditor; // conflict with Unity Remote utils public class... odd

using Net.FabreJean.UnityEditor;
using HutongGames.PlayMaker.Ecosystem;

namespace HutongGames.PlayMaker.Ecosystem.Publishing
{

	public class RepositoriesBrowser : EditorWindow {
		
		private static bool debug = false;
		
		private static string __namespace__ = "HutongGames.PlayMaker.Ecosystem.RepositoriesBrowser";

		private static bool ShowBuildSettings = true;
		private static bool ShowHelp = true;
		private static bool ShowToolBar = true;
		private static bool DiscreteTooBar = true;

		#region Skinning and editing internals
		Vector2 _scroll;
		
		private string editorPath;
		private string editorSkinPath;
		
		private GUISkin editorSkin;
		
		private Vector2 lastMousePosition;
		private int mouseOverRowIndex;
		private Rect[] rowsArea;
		
		private Texture2D bg;

		static private  RepositoriesBrowser window;

		#endregion
		static private EditorBuildSettingsScene[] CachedEditorBuildSettings;
		
		private GUIStyle GUIStyleArrowInBuildSettings;

	    // Add menu named "My Window" to the Window menu
		[MenuItem("PlayMaker/Addons/Ecosystem/Authoring/Repositories browser")]
	    static void Init () {
			
	        // Get existing open window or if none, make a new one:
			window  = (RepositoriesBrowser)EditorWindow.GetWindow (typeof (RepositoriesBrowser));
			window.minSize = new Vector2(250,100);
			
		

	    }

		/// <summary>
		/// Called by the system.
		/// This is for consistency during unity recompilation, and to avoid having to serialize items.
		/// </summary>
		protected virtual void OnEnable()
		{
			Debug.Log("################ OnEnable");

			// get editor prefs
			GetEditorPrefs();

			Authoring.GetAllRepositories();
		}

		void GetEditorPrefs()
		{
			// get editor prefs
			ShowHelp = EditorPrefs.GetInt(__namespace__+".ShowHelp",0)==1;
			ShowBuildSettings = EditorPrefs.GetInt(__namespace__+".ShowBuildSettings",0)==1;
			DiscreteTooBar =  EditorPrefs.GetInt(__namespace__+".DiscreteToolBar",0)==1;

		}
			
		void OnGUI () {			
			wantsMouseMove = true;



			// set up the skin if not done yet.
			if (editorSkin==null)
			{
				editorSkin = MyUtils.Utils.GetGuiSkin("VolcanicGuiSkin",out editorSkinPath);
				bg = (Texture2D)(AssetDatabase.LoadAssetAtPath(editorSkinPath+"images/bg.png",typeof(Texture2D))); // Get the texture manually as we have some trickes for bg tiling
				
				GUIStyleArrowInBuildSettings = editorSkin.FindStyle("Help Arrow 90 degree");
			
			}
		
			// draw the bg properly. Haven't found a way to do it with guiskin only
			if(bg!=null)
			{
				if (bg.wrapMode!= TextureWrapMode.Repeat)
				{
					bg.wrapMode = TextureWrapMode.Repeat;
				}
				GUI.DrawTextureWithTexCoords(new Rect(0,0,position.width,position.height),bg,new Rect(0, 0, position.width / bg.width, position.height / bg.height));
			}


			if (!DiscreteTooBar || ShowToolBar || ShowHelp)
			{
				DrawToolStrip();
			}

			GUILayout.Space (10f);

			_scroll = GUILayout.BeginScrollView(_scroll);
			
			GUI.skin = editorSkin; // should design the scroll widgets so that it can be matching the skin.
			
			if (ShowHelp)
			{	
				BuildHelpUI();
			}

			if (Authoring.repositories != null)
			{
				if (Event.current.type == EventType.Repaint)
					rowsArea = new Rect[Authoring.repositories.Length];
				
				for (int i = 0; i < Authoring.repositories.Length; i++)
				{
					BuildRepositoryEntryUI (i);
				}
			}
			GUILayout.FlexibleSpace();
			
			
			if (ShowHelp)
			{
				GUILayout.BeginHorizontal();
					GUILayout.BeginVertical();
						GUILayout.FlexibleSpace();
						GUILayout.Label("","Label Jean Fabre Sign");
					GUILayout.EndVertical();
					GUILayout.BeginVertical();
						GUILayout.FlexibleSpace();
						GUILayout.Label("","Label Jean Fabre Url");
					GUILayout.EndVertical();
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				
			}

			GUILayout.EndScrollView();
			
			
			if (Event.current.type == EventType.MouseMove)
			{
	                Repaint ();
			}
			
			// detect mouse over top area of the browser window to toggle the toolbar visibility if required
		  	if (Event.current.type == EventType.Repaint)
			{
				if (lastMousePosition!= Event.current.mousePosition)
				{
					int topDelta = ShowToolBar || ShowHelp || !DiscreteTooBar ? 30:10;
					
					// check if we are few pixels above the first row
					if(new Rect(0,-15,position.width,30).Contains(Event.current.mousePosition))
					{
						ShowToolBar = true;
					}else{
						ShowToolBar = false;
					}
					
					int j=0;
					mouseOverRowIndex = -1;
					foreach(Rect _row in rowsArea)
					{
						Rect _temp = _row;
						_temp.x = _temp.x  -_scroll.x;
						_temp.y = _temp.y + topDelta -_scroll.y;
						if (_temp.Contains(Event.current.mousePosition))
						{
							mouseOverRowIndex = j;
							break;
						}
						j++;
					}
				}
				lastMousePosition = Event.current.mousePosition;
			}
			
			// User click on a row.
			if (Event.current.type == EventType.MouseDown && mouseOverRowIndex!=-1)
			{
				string path = Authoring.repositories[mouseOverRowIndex].LocalPath;
				if (!string.IsNullOrEmpty (path))
				{
					EditorUtility.RevealInFinder (path);
				}
			}
			
			
	    }
		
		void BuildHelpUI()
		{

			GUILayout.Space(8);
				
			
			GUILayout.BeginHorizontal();
				GUILayout.Space(15);
				GUILayout.Box(GUIContent.none,"Table Row Green Alone Help",GUILayout.Width(65));
				GUILayout.Label("Opened. All is ok","Label Row Plain");
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
				GUILayout.Space(15);
				GUILayout.Box(GUIContent.none,"Table Row Orange Alone Help",GUILayout.Width(65));
				GUILayout.Label("New, not saved yet","Label Row Plain");
				
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
				GUILayout.Space(15);
				GUILayout.Box(GUIContent.none,"Table Row Red Alone Help",GUILayout.Width(65));
				GUILayout.Label("Opened, not found in assets","Label Row Plain");
			GUILayout.EndHorizontal();
				
			
			if (ShowBuildSettings)
			{
				GUILayout.BeginHorizontal();
					GUILayout.Space(15);
				
					GUIStyleArrowInBuildSettings.overflow.bottom = 50;
					GUIStyleArrowInBuildSettings.overflow.right = 40;
				
					GUILayout.Label(GUIContent.none,GUIStyleArrowInBuildSettings);
					GUILayout.Space(40);
					GUILayout.Label("In the build settings","Label Row Plain",GUILayout.Height(10));
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
					GUILayout.Space(35);
					GUIStyleArrowInBuildSettings.overflow.bottom = 29;
						GUIStyleArrowInBuildSettings.overflow.right = 20;
					GUILayout.Label(GUIContent.none,GUIStyleArrowInBuildSettings);
					GUILayout.Space(20);
					GUILayout.Label("Is enabled in Build","Label Row Plain",GUILayout.Height(10));
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
					GUILayout.Space(55);
					GUIStyleArrowInBuildSettings.overflow.bottom = 8;
					GUIStyleArrowInBuildSettings.overflow.right = 0;
					GUILayout.Label(GUIContent.none,GUIStyleArrowInBuildSettings);
					GUILayout.Label("Scene Path, Click to Ping","Label Row Plain",GUILayout.Height(10));
				GUILayout.EndHorizontal();
	 
			}else{
				GUILayout.BeginHorizontal();
					GUILayout.Space(15);
					GUIStyleArrowInBuildSettings.overflow.bottom = 8;
					GUIStyleArrowInBuildSettings.overflow.right = 0;
					GUILayout.Label(GUIContent.none,GUIStyleArrowInBuildSettings);
					GUILayout.Label("Scene Path, Click to Ping","Label Row Plain",GUILayout.Height(10));
				GUILayout.EndHorizontal();
			}
			
		}
		
		void BuildRepositoryEntryUI(int repositoryIndex)
		{
			Repository _rep = Authoring.repositories [repositoryIndex];
			string name = _rep.Target.ToString();

			string rowStyle ="Middle";
		
			if (Authoring.repositories.Length==1)
			{
				rowStyle = "Alone";
			}else if (repositoryIndex==0) 
			{
				rowStyle = "First";
			}else if (repositoryIndex== (Authoring.repositories.Length-1) )
			{
				rowStyle = "Last";
			}
			
			string rowStyleType = "Plain";

			if (_rep.IsValidRepositoryPath)
			{
				rowStyleType = "Green";
			} else
			{
				if (_rep.IsPathValid)
				{
					rowStyleType = "Red";
				}
			}



			
			GUIContent bgContent = GUIContent.none;
			GUILayout.BeginHorizontal(bgContent,"Table Row "+rowStyleType+" "+rowStyle);

			/*
			if (ShowBuildSettings)
			{
				if (canIncludeInBuild)
				{
					bool inBuild = RepositoriesOnComputer[repositoryIndex]!=0;
					bool addtoBuild = GUILayout.Toggle(inBuild,"",GUILayout.Width(15f));
					if(addtoBuild!=inBuild)
					{
						if (addtoBuild)
						{
							AddSceneToBuild(repositoriesPaths[repositoryIndex]);
							
						}else{
							RemoveSceneFromBuild(repositoriesPaths[repositoryIndex]);
						}
						OnProjectChange();
						
						GUIUtility.ExitGUI();
						return;
					}
				}else
				{
					GUILayout.Toggle(false,"","Toggle Disabled",GUILayout.Width(15f));
				}
				
				bool inBuildEnabled = RepositoriesOnComputer[repositoryIndex]==1;
				bool inBuildEnable = inBuildEnabled;
				
				if (!name.Equals("") && RepositoriesOnComputer[repositoryIndex]== 1 )
				{
					inBuildEnable = GUILayout.Toggle(inBuildEnabled,"",GUILayout.Width(15f));
				}else if (!name.Equals("") && RepositoriesOnComputer[repositoryIndex]== -1){
					inBuildEnable = GUILayout.Toggle(inBuildEnabled,"",GUILayout.Width(15f));
				}else{
					GUILayout.Toggle(false,"","Toggle Disabled",GUILayout.Width(15f));
				}
				
				if (inBuildEnabled!=inBuildEnable)
				{
					EnableSceneFromBuild(repositoriesPaths[repositoryIndex],inBuildEnable);
					OnProjectChange();
				}
			}
			*/
		

			GUIContent _label = new GUIContent(name);
			var labelDimensions = GUI.skin.label.CalcSize(_label);
			
		
			int rigthSpace = (mouseOverRowIndex==repositoryIndex)?130:30;
		
			if (labelDimensions.x>(position.width-rigthSpace))
			{
				string trimmedName = name;
				if (trimmedName.Length!=name.Length)
				GUILayout.Label("(...)"+name,"Label Row "+rowStyleType);
			}else{
				GUILayout.Label(_label,"Label Row "+rowStyleType);
			}
			
			GUILayout.FlexibleSpace();
			
			if(mouseOverRowIndex==repositoryIndex )
			{
				if (GUILayout.Button("Locate","Button Small",GUILayout.Width(50)))
				{
					string _newPath = EditorUtility.OpenFolderPanel ("Locate Repository "+_rep.Target, _rep.LocalPath, "*.*");
					if (!string.IsNullOrEmpty(_newPath))
					{
						_rep.LocalPath = _newPath;
						_rep.CheckRepository ();
						Debug.Log(_rep);
					}

					GUIUtility.ExitGUI();
					return;
				}
			}
				
			GUILayout.EndHorizontal();
			
			
			if(rowsArea!=null && repositoryIndex<rowsArea.Length)
			{
				rowsArea[repositoryIndex] = GUILayoutUtility.GetLastRect();
			}

		}
		

		private static void AddSceneToBuild(string scenePath)
		{
			//Debug.Log("AddSceneToBuild "+scenePath);
			
			var original = EditorBuildSettings.scenes; 
			var newSettings = new EditorBuildSettingsScene[original.Length + 1];
			
			System.Array.Copy(original, newSettings, original.Length);
			
			var sceneToAdd = new EditorBuildSettingsScene(scenePath, true);
			
			newSettings[newSettings.Length - 1] = sceneToAdd;
			
			EditorBuildSettings.scenes = newSettings;
			
			
		}
		
		private static void RemoveSceneFromBuild(string scenePath)
		{
			EditorBuildSettingsScene[] copy = EditorBuildSettings.scenes; 
			int index = Array.FindIndex(copy, scene => scene.path == scenePath);
			if (index==-1)
			{
				if (debug) Debug.LogError("Scene not found in Assets!");
				return;
			}
			ArrayUtility.RemoveAt<EditorBuildSettingsScene>(ref copy,index);
			EditorBuildSettings.scenes = copy;

		}
		
		private static void EnableSceneFromBuild(string scenePath,bool enable)
		{
			EditorBuildSettingsScene[] copy = EditorBuildSettings.scenes; 
			int index = Array.FindIndex(copy, scene => scene.path == scenePath);
			if (index==-1)
			{
				if (debug) Debug.LogError("Scene not found in Assets!");
				return;
			}
			copy[index].enabled = enable;
			EditorBuildSettings.scenes = copy;
		}
		


		void DrawToolStrip() {
		 
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
					
				/*
				if (GUILayout.Button("Refresh", EditorStyles.toolbarButton)) 
				{
					OnProjectChange();
				}
				*/
				GUILayout.FlexibleSpace();
				
				bool _newShowHelp = GUILayout.Toggle(ShowHelp,"Help",EditorStyles.toolbarButton);
				if (_newShowHelp!=ShowHelp)
				{
					ShowHelp = _newShowHelp;
					EditorPrefs.SetInt(__namespace__+".ShowHelp",ShowHelp?1:0);
				}
			    if (GUILayout.Button("Settings", EditorStyles.toolbarDropDown)) {
			        GenericMenu toolsMenu = new GenericMenu();
			        toolsMenu.AddItem(new GUIContent("Display Build Settings"),ShowBuildSettings, OnTools_ToggleShowBuildSettings);
				  	toolsMenu.AddItem(new GUIContent("Discrete ToolBar"),DiscreteTooBar, OnTools_ToggleDiscreteTooBar);
				
			        toolsMenu.AddSeparator("");
			 
			        toolsMenu.AddItem(new GUIContent("Help..."), false, OnTools_Help);
			 
			        // Offset menu from right of editor window
			        toolsMenu.DropDown(new Rect(Screen.width-150, 0, 0, 16));
			        EditorGUIUtility.ExitGUI();
			    }
				
			
				
			GUILayout.EndHorizontal();
		}
		 
		void OnMenu_Create() 
		{
		    // Do something!
		}
		 
		void OnTools_ToggleShowBuildSettings() 
		{
		    ShowBuildSettings = !ShowBuildSettings;
			EditorPrefs.SetInt(__namespace__+".ShowBuildSettings",ShowBuildSettings?1:0);
				
		}
		
		void OnTools_ToggleDiscreteTooBar()
		{
			DiscreteTooBar = !DiscreteTooBar;
			EditorPrefs.SetInt(__namespace__+".DiscreteTooBar",DiscreteTooBar?1:0);
		}
		 
		void OnTools_Help() 
		{
		    Help.BrowseURL("http://fabrejean.net");
		}	
	}
}