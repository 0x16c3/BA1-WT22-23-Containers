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
    [Tooltip("For slowing Player down")]
    public PlayerLocomotion PlayerLocomotion;
    [Tooltip("Object to be shown while holding nails")]
    public GameObject nailsBox;
    [Tooltip("Prefab to spawn after dropping nails box")]
    public GameObject nailsBoxPrefab;
    [HideInInspector]
    public int AmountRepaired = 0;

    int _nailsAmount = 0;
    bool _nearBarrel = false, _repairing = false;
    float _timePassed = 0, _repairTime = 2f;

    private void Start()
    {
        if (PlayerLocomotion == null)
            PlayerLocomotion = gameObject.GetComponent<PlayerLocomotion>();

        if (nailsBox == null)
            nailsBox = GameObject.Find("NailsBox");

        nailsBox.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _nearBarrel)
        {
            _nailsAmount += NailsPerOnce;
            nailsBox.SetActive(true);
        }

        if (_nailsAmount != 0)
        {
            PlayerLocomotion.MovementSpeed = 6f;

            if (_nailsAmount >= NailsPerOnce)
                _nailsAmount = NailsPerOnce;

        }
        else if (_nailsAmount <= 0)
        {
            nailsBox.SetActive(false);
            PlayerLocomotion.MovementSpeed = PlayerLocomotion.InitialSpeed;
        }
        else if (_nailsAmount < 0)
            _nailsAmount = 0;

        if(Input.GetKeyDown(KeyCode.X))
        {
            nailsBox.SetActive(false);
            _nailsAmount = 0;
            GameObject _instantiatedObj = Instantiate(nailsBoxPrefab);
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
        if (other.CompareTag("Barrel"))
        {
            //display hint "Press X to pick up"
            _nearBarrel = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Barrel"))
            _nearBarrel = false;
    }

    private void OnGUI()
    {
        GUI.skin.GetStyle("label").fontSize = 20;
        GUILayout.Label("Nails: " + _nailsAmount);
        
        string _GUItext;
        if (_repairing)
            _GUItext = "Seconds repair: " + _repairTime + "| Reparing..." + AmountRepaired;
        else
            _GUItext = "";
        GUILayout.Label(_GUItext);
    }
}
