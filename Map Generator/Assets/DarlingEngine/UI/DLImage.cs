using DarlingEngine.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace DarlingEngine.UI
{
	public class DLImage : DLComponent<Image>
	{
		public Sprite sprite
		{
			get
			{
				return base.Target.sprite;
			}
			set
			{
				base.Target.sprite = value;
			}
		}

		public void SetSprite(Sprite sprite)
		{
			base.Target.sprite = sprite;
		}
	}
}