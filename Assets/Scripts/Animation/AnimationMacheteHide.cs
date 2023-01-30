using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMacheteHide : MonoBehaviour
{
    public bool IsBeltMachete;
    Animator _parentAnimator;
    MeshRenderer _renderer;
    void Start()
    {
        _renderer= GetComponent<MeshRenderer>();
        _parentAnimator = GetComponentInParent<Animator>();
        if (_parentAnimator == null) { Debug.LogWarning("Machete not attached to parent with animator component."); }
    }
    private void FixedUpdate()
    {
        switch (IsBeltMachete)
        {
            case true:
                _renderer.enabled = !_parentAnimator.GetBool("IsAttacking");
                break;

            case false:
                _renderer.enabled = _parentAnimator.GetBool("IsAttacking");
                break;

        }
    }
}
