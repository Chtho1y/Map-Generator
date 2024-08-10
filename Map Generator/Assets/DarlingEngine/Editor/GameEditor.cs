using UnityEditor;
using UnityEditor.SceneManagement;
using DarlingEngine.Engine.Bundle;

namespace DarlingEngine.Editor
{
	internal class GameEditor : UnityEditor.Editor
	{

		[MenuItem("Tools/Game Tool/Start Game")]
		private static void StartGame()
		{
			EditorSceneManager.OpenScene(PathProtocol.InitScenePath);
			EditorApplication.isPlaying = true;
		}
	}
}
