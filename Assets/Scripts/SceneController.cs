using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public ScoreboardController scoreboard;
    public SnakeController snakeController;
    public GameObject snake;
    public ParticleSystem explosionParticle;
    public Button restartButton;
    public Canvas canvas;
    public TextMeshProUGUI gameOverText;

    public bool isGameActive;

    // Start is called before the first frame update
    void Start()
    {
        QuitOnConnectionErrors();
        isGameActive = true;
        explosionParticle.transform.localScale = new Vector3(.01f, .01f, .01f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ProcessTouches();
        

        scoreboard.SetScore(snakeController.GetLength());
    }

    private void QuitOnConnectionErrors()
    {
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            StartCoroutine(CodelabUtils.ToastAndExit(
                "Camera permission is needed to run this application.", 5));
        }
        else if (Session.Status.IsError())
        {
            // This covers a variety of errors.  See reference for details
            // https://developers.google.com/ar/reference/unity/namespace/GoogleARCore
            StartCoroutine(CodelabUtils.ToastAndExit(
                "ARCore encountered a problem connecting. Please restart the app.", 5));
        }
    }

    private void ProcessTouches()
    {
        Touch touch;
        if (Input.touchCount != 1 ||
            (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter =
            TrackableHitFlags.PlaneWithinBounds |
            TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            SetSelectedPlane(hit.Trackable as DetectedPlane);
        }

        if (isGameActive)
        {
            GetComponent<FoodController>().SpawnFoodInstance();
        }
    }

    private void SetSelectedPlane(DetectedPlane selectedPlane)
    {
        Debug.Log("Selected plane centered at " + selectedPlane.CenterPose.position);
        scoreboard.SetSelectedPlane(selectedPlane);
        snakeController.SetPlane(selectedPlane);
        GetComponent<FoodController>().SetSelectedPlane(selectedPlane);
    }

    public void GameOver()
    {
        explosionParticle.Play();
        canvas.gameObject.SetActive(true);
        isGameActive = false;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        // restartButton.GetComponentInChildren<Text>().text = "RESTART";
        snakeController.playerAudio.PlayOneShot(snakeController.dieSound, 1.0f);
        StartCoroutine(WaitAndDisable(1.5f));
        Debug.Log("Game Over!!!");
    }

    // suspend execution for waitTime seconds
    IEnumerator WaitAndDisable(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        snake.SetActive(isGameActive);
    }

    public void RestartGame()
    {
        isGameActive = true;
        snake.SetActive(true);
        canvas.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }
    public void doExitGame()
    {
        Application.Quit();
    }

}
