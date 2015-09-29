using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;

namespace Net.FabreJean.UnityEditor
{
	/// <summary>
	/// Utility class for Editors from Net.FabreJean namespace
	/// </summary> 
	public class Utils
	{

		/// <summary>
		/// caching the reflected method to Get the PlayMaker version, jsut to avoid doing the work everytime.
		/// </summary>
		static MethodInfo _GetAssemblyInformationalVersion_Method;

		/// <summary>
		/// Gets the playMaker version via reflection so that no errors is fired if PlayMaker is not installed
		/// </summary>
		/// <returns>The play maker version.</returns>
		public static string GetPlayMakerVersion()
		{
			if (_GetAssemblyInformationalVersion_Method==null)
			{
				string path = Application.dataPath+"/PlayMaker/Editor/PlayMakerEditor.dll";

				try{
					Type[] typelist = System.Reflection.Assembly.LoadFile(path).GetTypes();
					foreach(var item in typelist )
					{
						if (item.IsClass && item.Name.Equals("VersionInfo"))
						{
							_GetAssemblyInformationalVersion_Method = item.GetMethod("GetAssemblyInformationalVersion");
							break;
						}
					}
				}catch
				{
					return "n/a";
				}
			}

			if (_GetAssemblyInformationalVersion_Method!=null)
			{
				return _GetAssemblyInformationalVersion_Method.Invoke(null,null) as string;
			}

			return "n/a";
		}

		/// <summary>
		/// Check if PlayMaker is installed. Doesn't throw exceptions if PlayMaker is not installed.
		/// </summary>
		/// <returns><c>true</c>, if play maker installed was ised, <c>false</c> otherwise.</returns>
		public static bool isPlayMakerInstalled()
		{
			string path = Application.dataPath+"/PlayMaker/Editor/PlayMakerEditor.dll";
			return File.Exists(path);
			//return isClassDefined("HutongGames.PlayMakerEditor.AboutWindow"); // doens't always work.. odd
		}

		/// <summary>
		/// Check if an file exists in the Assets folder of the current Project. 
		/// </summary>
		/// <returns><c>true</c>, if file found, <c>false</c> otherwise.</returns>
		public static bool DoesFileExistsAssets(string _filePath)
		{
			string path = Application.dataPath;
			if (!_filePath.StartsWith("/"))
			{
				path += "/";
			}
			 path += _filePath;

			return File.Exists(path);
	
		}
		
		public static bool isClassFullNameDefined(string classFullName)
		{
			System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
			foreach (var A in AS)
			{
				System.Type[] types = A.GetTypes();
				foreach (var T in types)
				{
					if (T.FullName == classFullName)
					{
						return true;
					}
					
				}
			}

			return false;
		}

		public static bool isNamespaceDefined(string _namespace)
		{
			bool namespaceFound = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
			                      from type in assembly.GetTypes()
			                       where type.Namespace == _namespace
			                      select type).Any();
			
			return namespaceFound;
		}




		public static bool isClassDefined(string classType)
		{
			bool _classFound = System.Type.GetType(classType) != null;

			// just for convenience double check with fully qualified typicaly class.
			if (!_classFound)
			{
				_classFound = System.Type.GetType(classType+", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") !=null;
			}
		
			if (!_classFound)
			{
				_classFound = isClassFullNameDefined(classType);
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
				return null;
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

		public static string GetAssetAbsolutePath(string path)
		{
			string assetPath = Application.dataPath;
			
			if (path.StartsWith("Assets"))
			{
				assetPath += path.Substring(6);
			}else{
				// did not work on some project path for some unknown reasons...
				//assetPath = assetPath.TrimEnd("Assets/".ToCharArray()) +"/";
				assetPath = assetPath.Substring(0,assetPath.Length-6);
				assetPath = assetPath + path;
			}
			return assetPath;
		}


		public static void OnGUILayout_BeginHorizontalCentered()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
		}

		public static void OnGUILayout_EndHorizontalCentered()
		{
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

		}

		public static void MountScriptingDefineSymbolToAllTargets(string defineSymbol)
		{
			foreach (BuildTargetGroup _group in Enum.GetValues(typeof(BuildTargetGroup)))
			{
				if (_group == BuildTargetGroup.Unknown) continue;
				
				List<string> _defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_group).Split(';').Select(d => d.Trim()).ToList();
				
				if (!_defineSymbols.Contains(defineSymbol))
				{
					_defineSymbols.Add(defineSymbol);
					PlayerSettings.SetScriptingDefineSymbolsForGroup(_group, string.Join(";", _defineSymbols.ToArray()));
				}
			}
		}
		
		public static void UnMountScriptingDefineSymbolToAllTargets(string defineSymbol)
		{
			foreach (BuildTargetGroup _group in Enum.GetValues(typeof(BuildTargetGroup)))
			{
				if (_group == BuildTargetGroup.Unknown) continue;
				
				List<string> _defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_group).Split(';').Select(d => d.Trim()).ToList();
				
				if (_defineSymbols.Contains(defineSymbol))
				{
					_defineSymbols.Remove(defineSymbol);
					PlayerSettings.SetScriptingDefineSymbolsForGroup(_group, string.Join(";", _defineSymbols.ToArray()));
				}
			}
		}
	}
}
