using DarlingEngine.Engine;
using UnityEngine.UI;

namespace DarlingEngine.UI
{
	public class DLInternationalText : DLComponent<Text>
	{
		public string Key;

		public void SetText(string text)
		{
			base.Target.text = text;
		}
	}
}