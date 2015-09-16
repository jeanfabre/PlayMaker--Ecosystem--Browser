using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using UnityEngine;
using UnityEditor;

using Net.FabreJean.UnityEditor;
using MyUtils = Net.FabreJean.UnityEditor.Utils;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
	public class ProjectScanner
	{

		static ProjectScanner _instance;

		public static ProjectScanner instance
		{
			get { 
				if (_instance==null)
				{
					_instance = new ProjectScanner();
				}
				return _instance;
			}
		}

		//[MenuItem ("PlayMaker/Addons/Ecosystem/Scan Project",true)]
		static bool ScanProjectMenuValidation()
		{
			return ! instance.IsScanning;
		}

		//[MenuItem ("PlayMaker/Addons/Ecosystem/Scan Project")]
		public static void ScanProject()	
		{
			instance.LaunchScanningProcess(true);
		}

	 

		bool _isScanning;
		public bool IsScanning
		{
			get{ return _isScanning;}
		}

		bool _isProjectScanned;
		public bool isProjectScanned
		{
			get{ return _isProjectScanned;}
		}

		int _foundAssetsCountInProject;
		public int foundAssetsCountInProject
		{
			get{ return _foundAssetsCountInProject;}
		}


		public int AssetsCount
		{
			get{
				return AssetsList.Count;
			}
		}

		public Dictionary<string,AssetItem> AssetsList = new Dictionary<string, AssetItem>();

		public string[] AssetsFoundList;

		public bool OutputInConsole = true;


		bool _cancelFlag;

		HttpWrapper _wwwWrapper;

		/// <summary>
		/// Launch the scanning process.
		/// </summary>
		public void LaunchScanningProcess(bool ConsoleOutput)
		{
			OutputInConsole = ConsoleOutput;

			if (OutputInConsole) Debug.Log("Project Scanner: Downloading Assets Description");

			_isScanning = true;
			AssetsList = new Dictionary<string, AssetItem>();

			_wwwWrapper = new HttpWrapper();

			WWWForm _form = new WWWForm();

			_form.AddField("UnityVersion",Application.unityVersion);
			_form.AddField("PlayMakerVersion",MyUtils.GetPlayMakerVersion());

			_wwwWrapper.GET
			(
				"http://www.fabrejean.net/projects/playmaker_ecosystem/assetsDescription"
				,
				_form
				,
				(WWW www) => 
				{
					if (!string.IsNullOrEmpty(www.error))
					{
						Debug.LogError("Project Scanner: Error downloading assets definition :"+www.error);
						_isScanning = false;
					}else{
						EditorCoroutine.start(DoScanProject(www.text));
					}
				}
			);
	
		}

		public void CancelScanningProcess()
		{
			// cancel async processes;
			_cancelFlag = true;
			if (_wwwWrapper!=null)
			{
				_wwwWrapper.Cancel();
			}

			// reset variables
			_isProjectScanned = false;
			_isScanning = false;
			_foundAssetsCountInProject = 0;
			AssetsFoundList = new string[0];
			AssetsList = new Dictionary<string, AssetItem>();

			Debug.Log("Project Scanning operation cancelled");

		}


		IEnumerator DoScanProject(string assetsDescription)
		{
			_isProjectScanned = false;
			_cancelFlag = false;
			_isScanning = true;
			AssetsList = new Dictionary<string, AssetItem>();

			Hashtable _assets = (Hashtable)JSON.JsonDecode(assetsDescription);

			AssetsFoundList = new string[0];

			if (_assets == null)
			{
				Debug.LogError("Ecosystem Asset Description is invalid");
				_isScanning = false;
				yield break;

			}

			yield return null;

			foreach(DictionaryEntry entry in _assets)
			{
				yield return null;

				EditorCoroutine _findAssetCoroutine = EditorCoroutine.startManual(FindAsset((Hashtable)entry.Value));
				while (_findAssetCoroutine.routine.MoveNext()) {
				
					yield return _findAssetCoroutine.routine.Current;
				}

				yield return null;
			}

			_foundAssetsCountInProject = AssetsFoundList.Length;

			_isScanning = false;

			if (!_cancelFlag)
			{
				_isProjectScanned = true;

				if (OutputInConsole) Debug.Log("Project Scanner scanned "+AssetsList.Count+" Assets descriptions");

				if (OutputInConsole) Debug.Log(GetScanSummary());
			}

			yield break;
		}

		IEnumerator FindAsset(Hashtable _definition)
		{

			// just for nice asynch effect
			for(int i=0;i<10;i++)
			{
				yield return null;
			}

			if (_cancelFlag) yield break;

			if (_definition==null)
			{
				Debug.LogWarning("FindAsset failed, details are missing");
				yield break;
			}

			string _name = (string)_definition["Name"];

			AssetItem _item = AssetItem.AssetItemFromHashTable(_definition);

			AssetsList[_name] = _item;

			yield return null;

			// get the scan methods
			ArrayList _scanMethods = (ArrayList)_definition["ScanMethods"];

			if (_scanMethods==null)
			{
				if (OutputInConsole) Debug.LogWarning("Scanning failed for "+_definition["Name"]+": missing 'ScanMethod' definitions" );
				yield break;
			}

			bool _found = false;

			foreach(Hashtable entry in _scanMethods)
			{
				if (entry.ContainsKey("FindByFile"))
				{
					_found= MyUtils.DoesFileExistsAssets((string)entry["FindByFile"]);

				}else if(entry.ContainsKey("FindByClass"))
				{
					_found = MyUtils.isClassDefined((string)entry["FindByClass"]);
				}

				if (_found)
				{
					// get the version
					_item.ProjectVersion = new VersionInfo(GetAssetVersion(_definition));
					if (OutputInConsole) Debug.Log(_definition["Name"]+" <color=green>found</color> in Project, version: "+_item.ProjectVersion);

					_item.FoundInProject = true;

					ArrayUtility.Add<string>(ref AssetsFoundList,_name);

					yield break;
				}
				yield return null;
			}

			if (OutputInConsole) Debug.Log(_definition["Name"]+" <color=red>not found</color> in Project");

			yield break;
		}

		string GetAssetVersion(Hashtable _definition)
		{
			string name = (string)_definition["Name"];

			if (name.Equals("PlayMaker"))
			{
				return MyUtils.GetPlayMakerVersion();
			}

			if (_definition.ContainsKey("VersionScanMethod"))
			{
				Hashtable _versionScanDetails = (Hashtable)_definition["VersionScanMethod"];
				if (_versionScanDetails.ContainsKey("FindInTextFile"))
				{
					try
					{
					Regex pattern = new Regex((string)_versionScanDetails["VersionRegexPattern"]);

					using (StreamReader inputReader = new StreamReader(Application.dataPath+_versionScanDetails["FindInTextFile"]))
					{
						while (!inputReader.EndOfStream)                     
						{
							try 
							{
								Match m = pattern.Match(inputReader.ReadLine());
								if (m.Success)
								{
									return m.Value;
								}
							}
							catch (FormatException) {}
							catch (OverflowException) {}
						}
					}
					}catch(Exception e)
					{
						Debug.LogError("Project Scanning error for version scanning of "+name+" :"+e.Message);
					}

				}else if (_versionScanDetails.ContainsKey("FindInVersionInfo"))
				{
					string _jsonText = File.ReadAllText(Application.dataPath+_versionScanDetails["FindInVersionInfo"]);
					return VersionInfo.VersionInfoFromJson(_jsonText).ToString();
				}
			}

			return "n/a";
		}

		public string GetScanSummary()
		{
			if (AssetsList ==null || !isProjectScanned)
			{
				return "Please scan project first";
			}

			if (AssetsCount ==0)
			{
				return "No Known Assets detected in Project";
			}

			string _result = "Project scanning result:";
			_result += "\n"+SystemInfo.operatingSystem;
			_result += "\nUnity "+Application.unityVersion+" "+(Application.HasProLicense()?"Pro":"") +" targeting:"+Application.platform.ToString();

			foreach( KeyValuePair<string,AssetItem> _entry in AssetsList)
			{
				AssetItem _item = _entry.Value;

				if (_item.FoundInProject)
				{
					_result += "\n"+_entry.Key+" : "+_item.ProjectVersion.ToString();
				}
			}

			return _result;
		}
	}

	/// <summary>
	/// ProjectScanner Asset item.
	/// </summary>
	public class AssetItem
	{
		public string Name= "n/a";
		public string PublisherName= "n/a";
		public VersionInfo ProjectVersion = new VersionInfo();
		public VersionInfo LatestVersion = new VersionInfo();
		public string Type = "Asset";
		public string Url="";
		public int AssetStoreId = 0;
		
		public bool FoundInProject = false;


		public static AssetItem AssetItemFromHashTable(Hashtable details)
		{
			AssetItem _item =  new AssetItem();
			if (details == null)
			{
				return _item;
			}

			if (details.ContainsKey("Type")) _item.Type = (string)details["Type"];
			if (details.ContainsKey("Name")) _item.Name = (string)details["Name"];
			if (details.ContainsKey("Publisher")) _item.PublisherName = (string)details["Publisher"];
			if (details.ContainsKey("Url")) _item.Url = (string)details["Url"];
			if (details.ContainsKey("AssetStoreId"))
			{
				_item.AssetStoreId = (int)details["AssetStoreId"];
			}
			if (details.ContainsKey("Version")) _item.LatestVersion = VersionInfo.VersionInfoFromJson((string)details["Version"]);
			
			return _item;
		}

		public static AssetItem AssetItemFromJson(String jsonString)
		{
			AssetItem _item =  new AssetItem();
			if (string.IsNullOrEmpty(jsonString))
			{
				return _item;
			}
			
			Hashtable _details = (Hashtable)JSON.JsonDecode(jsonString);

			return AssetItemFromHashTable(_details);
		}
	}
}
