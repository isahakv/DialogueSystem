using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
	public abstract class DialogueMessageBox : MonoBehaviour
	{
		public Image speakerIcon;
		public Text speakerName;
		public Text msg;

		bool isLeft = true;

		public void SetMessage(DialogueActor speakerActor, string _msg)
		{
			speakerName.text = speakerActor.Name;
			if (isLeft != speakerActor.IsLeft)
				ChangeMessageDirection(speakerActor.IsLeft);
			msg.text = _msg;
		}

		void ChangeMessageDirection(bool _isLeft)
		{
			isLeft = _isLeft;
			GetComponent<RectTransform>().Rotate(0f, isLeft ? -180f : 180f, 0f);
			speakerIcon.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
			speakerName.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
			msg.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
		}
	}
}
