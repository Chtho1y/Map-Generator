using DarlingEngine.Engine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DarlingEngine.UI
{
	public class DLSlider : DLComponent<Slider>
	{
		public float value
		{
			get
			{
				return base.Target.value;
			}
			set
			{
				base.Target.value = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			((UnityEventBase)base.Target.onValueChanged).RemoveAllListeners();
			((UnityEvent<float>)(object)base.Target.onValueChanged).AddListener((UnityAction<float>)ValueChanged);
		}

		private void ValueChanged(float value)
		{
			SendEvent("ValueChanged", value);
		}
	}
}
