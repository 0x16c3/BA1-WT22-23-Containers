using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandsHide : MonoBehaviour
{
    public static bool _areFakeHandsVisible;
    PlayerGrab _grab;
    List<Transform> _handsTransform;
    Transform _parentTransform;
    void Start()
    {
        _handsTransform = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            _handsTransform.Add(transform.GetChild(i));
        }

        _grab = GetComponentInParent<PlayerGrab>();
        _parentTransform = transform.parent;
        if (_grab == null) { Debug.LogWarning("Grab Script not found in player"); }
        
    }
    private void FixedUpdate()
    {
        if (_grab.GrabbedObject != null)
        {
            transform.SetParent(_grab.GrabbedObject.transform);
            transform.position = Vector3.MoveTowards(transform.position, _grab.GrabbedObject.transform.position, 2 * Time.fixedDeltaTime);
            transform.rotation = _grab.GrabbedObject.transform.rotation;
            _areFakeHandsVisible = true;
            foreach (Transform t in _handsTransform)
            {
                MeshRenderer mesh = t.GetComponent<MeshRenderer>();
                mesh.enabled = true;
            }

        }

        else
        {
            transform.SetParent(_parentTransform);
            Vector3 idlePosition = _parentTransform.position;
            idlePosition += Vector3.up;
            transform.position = Vector3.MoveTowards(transform.position, idlePosition, 2 * Time.fixedDeltaTime);
            if (transform.position == idlePosition)
            {
                _areFakeHandsVisible = false;
                foreach (Transform t in _handsTransform)
                {
                    MeshRenderer mesh = t.GetComponent<MeshRenderer>();
                    mesh.enabled = false;
                }

            }

        }

    }
}
