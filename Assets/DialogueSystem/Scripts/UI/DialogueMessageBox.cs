using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
	public abstract class DialogueMessageBox : MonoBehaviour
	{
		public Text msg;

		public virtual void SetMessage(DialogueActor speakerActor, string _msg)
		{
			msg.text = _msg;
		}
	}
}
