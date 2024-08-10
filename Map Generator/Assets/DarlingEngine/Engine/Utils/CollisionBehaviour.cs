using System;
using UnityEngine;

namespace DarlingEngine.Engine
{
	public class CollisionBehaviour : MonoBehaviour
	{
		public Action<Collision> onCollisionEnter;

		public Action<Collision> onCollisionStay;

		public Action<Collision> onCollisionExit;

		protected void OnCollisionEnter(Collision collision)
		{
			onCollisionEnter?.Invoke(collision);
		}

		protected void OnCollisionStay(Collision collision)
		{
			onCollisionStay?.Invoke(collision);
		}

		protected void OnCollisionExit(Collision collision)
		{
			onCollisionExit?.Invoke(collision);
		}

		protected void OnDestroy()
		{
			onCollisionEnter = null;
			onCollisionStay = null;
			onCollisionExit = null;
		}
	}
}