using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
	public class DialogueActor
	{
		int id;
		string name;
		bool isLeft;
		Texture2D icon;

		public int ID => id;
		public string Name => name;
		public bool IsLeft => isLeft;

		public DialogueActor(int _id, string _name, bool _isLeft)
		{
			id = _id;
			name = _name;
			isLeft = _isLeft;
		}
	}
}
