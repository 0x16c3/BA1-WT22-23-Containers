using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProgressBar : MonoBehaviour
{
    float _progress;
    float _length;
    Vector2 _startPosition;
    Vector2 _endPosition;

    void Start()
    {
        _length = EventController.RoundTimeStatic;
        _startPosition = ((RectTransform)transform).anchoredPosition;
        _endPosition = new Vector2(_startPosition.x + 450, transform.position.y);
    }

    void Update()
    {
        _progress += Time.deltaTime / _length;
        ((RectTransform)transform).anchoredPosition = Vector2.Lerp(_startPosition, _endPosition, _progress);
    }
}