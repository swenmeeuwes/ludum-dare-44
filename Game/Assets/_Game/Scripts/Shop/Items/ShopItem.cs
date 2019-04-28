using UnityEngine;

namespace Swen.Shop.Items {
  public abstract class ShopItem : ScriptableObject {
    public string Name;
    public Sprite Sprite;
    public int Cost; // In healthpoints
    public bool OnlyAvailableOnce;
  }
}
