using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Position_X : MonoBehaviour
{
    private List<float> posX=new List<float>() { -2.81f,0.72f,4.22f};

    private void OnEnable()
    {
        transform.localPosition = new Vector3(posX[Random.Range(0,3)],transform.localPosition.y,0);
    }

}
