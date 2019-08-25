using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    private List<GameObject> activeTile;

    private Transform playerTransform;
    private float spawnZ = -15.0f;
    private float tileLength = 1000f;
    private float safeZone = 1015f;
    private int amountTileOnScreen = 2;
    private int lastPrefabIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        activeTile = new List<GameObject>();
        playerTransform = GameObject.Find("/Player").transform;
        for (int i = 0; i < amountTileOnScreen; i++)
        {
            if (i < 3)
                SpawnTile(0);
            else
                SpawnTile();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Checking tile is Used
        if (playerTransform.position.z - safeZone > (spawnZ - amountTileOnScreen * tileLength))
        {
            SpawnTile();
            DeleteTile();
        }
    }

    private void SpawnTile(int prefabIndex = -1)
    {
        GameObject tileSpawn;
        
        // Create tileSpawn
        if (prefabIndex == -1)
            tileSpawn = Instantiate(tilePrefabs[RandomPrefabIndex()]) as GameObject;
        else
            tileSpawn = Instantiate(tilePrefabs[prefabIndex]) as GameObject;
        
        // Set Parrent Tile to Tile Manager
        tileSpawn.transform.SetParent(transform);

        // Change position z to forward move
        Vector3 moveSpawn = Vector3.forward * spawnZ;
        tileSpawn.transform.position = new Vector3(tileSpawn.transform.position.x, tileSpawn.transform.position.y, moveSpawn.z);

        spawnZ += tileLength;

        // Add tileSpawn to List activeTile
        activeTile.Add(tileSpawn);
    }

    private void DeleteTile()
    {
        // Destroy and Remove tile which is used by user
        Destroy(activeTile[0]);
        activeTile.RemoveAt(0);
    }

    private int RandomPrefabIndex()
    {
        if (tilePrefabs.Length <= 1) return 0;

        int randomIndex = lastPrefabIndex;
        while (randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, tilePrefabs.Length);
        }

        lastPrefabIndex = randomIndex;
        return randomIndex;
    }
}
