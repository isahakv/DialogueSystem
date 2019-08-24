using System;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
	public class RPGDialogueOptionBox : DialogueOptionBox
	{
		public Image speakerIcon;
		public Text speakerName;

		public override void SetOption(DialogueActor speakerActor, string[] msges, Action<int> _onOptionSelected)
		{
			base.SetOption(speakerActor, msges, _onOptionSelected);
			speakerIcon.sprite = speakerActor.icon;
			speakerName.text = speakerActor.name;
		}

		protected override void ChangeOptionDirection(bool _isLeft)
		{
			base.ChangeOptionDirection(_isLeft);
			speakerIcon.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
			speakerName.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
		}
	}
}
