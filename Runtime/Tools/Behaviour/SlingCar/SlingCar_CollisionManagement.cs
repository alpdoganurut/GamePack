using System.Collections.Generic;
using UnityEngine;

namespace GamePack
{
    public partial class SlingCar
    {
        private static readonly List<Collision> HandledCollisions = new List<Collision>();

        private void CollisionHandled(Collision collision)
        {
            HandledCollisions.Add(collision);
        }

        private bool IsCollisionHandled(Collision collision)
        {
            return HandledCollisions.Contains(collision);
        }

        private void ClearHandledCollisions()
        {
            HandledCollisions.Clear();
        }
    }
}