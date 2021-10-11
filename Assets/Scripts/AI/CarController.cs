using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {
        private Rigidbody _rb;
        public Camera cam;

        [SerializeField]
        private float power;
        [SerializeField]
        private float torque;
        [SerializeField]
        private float maxSpeed;

        [SerializeField]
        private Vector2 movementVector;

        private void Awake() => _rb = GetComponent<Rigidbody>();

        public void Move(Vector2 movementInput) => movementVector = movementInput;

  
        private void FixedUpdate()
        {
            if(_rb.velocity.magnitude < maxSpeed)
            {
                _rb.AddForce(movementVector.y * transform.forward * power);
            }
            _rb.AddTorque(movementVector.x * Vector3.up * torque * movementVector.y);
        }

    }
}
