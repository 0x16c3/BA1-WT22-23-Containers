using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMachete : MonoBehaviour
{
    //public int chopsPerGrass = 2;

    Transform _playerModel;
    GrassBehaviour _grassBehaviour;
    Animator _playerAnimator;

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
            if (_grassBehaviour != null)
                _grassBehaviour.OnCut();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grass"))
            _grassBehaviour = other.GetComponent<GrassBehaviour>();
    }
}
