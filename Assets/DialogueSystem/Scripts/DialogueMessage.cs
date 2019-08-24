using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
	[System.Serializable]
	public class DialogueMessage
	{
		int id;
		int actorID;
		string text;

		public List<int> childMsgIDs;

		public int ID => id;
		public int ActorID => actorID;
		public string Text => text;

		public DialogueMessage(int _id, int _actorID, string _text)
		{
			id = _id;
			actorID = _actorID;
			text = _text;
			childMsgIDs = new List<int>();
		}
	}
}
