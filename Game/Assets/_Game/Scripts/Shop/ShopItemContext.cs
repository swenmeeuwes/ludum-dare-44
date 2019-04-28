using Swen.Shop.Items;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Item Context", menuName = "Context/Shop Item Context")]
public class ShopItemContext : ScriptableObject {
  public ShopItem[] Config;
}