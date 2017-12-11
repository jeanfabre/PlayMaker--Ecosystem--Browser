using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Net.FabreJean.Ecosystem
{
	/// <summary>
	/// Touch bar proxy.
	/// Implemented various menu items with shortcuts to bridge functions easily with External Devices, like the TouchBar. 
	/// These shortcuts so not exists built in Unity nor PlayMaker thus this proxy. All other features that already exists as a menu item WITH shortcut can be accessed as is already,
	/// </summary>
	public class TouchBarProxy : EditorWindow {
		
		[MenuItem ("PlayMaker/Addons/TouchBar/FsmEditor Maximize Toggle %&m",false)]
		static void ToggleFmsEditorMaximize () {
			#if PLAYMAKER	
			System.Reflection.Assembly assembly = typeof(HutongGames.PlayMakerEditor.FsmEditorWindow).Assembly;
			Type type = assembly.GetType("HutongGames.PlayMakerEditor.FsmEditorWindow");
			EditorWindow gameview = EditorWindow.GetWindow(type);
			
			gameview.maximized = !gameview.maximized;
			
			#else
			Debug.Log("PlayMaker is not installed. Function has no effect");
			#endif
		}
		
		[MenuItem ("PlayMaker/Addons/TouchBar/Import Package %&i",false)]
		static void ImportCustomPackage () {
			
			EditorApplication.ExecuteMenuItem ("Assets/Import Package/Custom Package...");
		}
		
	}
}