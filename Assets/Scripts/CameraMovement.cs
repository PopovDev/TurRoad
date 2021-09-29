using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SVS
{
    public class CameraMovement : MonoBehaviour
    {
        private Camera _gameCamera;
        [SerializeField]
        private float speed = 5f,maxSize = 5f,minSize = 0.5f,sensitivity = 10;
        [SerializeField]
        private bool camUp = true;
        public event Action<bool> CamModeChanged;
        private void Start() => _gameCamera = GetComponent<Camera>();

        public void MoveCamera(Vector3 inputVector)
        {
            var movementVector = Quaternion.Euler(0,30,0) * inputVector;
            _gameCamera.transform.position += movementVector * Time.deltaTime * speed;
        }

        private void Update()
        {
            CamModeChanged?.Invoke(camUp);
            var scrollInput = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            _gameCamera.orthographicSize = Mathf.Clamp(_gameCamera.orthographicSize - scrollInput, minSize, maxSize);
        }
    }
}