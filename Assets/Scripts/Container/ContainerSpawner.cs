using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerSpawner : MonoBehaviour
{
    public int CommonCount = 27;
    public int RareCount = 16;
    public int EpicCount = 12;
    public int LegendaryCount = 5;

    public int Total => CommonCount + RareCount + EpicCount + LegendaryCount;

    TileGrid _grid;

    Dictionary<ContainerRarity, int> _remaining = new Dictionary<ContainerRarity, int>();

    public GameObject ContainerPrefab;

    bool _initialized = false;

    void Update()
    {
        if (!_initialized)
        {
            Initialize();
            return;
        }
    }

    void Initialize()
    {
        _grid = TileGrid.FindTileGrid();
        if (!_grid.Initialized)
            return;

        Randomize();

        _initialized = true;
    }

    void SpawnContainers()
    {
        for (int i = 0; i < Total; i++)
        {
            // Loop through all the tiles
            var randomX = Random.Range(_grid.cellBounds.xMin + 2, _grid.cellBounds.xMax - 1);
            var randomY = Random.Range(_grid.cellBounds.yMin, _grid.cellBounds.yMax);

            int iter = 0;
            while (iter < 1000)
            {
                iter++;

                var tile = _grid.GetTile(new Vector2Int(randomX, randomY));
                if (tile == null)
                    continue;

                var highestCollider = HighestCollider(tile);
                if (highestCollider == null)
                    continue;

                // Spawn a container at the highest collider's top position + this colliders extents
                var container = Instantiate(ContainerPrefab, highestCollider.transform.position + new Vector3(0, highestCollider.GetComponent<Collider>().bounds.extents.y, 0), Quaternion.identity);
                container.transform.SetParent(transform);
                break;
            }
        }
    }

    void Randomize()
    {
        _remaining[ContainerRarity.Common] = CommonCount;
        _remaining[ContainerRarity.Rare] = RareCount;
        _remaining[ContainerRarity.Epic] = EpicCount;
        _remaining[ContainerRarity.Legendary] = LegendaryCount;

        // Loop throug all "Grabbable" tag objects
        foreach (var grabbable in GameObject.FindGameObjectsWithTag("Grabbable"))
        {
            // Get the container component
            var container = grabbable.GetComponent<ContainerGeneric>();
            if (container == null)
                continue;

            if ((int)container.Type >= 4)
                continue;

            // Select random container type
            var containerType = (ContainerType)Random.Range(0, 4);

            // Select random container rarity
            var containerRarity = (ContainerRarity)Random.Range(0, 4);

            // If there are no more containers of this rarity, select a new rarity
            int iter = 0;
            while (_remaining[containerRarity] == 0 && iter < 1000)
            {
                containerRarity = (ContainerRarity)Random.Range(0, (int)ContainerRarity.RARITY_MAX);
                iter++;
            }
            // Set the container type and rarity
            container.SetType(containerType);
            container.SetRarity(containerRarity);

            // Decrement the remaining count for this rarity
            _remaining[containerRarity]--;
        }
    }

    GameObject HighestCollider(TileGeneric tile)
    {
        List<GameObject> objects = tile.GetObjects();
        List<GameObject> objectsWithColliders = new List<GameObject>();

        for (int i = 0; i < objects.Count; i++)
        {
            // Ignore if ground layer
            if (objects[i].layer == 3)
                continue;

            // Ignore if clip brush
            if (objects[i].layer == 4)
                continue;

            var gridCell = objects[i].GetComponent<TileGridCell>();
            if (gridCell != null && gridCell.Broken)
                continue;

            var damageable = objects[i].GetComponent<TileDamageable>();
            if (damageable != null && (damageable.OnFire || damageable.Health <= 0))
                continue;

            // Ignore if current tile is the object
            if (objects[i].transform.position == tile.WorldCenter)
                continue;

            var collider = objects[i].GetComponent<Collider>();
            if (collider != null && !collider.isTrigger)
                objectsWithColliders.Add(objects[i]);
        }

        if (objectsWithColliders.Count >= 3)
        {
            // Get the highest object
            GameObject highest = objectsWithColliders[0];
            for (int i = 1; i < objectsWithColliders.Count; i++)
            {
                if (objectsWithColliders[i].transform.position.y > highest.transform.position.y)
                    highest = objectsWithColliders[i];
            }

            return highest;
        }

        return null;
    }
}
