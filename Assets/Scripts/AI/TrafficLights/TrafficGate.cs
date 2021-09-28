using UnityEngine;
public class TrafficGate : MonoBehaviour
{
    public bool canRun;
    private SmartRoad _road;
    private void Start() => _road = GetComponentInParent<SmartRoad>();
    private void OnTriggerExit(Collider other)
    {
        if(_road.exited.Contains(other.gameObject)) return;
        _road.exited.Add(other.gameObject);
        _road.exited.RemoveAll(x => x == null);
    }
    private void OnTriggerStay(Collider other)
    {
        if(_road.exited.Contains(other.gameObject)) return;
        var ai = other.GetComponent<CarAI>();
        if (ai == null) return;
        ai.Stop = !canRun;
    }

}
