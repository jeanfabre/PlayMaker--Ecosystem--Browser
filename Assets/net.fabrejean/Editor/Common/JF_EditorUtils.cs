using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class JF_EditorUtils : Editor {
	
	public static bool isClassDefined(string classType)
	{
		bool _classFound = System.Type.GetType(classType) != null;

		// just for convenience double check with fully qualified typicaly class.
		if (!_classFound)
		{
			_classFound = System.Type.GetType(classType+", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") !=null;
		}
	
		return _classFound;
	}
	public static GUISkin GetGuiSkin(string guiSkinName)
	{
		string path = "";
		return GetGuiSkin(guiSkinName,out path);
	}
	
	public static GUISkin GetGuiSkin(string guiSkinName,out string assetPath)
	{
		DirectoryInfo _dataPath = new DirectoryInfo(Application.dataPath);

		string _dataPathFullName = _dataPath.FullName;

		string _pathDelimiter = _dataPathFullName.Substring(_dataPathFullName.Length-7,1);

	
		string[] allFiles = Directory.GetFiles(_dataPath.FullName, guiSkinName+".guiskin", SearchOption.AllDirectories);
		
		if (allFiles.Length==0)
		{
			assetPath = "";
			Debug.LogWarning("Could not find GuiSkin "+guiSkinName);
		}
//		Debug.Log(_pathDelimiter);

		string fullPath = allFiles[0].Substring(allFiles[0].LastIndexOf(_pathDelimiter+"Assets"+_pathDelimiter)+1);
		
	//	Debug.Log(fullPath);
		GUISkin _skin = (GUISkin)AssetDatabase.LoadAssetAtPath(fullPath,typeof(GUISkin));
		
		if (_skin==null || !_skin.name.Equals(guiSkinName))
		{
			Debug.LogError("Guiskin <"+guiSkinName+"> could not be found");
			assetPath = "";
			return null;
		}

		assetPath = fullPath.Substring(0,fullPath.Length - (guiSkinName+".guiskin").Length);
		//Debug.Log(assetPath);

		return _skin;
	}
	
}
