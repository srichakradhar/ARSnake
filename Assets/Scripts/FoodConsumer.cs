using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodConsumer : MonoBehaviour
{
    // Start is called before the first frame update
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

            GetComponent<SceneController>().GameOver();
        }

        /*else if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "bad")
        {
            Debug.Log("Don't eat Bad food, or yourself!");
            GetComponent<SceneController>().GameOver();
        }*/
    }
}
