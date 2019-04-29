using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
  [SerializeField] private Transform[] _points;
  public Transform[] Points { get => _points; }

  public Vector2 GetRandomSpawnPosition() {
    return _points[Random.Range(0, _points.Length)].position;
  }

  public List<Vector2> GetRandomSpawnPositions(int amount) {
    var randomPoints = new List<Vector2>();
    var availablePoints = Points.Select(p => (Vector2)p.position).ToList(); // Copy points array

    // Fail safe if requesting more points than available
    if (amount >= availablePoints.Count) {
      return availablePoints;
    }

    for (int i = 0; i < amount; i++) {
      var randomPoint = availablePoints[Random.Range(0, availablePoints.Count)];
      availablePoints.Remove(randomPoint);
      randomPoints.Add(randomPoint);
    }

    return randomPoints;
  }
}
