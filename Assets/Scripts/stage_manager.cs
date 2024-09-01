using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage_manager : MonoBehaviour
{
    [System.Serializable]
    public struct StagePrefab
    {
        public GameObject prefab;
        public float probability;
    }

    public StagePrefab[] stagePrefabs; // Array to hold the stage prefabs and their probabilities
    public Transform player; // Reference to the player transform
    public float stageLength = 116f; // Length of each stage
    public float spawnThreshold = 58f; // Distance from the player's position to trigger a new stage spawn
    public float deleteThreshold = 200f; // Distance behind the player to delete the stage
    public GameObject game_manager; // Reference to the GameManager object

    private List<GameObject> spawnedStages = new List<GameObject>(); // Keep track of spawned stages
    private List<int> stageOrder = new List<int>(); // List to keep track of spawned stage indices
    private int currentStageIndex = 0; // Index of the current stage
    private float totalProbability; // Total of all probabilities for weighted random selection
    private Game_manager gameManagerComponent; // Reference to the GameManager component

    void Start()
    {
        // Calculate the total probability for random selection
        foreach (StagePrefab stagePrefab in stagePrefabs)
        {
            totalProbability += stagePrefab.probability;
        }

        // Spawn the initial stage at the starting position
        SpawnStage(Vector3.zero);

        // Get the GameManager component
        if (game_manager != null)
        {
            gameManagerComponent = game_manager.GetComponent<Game_manager>();
        }
    }

    void Update()
    {
        // Check if the game is finished
        if (gameManagerComponent != null && gameManagerComponent.isGameFinished)
        {
            // Game is finished, stop spawning and send the stringified list to GameManager
  
            return;
        }

        // Check if the player has reached the spawn threshold for the next stage
        float nextStagePositionX = (currentStageIndex + 1) * stageLength;
        if (player.position.x > nextStagePositionX - spawnThreshold)
        {
            SpawnStage(new Vector3(nextStagePositionX, 0, 2));
            currentStageIndex++;
        }

        // Check and remove old stages behind the player
        if (spawnedStages.Count > 0)
        {
            GameObject oldestStage = spawnedStages[0];
            if (player.position.x - oldestStage.transform.position.x > deleteThreshold)
            {
                Destroy(oldestStage);
                spawnedStages.RemoveAt(0);
            }
        }
    }

    void SpawnStage(Vector3 spawnPosition)
    {
        // Select a random stage prefab based on the weighted probabilities
        GameObject selectedPrefab = GetRandomStagePrefab();

        // Find the index of the selected prefab
        int selectedPrefabIndex = System.Array.FindIndex(stagePrefabs, s => s.prefab == selectedPrefab);

        // Spawn the selected stage at the given position
        GameObject newStage = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // Add the new stage to the list of spawned stages
        spawnedStages.Add(newStage);

        // Track the order of the spawned stage indices
        stageOrder.Add(selectedPrefabIndex + 1); // Add 1 to make it 1-based indexing
    }

    GameObject GetRandomStagePrefab()
    {
        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0f;

        foreach (StagePrefab stagePrefab in stagePrefabs)
        {
            cumulativeProbability += stagePrefab.probability;
            if (randomValue <= cumulativeProbability)
            {
                return stagePrefab.prefab;
            }
        }

        return stagePrefabs[0].prefab; // Fallback, should never be reached if probabilities are correctly set
    }

    // Method to stringify the list of stage indices
    public string GetStringifiedStageOrder()
    {
        return string.Join(",", stageOrder);
    }

}
