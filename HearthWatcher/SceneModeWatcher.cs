using System;
using HearthMirror;
using HearthMirror.Enums;

namespace HearthWatcher
{
	public class SceneModeWatcher : Watcher
	{
		private SceneMode _sceneMode;
		public event Action<SceneMode, SceneMode> OnSceneModeChanged;

		public override void Update()
		{
			var sceneMode = Reflection.GetCurrentSceneMode();
			if(sceneMode != SceneMode.INVALID && sceneMode != _sceneMode)
			{
				OnSceneModeChanged?.Invoke(sceneMode, _sceneMode);
				_sceneMode = sceneMode;
			}
		}

		protected override void Reset()
		{
			_sceneMode = SceneMode.INVALID;
		}
	}
}