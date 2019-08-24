using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
	public class DialogueActor
	{
		public int id { get; }
		public string name { get; }
		public bool isLeft { get; }
		public Sprite icon { get; }

		public DialogueActor(int _id, string _name, bool _isLeft, Sprite _icon)
		{
			id = _id;
			name = _name;
			isLeft = _isLeft;
			icon = _icon;
		}
	}
}
