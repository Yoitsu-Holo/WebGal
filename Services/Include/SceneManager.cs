using WebGal.Event;
using WebGal.Libs.Base;
using WebGal.Types;

namespace WebGal.Services.Include;

/// <summary>
/// 简单的键值对映射 [string]name -> [Scene]scene
/// </summary>
public class SceneManager
{
	private readonly List<Scene> _scenes = [];
	public bool IsStatic;

	public readonly Queue<string> SceneNameList = new();
	public bool ContainsScene(string sceneName) => _scenes.ContainsKey(sceneName);


	/// <summary>
	/// 放入一个场景
	/// </summary>
	/// <param name="sceneName"></param>
	/// <param name="scene"></param>
	public void PushScene(string sceneName, Scene scene) => _scenes[sceneName] = scene;

	/// <summary>
	/// 通过名字获取一个场景
	/// </summary>
	/// <param name="sceneName"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public Scene LoadScene(string sceneName) => _scenes.ContainsKey(sceneName) ? _scenes[sceneName] : throw new Exception("LoadScene: no key");


	/// <summary>
	/// 通过场景名字删除场景
	/// </summary>
	/// <param name="sceneName"></param>
	public bool RemoveScene(string sceneName) => _scenes.ContainsKey(sceneName) ? _scenes.Remove(sceneName) : throw new Exception("RemoveScene: no key");

	public void Clear()
	{
		LoopAudioSet.Clear();
		OneShotAudioSet.Clear();
		_scenes.Clear();
		SceneNameList.Clear();
	}

	// Action
	private readonly Dictionary<string, List<ActionStructure>> _leftClickAction = new();
	private readonly Dictionary<string, List<ActionStructure>> _rightClickAction = new();
	private readonly Dictionary<string, List<ActionStructure>> _holdAction = new();
	private readonly Dictionary<string, List<ActionStructure>> _moveOnAction = new();

	private readonly List<(string, MouseEvent, List<ActionStructure>)> _mouseEvents = new();
	private readonly List<(MouseEvent, List<ActionStructure>)> _mouseDefaultEvents = new();

	public void RegitserMouseAction(string trigerLayerName, MouseEvent mouseEvent, List<ActionStructure> actions)
	{
		_mouseEvents.Add((trigerLayerName, mouseEvent, actions));
	}

	public void RegitserMouseDefaultAction(MouseEvent mouseEvent, List<ActionStructure> actions)
	{
		_mouseDefaultEvents.Add((mouseEvent, actions));
	}

	public bool DoMouseEvent(MouseEvent mouseEvent)
	{
		bool trigered = false;
		foreach (var (trigerLayerName, targetEvent, actions) in _mouseEvents)
		{
			if (targetEvent.Button != mouseEvent.Button || targetEvent.Status != mouseEvent.Status)
				continue;

			trigered = true;
			int trigerLayerId = _layersId[trigerLayerName];
			Libs.Base.Layer trigerLayer = Layers[trigerLayerId];

			var point = mouseEvent.Position;

			if (RangeComp.OutRange(new(trigerLayer.Pos.X, trigerLayer.Pos.X + trigerLayer.WinSize.Width), point.X) ||
				RangeComp.OutRange(new(trigerLayer.Pos.Y, trigerLayer.Pos.Y + trigerLayer.WinSize.Height), point.Y))
				continue;

			StateHasChange = true;

			foreach (var action in actions)
				DoActions(action);
		}

		if (trigered)
			return trigered;

		// 执行默认动作
		foreach (var (targetEvent, actions) in _mouseDefaultEvents)
		{
			if (targetEvent.Button != mouseEvent.Button || targetEvent.Status != mouseEvent.Status)
				continue;
			trigered = true;
			StateHasChange = true;
			foreach (var action in actions)
				DoActions(action);
		}
		return trigered;
	}

	private void DoActions(ActionStructure action)
	{
		if (action.LayerName is not null)
		{
			var actionLayerId = _layersId[action.LayerName];
			Layers[actionLayerId].DynamicAttribute = action.Attribute;
		}
		if (action.JumpNodeLabel is not null)
		{
			OnJump?.Invoke(this, new JumpEventArgs()
			{
				JumpNodeLabel = action.JumpNodeLabel,
				JumpSceneLabel = action.JumpSceneLabel
			});
			// Console.WriteLine($"jump to {action.JumpNodeLabel}");
		}
	}
}