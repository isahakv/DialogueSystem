using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DialogueSystem.Editor
{
	public enum ConnectionPointType
	{
		In,
		Out
	}

	public class NodeConnectionPoint
	{
		public Rect rect;
		public DialogueNode ownerNode;
		public ConnectionPointType type;

		Action <NodeConnectionPoint> onClickedConnectionPoint;

		public NodeConnectionPoint(DialogueNode _ownerNode, ConnectionPointType _type, Action<NodeConnectionPoint> _onClickedConnectionPoint)
		{
			rect = new Rect(0f, 0f, 30f, 16f);
			ownerNode = _ownerNode;
			type = _type;
			onClickedConnectionPoint = _onClickedConnectionPoint;
		}

		public void Draw()
		{
			rect.x = (ownerNode.rect.width * 0.5f) - (rect.width * 0.5f);

			switch (type)
			{
				case ConnectionPointType.In:
					rect.y = 0;
					break;
				case ConnectionPointType.Out:
					rect.y = ownerNode.rect.height - rect.height;
					break;
			}

			if (GUI.Button(rect, ""))
				onClickedConnectionPoint?.Invoke(this);
		}
	}
}
