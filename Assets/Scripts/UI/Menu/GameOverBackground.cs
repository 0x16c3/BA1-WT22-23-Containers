using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBackground : MonoBehaviour
{
    RawImage _image;
    public Texture GameOverImage; 
    private void Start()
    {
        _image= GetComponent<RawImage>();
        if (ScoreSystem.DidShipDie == true)
        {
            _image.texture = GameOverImage;
        }
    }
}
