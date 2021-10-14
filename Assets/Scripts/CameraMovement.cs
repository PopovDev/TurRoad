using System;
using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    private Camera _gameCamera;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float sensitivity = 10;
    [SerializeField]private float speedUp = 2f;

    private Vector3 _inputVector;
    private float _scrollInput;
    private void Start()
    {
        _gameCamera = GetComponent<Camera>();
        Application.targetFrameRate = 60;
    }

    private void MoveCamera()
    {
        var ax = Input.GetAxisRaw("Horizontal");
        var az = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ax *= speedUp;
            az *= speedUp;
        }
        _inputVector = new Vector3(Mathf.Lerp(_inputVector.x,ax,0.3f),0, Mathf.Lerp(_inputVector.z,az,0.3f));
        var movementVector = Quaternion.Euler(0, 30, 0) * _inputVector;
        _gameCamera.transform.position += movementVector * speed* 0.028f;
    }

    private void Update()
    {
        var ay =  Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        _scrollInput = Mathf.Lerp(_scrollInput, ay, 0.05f);
        _gameCamera.orthographicSize = Mathf.Clamp(_gameCamera.orthographicSize - _scrollInput, minSize, maxSize);
        MoveCamera();
    }
}