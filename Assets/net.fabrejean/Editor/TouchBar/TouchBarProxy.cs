
using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Net.FabreJean.PlayMaker.Ecosystem
{
	public class TouchBarProxy : EditorWindow {
		
		[MenuItem ("PlayMaker/Addons/TouchBar/FsmEditor Maximize Toggle %&m",false)]
		static void toggleFmsEditorMaximize () {
			#if PLAYMAKER	
				System.Reflection.Assembly assembly = typeof(HutongGames.PlayMakerEditor.FsmEditorWindow).Assembly;
				Type type = assembly.GetType("HutongGames.PlayMakerEditor.FsmEditorWindow");
				EditorWindow gameview = EditorWindow.GetWindow(type);

				gameview.maximized = !gameview.maximized;

			#else
				Debug.Log("PlayMaker is not installed. Function has no effect");
			#endif
		}

	}
}