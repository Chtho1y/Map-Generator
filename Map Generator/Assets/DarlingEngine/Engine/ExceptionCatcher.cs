using UnityEngine;
using UnityEngine.EventSystems;

namespace DarlingEngine.Engine
{
	public class ExceptionCatcher : MonoSingleton<ExceptionCatcher>
	{
		private string AppException = null;

		private Rect WindowRect;

		private GUIStyle Style;

		protected override void OnSingletonAwake()
		{
			hideFlags = (HideFlags)2;
			float num = Screen.width * 0.8f;
			float num2 = Screen.height * 0.8f;
			WindowRect = new Rect(Screen.width * 0.5f - num * 0.5f, Screen.height * 0.5f - num2 * 0.5f, num, num2);
			Style = new GUIStyle();
			Style.normal.textColor = Color.cyan;
			Style.fontSize = 35;
			Style.fontStyle = FontStyle.Normal;
			Style.alignment = TextAnchor.UpperLeft;
			Style.wordWrap = true;
			Style.clipping = TextClipping.Clip;
			Application.logMessageReceived += new Application.LogCallback(ReceiveMessage);
		}

		private void ReceiveMessage(string msg, string trace, LogType type)
		{
			if ((int)type == 4)
			{
				AppException = $"{msg}\n{trace}";
				EventSystem[] array = FindObjectsOfType<EventSystem>();
				EventSystem[] array2 = array;
				foreach (EventSystem val in array2)
				{
					val.enabled = false;
				}
				if (MonoSingleton<GameManager>.Instance != null)
				{
					MonoSingleton<GameManager>.Instance.OnException(AppException);
				}
			}
		}

		protected void OnGUI()
		{
			if (AppException != null)
			{
				GUI.color = Color.white;
				WindowRect = GUI.Window(0, WindowRect, new GUI.WindowFunction(DrawWindow), "System Exception");
			}
		}

		private void DrawWindow(int windowId)
		{
			GUI.Label(new Rect(10f, 50f, WindowRect.width - 10f, WindowRect.height - 100f), AppException, Style);
			int num = 80;
			int num2 = 50;
			int num3 = 200;
			if (GUI.Button(new Rect(WindowRect.width * 0.5f - num / 2f - num3 / 2f, WindowRect.height - 70f, num, num2), "Close"))
			{
				AppException = null;
			}
			if (GUI.Button(new Rect(WindowRect.width * 0.5f - num / 2f + num3 / 2f, WindowRect.height - 70f, num, num2), "Exit"))
			{
				Application.Quit();
			}
			GUI.color = Color.white;
		}
	}
}