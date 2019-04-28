using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour // todo: ChargeableWeapon?
{
  [SerializeField] protected float _rotationCompesation = 90f; // 90f when sprite faces right in source
  [SerializeField] protected float _timeInSecondsForFullCharge = 2f;
  [SerializeField] protected float _initialFirePower = 5f;

  [SerializeField] protected Sprite _idleStage;
  [SerializeField] protected Sprite[] _chargeStages;
  [SerializeField] protected SpriteRenderer _renderer;
  [SerializeField] protected ParticleSystem _criticalHitParticleSystem;

  public float AddedFirePower { get; set; } // For upgrades
  public float FirePower { get => _initialFirePower + AddedFirePower;  }

  public bool IsCharging { get; private set; }

  protected float _chargeStartTime;

  protected float ChargeTimeSinceChargeStart { get => Time.time - _chargeStartTime; }

  protected Vector2 Direction { get => transform.rotation * Vector2.right; }
  protected float ChargedFirePower { get => Mathf.Clamp01(ChargeTimeSinceChargeStart / _timeInSecondsForFullCharge) * FirePower; }
  protected bool WillCrit { get => Mathf.Clamp01(ChargeTimeSinceChargeStart / _timeInSecondsForFullCharge) == 1; }

  protected virtual void Update() {
    LookAtMouse();
  }

  public virtual void Fire() {
    _renderer.sprite = _idleStage;
    IsCharging = false;

    if (WillCrit) {
      _criticalHitParticleSystem.Play();
    }
  }

  public void Charge() {
    if (!IsCharging) {
      _chargeStartTime = Time.time;
      IsCharging = true;
    }

    UpdateSpriteAccordingToCharge();
  }

  protected void UpdateSpriteAccordingToCharge() {
    var chargePercentage = Mathf.Clamp01(ChargeTimeSinceChargeStart / _timeInSecondsForFullCharge);
    var currentChargeStageIndex = Mathf.FloorToInt(chargePercentage * (_chargeStages.Length - 1));

    _renderer.sprite = _chargeStages[currentChargeStageIndex];
  }

  protected void LookAtMouse() {
    var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    var newRotation = Quaternion.LookRotation(Vector3.forward, mouseWorldPosition - transform.position);
    newRotation.eulerAngles += new Vector3(0, 0, _rotationCompesation);
    transform.rotation = newRotation;
  }
}
