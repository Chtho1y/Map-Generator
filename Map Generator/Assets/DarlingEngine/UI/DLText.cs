using DarlingEngine.Engine;
using UnityEngine.UI;

namespace DarlingEngine.UI
{
	public class DLText : DLComponent<Text>
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

		public void SetText(string text)
		{
			base.Target.text = text;
		}
	}
}