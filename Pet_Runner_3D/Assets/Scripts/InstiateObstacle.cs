using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstiateObstacle : MonoBehaviour
{
    [SerializeField] PlatForm PlatForm;
    [SerializeField] bool vehicle;
    [SerializeField] GameObject Obstacle = null;
    private int random; // Removed SerializeField to generate dynamically

    private void Start()
    {
        random = Random.Range(0, 2); // Generates either 0 or 1

        if (!vehicle)
        {
            // Spawn a random obstacle
            Obstacle = Instantiate(PlatForm.Obstacles_Prefabs[Random.Range(0, PlatForm.Obstacles_Prefabs.Count)], transform);
        }
        else
        {
            if (random == 0)
            {
                // Spawn a static vehicle
                Obstacle = Instantiate(PlatForm.Static_VehiclesPrefabs[Random.Range(0, PlatForm.Static_VehiclesPrefabs.Count)], transform);
            }
            else
            {
                // Spawn a random obstacle (if no vehicles or random == 1)
                Obstacle = Instantiate(PlatForm.Obstacles_Prefabs[Random.Range(0, PlatForm.Obstacles_Prefabs.Count)], transform);
            }
        }
    }
}