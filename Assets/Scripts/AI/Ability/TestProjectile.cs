using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestProjectile : MonoBehaviour
{
    Rigidbody _rigidbody;

    List<ContactPoint> _collisions = new List<ContactPoint>();

    void Start()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Add collision contacts to list
        _collisions.AddRange(collision.contacts);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.color = Color.magenta;

        // Draw on top of everything
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        foreach (var collision in _collisions)
        {
            Handles.DrawWireDisc(collision.point, collision.normal, 0.3f);
        }
    }
#endif
}
