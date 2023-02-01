using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerCameraControl : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 10f;

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    private Vector3 _lastMousePos;

    PlayerCameraShake _shake;
    Vector3 _originalPosition;
    Quaternion _originalRotation;

    GameObject _clone;

    private void Start()
    {
        _lastMousePos = Input.mousePosition;
        _shake = new PlayerCameraShake();
        _clone = transform.parent.Find("Camera Rotation").gameObject;
        _originalPosition = transform.localPosition;
        _originalRotation = transform.localRotation;
    }

    bool _resetMousePos = false;

    private void Update()
    {
        // If the mouse is not being held down, don't rotate
        if (Input.GetMouseButton(1))
        {
            // Rotate around player on the Y axis
            Vector3 mouseDelta = Input.mousePosition - _lastMousePos;
            _clone.transform.RotateAround(transform.parent.position, Vector3.up, mouseDelta.x * _rotationSpeed * Time.deltaTime);

            // Update original rotation
            _originalPosition = _clone.transform.localPosition;
            _originalRotation = _clone.transform.localRotation;

            _resetMousePos = true;
        }
        else if (_resetMousePos)
        {
            Vector2Int mousePos = new Vector2Int(0, 0);

#if UNITY_EDITOR
            var editorWindowScreenPos = UnityEditor.EditorWindow.focusedWindow.position.position;
            mousePos.x = (int)(editorWindowScreenPos.x + Screen.width / 2);
            mousePos.y = (int)(editorWindowScreenPos.y + Screen.height / 2);
#else
            mousePos.x = Screen.width / 2;
            mousePos.y = Screen.height / 2;
#endif

            SetCursorPos(mousePos.x, mousePos.y);
            _resetMousePos = false;
        }

        var shake = _shake.GetShake();
        transform.localPosition = _originalPosition + shake;
        transform.localRotation = _originalRotation * Quaternion.Euler(shake * 2f);

        _lastMousePos = Input.mousePosition;
    }
}
