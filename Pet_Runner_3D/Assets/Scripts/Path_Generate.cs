using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_Generate : MonoBehaviour
{
    public GameObject Tile1;
    public GameObject Tile2;
    public GameObject StartTile;

    public float nextTilePosition = 15.971f * 5; // Next tile position after StartTiles
    public float Speed = -4;

    private void Start()
    {
        // Instantiate Initial Tiles
        for (int i = 0; i < 5; i++)
        {
            GameObject startTile = Instantiate(StartTile, transform);
            startTile.transform.position = new Vector3(0, 0, i * 15.971f);
        }
    }

    private void Update()
    {
        // Move existing path backward
        transform.position += new Vector3(0, 0, Speed * Time.deltaTime);

        // Generate new tiles ahead
        if (transform.position.z <= nextTilePosition - 15.971f)
        {
            //SpawnTile();
            //nextTilePosition += 15.971f;
        }
    }

    private void SpawnTile()
    {
        int RandomInt = Random.Range(0, 2);
        GameObject newTile = Instantiate(RandomInt == 1 ? Tile1 : Tile2, transform);
        newTile.transform.position = new Vector3(0, 0, nextTilePosition);
    }
}
