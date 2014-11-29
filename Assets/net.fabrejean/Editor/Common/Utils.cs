using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

namespace Net.FabreJean.UnityEditor
{
	/// <summary>
	/// Utility class for Editors done within this NameSpace.
	/// </summary> 
	public class Utils {
		
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

		public static VersionInfo UpdateVersion(string versionSourcePath)
		{
			//Debug.Log("UpdateVersion "+versionSourcePath);
			string versionText =  Utils.GetFileContents( versionSourcePath );
			if (versionText != null)
			{
				Hashtable _version = (Hashtable)JSON.JsonDecode(versionText);
				int Major = (int)_version["Major"];
				int Minor = (int)_version["Minor"];
				int Patch = (int)_version["Patch"];
				int Build = (int)_version["Build"];

				VersionInfo.VersionType Type = VersionInfo.GetVersionTypeFromString((string)_version["Type"]);

				#if AUTHORING
				Build++;
				_version["Build"] = Build;
				Utils.PutFileContents(versionSourcePath, JSON.JsonEncode(_version) );
				#endif
				return new VersionInfo(Major,Minor,Patch,Type,Build);
			}else{
				Debug.LogError("UpdateVersion Could not find "+versionSourcePath);
			}

			return new VersionInfo();
		}

		public static string GetFileContents( string fileName )
		{			
			StreamReader streamReader = new StreamReader( fileName );
			if (streamReader!=null)
			{
				var fileContents = streamReader.ReadToEnd();
				streamReader.Close();
				return fileContents;
			}else{
				Debug.LogError("GetFileContents Could not find "+fileName);
			}
			return "";

		}
		
		public static void PutFileContents( string filePath, string content )
		{
			StreamWriter streamWriter = new StreamWriter( filePath );
			if (streamWriter!=null)
			{
				streamWriter.Write( content.Trim() );
				streamWriter.Flush();
				streamWriter.Close();
			}else{
				Debug.LogError("PutFileContents Could not find "+filePath);
			}
		}
		
	}
}
