using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRiggedHandsHide : MonoBehaviour
{
    MeshRenderer mesh;
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        if (AnimationHandsHide._areFakeHandsVisible == true)
        {
            mesh.enabled = false;
        }
        else
        {
            mesh.enabled = true;
        }
    }
}
