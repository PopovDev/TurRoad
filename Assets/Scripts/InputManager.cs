using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public event Action<Ray> OnMouseClick;
    public event Action<Ray> OnMouseHover;
    public event Action OnMouseUp;

    [SerializeField] private Camera mainCamera;
    private void Update()
    {
        CheckClickDownEvent();
        CheckClickHoldEvent();
        CheckClickUpEvent();
        CheckHoverEvent();
    }
    private void CheckClickHoldEvent()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
            OnMouseHover?.Invoke(mainCamera.ScreenPointToRay(Input.mousePosition));
    }
    private void CheckHoverEvent()
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
    public void ClearEvents()
    {
        OnMouseClick = null;
        OnMouseUp = null;
        OnMouseHover = null;
    }
}
