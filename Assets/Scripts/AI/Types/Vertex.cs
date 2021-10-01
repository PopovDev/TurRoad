using System;
using UnityEngine;

namespace AI.Types
{
    public class Vertex : IEquatable<Vertex>
    {
        public Vector3 Position { get; }
        public Vertex(Vector3 position) => Position = position;
        public bool Equals(Vertex other) => Vector3.SqrMagnitude(Position - other!.Position) < 0.0001f;
        public override string ToString() => Position.ToString();
    }
}

