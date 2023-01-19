using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMachete : MonoBehaviour
{
    //public int chopsPerGrass = 2;

    GameObject _machete;
    GrassBehaviour _grassBehaviour;
    Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _machete = GameObject.Find("Machete");
        if (_machete == null)
            Debug.LogWarning("No MACHETE object attached");

        _animator = _machete.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _animator.SetBool("Cutting", true);

            if (_grassBehaviour != null)
                _grassBehaviour.OnCut();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
            _animator.SetBool("Cutting", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grass"))
            _grassBehaviour = other.GetComponent<GrassBehaviour>();
    }
}
