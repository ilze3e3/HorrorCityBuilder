using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    public Vector2Int worldSize;
    public Tile[,] world;
    public GameObject grassTilePrefab;
    public GameObject forestTilePrefab;
    public int baseForestSpawnChance;
    public int weightedForestSpawnChance;
    bool worldStatus;

    public ResourceSystem resourceSystem;
    void Awake()
    {
        world = new Tile[worldSize.x, worldSize.y];
    }
    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
        worldStatus = true;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void GenerateWorld()
    {
        for(int y = 0; y < worldSize.y; y++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                world[x, y] = GenerateWorldRules(x,y);
            }
        }
    }

    Tile GenerateWorldRules(int x, int y)
    {
        GameObject spawnedTile = null;
        Tile toBeSpawned = null;
        
        /// The first tile is always a forest tile. 
        if(x == 0 && y==0)
        {
            spawnedTile = Instantiate(forestTilePrefab, new Vector3(x*10, 0, y*10), new Quaternion());
            toBeSpawned = spawnedTile.GetComponent<TileAttached>().tileAttached;
        }
        /// Otherwise follow this rule: 
        /// If a neighbouring tile is a forest tile, then there is a 40% chance of it being a forest tile. 
        /// If a neighbouring tile is a grass tile, then there is a 20% chance of it being a forest tile. 
        else
        {
            int chance = baseForestSpawnChance;
            if(x != 0 && world[x-1,y].tileName.Contains("Forest"))
            {
                chance = weightedForestSpawnChance;
            }
            else if(y != 0 && world[x,y-1].tileName.Contains("Forest"))
            {
                chance = weightedForestSpawnChance;

            }

            if(ChanceSystem.PercentChance(chance))
            {
                spawnedTile = Instantiate(forestTilePrefab, new Vector3(x*10, 0, y*10), new Quaternion());
                toBeSpawned = spawnedTile.GetComponent<TileAttached>().tileAttached;
            }
            else
            {
                spawnedTile = Instantiate(grassTilePrefab, new Vector3(x*10, 0, y*10), new Quaternion());
                toBeSpawned = spawnedTile.GetComponent<TileAttached>().tileAttached;
            }
        }

        resourceSystem.InsertTileRecord(toBeSpawned, spawnedTile.transform);
        if(toBeSpawned.tileEffects.Count != 0)
        {
            resourceSystem.ApplyResourceEffects(toBeSpawned.tileEffects);
        }
        return toBeSpawned;
    }

    public void DemolishBuilding(GameObject tileObj)
    {
        Debug.Log("Demolishing tile");
        Tile tile = tileObj.GetComponent<TileAttached>().tileAttached;
        resourceSystem.RemoveResourceEffects(tile.tileEffects);
        resourceSystem.RemoveTileRecord(tile, tileObj.transform);
        Vector3 tilePosition = tileObj.transform.position;
        Destroy(tileObj);
        
        GameObject newTile = Instantiate(grassTilePrefab, tilePosition, new Quaternion());
        resourceSystem.InsertTileRecord(newTile.GetComponent<TileAttached>().tileAttached, newTile.transform);

    }

}
