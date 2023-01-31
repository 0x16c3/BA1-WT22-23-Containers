using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventPins : MonoBehaviour
{
    public GameObject EventSymbol;
    public GameObject Canvas;
    float _progress;
    Vector2 _startPosition;
    Vector2 _endPosition;

    void Start()
    {
        _startPosition = ((RectTransform)transform).anchoredPosition;
    }

    private void OnEnable()
    {
        foreach (float time in EventController.EventTimeStatic)
        {
            float position = time / EventController.RoundTimeStatic;
            Debug.Log("time is " + time);
            if (position >= 1)
            {
                return;
            }
            GameObject newSymbol = Instantiate(EventSymbol, Canvas.transform);
            RectTransform symbolPos = newSymbol.GetComponent<RectTransform>();
            float assignedPosition = Mathf.Lerp(_startPosition.x, _startPosition.x + 450, position);
            symbolPos.localPosition = new Vector2(assignedPosition ,0);
        }
    }
}
