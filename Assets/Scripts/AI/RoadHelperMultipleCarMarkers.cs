using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class RoadHelperMultipleCarMarkers : RoadHelper
    {
        [SerializeField]
        protected List<Marker> incommingMarkers, outgoingMarkers;

        public override Marker GetPositionForCarToSpawn(Vector3 nextPathPosition)
        {
            return GetClosestMarkerTo(nextPathPosition, outgoingMarkers);
        }

        public override Marker GetPositionForCarToEnd(Vector3 previousPathPosition)
        {
            return GetClosestMarkerTo(previousPathPosition, incommingMarkers);
        }
    }
}
