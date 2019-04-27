using System;

public class GotoScreenRequestSignal {
  public Type ScreenType { get; set; }
  public IUIAnimation OpeningAnimation { get; set; }
  public IUIAnimation ClosingAnimation { get; set; }
}
