using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

namespace Net.FabreJean.PlayMaker.Ecosystem
{


	public class Authoring {

		static string[] OnWillSaveAssets (string[] paths) {
			Debug.Log("OnWillSaveAssets");
			foreach(string path in paths)
				Debug.Log(path);
			return paths;
		}

	}
}
