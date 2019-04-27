using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
  [SerializeField] private Transform[] _points;
  public Transform[] Points { get => _points; }
}
