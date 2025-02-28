using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class onCollision : MonoBehaviour
{
    public PlayerController controller;
    [SerializeField] string collisionTag = "EnemyObject";

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(collisionTag))
        {
            controller.HandleCollision(other);
        }
    }
}
