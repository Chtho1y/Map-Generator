using System;
using UnityEngine;

namespace DarlingEngine.Engine
{
	public class AnimationEventHandler : MonoBehaviour
	{
		public Action<string> handler;

		protected void OnAnimationEvent(string data)
		{
			handler?.Invoke(data);
		}
	}
}