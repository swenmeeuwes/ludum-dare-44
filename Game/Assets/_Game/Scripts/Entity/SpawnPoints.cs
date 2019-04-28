using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
  [SerializeField] private Transform[] _points;
  public Transform[] Points { get => _points; }

  public Vector2 GetRandomSpawnPosition() {
    return _points[Random.Range(0, _points.Length)].position;
  }
}
