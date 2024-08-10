using UnityEngine;

namespace DarlingEngine.Engine
{
	public class DLTransform : DLComponent<Transform>
	{
		public new GameObject gameObject => Target.gameObject;

		public void SetActive(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}