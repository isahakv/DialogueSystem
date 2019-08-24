using System;
using System.Collections.Generic;

namespace DialogueSystem.Editor.Serialization
{
	[System.Serializable]
	public class DialogueEditorSaveData
	{
		DialogueDataSerializer[] dialogueDataSerializers;

		public DialogueEditorSaveData(DialogueData[] _dialogues)
		{
			dialogueDataSerializers = new DialogueDataSerializer[_dialogues.Length];
			 for (int i = 0; i < _dialogues.Length; i++)
				dialogueDataSerializers[i] = new DialogueDataSerializer(_dialogues[i]);
		}

		public List<DialogueData> Deserialize(Action<NodeConnectionPoint> nodeConnectionPointClicked, Action<DialogueNode> removeNode,
											  Action<DialogueNode> nodeActorIDChanged, Action<NodeConnection> removeNodeConnection)
		{
			List<DialogueData> dialogues = new List<DialogueData>();
			for (int i = 0; i < dialogueDataSerializers.Length; i++)
			{
				DialogueData dialogue = new DialogueData(dialogueDataSerializers[i].id, dialogueDataSerializers[i].name);
				// Deserialize Nodes.
				DialogueNodeSerializer[] nodeSerializers = dialogueDataSerializers[i].nodes;
				for (int j = 0; j < nodeSerializers.Length; j++)
				{
					DialogueNode node = new DialogueNode(nodeSerializers[j], nodeConnectionPointClicked, removeNode, nodeActorIDChanged);
					dialogue.nodes.Add(node);
				}
				// Deserialize Connections.
				NodeConnectionSerializer[] connectionSerializers = dialogueDataSerializers[i].connections;
				for (int j = 0; j < connectionSerializers.Length; j++)
				{
					int inNodeID = connectionSerializers[j].inNodeID;
					int outNodeID = connectionSerializers[j].outNodeID;
					DialogueNode inNode = dialogue.GetNodeByID(inNodeID);
					DialogueNode outNode = dialogue.GetNodeByID(outNodeID);
					NodeConnection connection = new NodeConnection(inNode.inPoint, outNode.outPoint, removeNodeConnection);
					dialogue.connections.Add(connection);
				}
				dialogues.Add(dialogue);
			}
			return dialogues;
		}
	}

	[System.Serializable]
	public class DialogueDataSerializer
	{
		public int id;
		public string name;
		public DialogueNodeSerializer[] nodes;
		public NodeConnectionSerializer[] connections;

		public DialogueDataSerializer(DialogueData dialogueData)
		{
			id = dialogueData.id;
			name = dialogueData.name;
			nodes = new DialogueNodeSerializer[dialogueData.nodes.Count];
			for (int i = 0; i < dialogueData.nodes.Count; i++)
				nodes[i] = new DialogueNodeSerializer(dialogueData.nodes[i]);

			connections = new NodeConnectionSerializer[dialogueData.connections.Count];
			for (int i = 0; i < dialogueData.connections.Count; i++)
				connections[i] = new NodeConnectionSerializer(dialogueData.connections[i]);
		}
	}

	[System.Serializable]
	public class DialogueNodeSerializer
	{
		public float posX, posY;
		public float width, height;

		public int nodeID;
		public int actorID;
		public string text;

		public DialogueNodeSerializer(DialogueNode node)
		{
			posX = node.rect.x;
			posY = node.rect.y;
			width = node.rect.width;
			height = node.rect.height;
			nodeID = node.nodeID;
			actorID = node.actorID;
			text = node.text;
		}
	}

	[System.Serializable]
	public class NodeConnectionSerializer
	{
		public int inNodeID;
		public int outNodeID;

		public NodeConnectionSerializer(NodeConnection connection)
		{
			inNodeID = connection.inPoint.ownerNode.nodeID;
			outNodeID = connection.outPoint.ownerNode.nodeID;
		}
	}
}
