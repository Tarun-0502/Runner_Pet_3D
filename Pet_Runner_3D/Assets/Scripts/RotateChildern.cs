using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateChildern : MonoBehaviour
{
    [SerializeField] List<GameObject> coins;
    public float speed = 120.0f;
    public float rotationOffset = 50.0f;
    public void GetList(List<GameObject> list)
    {
        coins = list;
        foreach (GameObject g in coins)
        {
            Vector3 rot = Vector3.up * (g.transform.GetSiblingIndex() * rotationOffset);
            g.transform.localEulerAngles += rot;
        }
    }
    void Update()
    {
        foreach (GameObject g in coins)
        {
            g.transform.localEulerAngles += Vector3.up * Time.deltaTime * speed;
        }
    }
}
