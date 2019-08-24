using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
	public class RPGDialogueMessageBox : DialogueMessageBox
	{
		public Image speakerIcon;
		public Text speakerName;

		bool isLeft = true;

		public override void SetMessage(DialogueActor speakerActor, string _msg)
		{
			base.SetMessage(speakerActor, _msg);

			speakerIcon.sprite = speakerActor.icon;
			speakerName.text = speakerActor.name;
			if (isLeft != speakerActor.isLeft)
				ChangeMessageDirection(speakerActor.isLeft);
		}

		private void ChangeMessageDirection(bool _isLeft)
		{
			isLeft = _isLeft;
			GetComponent<RectTransform>().Rotate(0f, isLeft ? -180f : 180f, 0f);
			speakerIcon.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
			speakerName.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
			msg.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
		}
	}
}
