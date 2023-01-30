using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventPins : MonoBehaviour
{
    public GameObject EventControllerScript;
    float _progress;
    Vector2 _startPosition;
    Vector2 _endPosition;

    void Start()
    {
        _startPosition = ((RectTransform)transform).anchoredPosition;
        _endPosition = new Vector2(_startPosition.x + 450, transform.position.y);
    }
    void Update()
    {
        _progress = EventController.EventTimeStatic / EventController.RoundTimeStatic;
        ((RectTransform)transform).anchoredPosition = Vector2.Lerp(_startPosition, _endPosition, _progress);
    }
}
