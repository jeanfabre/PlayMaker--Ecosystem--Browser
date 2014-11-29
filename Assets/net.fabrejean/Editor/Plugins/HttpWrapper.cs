using System;
using UnityEngine;
using System.Collections;

namespace Net.FabreJean.UnityEditor
{
	public class HttpWrapper {

		public WWW GET(string url)
		{

			WWW www = new WWW(url);
			EditorCoroutine.start(ProcessRequest(www));
			return www;
		}

		IEnumerator ProcessRequest(WWW www)
		{
		
			while (!www.isDone) yield return null;
		}
	}
}
