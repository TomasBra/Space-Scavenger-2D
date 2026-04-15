using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static TileData;
using static UnityEngine.Rendering.DebugUI.Table;

public class MapManager : GameObject2D
{
    private static readonly Vector2Int[] directions4 = new Vector2Int[4]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

    private int maxQueenCount = 0;
    private int killedQueenCount = 0;

    [SerializeField]
    private ItemCounter itemCounter;

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

    // light
    [SerializeField]
    private Tilemap lightMap;

    [SerializeField]
    private TileBase lightTile;

    [SerializeField]
    private GameObject RangeEnemyPrefab;

    [SerializeField]
    private GameObject MeleeEnemyPrefab;

    [SerializeField]
    private GameObject QueenPrefab;

    private Dictionary<Vector3Int, TileData> tileDatas;

    // dirt bg
    [SerializeField]
    private Tilemap dirtBgMap;

    [SerializeField]
    private TileBase dirtBgTile;

    // space bg
    [SerializeField]
    private Tilemap spaceBgMap;

    [SerializeField]
    private TileBase[] spaceBGTiles;


    public const int MAP_WIDTH = 100;
    public const int MAP_HEIGHT = 150;
    public const int SKY_HEIGHT = 10;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileDatas = new Dictionary<Vector3Int, TileData>();

        TempTile[,] typeGrid = new TerrainGenerator(MAP_WIDTH, MAP_HEIGHT).GenerateTerrain();

        for (int i = -10; i < 10; i++)
        {
            for (int j = 0; j < MAP_WIDTH; j++)
            {
                int roll = Random.Range(0, spaceBGTiles.Length);
                TileBase tile = spaceBGTiles[roll];
                spaceBgMap.SetTile(RowCol2GridPosition(i, j), tile);
            }
        }

        for (int i = 0; i < MAP_HEIGHT; i++)
        {
            for (int j = 0; j < MAP_WIDTH; j++)
            {
                // background
                dirtBgMap.SetTile(RowCol2GridPosition(i, j), dirtBgTile);

                // tiles
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
                        maxQueenCount++;
                        SpawnQueenNestEnemies(i, j);
                        break;
                    default:
                        throw new System.Exception("PEPA UTOCI!!!");
                        break;
                }

                // tileDatas[gridPosition] = new TileData(dirtMap, gridPosition, tileType);
            }
        }

        itemCounter.SetSamples(killedQueenCount, maxQueenCount);
        //InitLightTiles();
    }

    /*void InitLightTiles()
    {
        for (int i = -1; i < MAP_HEIGHT + 1; i++)
        {
            for (int j = -1; j < MAP_WIDTH + 1; j++)
            {
                if (GetTileEmptyNighborsCount(i, j) == 0)
                {
                    //Debug.Log("placing lighttile on " + RowCol2GridPosition(i, j));
                    lightMap.SetTile(RowCol2GridPosition(i, j), lightTile);
                }
            }
        }
    } TODO: smazat */

    int GetTileEmptyNighborsCount(int row, int col)
    {
        int count = 0;
        foreach (Vector2Int direction in directions4)
        {
            int neighborRow = row + direction.y;
            int neighborCol = col + direction.x;

            if (dirtMap.GetTile(RowCol2GridPosition(neighborRow, neighborCol)) == null)
            {
                count++;
            }
        }

        return count;
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
        lightMap.SetTile(gridPosition, null);
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
        if (RangeEnemyPrefab == null)
            return;

        Vector3 GridPositon = RowCol2GridPosition(row, col);
        int enemiesCount = Random.Range(5, 20);

        for (int i = 0; i < enemiesCount; i++)
        {
            Instantiate(RangeEnemyPrefab, position: randomOffsettedPosition(GridPositon, 5), rotation: Quaternion.identity);
            Instantiate(MeleeEnemyPrefab, position: randomOffsettedPosition(GridPositon, 5), rotation: Quaternion.identity);
        }

        Instantiate(QueenPrefab, position: GridPositon, rotation: Quaternion.identity);
    }

    public void SpawnQueenNestEnemies(int row, int col)
    {
        if (QueenPrefab == null)
            return;

        Instantiate(QueenPrefab, position: RowCol2GridPosition(row, col), rotation: Quaternion.identity);
    }

    public TileData? GetTile(Vector2 wordPosition)
    {
        Vector3Int gridPosition = dirtMap.WorldToCell(wordPosition);
        // Debug.Log(gridPosition);

        if(!tileDatas.ContainsKey(gridPosition))
            return null;

        return tileDatas[gridPosition];
    }

    //vraci typ tilu, ktery byl vytezen, jestlize nebyl vytezen, tak vraci null
    public TileData? HitTile(Vector2 position, float damage)
    {
        Vector3Int gridPosition = dirtMap.WorldToCell(position);

        if (!tileDatas.ContainsKey(gridPosition))
            return null;

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

    public void QueenKilled()
    { 
        killedQueenCount++;

        itemCounter.SetSamples(killedQueenCount, maxQueenCount);

        if (killedQueenCount >= maxQueenCount)
        {
            Win();
        }
    }

    private void Win()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
