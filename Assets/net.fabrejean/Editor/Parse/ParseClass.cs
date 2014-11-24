/* this is heavily based on UniParse originally created by the great Simon Wittber @ http://www.differentmethods.com/ */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Net.FabreJean.UnityEditor.Parse
{
	public class ParseClass
	{
		public static ParseClass users = new ParseClass ("/users");
		public static ParseClass files = new ParseClass ("/files");
		
		/// <summary>
		/// Authenticate a user, returning a ParseInstance with the user details.
		/// </summary>
		/// <param name="username">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="password">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="ParseInstance"/>
		/// </returns>
		public static ParseInstance Authenticate (string username, string password)
		{
			var instance = new ParseInstance (users);
			instance.isDone = false;
			var queryString = WWW.EscapeURL ("username=" + username) + "&" + WWW.EscapeURL ("password=" + password);

			Parse.Request("/login" + "?"+ queryString);
			/*
			var r = Parse.Request ("GET", "/login" + "?" + queryString);
			r.Send (delegate(HTTP.Request request) {
				instance.code = request.response.status;
				try {
					instance.json = (Hashtable)JSON.JsonDecode (request.response.Bytes);
					
					if (instance.json.ContainsKey ("error")) {
						instance.error = (string)instance.json["error"];
					} else {
						Parse.SessionToken = instance.Get<string> ("sessionToken");
					}
				} catch (System.Exception e) {
					instance.error = e.ToString ();
				}
				if (instance.error != null)
					Debug.LogWarning (instance.error);
				instance.isDone = true;
			});
			*/
			return instance;
		}


		
		public string root = null;
		
		/// <summary>
		/// A ParseClass is used to created ParseInstances.
		/// </summary>
		/// <param name="root">
		/// A <see cref="System.String"/>. The root of the class, eg /classes/Comment
		/// </param>
		public ParseClass (string root)
		{
			this.root = root;
		}
		
		public string className {
			get {
				if (root == "/users")
					return "_User";
				var parts = root.Split ('/');
				if (parts.Length != 3)
					throw new System.IndexOutOfRangeException ("Invalid root");
				return parts[2];
			}
		}
		
		/// <summary>
		/// Create a new ParseInstance for this ParseClass.
		/// </summary>
		/// <returns>
		/// A <see cref="ParseInstance"/>
		/// </returns>
		public ParseInstance New ()
		{
			var instance = new ParseInstance (this);
			return instance;
		}
		
		/// <summary>
		/// Return a ParseInstance by objectId.
		/// </summary>
		/// <param name="objectId">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="ParseInstance"/>
		/// </returns>
		public ParseInstance Get (string objectId)
		{
			var instance = new ParseInstance (this);
			instance.isDone = false;
			/*
			var r = Parse.Request ("GET", string.Format ("{0}/{1}", root, objectId));
			r.Send (delegate(HTTP.Request request) {
				instance.code = request.response.status;
				instance.uri = request.uri.ToString ();
				try {
					instance.json = (Hashtable)JSON.JsonDecode (request.response.Bytes);
					if (instance.json.ContainsKey ("error")) {
						instance.error = (string)instance.json["error"];
					}
				} catch (System.Exception e) {
					instance.error = e.ToString ();
				}
				if (instance.error != null)
					Debug.LogWarning (instance.error);
				instance.isDone = true;
			});
			*/
			return instance;
		}
		
		/// <summary>
		/// Returns a ParseInstanceCollection of all instances belonging to this ParseClass.
		/// </summary>
		/// <returns>
		/// A <see cref="ParseInstanceCollection"/>
		/// </returns>
		public ParseInstanceCollection List ()
		{
			return List (null);
		}
		
		/// <summary>
		/// Returns a ParseInstanceCollection of instances belonging to this ParseClass.
		/// </summary>
		/// <returns>
		/// A <see cref="ParseInstanceCollection"/>. Query parameters to constrain the results in JSON.
		/// </returns>
		public ParseInstanceCollection List (string query)
		{
			var instance = new ParseInstanceCollection (this);
			instance.isDone = false;
			/*
			var r = Parse.Request ("GET", query == null ? string.Format ("{0}", root) : string.Format ("{0}?{1}", root, HTTP.URL.Encode (query)));
			r.Send (delegate(HTTP.Request request) {
				instance.code = request.response.status;
				instance.uri = request.uri.ToString ();
				Hashtable json = null;
				try {
					json = (Hashtable)JSON.JsonDecode (request.response.Bytes);
				} catch (System.Exception e) {
					instance.error = e.ToString ();
				}
				if (json != null) {
					if (json.ContainsKey ("error")) {
						instance.error = (string)json["error"];
					} else {
						var items = new List<ParseInstance> ();
						foreach (var result in (ArrayList)json["results"]) {
							var i = new ParseInstance (this);
							i.json = (Hashtable)result;
							items.Add (i);
						}
						instance.items = items.ToArray ();
					}
				}
				if (instance.error != null)
					Debug.LogWarning (instance.error);
				instance.isDone = true;
			});
*/
			return instance;
		}
		
	}


	public class ParseInstanceCollection
	{
		public ParseClass parseClass;
		public bool isDone = true;
		public int code = -1;
		public string error = null;
		public ParseInstance[] items;
		public string uri = null;
		
		public ParseInstanceCollection (ParseClass parseClass)
		{
			this.parseClass = parseClass;
		}
	}


	public class ParseInstance
	{
		public ParseClass parseClass;
		public bool isDone = true;
		public int code = -1;
		public string error = null;
		public string uri = null;
		public Hashtable json = new Hashtable ();
		List<string> dirtyFields = new List<string> ();
		
		public string objectId {
			get { return json["objectId"] as string; }
		}
		
		public System.DateTime createdAt {
			get { return System.DateTime.Parse (json["createdAt"] as string); }
		}
		
		public System.DateTime updatedAt {
			get { return System.DateTime.Parse (json["updatedAt"] as string); }
		}
		
		public ParseInstance (ParseClass parseClass)
		{
			this.parseClass = parseClass;
		}
		
		/// <summary>
		/// Get a field identified by key, casting to T.
		/// </summary>
		/// <param name="key">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="T"/>
		/// </returns>
		public T Get<T> (string key)
		{
			if (json.ContainsKey (key)) {
				return (T)json[key];
			}
			return default(T);
		}
		
		/// <summary>
		/// Get the ParseInstance pointed to by a field.
		/// </summary>
		/// <param name="key">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="ParseInstance"/>
		/// </returns>
		public ParseInstance GetPointer (string key)
		{
			var h = json[key] as Hashtable;
			if (h == null)
				return null;
			if ("Pointer" == (string)h["__type"]) {
				var className = (string)h["className"];
				ParseInstance pi;
				if (className == "_User")
					pi = ParseClass.users.Get ((string)h["objectId"]);
				else
					pi = new ParseClass ("/classes/" + className).Get ((string)h["objectId"]);
				return pi;
			} else
				return null;
		}
		
		/// <summary>
		/// Find all instances in otherClass which reference this instance in the field identified by key.
		/// </summary>
		/// <param name="otherClass">
		/// A <see cref="ParseClass"/>
		/// </param>
		/// <param name="key">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="ParseInstanceCollection"/>
		/// </returns>
		public ParseInstanceCollection FindReferencesIn(ParseClass otherClass, string key) {
			var q = "where={\"" + key + "\": {\"__type\":\"Pointer\", \"className\":\"" + parseClass.className + "\", \"objectId\":\"" + objectId + "\"}} ";
			return otherClass.List(q);
		}
		
		/// <summary>
		/// Sets a value identified by key. Values which are ParseInstance are converted to Pointers.
		/// </summary>
		/// <param name="key">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="value">
		/// A <see cref="System.Object"/>
		/// </param>
		public void Set (string key, object value)
		{
			var pi = value as ParseInstance;
			if (pi != null) {
				var h = new Hashtable ();
				h["__type"] = "Pointer";
				h["className"] = pi.parseClass.className;
				h["objectId"] = pi.objectId;
				json[key] = h;
			} else
				json[key] = value;
			
			dirtyFields.Add (key);
			
		}
		
		/// <summary>
		/// Delete this instance from the ParseClass.
		/// </summary>
		public void Delete ()
		{
			if (!isDone)
				throw new System.InvalidOperationException ("Cannot delete while operation is in progress.");
			isDone = false;
			/*
			var r = Parse.Request ("DELETE", string.Format ("{0}/{1}", parseClass.root, objectId));
			r.Send (delegate(HTTP.Request request) {
				code = request.response.status;
				try {
					json = (Hashtable)JSON.JsonDecode (request.response.Bytes);
				} catch (System.Exception e) {
					error = e.ToString ();
				}
				if (json != null) {
					error = Get<string> ("error");
				}
				if (error != null)
					Debug.LogWarning (error);
				isDone = true;
			});
			*/
		}
		
		/// <summary>
		/// Update the backend with any changed values.
		/// </summary>
		public void Update ()
		{
			if (!isDone)
				throw new System.InvalidOperationException ("Cannot update while operation is in progress.");
			isDone = false;
			var payLoad = new Hashtable ();
			foreach (var i in dirtyFields)
				payLoad[i] = json[i];
			dirtyFields.Clear ();
			/*
			var r = Parse.Request ("PUT", string.Format ("{0}/{1}", parseClass.root, objectId), payLoad);
			r.Send (delegate(HTTP.Request request) {
				code = request.response.status;
				Hashtable j = null;
				try {
					j = (Hashtable)JSON.JsonDecode (request.response.Bytes);
				} catch (System.Exception e) {
					error = e.ToString ();
				}
				if (j != null) {
					if (j.ContainsKey ("error"))
						error = (string)j["error"];
				}
				if (error != null)
					Debug.LogWarning (error);
				else
					json["updatedAt"] = j["updatedAt"];
				isDone = true;
			});
			*/
		}
		
		/// <summary>
		/// Create a new instance on the backend using this ParseInstance.
		/// </summary>
		public void Create ()
		{
			if (!isDone)
				throw new System.InvalidOperationException ("Cannot create while operation is in progress.");
			isDone = false;
			/*
			var r = Parse.Request ("POST", string.Format ("{0}", parseClass.root), json);
			r.Send (delegate(HTTP.Request request) {
				Debug.Log("request delegate");
				code = request.response.status;
				var u = request.response.GetHeader ("Location");
				if (u != "")
					uri = u;
				Hashtable j = null;
				try {
					j = (Hashtable)JSON.JsonDecode (request.response.Bytes);
				} catch (System.Exception e) {
					error = e.ToString ();
				}
				if (j != null) {
					if (j.ContainsKey ("error"))
						error = (string)j["error"];
				}
				if (error != null)
					Debug.LogWarning (error);
				else {
					json["objectId"] = j["objectId"];
					json["createdAt"] = j["createdAt"];
				}
				isDone = true;
			});
*/
		}
	}
}
