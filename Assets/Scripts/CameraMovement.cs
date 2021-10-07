using System;
using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    private Camera _gameCamera;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float sensitivity = 10;
    [SerializeField] private bool camUp = true;
    [SerializeField]private float speedUp = 2f;
    public event Action<bool> CamModeChanged;
    private void Start() => _gameCamera = GetComponent<Camera>();
    public void MoveCamera(float sceneSpeed)
    {
        var inputVector = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        
        var movementVector = Quaternion.Euler(0, 30, 0) * inputVector;
        if (Input.GetKey(KeyCode.LeftShift)) movementVector *= speedUp;
        _gameCamera.transform.position += movementVector * speed * Time.deltaTime / sceneSpeed;
    }

    private void Update()
    {
        CamModeChanged?.Invoke(camUp);
        var scrollInput = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        _gameCamera.orthographicSize = Mathf.Clamp(_gameCamera.orthographicSize - scrollInput, minSize, maxSize);
    }
}