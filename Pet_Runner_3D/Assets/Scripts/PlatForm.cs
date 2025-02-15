using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlatForm : MonoBehaviour
{
    [SerializeField] internal List<GameObject> Obstacles_Prefabs;
    [SerializeField] internal List<GameObject> Collctables_Prefabs;

    [SerializeField] Transform ObstaclesParent;
    [SerializeField] internal List<Transform> ObstaclePositions;
    [SerializeField] internal float length;

    bool Once;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !Once)
        {
            Once = true;
            Invoke("ResetValue", 3f);
            StartCoroutine(Path_Contoller.Instance.SpwanNextPath(1.2f));
        }
    }

    void ResetValue()
    {
        Once = false;
    }
}
