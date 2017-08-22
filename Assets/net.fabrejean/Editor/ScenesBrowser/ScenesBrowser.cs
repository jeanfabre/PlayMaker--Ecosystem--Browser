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

public class ScenesBrowser : EditorWindow {
	
	private static bool debug = false;
	
	private static string __namespace__ = "net.fabrejean.scenesbrowser";
	
	private static bool ShowBuildSettings = true;
	private static bool ShowHelp = true;
	private static bool ShowToolBar = true;
	private static bool DiscreteTooBar = true;
	
	
	static string[] scenePaths;
	
	static string[] scenes;
	static string[] sceneNames;
	
	static int[] scenesInBuild; // 0: not in build, -1: in build but inactive, 1: in build and active.
	
	static string currentScene;
	
	Vector2 _scroll;
	
	private string editorPath;
	private string editorSkinPath;
	
	private GUISkin editorSkin;
	
	private Vector2 lastMousePosition;
	private int mouseOverRowIndex;
	private Rect[] rowsArea;
	
	private Texture2D bg;
	
	
	
	static private  ScenesBrowser window;
	
	static private EditorBuildSettingsScene[] CachedEditorBuildSettings;
	
	private GUIStyle GUIStyleArrowInBuildSettings;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Scenes Browser &s")]
    static void Init () {
        // Get existing open window or if none, make a new one:
    	window  = (ScenesBrowser)EditorWindow.GetWindow (typeof (ScenesBrowser));
		window.minSize = new Vector2(250,100);
		
		// get editor prefs
		ShowHelp = EditorPrefs.GetInt(__namespace__+".ShowHelp",0)==1;
		ShowBuildSettings = EditorPrefs.GetInt(__namespace__+".ShowBuildSettings",0)==1;
		DiscreteTooBar =  EditorPrefs.GetInt(__namespace__+".DiscreteToolBar",0)==1;
		
		GetAllScenes();
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

		
		
		if (scenes==null)
		{
			//Debug.Log("Scenes = null");
			OnProjectChange();
			GUIUtility.ExitGUI();
			return;
		}
		
		if (!DiscreteTooBar || ShowToolBar || ShowHelp)
		{
			DrawToolStrip();
		}
		
		_scroll = GUILayout.BeginScrollView(_scroll);
		
		GUI.skin = editorSkin; // should design the scroll widgets so that it can be matching the skin.
		
		if (ShowHelp)
		{	
			BuildHelpUI();
		}
		
		if (Event.current.type == EventType.Repaint)
			rowsArea = new Rect[scenes.Length];
		
		for(int i=0;i<scenes.Length;i++)
		{
			BuildSceneEntryUI(i);
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
				int topDelta = ShowToolBar || ShowHelp || !DiscreteTooBar ? 20:0;
				
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
			UnityEngine.Object _scene =	AssetDatabase.LoadMainAssetAtPath("Assets/"+scenes[mouseOverRowIndex]+".unity");
			if (_scene!=null)
			{
				EditorGUIUtility.PingObject(_scene.GetInstanceID());	
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
	
	void BuildSceneEntryUI(int sceneIndex)
	{
	
		string scene = scenes[sceneIndex];
		
		bool isOpened = scenePaths[sceneIndex].Equals(currentScene) || scenePaths[sceneIndex].Equals("");
		bool canIncludeInBuild = !scene.Equals("");
		

		string rowStyle ="Middle";
	
		if (scenes.Length==1)
		{
			rowStyle = "Alone";
		}else if (sceneIndex==0) 
		{
			rowStyle = "First";
		}else if (sceneIndex== (scenes.Length-1) )
		{
			rowStyle = "Last";
		}
		
		string rowStyleType = "Plain";
		
		if (isOpened)
		{
			if (scene.Equals(""))
			{
				rowStyleType = "Orange";
			}else{
				if (scenePaths[sceneIndex].Equals(""))
				{
					rowStyleType = "Red";
				}else{
					rowStyleType = "Green";
				}
			}
			
		}
		
		GUIContent bgContent = GUIContent.none;
		GUILayout.BeginHorizontal(bgContent,"Table Row "+rowStyleType+" "+rowStyle);
		
		if (ShowBuildSettings)
		{
			if (canIncludeInBuild)
			{
				bool inBuild = scenesInBuild[sceneIndex]!=0;
				bool addtoBuild = GUILayout.Toggle(inBuild,"",GUILayout.Width(15f));
				if(addtoBuild!=inBuild)
				{
					if (addtoBuild)
					{
						AddSceneToBuild(scenePaths[sceneIndex]);
						
					}else{
						RemoveSceneFromBuild(scenePaths[sceneIndex]);
					}
					OnProjectChange();
					
					GUIUtility.ExitGUI();
					return;
				}
			}else
			{
				GUILayout.Toggle(false,"","Toggle Disabled",GUILayout.Width(15f));
			}
			
			bool inBuildEnabled = scenesInBuild[sceneIndex]==1;
			bool inBuildEnable = inBuildEnabled;
			
			if (!scene.Equals("") && scenesInBuild[sceneIndex]== 1 )
			{
				inBuildEnable = GUILayout.Toggle(inBuildEnabled,"",GUILayout.Width(15f));
			}else if (!scene.Equals("") && scenesInBuild[sceneIndex]== -1){
				inBuildEnable = GUILayout.Toggle(inBuildEnabled,"",GUILayout.Width(15f));
			}else{
				GUILayout.Toggle(false,"","Toggle Disabled",GUILayout.Width(15f));
			}
			
			if (inBuildEnabled!=inBuildEnable)
			{
				EnableSceneFromBuild(scenePaths[sceneIndex],inBuildEnable);
				OnProjectChange();
			}
		}
		
	
		
		if (scene.Equals(""))
		{
			GUILayout.Label("Untitled","Label Row "+rowStyleType);
			if(mouseOverRowIndex==sceneIndex )
			{
				if (scenePaths[sceneIndex].Equals("") || scenePaths[sceneIndex].Equals(currentScene))
				{
					GUILayout.Label("Opened","Label Row "+rowStyleType,GUILayout.Width(50));
				}
			}
			
		}else{
			
			GUIContent _label = new GUIContent(scene);
			var labelDimensions = GUI.skin.label.CalcSize(_label);
			
		
			int rigthSpace = (mouseOverRowIndex==sceneIndex && !isOpened) ?130:30;
		
			if (labelDimensions.x>(position.width-rigthSpace))
			{
				string trimmedName = sceneNames[sceneIndex];
				if (trimmedName.Length!=scene.Length)
				GUILayout.Label("(...)"+sceneNames[sceneIndex],"Label Row "+rowStyleType);
			}else{
				GUILayout.Label(_label,"Label Row "+rowStyleType);
			}
			
			GUILayout.FlexibleSpace();
			
			if(mouseOverRowIndex==sceneIndex )
			{
				if (scenePaths[sceneIndex].Equals("") || scenePaths[sceneIndex].Equals(currentScene))
				{
					GUILayout.Label("Opened","Label Row "+rowStyleType,GUILayout.Width(50));
				}else{
					if (GUILayout.Button("Open","Button Small",GUILayout.Width(50)))
					{
						EditorApplication.OpenScene("Assets/"+scene+".unity");
						GUIUtility.ExitGUI();
						return;
					}
				}
			}
			
		}
		
		
		GUILayout.EndHorizontal();
		
		
		if(rowsArea!=null && sceneIndex<rowsArea.Length)
		{
			rowsArea[sceneIndex] = GUILayoutUtility.GetLastRect();
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
	
	private static void GetAllScenes()
    {
		if (EditorApplication.currentScene.Equals(""))
		{
			currentScene = "";
		}else{
			currentScene = EditorApplication.currentScene;
		}
		
	    DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
	    FileInfo[] goFileInfo = directory.GetFiles("*.unity", SearchOption.AllDirectories);
	   
		List<string> fullPathTemp = new List<string>();
		List<string> temp = new List<string>();
		List<string> nameTemp =  new List<string>();
		
		if (currentScene.Equals(""))
		{
			fullPathTemp.Add("");
			temp.Add ("");
			nameTemp.Add ("");
		}
		
	    foreach(FileInfo inf in goFileInfo)
	    {
			
			string name = inf.FullName.Substring(inf.FullName.LastIndexOf("/Assets/")+8);
            fullPathTemp.Add("Assets/"+name);
			name = name.Substring(0,name.Length-6);
            temp.Add(name);
			
			// get just the name of the scene
			nameTemp.Add(inf.Name.Substring(0,inf.Name.Length-6));
	    }
		
		// check if we find the current scene in our files, else it means it's been deleted while editing it.
		if (!fullPathTemp.Contains(currentScene))
		{
			string name = currentScene.Substring(currentScene.LastIndexOf("/Assets/")+8);
            fullPathTemp.Insert(0,"");
			name = name.Substring(0,name.Length-6);
            temp.Insert(0,name);

		}
		
	
		scenePaths = fullPathTemp.ToArray();
		sceneNames = nameTemp.ToArray();
		scenes = temp.ToArray(); 
		
		
		ParseGetEditorBuildSettings();
    }
	
	private static void ParseGetEditorBuildSettings()
	{
		scenesInBuild = new int[scenes.Length];
		
		CachedEditorBuildSettings = EditorBuildSettings.scenes;
		
		foreach(EditorBuildSettingsScene _scene in CachedEditorBuildSettings)
		{
			int index = Array.IndexOf(scenePaths,_scene.path);
			if (index==-1)
			{
					if (debug) Debug.Log(_scene.path+" not found "+_scene.enabled);
				
				//scenesInBuild[0] = _scene.enabled?1:-1;
			}else{
				
				if (debug) Debug.Log(_scene.path+" "+index);
				scenesInBuild[index] = _scene.enabled?1:-1;
			}
		}
	}
	
	
	void OnProjectChange() 
	{
		if (debug) Debug.Log("OnProjectChange");
		GetAllScenes();
		GUIStyleArrowInBuildSettings = editorSkin.FindStyle("Help Arrow 90 degree");
		Repaint();
    }

	void OnInspectorUpdate() {
        
		if (!EditorApplication.currentScene.Equals(currentScene))
		{
			OnProjectChange();	
		}
		
		if (EditorWindow.focusedWindow!=null && EditorWindow.focusedWindow.ToString().Contains("BuildPlayerWindow"))
		{
			// not really working, can't seem to find a way to properly check for editorbuildsettings changes.
			if (! ArrayUtility.ArrayEquals<EditorBuildSettingsScene>(CachedEditorBuildSettings,EditorBuildSettings.scenes))
			{
				if (debug) Debug.Log("BuildPlayerWindow refresh");
				ParseGetEditorBuildSettings();
				Repaint();
			}
		}
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