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

    private void Start()
    {
        _lastMousePos = Input.mousePosition;
    }

    bool _resetMousePos = false;

    private void Update()
    {
        // If the mouse is not being held down, don't rotate
        if (Input.GetMouseButton(1))
        {
            // Rotate around player on the Y axis
            Vector3 mouseDelta = Input.mousePosition - _lastMousePos;
            transform.RotateAround(transform.parent.position, Vector3.up, mouseDelta.x * _rotationSpeed * Time.deltaTime);
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

        _lastMousePos = Input.mousePosition;
    }
}
