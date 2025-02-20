using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstiateObstacle : MonoBehaviour
{
    [SerializeField] PlatForm PlatForm;
    [SerializeField] GameObject Obstacle = null;

    private void Start()
    {
        Obstacle = GameObject.Instantiate(PlatForm.Obstacles_Prefabs[Random.Range(0, PlatForm.Obstacles_Prefabs.Count-1)], transform);
    }

}
