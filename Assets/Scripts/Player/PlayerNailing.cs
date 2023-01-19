using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNailing : MonoBehaviour
{
    [Tooltip("UI for displaying the amount of nails")]
    public GameObject MaterialsUI;
    [Tooltip("Amount of nails grabbed by one press")]
    public int NailsPerOnce;
    [Tooltip("Prefab to spawn after dropping nails box")]
    public GameObject _nailsBoxPrefab;
    [HideInInspector]
    public int AmountRepaired = 0;

    GameObject _nailsBox;
    PlayerLocomotion _playerLocomotion;
    int _nailsAmount = 0;
    bool _nearBarrel = false, _repairing = false;
    float _timePassed = 0, _repairTime = 2f;

    private void Start()
    {
        _playerLocomotion = gameObject.GetComponent<PlayerLocomotion>();

        if (_playerLocomotion == null)
            Debug.LogWarning("No PlayerLocomotion component attached");

        _nailsBox = transform.Find("NailsBox").gameObject;
        if (_nailsBox == null)
            Debug.LogWarning("No NAILS BOX object attached");

        if (NailsPerOnce == 0)
            NailsPerOnce = 10;

        _nailsBox.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _nearBarrel)
        {
            _nailsAmount += NailsPerOnce;
            _nailsBox.SetActive(true);
        }

        if (_nailsAmount != 0)
        {
            _playerLocomotion.MovementSpeed = 6f;

            if (_nailsAmount >= NailsPerOnce)
                _nailsAmount = NailsPerOnce;

        }
        else if (_nailsAmount <= 0)
        {
            _nailsBox.SetActive(false);
            _playerLocomotion.MovementSpeed = _playerLocomotion.InitialSpeed;
        }
        else if (_nailsAmount < 0)
            _nailsAmount = 0;

        if(Input.GetKeyDown(KeyCode.X) && _nailsAmount > 0)
        {
            _nailsBox.SetActive(false);
            _nailsAmount = 0;
            GameObject _instantiatedObj = Instantiate(_nailsBoxPrefab);
            _instantiatedObj.transform.localPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        }

        if (Input.GetKey(KeyCode.Mouse1) && _nailsAmount > 0)
        {
            _timePassed += Time.deltaTime;
            _repairing = true;

            if (_timePassed >= _repairTime)
            {
                _repairTime++;
                AmountRepaired++;
                _nailsAmount--;
                _timePassed = 0f;
            }
            //TileRepaired(AmountRepaired);
        }
        else
        {
            _repairTime = 2f;
            AmountRepaired = 0;
            _repairing = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chest"))
        {
            //display hint "Press X to pick up"
            _nearBarrel = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chest"))
            _nearBarrel = false;
    }

    private void OnGUI()
    {
        /*
        GUI.skin.GetStyle("label").fontSize = 20;
        GUILayout.Label("Nails: " + _nailsAmount);
        
        string _GUItext;
        if (_repairing)
            _GUItext = "Seconds repair: " + _repairTime + "| Reparing..." + AmountRepaired;
        else
            _GUItext = "";
        GUILayout.Label(_GUItext);
        */
    }
}
