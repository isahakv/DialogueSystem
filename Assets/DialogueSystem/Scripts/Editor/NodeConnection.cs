using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DialogueSystem.Editor
{
	public class NodeConnection
	{
		public NodeConnectionPoint inPoint;
		public NodeConnectionPoint outPoint;
		Action<NodeConnection> onClickedRemoveConnection;
		
		public NodeConnection(NodeConnectionPoint _inPoint, NodeConnectionPoint _outPoint, Action<NodeConnection> _onClickedRemoveConnection)
		{
			inPoint = _inPoint;
			outPoint = _outPoint;
			onClickedRemoveConnection = _onClickedRemoveConnection;
		}

		public void Draw()
		{
			Vector3 startPos = inPoint.ownerNode.rect.position + inPoint.rect.center;
			Vector3 endPos = outPoint.ownerNode.rect.position + outPoint.rect.center;
			Vector3 startTan = startPos - Vector3.right * 50f;
			Vector3 endTan = endPos + Vector3.right * 50f;

			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 2f);

			if (Handles.Button((startPos + endPos) * 0.5f, Quaternion.identity, 4f, 8f, Handles.RectangleHandleCap))
				onClickedRemoveConnection?.Invoke(this);
		}
	}
}
