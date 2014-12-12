//
// Copyright (c) 2013 Ancient Light Studios
// All Rights Reserved
// 
// http://www.ancientlightstudios.com
//

using UnityEditor;
using UnityEngine;

using System;
using System.Text;
using System.IO;
using System.Collections;

[UTDoc(title="Save a VersionInfo in Property", description="This action saves a versionInfo in the context which can be read later by other actions.")]
[UTActionInfo(actionCategory="VersionInfo")]
public class UTSaveVersionInProperty : UTAction
{
	[UTDoc(description="The path to the version info asset. Expect a text asset with a json format exactly like {\"Major\":0, \"Patch\":0, \"Build\":400, \"Type\":\"a\", \"Minor\":4} ")]
	[UTInspectorHint(required=true, order=0)]
	public UTString assetPath;

	[UTDoc(description="The name of the property to be set.")]
	[UTInspectorHint(required=true, order=1)]
	public UTString propertyName;
	
	[UTDoc(description="The format of the version, \"{0}.{1}.{2}\". {0} is Major, {1} is Minor, {2} is Patch, {3} is short Type, {4} is long type, 5 is Build")]
	[UTInspectorHint(order=2)]
	public UTString versionFormat;

	public override IEnumerator Execute (UTContext context)
	{
		var theName = propertyName.EvaluateIn (context);
		if (string.IsNullOrEmpty (theName)) {
			throw new UTFailBuildException ("The name of the property must not be empty.",this);
		}		

		// get the asset
		var pathToInstance = assetPath.EvaluateIn (context);
		pathToInstance = UTFileUtils.FullPathToProjectPath (pathToInstance);
		
		var theAsset = AssetDatabase.LoadMainAssetAtPath (pathToInstance);

	
		yield return "";
	}
	
	
	[MenuItem("Assets/Create/uTomate/VersionInfo/Save a VersionInfo in Property",  false, 200)]
	public static void AddAction ()
	{
		Create<UTSaveVersionInProperty> ();
	}
	
	
	
}

