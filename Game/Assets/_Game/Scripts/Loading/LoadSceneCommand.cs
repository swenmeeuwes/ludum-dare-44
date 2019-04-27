using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LoadSceneCommand : ICommand<LoadSceneSignal>
{
    private SignalBus _signalBus;
    private SceneLoader _sceneLoader;
    private MonoBehaviourUtil _monoBehaviourUtil;

    public LoadSceneCommand(SignalBus signalBus, SceneLoader sceneLoader, MonoBehaviourUtil monoBehaviourUtil)
    {
        _signalBus = signalBus;
        _sceneLoader = sceneLoader;
        _monoBehaviourUtil = monoBehaviourUtil;
    }

    public void Execute(LoadSceneSignal signal)
    {
        //_signalBus.Fire(new OpenScreenRequestSignal
        //{
        //    Type = ScreenType.Loading
        //});

        var loadOperation = _sceneLoader.LoadAsync(signal.SceneName);
    }
}
