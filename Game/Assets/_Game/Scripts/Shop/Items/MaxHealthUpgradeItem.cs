using UnityEngine;

namespace Swen.Shop.Items {
  [CreateAssetMenu(menuName = "Shop/Max Health Upgrade Item")]
  public class MaxHealthUpgradeItem : ShopItem {
    public int MaxHealthAddition;
    public int RestoreHealthAmount;
  }
}
