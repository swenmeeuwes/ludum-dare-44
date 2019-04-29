using Swen.Shop.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

public class ShopItemView : MonoBehaviour
{
  [SerializeField] private TMP_Text _itemNameTextField;
  [SerializeField] private Image _itemImage;
  [SerializeField] private HealthView _costView;
  [SerializeField] private Button _buyButton;
  [SerializeField] private GameObject _soldBanner;
  [SerializeField] private Transform _itemInfoContainer;

  private SignalBus _signalBus;

  private ShopItem _shopItem;
  public ShopItem ShopItem {
    get => _shopItem;
    set {
      _shopItem = value;
      Show();
    }
  }

  private bool _canBuy;
  public bool CanBuy {
    get => _canBuy;
    set {
      _canBuy = true;

      if (!SoldOut) {
        _buyButton.interactable = value;
      }
    }
  }

  public bool SoldOut { get; set; }

  [Inject]
  private void Construct(SignalBus signalBus) {
    _signalBus = signalBus;
  }

  private void Awake() {
    _soldBanner.SetActive(false);

    var buyButtonCanvasGroup = _buyButton.GetComponent<CanvasGroup>();
    var endPosition = _itemInfoContainer.position;

    buyButtonCanvasGroup.DOFade(0, 0);
    //_itemInfoContainer.DOMoveY(endPosition.y + Screen.height, 0);

    DOTween.Sequence()
      //.Append(_itemInfoContainer.DOMoveY(endPosition.y, 1.5f))
      .SetDelay(1f)
      .Append(buyButtonCanvasGroup.DOFade(1, .45f));
  }

  public void HandleBuyButton() {
    _signalBus.Fire(new ShopItemBoughtSignal {
      ShopItem = ShopItem
    });

    _soldBanner.SetActive(true);
    DOTween.Sequence()
      .Append(_soldBanner.transform.DOPunchScale(Vector3.one, .45f));

    _buyButton.GetComponentInChildren<TMP_Text>().text = "Sold Out";
    _buyButton.interactable = false;

    SoldOut = true;
  }

  private void Show() {
    _itemNameTextField.text = ShopItem.Name;
    _itemImage.sprite = ShopItem.Sprite;
    _costView.EnsureEnoughHeartsFor(ShopItem.Cost);
    _costView.ShowHealth(ShopItem.Cost);
  }
}
