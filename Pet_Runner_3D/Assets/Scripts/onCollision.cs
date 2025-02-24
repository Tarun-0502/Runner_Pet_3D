using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class onCollision : MonoBehaviour
{
    public PlayerController controller;
    public TagField TagField;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
            controller.HandleCollision(other);
        }
    }
}
