using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Make sure to include the TextMeshPro namespace
using System.Collections;

public class Game_manager : MonoBehaviour
{
    public bool isGameFinished;
    public GameObject panel;

    // Public variable to hold the reference to the TextMeshPro object
    public TextMeshProUGUI scoreTxt;

    // Private variable to hold the score
    private int score;

    // Public variable for external score increment
    public int ethRewardPoints = 10;

    // Start is called before the first frame update
    void Start()
    {
        // Start a coroutine to increase the score every second
        StartCoroutine(IncreaseScoreOverTime());
        UpdateScoreText(); // Update the score text at the start
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Method to reset the scene
    public void ResetScene()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Coroutine to increase the score by 1 every second
    private IEnumerator IncreaseScoreOverTime()
    {
        while (!isGameFinished)
        {
            yield return new WaitForSeconds(0.5f);
            score += 1;
            UpdateScoreText(); // Update the score text
        }
    }

    // Getter to read the score value
    public int GetScore()
    {
        return score;
    }

    // Method to increase the score by ethRewardPoints
    public void IncreaseScore()
    {
        score += ethRewardPoints;
        UpdateScoreText(); // Update the score text
    }

    // Method to update the score text on the screen
    private void UpdateScoreText()
    {
        if (scoreTxt != null)
        {
            scoreTxt.text = score.ToString();
        }
    }
}
