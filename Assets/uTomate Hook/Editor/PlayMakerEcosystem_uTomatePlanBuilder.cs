using UnityEngine;
using UnityEditor;
using System.Collections;
using AncientLightStudios.uTomate;
using AncientLightStudios.uTomate.API;

public class PlayMakerEcosystem_uTomatePlanBuilder : EditorWindow {

	[MenuItem ("PlayMaker/Addons/Ecosystem/Build uTomate Plan")]
	public static void CreateEcosystemPlan()
	{
	// get the folder selection

	// create a new plan at the currently selected folder.
		UTAutomationPlan plan = UTils.CreateAssetOfType<UTAutomationPlan>("name");
	
	// create a new instance of the new class we created above.
	var editorModel = new PlayMakerEcosystem_uTomateModel();

	editorModel.LoadPlan(plan);
	
		Selection.activeInstanceID = plan.GetInstanceID();

	// now you can create actions.

	// FIRST ACTION
		UTEchoAction echoAction = UTAction.Create<UTEchoAction>();

	// set their properties
	echoAction.text.Value = "Hello World";
	echoAction.text.UseExpression = false; // this toggles f(x)
	
	// add the action to the automation plan
	var echoActionEntry = editorModel.AddAction(echoAction);
		echoActionEntry.name = "wtf";

		Selection.activeInstanceID = plan.GetInstanceID();

	// SECOND ACTION
	UTEchoAction anotherAction = UTAction.Create<UTEchoAction>(); 


	// set their properties
	anotherAction.text.Value = "double dva";
	anotherAction.text.UseExpression = false; // this toggles f(x)

	// add it as well
	var anotherActionEntry = editorModel.AddAction(anotherAction);
	
	
	//CONNECT
	// now connect the first echo action to the other action using the Connect method we wrote
	editorModel.Connect(echoActionEntry, anotherActionEntry);
	
	// finally set the echo action as first action
	var echoActionNode = editorModel.Graph.GetNodeFor(echoActionEntry);
	editorModel.SetFirstNode(echoActionNode);
	

	// if you want you can beautify the graph
	// so not all nodes are located at (0,0)
	editorModel.RelayoutPlan();
	
	// finally you can execute your plan using 
	UTomate.Run(plan);
	}
}
