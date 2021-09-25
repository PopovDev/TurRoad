using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleCity.AI
{
    public class RoadHelperStraight : RoadHelper
    {
        [SerializeField]
        private Marker leftLaneMarker90, rightLaneMarker90;

        public override Marker GetPositionForCarToSpawn(Vector3 nextPathPosition)
        {
            var transform1 = transform;
            var angle = (int)transform1.rotation.eulerAngles.y;
            var direction = nextPathPosition - transform1.position;
            return GetCorrectMarker(angle, direction);
        }


        public override Marker GetPositionForCarToEnd(Vector3 previousPathPosition)
        {
            var transform1 = transform;
            var angle = (int)transform1.rotation.eulerAngles.y;
            var direction = transform1.position - previousPathPosition;
            return GetCorrectMarker(angle, direction);
        }


        private Marker GetCorrectMarker(int angle, Vector3 directionVector)
        {
            var direction = GetDirection(directionVector);
            return angle switch
            {
                0 when direction == Direction.Left => rightLaneMarker90,
                0 => leftLaneMarker90,
                90 when direction == Direction.Up => rightLaneMarker90,
                90 => leftLaneMarker90,
                270 when direction == Direction.Left => leftLaneMarker90,
                270 => rightLaneMarker90,
                _ => direction == Direction.Up ? leftLaneMarker90 : rightLaneMarker90
            };
        }

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private static Direction GetDirection(Vector3 direction)
        {
            if (Mathf.Abs(direction.z) > .5f)
                return direction.z > 0.5f ? Direction.Up : Direction.Down;
            return direction.x > 0.5f ? Direction.Right : Direction.Left;
        }
    }
}

