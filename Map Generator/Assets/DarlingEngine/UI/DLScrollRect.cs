using DarlingEngine.Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DarlingEngine.UI
{
	public class DLScrollRect : DLComponent<ScrollRect>
	{
		protected override void Awake()
		{
			base.Awake();
			((UnityEventBase)base.Target.onValueChanged).RemoveAllListeners();
			((UnityEvent<Vector2>)(object)base.Target.onValueChanged).AddListener((UnityAction<Vector2>)ValueChanged);
		}

		private void ValueChanged(Vector2 value)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			SendEvent("ValueChanged", value);
		}
	}
}