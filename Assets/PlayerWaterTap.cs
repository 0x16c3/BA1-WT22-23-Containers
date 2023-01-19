using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBucket : MonoBehaviour
{
    [Tooltip("The object in the scene where bucket is respawning")]
    public GameObject BucketObject;

    GameObject _bucket;
    bool _inBucketCollider = false, _bucketGrabbed = false, _filled = false;


    void Start()
    {
        _bucket = transform.Find("Bucket").gameObject;
        _bucket.SetActive(false);
        Debug.Log(_bucket.activeSelf);
        if (_bucket == null)
            Debug.LogWarning("No BUCKET object attached");
    }

    void Update()
    {
        #region Grabbing Bucket
        if (_inBucketCollider && Input.GetKeyDown(KeyCode.Mouse0))
            _bucketGrabbed = true;

        if (_bucketGrabbed && Input.GetKeyDown(KeyCode.X))
            _bucketGrabbed = false;

        if (_bucketGrabbed)
        {
            BucketObject.SetActive(false);
            _bucket.SetActive(true);
            if (_inBucketCollider && Input.GetKeyDown(KeyCode.E ))
                _filled = true;
        }
        else
        {
            BucketObject.SetActive(true);
            _bucket.SetActive(false);
        }
        #endregion

        #region Extinguish Fire
        //if near fire && Input.GetKeyDown(KeyCode.Mouse0)
        //  _filled = false;
        #endregion

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bucket"))
            _inBucketCollider = true;
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
