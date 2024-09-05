using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Make sure to include the TextMeshPro namespace
using System.Collections;
using System.Runtime.InteropServices;

public class Game_manager : MonoBehaviour
{

    [DllImport("__Internal")]private static extern void SendResult(string score, string stages, string inputs);

    [DllImport("__Internal")]private static extern void SendGameStartSignal();


    public bool isGameFinished;
    public GameObject panel;
    public Animator startPanel;
    public GameObject player;
    public Camera_controller camera_controller;
    public GameObject uinfo;
    public TextMeshProUGUI scoreTxt;

    private int score;
    public int ethRewardPoints = 5;
    public string actionDataString = "";
    public GameObject stagemanager;
    public bool isStarted;
    public bool awaitingSignature;
    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreText(); // Update the score text at the start
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        // Start the animation immediately
        startPanel.Play("start.canvas");
        Time.timeScale = 1f;
        // Start the coroutine to handle delayed activation
        StartCoroutine(ActivatePlayerAndCameraWithDelay(1.6f));
        StartCoroutine(IncreaseScoreOverTime());
    }

    private IEnumerator ActivatePlayerAndCameraWithDelay(float delay)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Activate the player and enable the camera controller
        uinfo.SetActive(true);
        player.SetActive(true);
        camera_controller.enabled = true;
        isStarted = true;
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

    public void SendResult()
    {
        if (!awaitingSignature)
        {
            string stringifiedstages = stagemanager.GetComponent<stage_manager>().GetStringifiedStageOrder();
            SendResult(score.ToString(), stringifiedstages, actionDataString);
            awaitingSignature = true;
        }
    }

    public void SendGameStart()
    {
        SendGameStartSignal();
    }

    public void SignedRefusedOrErrored()
    {
        awaitingSignature = false;
    }
    public void Signed()
    {
        awaitingSignature = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
