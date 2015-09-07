// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using Rotorz.ReorderableList;


namespace Net.FabreJean.PlayMaker.Ecosystem
{
	public class EcosystemPublisherWizard : EditorWindow
	{ 

		#region UI

		static string _unfocusControlName ="Unfocus";
		
		GUIStyle labelStyle;

		MonoScript _scriptTarget;


		public void OnGUI()
		{
			//FsmEditorStyles.Init();
			
			// set style ot use rich text.
			if (labelStyle==null)
			{
				labelStyle = GUI.skin.GetStyle("Label");
				labelStyle.richText = true;
			}
			
			// unfocus invisible field
			GUI.SetNextControlName(_unfocusControlName);
			GUI.TextField(new Rect(0,-100,100,20),"");
			
		//	FsmEditorGUILayout.ToolWindowLargeTitle(this, "Ecosystem Publisher");


			_scriptTarget = EditorGUILayout.ObjectField("Script Target",_scriptTarget, typeof(MonoScript), false) as MonoScript;


			if (_scriptTarget!=null  && GUILayout.Button("Create"))
			{
				CreateResource();
			}

			sniptId = GUILayout.TextField(sniptId);

			if (! string.IsNullOrEmpty(sniptId))
			{
				if (GUILayout.Button("Delete"))
				{
					DeleteResource(sniptId);
				}
			}


			if (wwwSniptRESTCall!=null)
			{
				GUILayout.Label("...");
			}

		}

		#endregion UI

		string sniptId = "";

		WWW wwwSniptRESTCall;

		void DeleteResource(string id)
		{
			if (wwwSniptRESTCall!=null)
			{
				Debug.LogError("Rest call pending");
				return;
			}
			WWWForm _form = new WWWForm();
			_form.AddField("action","DELETE");
			_form.AddField("snipt",id);

			string _url = EcosystemBrowser.__REST_URL_BASE__+"publish";
			wwwSniptRESTCall = new WWW(_url,_form);
		}

		void CreateResource()
		{
			if (wwwSniptRESTCall!=null)
			{
				Debug.LogError("Rest call pending");
				return;
			}
			if (_scriptTarget==null)
			{
				Debug.LogError("No script targeted");
				return;
			}

			Hashtable _payLoad = new Hashtable();
			_payLoad["title"] = _scriptTarget.name;
			_payLoad["lexer"] = "csharp";
			_payLoad["tags"] = "Ecosystem, PlayMaker, Unity3, ActionCategory.Test";

			_payLoad["code"] = _scriptTarget.text;

			string _payloadJson = JSON.JsonEncode(_payLoad);

			WWWForm _form = new WWWForm();
			_form.AddField("action","POST");
			_form.AddField("payload",_payloadJson);

			string _url = EcosystemBrowser.__REST_URL_BASE__+"publish";
			wwwSniptRESTCall = new WWW(_url,_form);

		}

		string _lastError;
		string rawSearchResult;

		void OnInspectorUpdate() {
			
			if (wwwSniptRESTCall!=null)
			{
				if (wwwSniptRESTCall.isDone)
				{

					rawSearchResult = "";
					if (!String.IsNullOrEmpty(wwwSniptRESTCall.error))
					{
						_lastError = "Publishing Error : "+wwwSniptRESTCall.error;
						
						Debug.LogWarning(_lastError);

					}else{
						try{
							rawSearchResult = wwwSniptRESTCall.text;
							
						}catch(Exception e)
						{
							_lastError = "Publishing result Error : "+e.Message;
							
							Debug.LogWarning(_lastError);
						}
					}
					
					wwwSniptRESTCall.Dispose();
					wwwSniptRESTCall = null;

					Repaint();

					Debug.Log(rawSearchResult);
				}
			}

		}


		
		#region Window Management
		
		public static EcosystemPublisherWizard Instance;
		
		// Add menu named "My Window" to the Window menu
		[MenuItem ("PlayMaker/Addons/Ecosystem/Publisher Wizard")]
		static public void Init () {
			
			// Get existing open window or if none, make a new one:
			Instance = (EcosystemPublisherWizard)EditorWindow.GetWindow (typeof (EcosystemPublisherWizard));
			
			Instance.Initialize();
		}
		
		public void Initialize()
		{
			Debug.Log("Init");
			Instance = this;
			
			InitWindowTitle();
			position =  new Rect(120,120,300,292);
			// initial fixed size
			minSize = new Vector2(300, 292);
			
			
		}
		
		public void InitWindowTitle()
		{
			title = "EcoPublisher";
		}
		
		
		protected virtual void OnEnable()
		{
			Debug.Log("OnEnable");

		}
		
		
		#endregion Window Management
	}
}