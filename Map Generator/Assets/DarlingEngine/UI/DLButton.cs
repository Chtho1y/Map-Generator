using DarlingEngine.Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DarlingEngine.UI
{
	[RequireComponent(typeof(Button))]
	public class DLButton : DLComponent<Button>
	{
		[SerializeField]
		private float coolTime = 0.1f;

		private float curTime = 0f;

		private bool isCooling = false;

		protected override void Awake()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			base.Awake();
			((UnityEventBase)base.Target.onClick).RemoveAllListeners();
			((UnityEvent)base.Target.onClick).AddListener(new UnityAction(Click));
		}

		private void Click()
		{
			if (!isCooling)
			{
				SendEvent("Click");
				isCooling = true;
				curTime = 0f;
			}
		}

		protected void Update()
		{
			if (isCooling)
			{
				curTime += Time.deltaTime;
				if (curTime > coolTime)
				{
					isCooling = false;
					curTime = 0f;
				}
			}
		}

		public void SetCoolTime(float coolTime)
		{
			if (coolTime < 0f)
			{
				coolTime = 0f;
			}
			this.coolTime = coolTime;
		}
	}
}