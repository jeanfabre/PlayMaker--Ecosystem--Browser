/* this is heavily based on UniParse originally created by the great Simon Wittber @ http://www.differentmethods.com/ */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Net.FabreJean.UnityEditor.Parse
{
	static public class Parse
	{
		static string ApiUrlFormat = "https://{0}:javascript-key={1}@api.parse.com/1";
		// TODO: move out of script and use a scritableObject instead to save is as "prefs"
		//This must be set with your parse.com Javascript API key.
		static string  JavascriptAPIKey = "Cs4d4pfuO8HSFha54F4fB9u3TuBPVQ7WCrtgr3pH"; 
		//This must be set with your parse.com applicationId.
		static string  ApplicationId = "GvUb5RvrzlmyqFAz3fwGQp7yGDy92MOU52bf8qvv";

		static public string SessionToken = null;

		/*
		static public HTTP.Request Request (string method, string path) {
			return Request(method, path, null);
		}
*/

		static public void Request(string path)
		{
			var url = string.Format(ApiUrlFormat,ApplicationId,JavascriptAPIKey);
			Debug.Log(url);

			EditorCoroutine.start(HTTPRequest(url+path));
		//	if(payLoad != null) r.Text = JSON.JsonEncode (payLoad);

		}

		static IEnumerator HTTPRequest(string url)
		{
			Hashtable headers = new Hashtable();
			headers["Content-Type"] = "application/json";
			if(SessionToken != null) headers.Add("X-Parse-Session-Token", SessionToken);

			WWW www = new WWW(url);//,pData,headers);
			while (!www.isDone) yield return null;
			Debug.Log("Result from www "+www.error+" "+www.text);
		}
	}
}

