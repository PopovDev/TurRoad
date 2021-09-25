using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleCity.AI
{
    public class RoadHelper : MonoBehaviour
    {
        [SerializeField]
        public List<Marker> carMarkers;
        [SerializeField]
        protected bool isCorner;
        [SerializeField]
        protected bool hasCrosswalks;

        private const float ApproximateThresholdCorner = 0.3f;

        [SerializeField]
        private Marker incomming, outgoing;
        

        public virtual Marker GetPositionForCarToSpawn(Vector3 nextPathPosition)
        {
            return outgoing;
        }

        public virtual Marker GetPositionForCarToEnd(Vector3 previousPathPosition) => incomming;

        protected static Marker GetClosestMarkerTo(Vector3 structurePosition, List<Marker> pedestrianMarkers)
        {
          
            Marker closestMarker = null;
            var distance = float.MaxValue;
            foreach (var marker in pedestrianMarkers)
            {
                var markerDistance = Vector3.Distance(structurePosition, marker.Position);
                if (!(distance > markerDistance)) continue;
                distance = markerDistance;
                closestMarker = marker;
            }
            return closestMarker;
        }
        public Vector3 GetClosestCarMarkerPosition(Vector3 p) => GetClosestMarkerTo(p, carMarkers).Position;
    }
}

