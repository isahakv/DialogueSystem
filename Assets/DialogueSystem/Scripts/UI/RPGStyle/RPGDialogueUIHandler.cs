using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
	public class RPGDialogueUIHandler : DialogueUIHandler
	{
		public Image background_UI;
		public Button showNextMessageButton;
		public float backgroundAlpha = 0.25f;

		private void Awake()
		{
			Color bgColor = background_UI.color;
			background_UI.color = new Color(bgColor.r, bgColor.g, bgColor.b, backgroundAlpha);
			showNextMessageButton.onClick.AddListener(GoToNextMessage_Handler);
		}

		protected override void ShowMessage(DialogueMessage[] messages)
		{
			base.ShowMessage(messages);

			if (messages.Length == 1) // If message.
				showNextMessageButton.gameObject.SetActive(true);
			else // If option.
				showNextMessageButton.gameObject.SetActive(false);
		}
	}
}
