using Swen.Shop.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

public class Shop : MonoBehaviour, IInitializable, IDisposable {
  [SerializeField] private GameObject _view;
  [SerializeField] private LayoutGroup _layoutGroup;

  private DiContainer _container;
  private SignalBus _signalBus;
  private ShopItemContext _shopItemContext;
  private ShopItemView _shopItemViewPrefab;
  private Player _player;

  private readonly List<ShopItemView> _shopItemViews = new List<ShopItemView>();
  private readonly List<ShopItem> _boughtShopItems = new List<ShopItem>();

  private List<ShopItem> _availableShopItems;

  private Vector3 _originalViewPosition;

  [Inject]
  private void Construct(DiContainer container, SignalBus signalBus, ShopItemContext shopItemContext, ShopItemView shopItemView, Player player) {
    _container = container;
    _signalBus = signalBus;
    _shopItemContext = shopItemContext;
    _shopItemViewPrefab = shopItemView;
    _player = player;
  }

  private void Awake() {
    _view.SetActive(false);
  }

  public void Initialize() {
    _availableShopItems = _shopItemContext.Config.ToList();
    _originalViewPosition = _view.transform.position;

    _signalBus.Subscribe<ShopItemBoughtSignal>(OnShopItemBought);
    _signalBus.Subscribe<PlayerHealthChangedSignal>(OnPlayerHealthChanged);
  }

  public void Dispose() {
    _signalBus.TryUnsubscribe<ShopItemBoughtSignal>(OnShopItemBought);
    _signalBus.TryUnsubscribe<PlayerHealthChangedSignal>(OnPlayerHealthChanged);
  }

  public void HandleCloseButton() {
    Close();
  }

  public void ShowRandomItems(int amount = 3) {
    _view.transform.position = _originalViewPosition + new Vector3(0, Screen.height, 0);

    // Clean up previous
    foreach (var shopItemView in _shopItemViews) {
      GameObject.Destroy(shopItemView.gameObject);
    }
    _shopItemViews.Clear();

    _view.SetActive(true);
    _view.transform.DOMoveY(_originalViewPosition.y, .85f);

    // Fetch random items
    var shopItems = _availableShopItems.ToList(); // ToList -> to clone the list (since we don't want to remove the shop items from available shop item list)
    var fetchAmount = shopItems.Count < amount ? shopItems.Count : amount;
    var randomItems = new List<ShopItem>();
    for (var i = 0; i < fetchAmount; i++) {
      var randomItem = shopItems[UnityEngine.Random.Range(0, shopItems.Count)];

      shopItems.Remove(randomItem);
      randomItems.Add(randomItem);
    }

    for (var i = 0; i < randomItems.Count; i++) {
      var shopItem = randomItems[i];

      var shopItemView = _container.InstantiatePrefabForComponent<ShopItemView>(_shopItemViewPrefab);
      shopItemView.transform.SetParent(_layoutGroup.transform, false);
      shopItemView.ShopItem = shopItem;

      shopItemView.CanBuy = _player.Health > shopItem.Cost; // Don't allow buying when player doesn't have enough health

      _shopItemViews.Add(shopItemView);
    }
  }

  public void Close() {
    _view.transform
      .DOMoveY((_originalViewPosition + new Vector3(0, Screen.height, 0)).y, .85f)
      .OnComplete(() => {
        _view.SetActive(false);

        _signalBus.Fire(new ShopClosedSignal { });
      });
  }

  private void OnShopItemBought(ShopItemBoughtSignal signal) {
    var boughtItem = signal.ShopItem;
    _boughtShopItems.Add(boughtItem);
    if (boughtItem.OnlyAvailableOnce) {
      _availableShopItems.Remove(boughtItem);
    }

    ProcessBoughtShopItem(boughtItem);
  }

  private void OnPlayerHealthChanged(PlayerHealthChangedSignal signal) {
    foreach (var shopItemView in _shopItemViews) {
      shopItemView.CanBuy = signal.NewHealth > shopItemView.ShopItem.Cost; // Don't allow buying when player doesn't have enough health
    }
  }

  private void ProcessBoughtShopItem(ShopItem shopItem) {
    // Breaking the SOLID L ;c -> todo: visitor time?
    if (shopItem is ScoreItem) {
      _signalBus.Fire(new AddScoreSignal { Amount = ((ScoreItem)shopItem).ScoreAmount });
      return;
    }

    if (shopItem is BowStringUpgradeItem) {
      _player.Weapon.AddedFirePower += ((BowStringUpgradeItem)shopItem).FirePowerAddition;
      return;
    }

    if (shopItem is MaxHealthUpgradeItem) {
      _player.MaxHealth += ((MaxHealthUpgradeItem)shopItem).MaxHealthAddition;
      _player.Health += ((MaxHealthUpgradeItem)shopItem).RestoreHealthAmount;
      return;
    }

    if (shopItem is MovementSpeedUpgradeItem) {
      _player.Movement.AddedMaxMovementSpeed += ((MovementSpeedUpgradeItem)shopItem).MovementSpeedAddition;
      return;
    }

    if (shopItem is JumpForceUpgradeItem) {
      _player.Movement.AddedJumpForce += ((JumpForceUpgradeItem)shopItem).JumpForceAddition;
      return;
    }
  }
}
