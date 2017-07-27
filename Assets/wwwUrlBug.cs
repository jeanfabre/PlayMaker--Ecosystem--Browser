using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wwwUrlBug : MonoBehaviour {

	public string url = "http://www.fabrejean.net/projects/playmaker_ecosystem/download?repository=jeanfabre%2FPlayMaker--Unity--UI&file=PlayMaker%2FEcosystem%2FCustom%20Packages%2FuGui%2FuGuiProxyFull.unitypackage&uid=1";
		
	// Use this for initialization
	IEnumerator Start () {

		WWW _www = new WWW (url);

		Debug.Log ("Downloading" + url);

		yield return _www;

		Debug.Log ("the _www.url is now "+_www.url);

	}

}
