using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodConsumer : MonoBehaviour
{
    private SceneController sceneController;
    private SnakeController snakeController;

    // Start is called before the first frame update
    private void Start()
    {
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        snakeController = GetComponentInParent<SnakeController>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "food")
        {
            collision.gameObject.SetActive(false);
            Slithering s = GetComponentInParent<Slithering>();

            if (s != null)
            {
                s.AddBodyPart();
            }

            snakeController.playerAudio.PlayOneShot(snakeController.eatSound, 1.0f);
        }
        else if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "bad")
        {
            Debug.Log("Don't eat Bad food, or yourself!");
            sceneController.GameOver();
        }
    }
}
