using UnityEngine;

namespace Swen.Shop.Items {
  [CreateAssetMenu(menuName = "Shop/Score Item")]
  public class ScoreItem : ShopItem {
    public int ScoreAmount;
  }
}
