using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_Spwan : MonoBehaviour
{
    [SerializeField] float Length;

    bool Once;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player" && !Once)
        {
            Once = true;
            Invoke("ResetValue",3f);
            StartCoroutine(Path_Contoller.Instance.SpwanNextPath(1.2f));
        }
    }

    void ResetValue()
    {
        Once = false;
    }

}
