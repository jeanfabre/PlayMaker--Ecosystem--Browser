//
// Copyright (c) 2015 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

//__ECO__ __UTOMATE__ __ACTION__

using System;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;
using System.Collections;
using AncientLightStudios.uTomate.API;

namespace Ecosystem.Utomate
{

	[UTDoc(title="Execute Menu Item", description="Invokes the menu item in the specified path")]
	[UTActionInfo(actionCategory="Run")]
	public class ExecuteMenuItem : UTAction
	{
		
		[UTInspectorHint(required=true, order=0)]
		[UTDoc(description="The menu path to invoke.")]
		public UTString menuItemPath;
		
		public override IEnumerator Execute (UTContext context)
		{
			string _menuItemPath = menuItemPath.EvaluateIn (context);

			EditorApplication.ExecuteMenuItem(_menuItemPath);

			yield return "";
		}
		
		[MenuItem("Assets/Create/uTomate/Run/Execute Menu Item", false)]
		public static void AddAction() {
			Create<ExecuteMenuItem>();
		}
	}


	[CustomEditor(typeof(ExecuteMenuItem))]
	public class ExecuteMenuItemEditor : UTInspectorBase {}

}


