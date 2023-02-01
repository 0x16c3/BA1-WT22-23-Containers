using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRamp : MonoBehaviour
{
    Rigidbody _rb;
    Collider _collider;

    PlayerLocomotion _locomotion;
    Rigidbody _playerRb;
    Collider _playerCollider;

    float Padding = 0.1f;

    Vector3 _origin, _destination, _offset, _delta, _target, _bottom, _top, _position;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _locomotion = FindObjectOfType<PlayerLocomotion>();
        _playerRb = _locomotion.GetComponent<Rigidbody>();
        _playerCollider = _locomotion.GetComponent<Collider>();

        // Get the highest-right point of the ramp
        _top = _collider.ClosestPoint(_collider.bounds.min + Vector3.forward * _collider.bounds.size.z + Vector3.up * 1000f);
        _top.y -= 0.05f;

        // Get the lowest point of the ramp that touches the ground plane
        var groundCollider = GameObject.FindGameObjectWithTag("Ground Plane").GetComponent<Collider>();
        _bottom = groundCollider.ClosestPoint(_collider.bounds.max - Vector3.forward * _collider.bounds.size.z + Vector3.up * 1000f);
        _bottom.y += 0.05f;
    }

    void FixedUpdate()
    {
        _position = _playerCollider.ClosestPoint(_playerRb.position + Vector3.down * _playerCollider.bounds.extents.y);
        _locomotion.OnSecondFloor = _position.y >= _top.y - Padding && !_locomotion.OnRamp;

        // If player z position is not between _bottom and _top, return
        if (_playerRb.position.z < _bottom.z || _playerRb.position.z > _top.z)
        {
            _locomotion.OnRamp = false;
            return;
        }

        // If on the second floor and close to the top edge line, immediately stick to the ramp
        if (_locomotion.OnSecondFloor && _position.x >= _top.x)
        {
            _locomotion.OnRamp = true;

            _destination = _collider.ClosestPoint(_position);
            _origin = _playerCollider.ClosestPoint(_destination);

            _delta = _destination - _origin;
            _offset = _origin - _playerRb.position;

            if (_destination.x <= _top.x && _locomotion.InputVector.x < 0)
            {
                _offset += Vector3.right * 0.1f;
            }

            if (_origin.x > _bottom.x - Padding && _origin.y <= _bottom.y + Padding)
            {
                _playerRb.MovePosition(_playerRb.position + _delta);
            }
            return;
        }

        if (!_locomotion.OnRamp)
            return;

        // If player position is left of the ramp top, return
        if (_position.x < _top.x)
            return;

        // Add upwards destination if the player is trying to move left
        // And check the player position so it doesnt keep bobbing if standing
        if (_locomotion.InputVector.x == 0 && _origin != Vector3.zero)
        {
            _position = _origin;
        }
        else if (_locomotion.InputVector.x < 0)
        {
            _position.y += 0.5f;
            _position.x -= Padding;
        }
        else if (_locomotion.InputVector.x > 0 && _position.y < _top.y)
        {
            _position.y += 0.2f;
        }

        _destination = _collider.ClosestPoint(_position);
        _origin = _playerCollider.ClosestPoint(_destination);

        _delta = _destination - _origin;
        _offset = _origin - _playerRb.position;

        if (_destination.x <= _top.x && _locomotion.InputVector.x < 0)
        {
            _offset += Vector3.right * 0.1f;
        }

        if (_origin.x > _bottom.x - Padding && _origin.y <= _bottom.y + Padding)
        {
            return;
        }
        else if (_origin.x > _top.x - Padding && _origin.y >= _top.y && _locomotion.InputVector.x > 0)
        {
            return;
        }

        _target = _destination - _offset;

        _playerRb.MovePosition(_target);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.rigidbody == _playerRb)
            _locomotion.OnRamp = true;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.rigidbody == _playerRb)
            _locomotion.OnRamp = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_origin, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_destination, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_origin, _delta);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_target, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_bottom, Vector3.one * 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(_top, Vector3.one * 0.1f);
    }
}
