using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelInterpreter {
  private SurviveLevelHandler _surviveLevelHandler;

  [Inject]
  private void Construct(SurviveLevelHandler surviveLevelHandler) {
    _surviveLevelHandler = surviveLevelHandler;
  }

  public LevelHandler Interpret(Level level) {
    // todo: refactor
    if (level is SurviveLevel) {
      return _surviveLevelHandler;
    }

    throw new System.Exception("No handler exists for this level type: " + level.GetType());
  }
}
