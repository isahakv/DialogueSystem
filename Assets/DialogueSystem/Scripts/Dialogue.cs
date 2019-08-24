using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
	[System.Serializable]
	public class Dialogue
	{
		int id;
		string name;

		public int[] rootMessageIDs;
		public DialogueMessage[] messages;

		public Dialogue(int _id, string _name)
		{
			id = _id;
			name = _name;
		}

		public DialogueMessage[] GetRootMessages()
		{
			DialogueMessage[] rootMessages = new DialogueMessage[rootMessageIDs.Length];
			for (int i = 0; i < rootMessages.Length; i++)
				rootMessages[i] = GetMessageByID(rootMessageIDs[i]);
			return rootMessages;
		}

		public DialogueMessage[] GetChildMessages(DialogueMessage parentMessage)
		{
			return GetMessagesByID(parentMessage.childMsgIDs.ToArray());
		}

		public DialogueMessage GetMessageByID(int id)
		{
			return GetMessageByID(messages, id);
		}

		public DialogueMessage[] GetMessagesByID(int[] ids)
		{
			return GetMessagesByID(messages, ids);
		}

		public static DialogueMessage GetMessageByID(DialogueMessage[] messages, int id)
		{
			foreach (DialogueMessage message in messages)
			{
				if (message.ID == id)
					return message;
			}
			return null;
		}

		public static DialogueMessage[] GetMessagesByID(DialogueMessage[] messages, int[] ids)
		{
			List<int> IDs = new List<int>();
			foreach (int id in ids)
				IDs.Add(id);

			List<DialogueMessage> matchedMessages = new List<DialogueMessage>();
			foreach (DialogueMessage message in messages)
			{
				for (int i = 0; i < IDs.Count; i++)
				{
					if (message.ID == IDs[i])
					{
						matchedMessages.Add(message);
						IDs.RemoveAt(i);
						break;
					}
				}
			}
			return matchedMessages.ToArray();
		}

		public int GetID() { return id; }
	}
}
