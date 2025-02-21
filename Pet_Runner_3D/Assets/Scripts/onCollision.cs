using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class onCollision : MonoBehaviour
{
    public PlayerController controller;
    public TagField TagField;

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.transform.tag=="Player")
            return;
        //controller.OnCharacterColliderHit(collision.collider);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            controller.HandleCollision(other);
        }
    }
}
