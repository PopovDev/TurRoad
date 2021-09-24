using UnityEngine;

namespace SVS
{

    public class CameraMovement : MonoBehaviour
    {
        private Camera _gameCamera;
        public float cameraMovementSpeed = 5f;
        public float maxOrthographicSize = 5f, minOrthographicSize = 0.5f;
        public float sensitivity = 10;
        private void Start() => _gameCamera = GetComponent<Camera>();

        public void MoveCamera(Vector3 inputVector)
        {
            var movementVector = Quaternion.Euler(0,30,0) * inputVector;
            _gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
        }

        private void Update()
        {
            var scrollInput = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            _gameCamera.orthographicSize = Mathf.Clamp(_gameCamera.orthographicSize - scrollInput, minOrthographicSize, maxOrthographicSize);
        }
    }
}