using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class obstacles
{
    public Transform Barrier;
}
[System.Serializable]
public class Collctables
{
    public Transform Coin;
    public Transform Magnet;
    public Transform Jet;
    public Transform Coin_2x;
    public Transform HoverBoard;
}

public class PlatForm : MonoBehaviour
{
    [SerializeField] obstacles Obstacles_Prefabs;
    [SerializeField] Collctables Collctables_Prefabs;

    [SerializeField] List<GameObject> Obstacles = new List<GameObject>();
    [SerializeField] List<Transform> ObstaclePositions;
    [SerializeField] internal float length;

    void Instiate_Obstacles()
    {

    }

    void Instiate_Collectables()
    {

    }

}
