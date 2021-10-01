using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class CarSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] carPrefabs;
        private void Start() => Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], transform);
    }
}
