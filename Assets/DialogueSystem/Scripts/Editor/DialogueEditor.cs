#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DialogueSystem.Editor.Serialization;

namespace DialogueSystem.Editor
{
	public class DialogueEditor : EditorWindow
	{
		List<DialogueData> dialogues = new List<DialogueData>();
		string[] dialogueNames;
		int currDialogueIdx = 0;
		List<DialogueNode> nodes;
		List<NodeConnection> connections;
		NodeConnectionPoint selectedConnectionPoint;

		Vector2 mousePos;
		Vector2 scrollPos, scrollSize;
		float nodeWidth = 200f, nodeHeigth = 150f;

		Rect menuSection;
		Rect editDialogueSection;

		string editorDialogueSavePath = "/DialogueSystem/DialogueEditorData/";
		
		[MenuItem("Editors/Dialogue Editor")]
		static void Init()
		{
			DialogueEditor window = GetWindow<DialogueEditor>();
			window.titleContent = new GUIContent("Dialogue Editor");
			window.minSize = new Vector2(1024, 720);
			window.Show();
		}

		private void OnEnable()
		{
			LoadDialogues();
			dialogueNames = GetDialogueNames();
		}

		private void OnGUI()
		{
			DrawLayout();
			DrawMenu();
			DrawEditDialogue();
		}

		private void DrawLayout()
		{
			menuSection.x = 0;
			menuSection.y = 0;
			menuSection.width = Screen.width;
			menuSection.height = 100f;

			editDialogueSection.x = 0;
			editDialogueSection.y = menuSection.height;
			editDialogueSection.width = Screen.width;
			editDialogueSection.height = Screen.height - menuSection.height - 20f;
			scrollSize = new Vector2(editDialogueSection.width * 3.0f, editDialogueSection.height * 3.0f);
		}

		private void DrawMenu()
		{
			GUILayout.BeginArea(menuSection);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Create Dialogue", GUILayout.Height(20f)))
			{
				CreateDialogue();
			}
			EditorGUI.BeginDisabledGroup(dialogues.Count == 0);
			if (GUILayout.Button("Delete Dialogue", GUILayout.Height(20f)))
			{
				DeleteDialogue();
			}
			if (GUILayout.Button("Save Dialogue", GUILayout.Height(20f)))
			{
				// If name is empty.
				if (string.IsNullOrEmpty(dialogues[currDialogueIdx].name))
					EditorUtility.DisplayDialog("ERROR", "Enter Dialogue Name Before Saving It!", "OK");
				else
					SaveDialogues();
			}
			if (GUILayout.Button("Save As Gameplay Dialogue", GUILayout.Height(20f)))
			{
				// If name is empty.
				if (string.IsNullOrEmpty(dialogues[currDialogueIdx].name))
					EditorUtility.DisplayDialog("ERROR", "Enter Dialogue Name Before Saving It!", "OK");
				else
					SaveGameplayDialogues();
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.Label("Dialogues: ", GUILayout.Width(70f));
			int newDialogueIdx = EditorGUILayout.Popup(currDialogueIdx, dialogueNames);
			if (currDialogueIdx != newDialogueIdx)
				ChangeCurrentDialogue(newDialogueIdx);

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			// If we need to show dialogue options menu.
			if (dialogues.Count > 0)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUIUtility.labelWidth = 80f;
				dialogues[currDialogueIdx].id = EditorGUILayout.IntField(new GUIContent("Dialogue ID: ", "It is not recommended to manually change dialogue ID!"),
																		dialogues[currDialogueIdx].id, GUILayout.Width(120f));
				dialogues[currDialogueIdx].id = Mathf.Clamp(dialogues[currDialogueIdx].id, 0, 999);

				EditorGUIUtility.labelWidth = 100f;
				dialogues[currDialogueIdx].name = EditorGUILayout.TextField("Dialogue Name: ", dialogues[currDialogueIdx].name);
				dialogueNames[currDialogueIdx] = dialogues[currDialogueIdx].id + ": " + dialogues[currDialogueIdx].name;

				if (GUILayout.Button("Create Node", GUILayout.Height(20f)))
				{
					AddNode(new Vector2(position.width / 2, position.height / 2));
				}
				EditorGUILayout.EndHorizontal();

				// If name is empty.
				if (string.IsNullOrEmpty(dialogues[currDialogueIdx].name))
					EditorGUILayout.HelpBox("Enter Name For Dialogue!", MessageType.Warning);
			}

			GUILayout.EndArea();
		}

		private void DrawEditDialogue()
		{
			if (dialogues.Count == 0)
				return;

			scrollPos = GUI.BeginScrollView(editDialogueSection, scrollPos, new Rect(0, 0, scrollSize.x, scrollSize.y));
			
			ProcessNodeEvents(Event.current);

			DrawGrid(20, 0.2f, Color.gray);
			DrawGrid(100, 0.4f, Color.gray);
			DrawNodes();
			DrawConnections();
			DrawConnectionLine(Event.current);

			ProcessEvents(Event.current);

			GUI.EndScrollView();

			// if (GUI.changed)
			Repaint();
		}

		private void ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 0)
						ClearConnectionSelection();
					else if (e.button == 1)
						ProcessContextMenu(e.mousePosition);
					break;
			}
		}

		private void ProcessNodeEvents(Event e)
		{
			foreach (DialogueNode node in nodes)
				node.ProcessEvents(e);
		}

		private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
		{
			int widthDivs = Mathf.CeilToInt(scrollSize.x / gridSpacing);
			int heightDivs = Mathf.CeilToInt(scrollSize.y / gridSpacing);

			Handles.BeginGUI();
			Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

			for (int i = 0; i < widthDivs; i++)
				Handles.DrawLine(new Vector3(i * gridSpacing, 0f, 0f), new Vector3(i * gridSpacing, scrollSize.y, 0f));

			for (int i = 0; i < heightDivs; i++)
				Handles.DrawLine(new Vector3(0, i * gridSpacing, 0f), new Vector3(scrollSize.x, i * gridSpacing, 0f));

			Handles.EndGUI();
		}

		private void DrawNodes()
		{
			BeginWindows();

			for (int i = 0; i < nodes.Count; i++)
				nodes[i].rect = GUI.Window(i, nodes[i].rect, DrawNodeWindow, "");

			EndWindows();
		}

		private void DrawNodeWindow(int idx)
		{
			nodes[idx].DrawNode(); // TODO: Fix.
			GUI.DragWindow();
		}

		private void DrawConnections()
		{
			for (int i = connections.Count - 1; i >= 0; i--)
				connections[i].Draw();
	}

		private void DrawConnectionLine(Event e)
		{
			if (selectedConnectionPoint == null)
				return;

			Vector3 startPos = selectedConnectionPoint.ownerNode.rect.position + selectedConnectionPoint.rect.center;
			Vector3 endPos = e.mousePosition;
			Vector3 startTan = startPos - Vector3.right * 50f;
			Vector3 endTan = endPos + Vector3.right * 50f;

			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 2f);
		}
		
		private void ProcessContextMenu(Vector2 mousePos)
		{
			// Create context menu.
			GenericMenu contextMenu = new GenericMenu();
			contextMenu.AddItem(new GUIContent("Create Node"), false, () => AddNode(mousePos));
			contextMenu.ShowAsContext();
		}

		private void AddNode(Vector2 pos)
		{
			DialogueNode node = new DialogueNode(GenerateNodeID(), pos, nodeWidth, nodeHeigth, NodeConnectionPointClicked, RemoveNode, NodeActorIDChanged);
			nodes.Add(node);
		}

		private void RemoveNode(DialogueNode node)
		{
			// Removing all connections related to this node.
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
					RemoveNodeConnection(connections[i]);
			}
			nodes.Remove(node);
		}

		private void NodeActorIDChanged(DialogueNode node)
		{
			// Finding parent Nodes.
			foreach (NodeConnection connection in connections)
			{
				DialogueNode parentNode;
				if (connection.inPoint == node.inPoint)
					parentNode = connection.outPoint.ownerNode;
				else continue;

				// Finding other child nodes of that parent node.
				foreach (NodeConnection c in connections)
				{
					// Avoiding connection with our target node.
					if (c == connection)
						continue;

					if (parentNode.outPoint == c.outPoint)
						c.inPoint.ownerNode.actorID = node.actorID;
				}
			}
		}

		private void AddNodeConnection(NodeConnection connection)
		{
			// Check if we already have that connection.
			foreach(NodeConnection c in connections)
			{
				// Check if we trying to make already existing connection.
				if (c.inPoint == connection.inPoint && c.outPoint == connection.outPoint)
					return;

				// If they have same parent and different actor id-s, then avoid connection and show error.
				if (c.outPoint.ownerNode == connection.outPoint.ownerNode && c.inPoint.ownerNode.actorID != connection.inPoint.ownerNode.actorID)
				{
					EditorUtility.DisplayDialog("ERROR", "Nodes of the same parent node must have same actor IDs!", "OK");
					return;
				}
			}

			connections.Add(connection);
		}

		private void RemoveNodeConnection(NodeConnection connection)
		{
			connections.Remove(connection);
		}

		private void NodeConnectionPointClicked(NodeConnectionPoint connectionPoint)
		{
			if (selectedConnectionPoint == null)
			{
				selectedConnectionPoint = connectionPoint;
				return;
			}

			if (selectedConnectionPoint.type != connectionPoint.type && selectedConnectionPoint.ownerNode != connectionPoint.ownerNode)
			{
				if (selectedConnectionPoint.type == ConnectionPointType.In)
					AddNodeConnection(new NodeConnection(selectedConnectionPoint, connectionPoint, RemoveNodeConnection));
				else
					AddNodeConnection(new NodeConnection(connectionPoint, selectedConnectionPoint, RemoveNodeConnection));
				ClearConnectionSelection();
			}
		}
		
		private void ClearConnectionSelection()
		{
			selectedConnectionPoint = null;
		}

		private int GenerateDialogueID()
		{
			if (dialogues.Count == 0)
				return 0;

			int id = 0;
			for (int i = 0; i < dialogues.Count; i++)
			{
				if (id == dialogues[i].id)
					id++;
				else if (id < dialogues[i].id)
					return id;
			}
			return id;
		}

		private int GenerateNodeID()
		{
			if (nodes.Count == 0)
				return 0;

			int id = 0;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (id == nodes[i].nodeID)
					id++;
				else if (id < nodes[i].nodeID)
					return id;
			}
			return id;
		}

		private string[] GetDialogueNames()
		{
			string[] names;
			if (dialogues.Count == 0)
			{
				names = new string[1];
				names[0] = "No Dialogues!";
				return names;
			}

			names = new string[dialogues.Count];
			for (int i = 0; i < dialogues.Count; i++)
				names[i] = dialogues[i].id + ": " + dialogues[i].name;

			return names;
		}

		private void ChangeCurrentDialogue(int newDialogueIdx)
		{
			currDialogueIdx = newDialogueIdx;
			nodes = dialogues[currDialogueIdx].nodes;
			connections = dialogues[currDialogueIdx].connections;
		}

		private void CreateDialogue()
		{
			DialogueData dialogue = new DialogueData(GenerateDialogueID(), "");
			currDialogueIdx = dialogues.Count;
			dialogues.Add(dialogue);
			nodes = dialogue.nodes;
			connections = dialogue.connections;
			dialogueNames = GetDialogueNames();
		}

		private void DeleteDialogue()
		{
			dialogues.RemoveAt(currDialogueIdx);
			dialogueNames = GetDialogueNames();
		}

		private void LoadDialogues()
		{
			string filePath = Application.dataPath + editorDialogueSavePath + "dialogueeditor.data";
			DialogueEditorSaveData dialogueEditorSaveData = BinaryIO.ReadFromBinaryFile<DialogueEditorSaveData>(filePath);
			if (dialogueEditorSaveData == null)
				return;

			dialogues = dialogueEditorSaveData.Deserialize(NodeConnectionPointClicked, RemoveNode, NodeActorIDChanged, RemoveNodeConnection);
			ChangeCurrentDialogue(dialogues.Count - 1);
		}

		private void SaveDialogues()
		{
			string filePath = Application.dataPath + editorDialogueSavePath + "dialogueeditor.data";

			DialogueEditorSaveData dialogueEditorSaveData = new DialogueEditorSaveData(dialogues.ToArray());
			BinaryIO.WriteToBinaryFile(filePath, dialogueEditorSaveData);
		}

		private void SaveGameplayDialogues()
		{
			Dialogue dialogue = new Dialogue(dialogues[currDialogueIdx].id, dialogues[currDialogueIdx].name);
			DialogueMessage[] messages = new DialogueMessage[nodes.Count];
			for (int i = 0; i < messages.Length; i++)
				messages[i] = new DialogueMessage(nodes[i].nodeID, nodes[i].actorID, nodes[i].text);

			// Making Message Hierarchy.
			for (int i = 0; i < connections.Count; i++)
			{
				int parentMsgID = connections[i].outPoint.ownerNode.nodeID;
				int childMsgID = connections[i].inPoint.ownerNode.nodeID;
				DialogueMessage parentMsg = Dialogue.GetMessageByID(messages, parentMsgID);
				parentMsg.childMsgIDs.Add(childMsgID);
			}

			dialogue.rootMessageIDs = new int[1];
			dialogue.rootMessageIDs[0] = 0;
			dialogue.messages = messages;

			string filePath = DialogueManager.GetDialogueFilePath(dialogue.GetID());
			BinaryIO.WriteToBinaryFile(filePath, dialogue);
		}
	}
}
#endif
