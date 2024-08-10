using DarlingEngine.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace DarlingEngine.UI
{
	public class DLRawImage : DLComponent<RawImage>
	{
		public Texture texture
		{
			get
			{
				return base.Target.texture;
			}
			set
			{
				base.Target.texture = value;
			}
		}

		public void SetTexture(Texture texture)
		{
			base.Target.texture = texture;
		}
	}
}
