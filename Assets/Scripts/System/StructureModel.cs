using System.Collections.Generic;
using AI;
using UnityEngine;

namespace System
{
    public class StructureModel : MonoBehaviour
    {
        private float _yHeight;
        public int ObjIndex { get; private set; }
        public CellType CellT { get; private set; }
        public Vector3Int Pos { get; private set; }

        public IReadOnlyList<Vector3Int> RoadPosition { get; set; }

        public void CreateModel(GameObject obj, int i = -1, CellType cellType = CellType.Road, Vector3Int? vt3 = null)
        {
            if (vt3.HasValue)
                Pos = vt3.Value;
            CellT = cellType;
            var structure = Instantiate(obj, transform);
            _yHeight = structure.transform.position.y;
        }
        public void SwapModel(GameObject model, Quaternion rotation)
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
            var structure = Instantiate(model, transform);
            structure.transform.localPosition = new Vector3(0, _yHeight, 0);
            structure.transform.localRotation = rotation;
        }


        internal List<Marker> GetCarMarkers()
        {
            return transform.GetChild(0).GetComponent<RoadHelper>().carMarkers;
        }

        public Vector3 GetNearestCarMarkerTo(Vector3 position)
        {
            return transform.GetChild(0).GetComponent<RoadHelper>().GetClosestCarMarkerPosition(position);
        }

        public Marker GetCarSpawnMarker(Vector3Int nextPathPosition)
        {
            return transform.GetChild(0).GetComponent<RoadHelper>().GetPositionForCarToSpawn(nextPathPosition);
        }

        public Marker GetCarEndMarker(Vector3Int previousPathPosition)
        {
            return transform.GetChild(0).GetComponent<RoadHelper>().GetPositionForCarToEnd(previousPathPosition);
        }


    }
}
