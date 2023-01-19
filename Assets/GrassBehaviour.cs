using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBehaviour : MonoBehaviour
{
    public int NumberOfStages = 4;
    public float GrowTime = 5f;
    [HideInInspector]
    public int CurrCut;

    List<Transform> _modelsList = new List<Transform>();
    float _timePassed;
    int _currIteration = 0;
    bool _cutting = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform _transform in transform)
        {
            //To use instead of assebling prefab children manually
            /*float _spawnPositionY = -0.01f;   
            _transform.position = new Vector3(transform.parent.position.x, _spawnPositionY, transform.parent.position.z);
            */
            _modelsList.Add(_transform);
            _transform.gameObject.SetActive(false);
        }

        if (_modelsList.Count > NumberOfStages)
            Debug.LogWarning("In grass object more children than declared");
    }

    // Update is called once per frame
    void Update()
    {
        _timePassed += Time.deltaTime;

        if (_timePassed >= GrowTime && _currIteration < NumberOfStages && !_cutting)
        {
            _modelsList[_currIteration].gameObject.SetActive(true);
            if (_currIteration != 0)
                _modelsList[_currIteration - 1].gameObject.SetActive(false);
            _currIteration++;
            _timePassed = 0;
        }
        else if (_currIteration <= 0 && _cutting)
            Destroy(gameObject);
    }

    public void OnCut()
    {
        _currIteration--;
        _cutting = true;
    }

    public void OnStopCut()
    {
        _cutting = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            OnStopCut();
    }

    private void OnGUI()
    {
        GUI.skin.GetStyle("label").fontSize = 20;
        GUILayout.Label("Grass height: " + _currIteration);

        string _GUItext;
        if (_cutting)
            _GUItext = "Cutting grass";
        else
            _GUItext = "";
        GUILayout.Label(_GUItext);
    }
}
