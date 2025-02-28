using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeCoins : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float zOffset;
    [SerializeField] LayerMask layer;
    [SerializeField] CollectablesManager cMgr;

    int posIndex = 1; // Start at 0 position
    float[] positions = { -2.7f, 0.0f, 2.7f };
    int zPos;
    bool placeCoins = true;

    private void Start()
    {
        zPos = Mathf.CeilToInt(transform.position.z);
    }

    private void FixedUpdate()
    {
        // Move towards designated lane position
        Vector3 pos = transform.position;
        pos.z = playerTransform.position.z + zOffset;
        pos.x = Mathf.Lerp(pos.x, positions[posIndex], 8.0f * Time.deltaTime);
        transform.position = pos;

        DodgeObstacles();

        // Place coins at specific intervals
        if ((int)transform.position.z > zPos + 1 && placeCoins)
        {
            //Debug.Log("Coin Placed");
            zPos = Mathf.CeilToInt(transform.position.z);
            cMgr.PlaceCoin(transform, positions[posIndex]); // Ensures correct X position
        }

        HandleCoinPlacementTiming();
    }

    bool waitBool = false;
    void HandleCoinPlacementTiming()
    {
        int currentTime = Mathf.FloorToInt(Time.time);

        if (!waitBool && currentTime % 1 == 0) // Check every 1 second
        {
            placeCoins = true;
            waitBool = true;
        }
        else if (waitBool && currentTime % 5 == 0) // Stop placing after 5 seconds
        {
            placeCoins = false;
            waitBool = false;
        }
    }

    void DodgeObstacles()
    {
        if (Physics.Raycast(transform.position, Vector3.forward, 5.0f, layer))
        {
            CalcNewIndex();
            Debug.DrawRay(transform.position, Vector3.forward * 2.0f, Color.blue);
        }

        if (Mathf.FloorToInt(Time.time) % 3 == 0)
        {
            CalcNewIndex();
        }
    }

    bool isPositive = false;
    void CalcNewIndex()
    {
        posIndex = isPositive ? posIndex + 1 : posIndex - 1;
        posIndex = Mathf.Clamp(posIndex, 0, 2);
        isPositive = posIndex == 2 ? false : (posIndex == 0 ? true : isPositive);
    }
}
