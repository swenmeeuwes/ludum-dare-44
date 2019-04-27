using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader {
  private readonly MonoBehaviourUtil _monoBehaviourUtil;

  public AsyncOperation CurrentAsyncOperation { get; private set; }

  public SceneLoader(MonoBehaviourUtil monoBehaviourUtil) {
    _monoBehaviourUtil = monoBehaviourUtil;
  }

  public AsyncOperation LoadAsync(string sceneName) {
    CurrentAsyncOperation = SceneManager.LoadSceneAsync(sceneName);
    _monoBehaviourUtil.StartCoroutine(HandleAsyncLoading(CurrentAsyncOperation));

    return CurrentAsyncOperation;
  }

  private IEnumerator HandleAsyncLoading(AsyncOperation operation) {
    operation.allowSceneActivation = false;

    // To prevent a flashing loading screen
    yield return new WaitForSeconds(0.2f);

    while (!operation.isDone) {
      if (operation.progress >= 0.9f) {
        // When 'allowSceneActivation' is false it will not load beyond 0.9f (even is the the operation is done)
        operation.allowSceneActivation = true;
        CurrentAsyncOperation = null;
      }

      yield return null;
    }
  }
}
