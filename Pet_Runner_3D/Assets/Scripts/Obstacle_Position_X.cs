using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Position_X : MonoBehaviour
{
    private List<float> posX = new List<float>() { -3f, 0f, 3f };

    private void OnEnable()
    {
        if (Path_Contoller.Instance == null)
        {
            Debug.LogError("Path_Contoller instance is null!");
            return;
        }

        if (Path_Contoller.Instance._X < 0 || Path_Contoller.Instance._X >= posX.Count)
        {
            Debug.LogError("Invalid _X value: " + Path_Contoller.Instance._X);
            return;
        }

        transform.localPosition = new Vector3(posX[Path_Contoller.Instance._X], transform.localPosition.y, 0);
        //Debug.Log("Position Set: " + posX[Path_Contoller.Instance._X]);

        switch (Path_Contoller.Instance._X)
        {
            case 0:
                Path_Contoller.Instance._X = 1;
                break;
            case 1:
                Path_Contoller.Instance._X = 2;
                break;
            case 2:
                Path_Contoller.Instance._X = 0;
                break;
        }
    }
}
