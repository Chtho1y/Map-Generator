using DarlingEngine.Engine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarlingEngine.UI
{
	[RequireComponent(typeof(DragButton))]
	public class DLDragButton : DLComponent<DragButton>
	{
		protected override void Awake()
		{
			base.Awake();
			base.Target.onDrag = Drag;
			base.Target.onDown = Down;
			base.Target.onUp = Up;
		}

		private void Drag(PointerEventData eventData)
		{
			SendEvent("Drag", eventData);
		}

		private void Down(PointerEventData eventData)
		{
			SendEvent("Down", eventData);
		}

		private void Up(PointerEventData eventData)
		{
			SendEvent("Up", eventData);
		}
	}
}