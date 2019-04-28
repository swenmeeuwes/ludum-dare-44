using Swen.Shop.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Shop : MonoBehaviour, IInitializable, IDisposable {
  [SerializeField] private GameObject _view;
  [SerializeField] private LayoutGroup _layoutGroup;

  private DiContainer _container;
  private SignalBus _signalBus;
  private ShopItemContext _shopItemContext;
  private ShopItemView _shopItemViewPrefab;
  private Player _player;

  private readonly List<ShopItemView> _shopItemViews = new List<ShopItemView>();

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
    _signalBus.Subscribe<ShopItemBoughtSignal>(OnShopItemBought);
  }

  public void Dispose() {
    _signalBus.TryUnsubscribe<ShopItemBoughtSignal>(OnShopItemBought);
  }

  public void HandleCloseButton() {
    Close();
  }

  public void ShowRandomItems(int amount = 3) {
    _view.SetActive(true);

    // Clean up previous
    foreach (var shopItemView in _shopItemViews) {
      Destroy(shopItemView);
    }

    // Fetch random items
    var fetchAmount = _shopItemContext.Config.Length < amount ? _shopItemContext.Config.Length : amount;
    var randomItems = new List<ShopItem>();
    var availableShopItems = _shopItemContext.Config.ToList();
    for (var i = 0; i < fetchAmount; i++) {
      var randomItem = availableShopItems[UnityEngine.Random.Range(0, availableShopItems.Count)];

      availableShopItems.Remove(randomItem);
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
    _view.SetActive(false);

    _signalBus.Fire(new ShopClosedSignal { });
  }

  private void OnShopItemBought(ShopItemBoughtSignal signal) {
    foreach (var shopItemView in _shopItemViews) {
      shopItemView.CanBuy = _player.Health > shopItemView.ShopItem.Cost; // Don't allow buying when player doesn't have enough health
    }
  }
}
