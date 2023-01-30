using System.Linq;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public TextAsset jsonDatabase;

    public int TileBreakPoints = 2;
    public int TileRepairPoints = 1;

    ScoreTable _scoreTable;
    int _totalScore = 0;

    [HideInInspector]
    public int ShipCondition => _shipCondition;
    public int MaxShipCondition => _maxShipCondition;

    int _shipCondition = -1;
    int _maxShipCondition = -1;

    bool _initialized = false;

    public void OnTileBreak() => _shipCondition = Mathf.Clamp(ShipCondition - TileBreakPoints, 0, MaxShipCondition);
    public void OnTileRepair() => _shipCondition = Mathf.Clamp(ShipCondition + TileRepairPoints, 0, MaxShipCondition);

    void Initialize()
    {
        // Get all the tiles
        var grid = TileGrid.FindTileGrid();
        if (!grid.Initialized)
            return;

        _shipCondition = grid.cellBounds.size.x * grid.cellBounds.size.y;
        _maxShipCondition = ShipCondition;

        // Load the JSON file
        _scoreTable = JsonUtility.FromJson<ScoreTable>(jsonDatabase.text);
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized)
        {
            Initialize();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CalculateScore();
            Debug.Log($"Ship Condition: {ShipCondition} / {MaxShipCondition}");
        }

        if (ShipCondition <= 0)
        {
            Debug.Log("Game Over");
        }
    }

    public void CalculateScore()
    {
        int totalScore = 0;

        // Get every container in the scene
        var containers = FindObjectsOfType<ContainerGeneric>();

        // Loop through each container
        foreach (var container in containers)
        {
            int score = 0;

            if (container.GetComponent<AIBehavior>() != null)
            {
                var scoreData = _scoreTable.scoreValues.Beasts;
                score = scoreData.GetScore(container.Type);
            }
            else
            {
                var scoreData = _scoreTable.scoreValues.Containers.FirstOrDefault(x => x.ContainerType == container.Type);
                score = scoreData.GetScore(container.Rarity);
            }

            // 25% dropoff per 25% health lost compared to max health
            var healthPercent = (float)container.Health / container.MaxHealth;
            var dropoffPercent = 1f - healthPercent;
            var dropoff = Mathf.FloorToInt(dropoffPercent / 0.25f);
            score = Mathf.FloorToInt(score * Mathf.Pow(0.75f, dropoff));

            totalScore += score;
        }

        _totalScore = totalScore;
        Debug.Log($"Total Score: {totalScore}");
    }
}
