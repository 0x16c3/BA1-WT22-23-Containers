using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIContainerStatus : MonoBehaviour
{
    int _containerAmount;
    TMPro.TextMeshProUGUI _textMeshPro;

    private void Start()
    {
       Transform number = transform.GetChild(0);
       _textMeshPro = number.GetComponent<TMPro.TextMeshProUGUI>();
    }
    void FixedUpdate()
    {
        var targets = GameObject.FindGameObjectsWithTag("Grabbable").Where(x => x != gameObject && x.GetComponent<AIBehavior>() == null);
        _containerAmount = targets.Count();
        _textMeshPro.text = _containerAmount.ToString();
    }
}
