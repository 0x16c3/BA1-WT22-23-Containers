using System.Linq;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public TextAsset jsonDatabase;

    ScoreTable _scoreTable;
    int _totalScore = 0;

    void Start()
    {
        // Load the JSON file
        _scoreTable = JsonUtility.FromJson<ScoreTable>(jsonDatabase.text);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CalculateScore();
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
