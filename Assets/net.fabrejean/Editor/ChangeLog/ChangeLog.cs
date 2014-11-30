using UnityEngine;
using UnityEditor;

namespace Net.FabreJean.UnityEditor
{
	public class ChangeLog : ScriptableObject
	{
		public VersionInfo version;

		public string content;
		public string processedContent;

		/// <summary>
		/// Creates a new automation plan from the editor's context menu.
		/// </summary>
		[MenuItem("Assets/Create/FabreJean/ChangeLog",false, 0)]
		public static void Create ()
		{
			
			UTils.CreateAssetOfType<ChangeLog> ("ChangeLog");
		}
	}
}