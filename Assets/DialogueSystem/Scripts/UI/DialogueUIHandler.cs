using System;
using UnityEngine;

namespace DialogueSystem.UI
{
	public abstract class DialogueUIHandler : MonoBehaviour
	{
		public GameObject content_UI, messageBox_UI, optionBox_UI;

		protected Dialogue openedDialogue;
		protected DialogueActor[] actors;
		protected DialogueMessage[] currentMessages;
		protected Action onDialogueClosed;

		public virtual void OpenDialogue(Dialogue dialogue, Action _onDialogueClosed, params DialogueActor[] _actors)
		{
			// Check is opening dialog valid.
			if (dialogue == null)
				return;

			openedDialogue = dialogue;
			onDialogueClosed = _onDialogueClosed;
			actors = _actors;
			content_UI.SetActive(true);
			ShowMessage(dialogue.GetRootMessages());
		}

		protected virtual void CloseDialogue()
		{
			openedDialogue = null;
			actors = null;
			content_UI.SetActive(false);
			// Call Callback For DialogueManager.
			onDialogueClosed?.Invoke();
		}

		protected virtual void ShowMessage(DialogueMessage[] messages)
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

					messageBox_UI.GetComponent<DialogueMessageBox>().SetMessage(actor, messages[0].Text);
				}
				else // If option.
				{
					messageBox_UI.SetActive(false);
					optionBox_UI.SetActive(true);

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

		protected DialogueActor GetActorByID(int actorID)
		{
			if (actors == null)
				return null;

			foreach (DialogueActor actor in actors)
			{
				if (actor.id == actorID)
					return actor;
			}
			return null;
		}

		protected virtual void GoToNextMessage_Handler()
		{
			ShowMessage(openedDialogue.GetChildMessages(currentMessages[0]));
		}

		protected virtual void OptionSelected(int optionIdx)
		{
			ShowMessage(openedDialogue.GetChildMessages(currentMessages[optionIdx]));
		}
	}
}
