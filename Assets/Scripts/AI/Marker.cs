using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleCity.AI
{
    public class Marker : MonoBehaviour
    {
        public Vector3 Position => transform.position;

        [SerializeField] 
        public List<Marker> adjacentMarkers;

        [SerializeField]
        private bool openForConnections;

        public bool OpenForConnection => openForConnections;
        

        private void OnDrawGizmos()
        {
            //if (Selection.activeObject != gameObject) return;
            Gizmos.color = Color.red;
            if (adjacentMarkers.Count > 0)
                foreach (var item in adjacentMarkers)
                    Gizmos.DrawLine(transform.position, item.Position);
            Gizmos.color = Color.white;
        }
    }

}
