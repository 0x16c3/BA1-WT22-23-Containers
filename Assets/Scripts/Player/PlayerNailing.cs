using System.Linq;
using UnityEngine;

public class PlayerNailing : MonoBehaviour
{

    public float RepairRadius = 1f;
    public float RepairTime = 2f;
    public GameObject RepairUI;
    public GameObject NailsBox;
    public GameObject Hammer;
    public GameObject Planks;
    public int MaterialAmount;
    [HideInInspector]
    public static int HasMaterials;

    [HideInInspector]
    public int AmountRepaired = 0;

    PlayerLocomotion _playerLocomotion;
    TileGrid _tileGrid;

    public static bool NearChest = false;
    bool _repairing = false;
    float _timePassed = 0;

    GameObject _progressBar;
    GameObject _selectedObject;
    TileGeneric _selectedTile;

    Animator _boxAnimator;
    Animator _playerAnimator;
    Transform _playerModel;

    private void Start()
    {
        Hammer.SetActive(false);
        Planks.SetActive(false);

        _playerModel = transform.Find("Jeffrey");
        _playerAnimator = _playerModel.GetComponent<Animator>();
        _boxAnimator = NailsBox.GetComponent<Animator>();

        _playerLocomotion = gameObject.GetComponent<PlayerLocomotion>();
        if (_playerLocomotion == null)
            Debug.LogWarning("No PlayerLocomotion component attached");

        _tileGrid = TileGrid.FindTileGrid();
        if (_tileGrid == null)
            Debug.LogError("No TileGrid component attached");

        if (RepairUI != null)
        {
            RepairUI.SetActive(false);
            _progressBar = RepairUI.transform.Find("Progress").gameObject;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && NearChest)
        {
            _playerAnimator.SetBool("IsGrabbing", true);
            _boxAnimator.SetBool("BoxOpening", true);
            HasMaterials = MaterialAmount;
            Debug.Log("HasMaterials: " + HasMaterials);
            AudioController.instance.PlayAudio("Grab");
        }
        if (Input.GetKeyDown(KeyCode.X) && HasMaterials > 0)
        {
            _playerAnimator.SetBool("IsGrabbing", true);
            _boxAnimator.SetBool("BoxOpening", true);
            HasMaterials = 0;
            Debug.Log("HasMaterials: " + HasMaterials);
            AudioController.instance.PlayAudio("Grab");
        }

        if (HasMaterials > 0)
        {
            Planks.SetActive(true);
            Hammer.SetActive(true);
            _playerLocomotion.HasSlowEffect = true;
        }
        else
        {
            Planks.SetActive(false);
            Hammer.SetActive(false);
            _playerLocomotion.HasSlowEffect = false; 
        }

        _selectedTile = SelectTile();

        // Set UI position above selected tile
        if (_selectedObject != null)
        {
            RepairUI.transform.position = _selectedObject.transform.position + new Vector3(0, -0.5f, 0);
        }

        if (Input.GetKey(KeyCode.Mouse1) && HasMaterials > 0 && _selectedTile != null)
        {
            if (AudioController.instance.IsAudioPlaying("Repair") == false)
            {
                AudioController.instance.PlayAudio("Repair");
            }

            _timePassed += Time.deltaTime;
            _repairing = true;

            if (_timePassed > RepairTime)
            {
                _timePassed = 0f;
                AmountRepaired++;
                HasMaterials--;

                _selectedTile.Damageable.Heal(99);
            }
        }
        else
        {
            _timePassed = 0f;
            _repairing = false;
            AmountRepaired = 0;
        }

        if (RepairUI != null)
        {
            RepairUI.SetActive(_repairing);

            if (_progressBar)
            {
                var progress = Mathf.Clamp(_timePassed / RepairTime, 0, 1);
                _progressBar.transform.localScale = new Vector3(progress, 1, 1);
            }
        }
    }

    TileGeneric SelectTile()
    {
        // Get tile at mouse position
        var tile = _tileGrid.GetTile(_playerLocomotion.MousePos);

        if (tile != null)
        {
            // Check distance
            var distance = Vector3.Distance(_playerLocomotion.transform.position, tile.WorldCenter);
            if (distance > RepairRadius)
                tile = null;
            else if (tile != null)
                _selectedObject = tile.GetObjects().Where(x => x.CompareTag("Grid Cell")).FirstOrDefault();
        }

        if (tile == null)
        {
            // Get closest broken tile from player
            var playerPos = _playerLocomotion.transform.position;

            // Overlap sphere
            var colliders = Physics.OverlapSphere(playerPos, RepairRadius);
            float minDistance = float.MaxValue;
            foreach (var collider in colliders)
            {
                // Get tile
                var damageable = collider.GetComponent<TileDamageable>();
                if (damageable != null && damageable.Health < damageable.MaxHealth)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Nail Chest"))
        {
            //display hint "Press X to pick up"
            NearChest = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Nail Chest"))
            NearChest = false;
    }

    void OnDrawGizmos()
    {
        if (_selectedTile != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_selectedObject.transform.position, _selectedObject.transform.localScale);
        }
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
