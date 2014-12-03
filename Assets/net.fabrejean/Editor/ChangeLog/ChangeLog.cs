using System;
using System.Collections;

using UnityEngine;
using UnityEditor;

using Net.FabreJean.UnityEditor;

namespace Net.FabreJean.UnityEditor
{
	[Serializable]
	public struct ChangeLogEntry
	{
		public VersionInfo Version;
		public string Text;
	}

	public class ChangeLog : ScriptableObject
	{
		public VersionInfo CurrentVersion = new VersionInfo(1,0,0,VersionInfo.VersionType.Final,0);

		public VersionInfo PackagedVersion = new VersionInfo();

		public string Readme;

		//public string Content;
		//public string ProcessedContent;

		public bool RequestEditingFlag;

		public ChangeLogEntry[] Log = new ChangeLogEntry[0];


		/// <summary>
		/// Creates a new automation plan from the editor's context menu.
		/// </summary>
		[MenuItem("Assets/Create/ChangeLog/New Change Log",false, 10)]
		public static void Create ()
		{
			UTils.CreateAssetOfType<ChangeLog> ("Change Log");
		}

		[MenuItem("CONTEXT/ChangeLog/Enable editing",false,400)]
		public static void Init()
		{
			ChangeLog log = Selection.activeObject as ChangeLog;

			log.RequestEditing();
		}

		public void RequestEditing()
		{
			EditorCoroutine.start(RequestEditingDisplayTemporaryDialog());
		}

		IEnumerator RequestEditingDisplayTemporaryDialog()
		{
			RequestEditingFlag = true;
			
			yield return new WaitForSeconds(5f);
			
			RequestEditingFlag = false;
		}

		public ChangeLogEntry CreateEntry(VersionInfo version,string text)
		{
			ChangeLogEntry entry = new ChangeLogEntry();
			entry.Version = version;
			entry.Text = text;

			return entry;
		}
	}
}