using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    GameObject Player;

    void Start()
    {
        Player = transform.parent.gameObject;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
