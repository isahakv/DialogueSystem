using System;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.UI;

namespace DialogueSystem
{
	public class DialogueManager : MonoBehaviour
	{
		public static DialogueManager Instance { get; private set; }
		public DialogueUIHandler[] dialogueUIHanlders;

		List<Dialogue> pooledDialogues;

		void Awake()
		{
			if (Instance == null)
			{
				DontDestroyOnLoad(this);
				Instance = this;
			}
			else if (Instance != this)
				DestroyImmediate(this);
		}

		private void Start()
		{
			DialogueActor[] actors = new DialogueActor[2];
			actors[0] = new DialogueActor(0, "Artyom", false, null);
			actors[1] = new DialogueActor(1, "Tony", true, null);
			OpenDialogue(267, typeof(RPGDialogueUIHandler), false, null, actors);
		}

		public void OpenDialogue(int id, Type dialogueUIHandlerType, bool poolDialogue, Action onDialogueClosed, params DialogueActor[] actors)
		{
			if (!(dialogueUIHandlerType.IsSubclassOf(typeof(DialogueUIHandler))))
				return;

			DialogueUIHandler dialogueUIHandler = null;
			// Find the object of the right type in "dialogueUIPrefabs" array.
			foreach (DialogueUIHandler dialogueUI in dialogueUIHanlders)
			{
				if (dialogueUI.GetType() == dialogueUIHandlerType)
					dialogueUIHandler = dialogueUI;
			}
			if (dialogueUIHandler == null)
				return;

			Dialogue dialogue = GetPooledDialogue(id);
			// If Dialogue is not pooled, then load from file.
			if (dialogue == null)
			{
				string filePath = GetDialogueFilePath(id);
				dialogue = BinaryIO.ReadFromBinaryFile<Dialogue>(filePath);
				if (dialogue == null)
					return;
			}
			// Pool dialogue if "poolDialogue" is marked true.
			if (poolDialogue)
				AddPooledDialogue(dialogue);

			// Open dialogue in DialogueUIHandler.
			dialogueUIHandler.OpenDialogue(dialogue, onDialogueClosed, actors);
		}

		public static string GetDialogueFilePath(int id)
		{
			string dialogueSavePath = "/DialogueSystem/DialogueSaveFiles/" + "dialogue_" + id + ".data";
			return Application.dataPath + dialogueSavePath;
		}

		private void AddPooledDialogue(Dialogue dialogue)
		{
			if (pooledDialogues == null)
				pooledDialogues = new List<Dialogue>();

			pooledDialogues.Add(dialogue);
		}

		private Dialogue GetPooledDialogue(int dialogueID)
		{
			if (pooledDialogues == null)
				return null;

			foreach (Dialogue dialogue in pooledDialogues)
			{
				if (dialogue.GetID() == dialogueID)
					return dialogue;
			}
			return null;
		}
	}
}
