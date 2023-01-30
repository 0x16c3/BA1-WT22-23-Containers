using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GD.MinMaxSlider;

public class WhirlwindController : MonoBehaviour, IEvent
{

    [Range(0, 120)]
    public float EventDuration = 45;
    public int EventEffectDuration = 20;

    GameObject _grabbableParent;
    GameObject _tileGrid;
    float _timePassed, _initialAmbientIntensity, _initialWaveStrength, _initialWaveSpeed, _initialWaveHeight;
    Material _darkOceanMat, _lightOceanMat;
    bool _eventTriggered = false;
    ParticleSystem _rainPS;

    int _rn;
    Vector3 _rv;

    [HideInInspector]
    EventData _data;
    public EventData Data
    {
        get => _data;
        set => _data = value;
    }

    [MinMaxSlider(0, 180)]
    public Vector2Int _duration = new Vector2Int(45, 90);
    public Vector2Int Duration
    {
        get => _duration;
        set => _duration = value;
    }


    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        _tileGrid = GameObject.Find("Tilemap");
        _grabbableParent = GameObject.Find("Grabbables");
        if (_grabbableParent == null)
            Debug.LogError("Grabbable not asigned");

        _rn = Random.Range(0, 2); //random Wave strength
        _rv = new Vector3(Random.Range(0, 90), Random.Range(0, 10), Random.Range(0, 90)); //random wave direction

        _rainPS = GameObject.Find("Rain").GetComponent<ParticleSystem>();
        _rainPS.Play();

        //EventDuration = _data.Duration;
        EventDuration = 25;

        _initialAmbientIntensity = RenderSettings.ambientIntensity;

        _darkOceanMat = GameObject.Find("OceanPlate02").GetComponent<MeshRenderer>().materials[0];
        _lightOceanMat = GameObject.Find("OceanPlate03").GetComponent<MeshRenderer>().materials[0];
        _initialWaveStrength = _darkOceanMat.GetFloat("_Wave_Strength");
        _initialWaveHeight = _darkOceanMat.GetFloat("_Wave_Height");
        _initialWaveSpeed = _darkOceanMat.GetFloat("_Wave_Speed");
    }

    /// <param name="_whirlwindPower">0 = small wave, 1 = medium wave, 2 = big wave</param>
    public void StartWhirlwind(int _whirlwindPower, Vector3 _swayDirection, float _eventDuration = 60, int _eventEffectDuration = 10)
    {
        Vector3 _rv; //random wave direction

        switch (_whirlwindPower)
        {

            case 0:
                _rv = new Vector3(RandomExclusiveNumber(10, 5), Random.Range(1, 2), RandomExclusiveNumber(10, 5));
                StartEvent(1, 7, 2, _rv);
                break;
            case 1:
                _rv = new Vector3(RandomExclusiveNumber(15, 10), Random.Range(2, 3), RandomExclusiveNumber(15, 10));
                StartEvent(2, 4, 5, _rv);
                break;
            case 2:
                _rv = new Vector3(RandomExclusiveNumber(20, 10), Random.Range(3, 5), RandomExclusiveNumber(20, 10));
                StartEvent(3, 1, 7, _rv);
                break;
        }
        _eventTriggered = true;
    }

    // The bounds should be always supperior of the exclusion value
    float RandomExclusiveNumber(float _bounds, float _exclusion)
    {
        bool _toInvert = Random.Range(0, 2) != 0;

        if (_exclusion > _bounds)
            return 0;

        float _randomFloat = Random.Range(_exclusion, _bounds);

        if (_toInvert)
            return -_randomFloat;
        else
            return _randomFloat;

    }


    void StartEvent(int _numOfDamagedTiles, int _tileDamagedAmount, int _numOfThrownContainers, Vector3 _swayDirection)
    {
        DamagingTiles(_numOfDamagedTiles, _tileDamagedAmount);
        ThrowingContainers(_numOfThrownContainers, _swayDirection);
    }

    void DamagingTiles(int _numOfDamagedTiles, int _tileDamagedAmount)
    {
        int[] _randomNumbers = new int[_numOfDamagedTiles];

        for (int i = 0; i < _numOfDamagedTiles; i++)
        {
            int _rn = 0; // It's the current id for randomly damage a tile
            foreach (int _number in _randomNumbers)
            {
                _rn = RandomUniqueNumber(0, _tileGrid.transform.childCount - 1, _number);
            }

            _randomNumbers[i] = _rn;

            Transform _tile = _tileGrid.transform.GetChild(_rn);
            _tile.GetComponent<TileDamageable>().Damage(_tileDamagedAmount);
        }
    }

    void ThrowingContainers(int _numOfThrownContainers, Vector3 _swayDirection)
    {
        int[] _randomNumbers = new int[_numOfThrownContainers];

        for (int i = 0; i < _numOfThrownContainers; i++)
        {
            int _rn = 0; // It's the current id for randomly damage a tile
            foreach (int _number in _randomNumbers)
            {
                _rn = RandomUniqueNumber(0, _grabbableParent.transform.childCount - 1, _number);
            }

            _randomNumbers[i] = _rn;

            Transform _container = _grabbableParent.transform.GetChild(_rn);
            Vector3 _forceVec = _swayDirection /** 0.1f*/;


            _container.GetComponent<Rigidbody>().AddForce(_forceVec, ForceMode.Impulse);
        }
    }

    // Creating a unique number for avoid damaging multiple times the same grid
    int RandomUniqueNumber(int _min, int _max, int _previousNumber)
    {
        int _rn = Random.Range(_min, _max);
        if (_rn == _previousNumber)
            return RandomUniqueNumber(_min, _max, _previousNumber);
        else
            return _rn;
    }

    // Update is called once per frame
    void Update()
    {
        _timePassed += Time.deltaTime;
        float percentageComplete = _timePassed / EventEffectDuration;
        _darkOceanMat.SetFloat("_Wave_Height", Mathf.Lerp(_initialWaveHeight, 3.5f, percentageComplete));
        //_darkOceanMat.SetFloat("_Wave_Strength", Mathf.Lerp(_initialWaveStrength, 4f, percentageComplete));
        _darkOceanMat.SetFloat("_Wave_Speed", Mathf.Lerp(_initialWaveSpeed, 0.4f, percentageComplete));
        RenderSettings.ambientIntensity = Mathf.Lerp(_initialAmbientIntensity, 0, percentageComplete);

        if (percentageComplete >= 1 && _eventTriggered == false)
            StartWhirlwind(_rn, _rv, EventDuration);


        if (_timePassed >= EventDuration)
        {
            percentageComplete = (_timePassed - EventDuration) / EventEffectDuration;
            _darkOceanMat.SetFloat("_Wave_Height", Mathf.Lerp(3.5f, _initialWaveHeight, percentageComplete));
            //_darkOceanMat.SetFloat("_Wave_Strength", Mathf.Lerp(4f, _initialWaveStrength, percentageComplete));
            _darkOceanMat.SetFloat("_Wave_Speed", Mathf.Lerp(0.4f, _initialWaveSpeed, percentageComplete));
            RenderSettings.ambientIntensity = Mathf.Lerp(0, _initialAmbientIntensity, percentageComplete);
            //Debug.Log(Mathf.Lerp(0.4f, _initialWaveSpeed, percentageComplete));
            _rainPS.Stop();
        }
        else
        {
            //Debug.Log(_timePassed + " " + EventDuration);
        }
    }
}
