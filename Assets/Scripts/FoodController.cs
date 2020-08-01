using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class FoodController : MonoBehaviour
{
    private DetectedPlane detectedPlane;
    private GameObject foodInstance;
    private float foodAge;
    private readonly float maxAge = 10f;

    public GameObject[] foodModels;
    public GameObject[] badModels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (detectedPlane == null)
        {
            return;
        }

        if (detectedPlane.TrackingState != TrackingState.Tracking)
        {
            return;
        }

        // Check for the plane being subsumed
        // If the plane has been subsumed switch attachment to the subsuming plane.
        while (detectedPlane.SubsumedBy != null)
        {
            detectedPlane = detectedPlane.SubsumedBy;
        }

        if ((foodInstance == null || foodInstance.activeSelf == false) && GetComponent<SceneController>().isGameActive)
        {
            SpawnFoodInstance();
            return;
        }

        foodAge += Time.deltaTime;
        if (foodAge >= maxAge)
        {
            Destroy(foodInstance);
            foodInstance = null;
        }
    }

    public void SetSelectedPlane(DetectedPlane selectedPlane)
    {
        detectedPlane = selectedPlane;
    }

    public void SpawnFoodInstance()
    {

        int foodOrBad = Random.Range(0, 10);
        GameObject foodItem;

        foodItem = foodModels[Random.Range(0, foodModels.Length)];
        
        /*
        if(foodOrBad > 6)
        {
            foodItem = foodModels[Random.Range(0, foodModels.Length)];
        }
        else
        {
            foodItem = badModels[Random.Range(0, badModels.Length)];
        }
        */

        // Pick a location.  This is done by selecting a vertex at random and then
        // a random point between it and the center of the plane.
        List<Vector3> vertices = new List<Vector3>();
        detectedPlane.GetBoundaryPolygon(vertices);
        Vector3 pt = vertices[Random.Range(0, vertices.Count)];
        float dist = Random.Range(0.05f, 1f);
        Vector3 position = Vector3.Lerp(pt, detectedPlane.CenterPose.position, dist);
        // Move the object above the plane.
        position.y += .05f;


        Anchor anchor = detectedPlane.CreateAnchor(new Pose(position, Quaternion.identity));

        foodInstance = Instantiate(foodItem, position, Quaternion.identity,
                 anchor.transform);

        foodInstance.tag = "food";

        // Set the tag.
        /*
        if (foodOrBad > 6)
        {
            foodInstance.tag = "food";
        }
        else
        {
            foodInstance.tag = "bad";
        }
        */

        foodInstance.transform.localScale = new Vector3(.025f, .025f, .025f);
        foodInstance.transform.SetParent(anchor.transform);
        foodAge = 0;

        foodInstance.AddComponent<FoodMotion>();
    }
}
