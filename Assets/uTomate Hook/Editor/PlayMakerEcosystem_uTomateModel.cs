using UnityEngine;
using AncientLightStudios.uTomate;
using AncientLightStudios.uTomate.API;

public class PlayMakerEcosystem_uTomateModel : UTNodeEditorModel {
	
	public UTNode GetNodeForEntry(UTAutomationPlanEntry entry) {
		return Graph.GetNodeFor(entry); 
	}
	
	public UTAutomationPlanEntry AddAction(UTAction action) {
		return AddAction(action,Vector2.zero);
	}

	public new UTAutomationPlanEntry AddAction(UTAction action, Vector2 position) {
		var entry = AddEntry<UTAutomationPlanSingleActionEntry>(position);
		entry.action = action;
		return entry;
	}
	
	public void Connect(UTAutomationPlanEntry firstEntry, UTAutomationPlanEntry secondEntry) {
		var firstNode = Graph.GetNodeFor(firstEntry);
		var secondNode = Graph.GetNodeFor(secondEntry);
		
		// find the "nextEntry" connector of the first node
		UTNode.Connector nextEntryConnector = null; 
		foreach(var connector in firstNode.Connectors) {
			if (connector.property == "nextEntry") {
				nextEntryConnector = connector;
				break;
			}
		}
		if (nextEntryConnector != null) {
			AddConnection(nextEntryConnector, secondNode);
		}
	}

}