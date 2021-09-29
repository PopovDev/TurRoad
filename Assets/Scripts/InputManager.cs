using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public event Action<Ray> OnMouseClick;
    public event Action OnMouseUp, OnEscape, OnE, OnQ, OnR;
    public Vector3 CameraMovementVector { get; private set; } = Vector3.zero;

    [SerializeField] private Camera mainCamera;
    private void Update()
    {
        CheckClickDownEvent();
        CheckClickHoldEvent();
        CheckClickUpEvent();
        CheckArrowInput();
        CheckEscClick();
        CheckClicks();
    }
    private void CheckClicks()
    {
        if (Input.GetKeyDown(KeyCode.E)) OnE?.Invoke();
        if (Input.GetKeyDown(KeyCode.Q)) OnQ?.Invoke();
        if (Input.GetKeyDown(KeyCode.R)) OnR?.Invoke();
    }
    private void CheckClickHoldEvent()
    {
        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
            OnMouseClick?.Invoke(mainCamera.ScreenPointToRay(Input.mousePosition));
    }

    private void CheckClickUpEvent()
    {
        if (Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false) OnMouseUp?.Invoke();
    }

    private void CheckClickDownEvent()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
            OnMouseClick?.Invoke(mainCamera.ScreenPointToRay(Input.mousePosition));
    }

    private void CheckEscClick()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OnEscape?.Invoke();
    }

    private void CheckArrowInput() => CameraMovementVector = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));

    public void ClearEvents()
    {
        OnMouseClick = null;
        OnEscape = null;
        OnMouseUp = null;
        OnE = null;
    }
}
