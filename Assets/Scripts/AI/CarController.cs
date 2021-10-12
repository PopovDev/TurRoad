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
        internal bool Stop = false;
        [SerializeField]
        private float power;
        [SerializeField]
        private float torque;
        [SerializeField] private float maxSpeed;
        [SerializeField] public float speedScale =1;

        [SerializeField]
        private Vector2 movementVector;

        private void Awake() => _rb = GetComponent<Rigidbody>();

        public void Move(Vector2 movementInput) => movementVector = movementInput;

  
        private void FixedUpdate()
        {
            if(Stop) return;
            if(_rb.velocity.magnitude < maxSpeed*speedScale)
            {
                _rb.AddForce(movementVector.y * transform.forward * power*speedScale);
            }
            _rb.AddTorque(movementVector.x * Vector3.up * torque * movementVector.y*speedScale);
        }

    }
}
