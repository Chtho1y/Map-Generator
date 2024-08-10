using System.Diagnostics;
using DarlingEngine.Engine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DarlingEngine.UI
{
	public class DLInputField : DLComponent<InputField>
	{
		public string text
		{
			get
			{
				return base.Target.text;
			}
			set
			{
				base.Target.text = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			((UnityEventBase)base.Target.onValueChanged).RemoveAllListeners();
			((UnityEvent<string>)(object)base.Target.onValueChanged).AddListener((UnityAction<string>)ValueChanged);
			((UnityEvent<string>)(object)base.Target.onEndEdit).AddListener((UnityAction<string>)EndEdit);
			((UnityEvent<string>)(object)base.Target.onSubmit).AddListener((UnityAction<string>)Submit);
		}

		private void ValueChanged(string value)
		{
			SendEvent("ValueChanged", value);
		}

		private void EndEdit(string value)
		{
			SendEvent("EndEdit", value);
		}

		private void Submit(string value)
		{
			SendEvent("Submit", value);
		}
	}
}