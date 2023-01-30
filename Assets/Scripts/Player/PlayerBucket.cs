using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerBucket : MonoBehaviour
{
    public static bool HoldingBucket = false;
    public static bool WithinBucket = false;
    public static bool Filled = false;
    [Tooltip("The object in the scene where bucket is respawning")]
    public GameObject BucketTapBucket;
    public GameObject BucketInPlayer;
    public List<GameObject> AnimationObjects = new List<GameObject>();

    public float Reach = 1f;

    bool _animationHappening;

    Animator _playerAnimator;
    Transform _playerModel;
    GameObject _tapBucketFilled;
    GameObject _handBucketFilled;

    PlayerLocomotion _playerLocomotion;
    TileGrid _tileGrid;

    GameObject _selectedObject;
    TileGeneric _selectedTile;

    void Start()
    {
        //BucketInPlayer = transform.Find("Bucket").gameObject;
        Transform filledTapBucket = BucketTapBucket.transform.Find("Water_Bucket");
        _tapBucketFilled = filledTapBucket.gameObject;
        _tapBucketFilled.SetActive(false);

        Transform filledHandBucket = BucketInPlayer.transform.Find("Water_Bucket");
        _handBucketFilled = filledHandBucket.gameObject;
        _handBucketFilled.SetActive(false);

        _playerModel = transform.Find("Jeffrey");
        _playerAnimator = _playerModel.GetComponent<Animator>();
        BucketInPlayer.SetActive(false);
        Debug.Log(BucketInPlayer.activeSelf);
        if (BucketInPlayer == null)
            Debug.LogWarning("No BUCKET object attached");

        _playerLocomotion = gameObject.GetComponent<PlayerLocomotion>();
        if (_playerLocomotion == null)
            Debug.LogWarning("No PlayerLocomotion component attached");

        _tileGrid = TileGrid.FindTileGrid();
        if (_tileGrid == null)
            Debug.LogError("No TileGrid component attached");
    }

    void Update()
    {
        _tapBucketFilled.SetActive(Filled); // Update Buckets to filled or not
        _handBucketFilled.SetActive(Filled);

        _selectedTile = SelectTile();

        // don't read this, it works this is for grabbing and animations
        #region Grabbing Bucket
        if (WithinBucket && Filled && Input.GetKeyDown(KeyCode.E)) // Grab bucket if bucket full
        {
            HoldingBucket = true;
            _playerAnimator.SetBool("IsGrabbing", true);
        }


        if (!HoldingBucket && WithinBucket && !Filled && !_animationHappening && Input.GetKeyDown(KeyCode.E)) // fill bucket if empty
        {
            foreach (var obj in AnimationObjects)
            {
                Animator animator = obj.GetComponent<Animator>();
                animator.SetBool("UsingBucket", true);
            }

            _animationHappening = true;
        }

        if (IsAnimationHappening() == false && _animationHappening) // signal bucket is full once animation is done
        {
            Filled = true;
            _animationHappening = false;
        }

        if (!Filled && HoldingBucket && Input.GetKeyDown(KeyCode.Mouse0)) // drop bucket when in range of tap
        {
            _playerAnimator.SetBool("IsGrabbing", true);
            HoldingBucket = false;
        }

        if (HoldingBucket) // hide bucket in tap and show bucket in hands
        {
            BucketTapBucket.SetActive(false);
            BucketInPlayer.SetActive(true);

        }
        else // the opposite
        {
            BucketTapBucket.SetActive(true);
            BucketInPlayer.SetActive(false);
        }



        #endregion 

        #region Extinguish Fire
        if (Filled && HoldingBucket && !WithinBucket && Input.GetKeyDown(KeyCode.Mouse0) && _selectedTile != null)  // empty bucket in hand and extinguish fire (emir you do this one)
        {
            Filled = false;

            _selectedTile.Damageable.SetFire(false);
        }
        #endregion

    }

    bool IsAnimationHappening()
    {
        GameObject animationTransform = AnimationObjects[0];
        Animator animator = animationTransform.GetComponent<Animator>();
        bool animationState = animator.GetBool("UsingBucket");
        return animationState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bucket"))
            WithinBucket = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bucket"))
            WithinBucket = false;
    }

    TileGeneric SelectTile()
    {
        // Get tile at mouse position
        var tile = _tileGrid.GetTile(_playerLocomotion.MousePos);

        if (tile != null)
        {
            // Check distance
            var distance = Vector3.Distance(_playerLocomotion.transform.position, tile.WorldCenter);
            if (distance > Reach || !tile.Damageable.OnFire)
                tile = null;
            else if (tile != null)
                _selectedObject = tile.GetObjects().Where(x => x.CompareTag("Grid Cell")).FirstOrDefault();
        }

        if (tile == null)
        {
            // Get closest broken tile from player
            var playerPos = _playerLocomotion.transform.position;

            // Overlap sphere
            var colliders = Physics.OverlapSphere(playerPos, Reach);
            float minDistance = float.MaxValue;
            foreach (var collider in colliders)
            {
                // Get tile
                var damageable = collider.GetComponent<TileDamageable>();
                if (damageable != null && damageable.OnFire)
                {
                    // Get distance
                    var distance = Vector3.Distance(playerPos, collider.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        tile = _tileGrid.GetTile(collider.transform.position);
                        _selectedObject = collider.gameObject;
                    }
                }
            }
        }

        return tile;
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
