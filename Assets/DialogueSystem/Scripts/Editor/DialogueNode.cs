using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DialogueSystem.Editor
{
	public class DialogueNode : ScriptableObject
	{
		public Rect rect;
		public int nodeID;
		public int actorID;
		public string text;

		public NodeConnectionPoint inPoint;
		public NodeConnectionPoint outPoint;
		Action<DialogueNode> onClickedRemoveNode, onActorIDChanged;

		public DialogueNode(int _nodeID, Vector2 pos, float width, float height,
							Action<NodeConnectionPoint> onClickedPoint,
							Action<DialogueNode> _onClickedRemoveNode,
							Action<DialogueNode> _onActorIDChanged)
		{
			nodeID = _nodeID;
			rect = new Rect(pos.x, pos.y, width, height);
			inPoint = new NodeConnectionPoint(this, ConnectionPointType.In, onClickedPoint);
			outPoint = new NodeConnectionPoint(this, ConnectionPointType.Out, onClickedPoint);
			onClickedRemoveNode = _onClickedRemoveNode;
			onActorIDChanged = _onActorIDChanged;
		}

		public DialogueNode(Serialization.DialogueNodeSerializer nodeSerializer,
							Action<NodeConnectionPoint> onClickedPoint,
							Action<DialogueNode> _onClickedRemoveNode,
							Action<DialogueNode> _onActorIDChanged)
			: this(nodeSerializer.nodeID, new Vector2(nodeSerializer.posX, nodeSerializer.posY), nodeSerializer.width, nodeSerializer.height,
				   onClickedPoint, _onClickedRemoveNode, _onActorIDChanged)
		{
			actorID = nodeSerializer.actorID;
			text = nodeSerializer.text;
		}

		public void DrawNode()
		{
			GUI.Label(new Rect(0f, 0f, 50f, 16f), "ID: " + nodeID);

			EditorGUIUtility.labelWidth = 60f;
			int oldActorID = actorID;
			actorID = EditorGUI.IntField(new Rect(rect.width - 80f, 0f, 80f, 16f), "Actor ID: ", actorID);
			actorID = Mathf.Clamp(actorID, 0, 99);
			if (actorID != oldActorID)
				onActorIDChanged?.Invoke(this);

			text = GUI.TextArea(new Rect(0f, 15f, rect.width, rect.height - 30f), text);
			inPoint.Draw();
			outPoint.Draw();
		}

		public void ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 1 && rect.Contains(e.mousePosition))
					{
						ProcessContextMenu();
						e.Use();
					}
					break;
			}
		}

		private void ProcessContextMenu()
		{
			// Create context menu.
			GenericMenu contextMenu = new GenericMenu();
			contextMenu.AddItem(new GUIContent("Delete Node"), false, () => onClickedRemoveNode?.Invoke(this));
			contextMenu.ShowAsContext();
		}
	}
}
