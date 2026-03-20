using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileData;

public class MapManager : MonoBehaviour
{
    // dirt
    [SerializeField]
    private Tilemap dirtMap;

    [SerializeField]
    private TileBase dirtTile;

    // materials
    [SerializeField]
    private Tilemap tileTypeMap;

    [SerializeField]
    private TileBase ironTile;
    [SerializeField]
    private TileBase copperTile;
    [SerializeField]
    private TileBase goldTile;
    [SerializeField]
    private TileBase nestBorderTile;
    [SerializeField]
    private TileBase bedrockTile;

    // cracks
    [SerializeField]
    private Tilemap crackMap;

    [SerializeField]
    private TileBase crackTile1;
    [SerializeField]
    private TileBase crackTile2;
    [SerializeField]
    private TileBase crackTile3;

    [SerializeField]
    private GameObject EnemyPrefab;

    [SerializeField]
    private GameObject QueenPrefab;

    private Dictionary<Vector3Int, TileData> tileDatas;



    const int MAP_WIDTH = 100;
    const int MAP_HEIGHT = 150;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileDatas = new Dictionary<Vector3Int, TileData>();

        TempTile[,] typeGrid = new TerrainGenerator(MAP_WIDTH, MAP_HEIGHT).GenerateTerrain();

        for (int i = 0; i < MAP_HEIGHT; i++)
        {
            for (int j = 0; j < MAP_WIDTH; j++)
            {
                switch (typeGrid[i, j].type)
                {
                    case TempTile.TileType.EMPTY:
                        break;
                    case TempTile.TileType.DIRT:
                        AddTile(i, j, TileData.TileType.DIRT);
                        break;
                    case TempTile.TileType.IRON:
                        AddTile(i, j, TileData.TileType.IRON);
                        break;
                    case TempTile.TileType.COPPER:
                        AddTile(i, j, TileData.TileType.COPPER);
                        break;
                    case TempTile.TileType.GOLD:
                        AddTile(i, j, TileData.TileType.GOLD);
                        break;
                    case TempTile.TileType.BEDROCK:
                        AddTile(i, j, TileData.TileType.BEDROCK);
                        break;
                    case TempTile.TileType.NEST_BORDER:
                        AddTile(i, j, TileData.TileType.NEST_BORDER);
                        break;
                    case TempTile.TileType.NEST_CENTER:
                        SpawnNestEnemies(i, j);
                        break;
                    case TempTile.TileType.QUEEN:
                        SpawnQueenNestEnemies(i, j);
                        break;
                    default:
                        throw new System.Exception("PEPA UTOCI!!!");
                        break;
                }

                // tileDatas[gridPosition] = new TileData(dirtMap, gridPosition, tileType);
            }
        }
    }

    Vector3Int RowCol2GridPosition(int row, int col)
    {
        return new Vector3Int(col, -row, 0);
    }

    public void AddTile(int row, int col, TileData.TileType tileType)
    {
        // Debug.Log("adding tile: " + tileType + "to: " + row + "," + col);
        // hlina tam je vzdycky
        Vector3Int gridPosition = RowCol2GridPosition(row, col);
        dirtMap.SetTile(gridPosition, dirtTile);

        // kazdej tile ma svoje TileData
        tileDatas[gridPosition] = new TileData(this, row, col, tileType, Random.Range(1, 5));

        // materialovej overlay
        switch (tileType)
        {
            case TileData.TileType.DIRT:
                break;
            case TileData.TileType.IRON:
                tileTypeMap.SetTile(gridPosition, ironTile);
                break;
            case TileData.TileType.COPPER:
                tileTypeMap.SetTile(gridPosition, copperTile);
                break;
            case TileData.TileType.GOLD:
                tileTypeMap.SetTile(gridPosition, goldTile);
                break;
            case TileData.TileType.BEDROCK:
                tileTypeMap.SetTile(gridPosition, bedrockTile);
                break;
            case TileData.TileType.NEST_BORDER:
                tileTypeMap.SetTile(gridPosition, nestBorderTile);
                break;
            default:
                throw new System.Exception("PEPA UTOCI!!!");
                break;
        }
    }

    public void RemoveTile(int row, int col)
    {
        Vector3Int gridPosition = RowCol2GridPosition(row, col);

        dirtMap.SetTile(gridPosition, null);
        tileTypeMap.SetTile(gridPosition, null);
        crackMap.SetTile(gridPosition, null);
    }

    public void RemoveTile(Vector3Int gridPosition)
    {
        dirtMap.SetTile(gridPosition, null);
        tileTypeMap.SetTile(gridPosition, null);
        crackMap.SetTile(gridPosition, null);
        tileDatas.Remove(gridPosition);
    }

    public void SpawnNestEnemies(int row, int col)
    {
        if (EnemyPrefab == null)
            return;

        int enemiesCount = Random.Range(5, 20);
        for (int i = 0; i < enemiesCount; i++)
        {
            Vector3 GridPositon = RowCol2GridPosition(row, col);
            Vector3 position = new Vector3(GridPositon.x + Random.value*2, GridPositon.y + Random.value*2, GridPositon.z);    
            Instantiate(EnemyPrefab, position: position, rotation: Quaternion.identity);
        }
    }

    public void SpawnQueenNestEnemies(int row, int col)
    {
        if (QueenPrefab == null)
            return;

        Instantiate(QueenPrefab, position: RowCol2GridPosition(row, col), rotation: Quaternion.identity);
    }

    //vraci typ tilu, ktery byl vytezen, jestlize nebyl vytezen, tak vraci null
    public TileData? HitTile(Vector2 position, float damage)
    {
        Vector3Int gridPosition = dirtMap.WorldToCell(position);
        // Debug.Log(gridPosition);

        float remainingDuration = tileDatas[gridPosition].Damage(damage);

        if (remainingDuration <= 0)
        {
            TileData tile = tileDatas[gridPosition];
            RemoveTile(gridPosition);
            return tile;
        }
        else if (remainingDuration < 2)
        {
            crackMap.SetTile(gridPosition, crackTile3);
        }
        else if (remainingDuration < 3)
        {
            crackMap.SetTile(gridPosition, crackTile2);
        }
        else if (remainingDuration < 4)
        {
            crackMap.SetTile(gridPosition, crackTile1);
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
