using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.UI
{
	public abstract class DialogueOptionBox : MonoBehaviour
	{
		public Image speakerIcon;
		public Text speakerName;
		public Transform optionsParent;
		public GameObject optionInstance;

		bool isLeft = true;
		GameObject[] optionGameObjects;
		Action<int> onOptionSelected;

		public void SetOption(DialogueActor speakerActor, string[] msges, Action<int> _onOptionSelected)
		{
			onOptionSelected = _onOptionSelected;
			speakerName.text = speakerActor.Name;
			
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

			if (isLeft != speakerActor.IsLeft)
				ChangeOptionDirection(speakerActor.IsLeft);
		}

		void ChangeOptionDirection(bool _isLeft)
		{
			isLeft = _isLeft;
			GetComponent<RectTransform>().Rotate(0f, isLeft ? -180f : 180f, 0f);
			speakerIcon.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
			speakerName.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
			foreach (GameObject option in optionGameObjects)
				option.GetComponent<RectTransform>().Rotate(0f, isLeft ? 180f : -180f, 0f);
		}

		void OnOptionSelected(int optionIdx)
		{
			onOptionSelected?.Invoke(optionIdx);
		}
	}
}
