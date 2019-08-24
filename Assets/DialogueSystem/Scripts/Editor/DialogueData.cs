using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Editor
{
	public class DialogueData
	{
		public int id;
		public string name;
		public List<DialogueNode> nodes;
		public List<NodeConnection> connections;

		public DialogueData(int _id, string _name)
		{
			id = _id;
			name = _name;
			nodes = new List<DialogueNode>();
			connections = new List<NodeConnection>();
		}

		public DialogueNode GetNodeByID(int nodeID)
		{
			foreach (DialogueNode node in nodes)
			{
				if (node.nodeID == nodeID)
					return node;
			}
			return null;
		}
	}

	
}
