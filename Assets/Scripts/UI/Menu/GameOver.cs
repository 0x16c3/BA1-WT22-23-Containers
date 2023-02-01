using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    TMPro.TextMeshProUGUI textMeshPro;
    private void Start()
    {
        textMeshPro = GetComponent<TMPro.TextMeshProUGUI>();
        textMeshPro.text = Convert.ToString(ScoreSystem.TotalScore);
    }
}
