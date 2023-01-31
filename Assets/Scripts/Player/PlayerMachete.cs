using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMachete : MonoBehaviour
{
    //public int chopsPerGrass = 2;

    Transform _playerModel;
    Animator _playerAnimator;
    PlayerLocomotion _player;


    bool _audioPLaying;
    // Start is called before the first frame update
    void Start()
    {
        _playerModel = transform.Find("Jeffrey");
        _playerAnimator = _playerModel.GetComponent<Animator>();

        if (_playerModel == null || _playerAnimator == null)
        {
            Debug.LogError("No player model found or player animator");
        }

        _player = GetComponent<PlayerLocomotion>();
        if (_player == null)
        {
            Debug.LogError("PlayerLocomotion not found on object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !PlayerBucket.HoldingBucket && !PlayerBucket.WithinBucket && !PlayerNailing.NearChest && PlayerNailing.HasMaterials <= 0)
        {
            _playerAnimator.SetBool("IsAttacking", true);
            if (AudioController.instance.IsAudioPlaying("Strike") == false)
            {
                AudioController.instance.PlayAudio("Strike");
            }

            // Get mouse position
            if (_player != null && _player.MouseHover != null)
            {
                // If doesn't have AI wander
                var aiBehavior = _player.MouseHover.GetComponent<AIBehavior>();
                if (aiBehavior == null)
                    return;

                // Return if too far away
                if (Vector3.Distance(transform.position, _player.transform.position) > 2f)
                    return;

                // Apply damage to the container
                aiBehavior.Damage(1);

                var rb = _player.MouseHover.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // If already moving up or down, don't add force
                    if (rb.velocity.y > 0.1f || rb.velocity.y < -0.1f)
                        return;

                    rb.AddForce(transform.up * 2f, ForceMode.Impulse);
                }
            }
        }
    }
}
