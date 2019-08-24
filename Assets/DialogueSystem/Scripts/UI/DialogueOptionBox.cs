using System;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
	public abstract class DialogueOptionBox : MonoBehaviour
	{
		public Transform optionsParent;
		public GameObject optionInstance;

		protected bool isLeft = true;
		protected GameObject[] optionGameObjects;
		protected Action<int> onOptionSelected;

		public virtual void SetOption(DialogueActor speakerActor, string[] msges, Action<int> _onOptionSelected)
		{
			onOptionSelected = _onOptionSelected;
			
			// Spawn Option GameObjects.
			optionGameObjects = new GameObject[msges.Length];
			for (int i = 0; i < optionGameObjects.Length; i++)
			{
				optionGameObjects[i] = Instantiate(optionInstance, optionsParent);
				optionGameObjects[i].SetActive(true);
				optionGameObjects[i].GetComponentInChildren<Text>().text = msges[i];
				int index = i;
				optionGameObjects[i].GetComponent<Button>().onClick.AddListener(() => OnOptionSelected(index));
			}

			if (isLeft != speakerActor.isLeft)
				ChangeOptionDirection(speakerActor.isLeft);
		}

		protected virtual void ChangeOptionDirection(bool _isLeft)
		{
			isLeft = _isLeft;
			GetComponent<RectTransform>().Rotate(0f, isLeft ? -180f : 180f, 0f);
			foreach (GameObject option in optionGameObjects)
				option.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
		}

		protected virtual void OnOptionSelected(int optionIdx)
		{
			onOptionSelected?.Invoke(optionIdx);
		}
	}
}
