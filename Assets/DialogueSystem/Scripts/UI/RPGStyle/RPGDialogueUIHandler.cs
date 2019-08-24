using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
	public class RPGDialogueUIHandler : DialogueUIHandler
	{
		public GameObject content_UI, messageBox_UI, optionBox_UI;
		public Image background_UI;
		public Button showNextMessageButton;
		public float backgroundAlpha = 0.25f;

		Dialogue openedDialogue;
		DialogueActor[] actors;
		DialogueMessage[] currentMessages;

		private void Awake()
		{
			Color bgColor = background_UI.color;
			background_UI.color = new Color(bgColor.r, bgColor.g, bgColor.b, backgroundAlpha);
			showNextMessageButton.onClick.AddListener(GoToNextMessage_Handler);
		}

		public override void OpenDialogue(Dialogue dialogue, params DialogueActor[] _actors)
		{
			// Check is opening dialog valid.
			if (dialogue == null)
				return;

			openedDialogue = dialogue;
			actors = _actors;
			content_UI.SetActive(true);
			ShowMessage(dialogue.GetRootMessages());
		}

		void ShowMessage(DialogueMessage[] messages)
		{
			if (messages.Length != 0)
			{
				DialogueActor actor = GetActorByID(messages[0].ActorID);
				if (actor == null)
					return;

				if (messages.Length == 1) // If message.
				{
					messageBox_UI.SetActive(true);
					optionBox_UI.SetActive(false);
					showNextMessageButton.gameObject.SetActive(true);

					messageBox_UI.GetComponent<DialogueMessageBox>().SetMessage(actor, messages[0].Text);
				}
				else // If option.
				{
					messageBox_UI.SetActive(false);
					optionBox_UI.SetActive(true);
					showNextMessageButton.gameObject.SetActive(false);

					string[] texts = new string[messages.Length];
					for (int i = 0; i < texts.Length; i++)
						texts[i] = messages[i].Text;
					optionBox_UI.GetComponent<DialogueOptionBox>().SetOption(actor, texts, OptionSelected);
				}
				currentMessages = messages;
			}
			else
				CloseDialogue();
		}

		void CloseDialogue()
		{
			openedDialogue = null;
			actors = null;
			content_UI.SetActive(false);
			// TODO: Call Callback For DialogueManager.
		}

		DialogueActor GetActorByID(int actorID)
		{
			if (actors == null)
				return null;

			foreach (DialogueActor actor in actors)
			{
				if (actor.ID == actorID)
					return actor;
			}
			return null;
		}

		/** Called when Pressed on screen. */
		void GoToNextMessage_Handler()
		{
			ShowMessage(openedDialogue.GetChildMessages(currentMessages[0]));
		}

		public void OptionSelected(int optionIdx)
		{
			ShowMessage(openedDialogue.GetChildMessages(currentMessages[optionIdx]));
		}
	}
}
