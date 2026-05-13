using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MapManager : GameObject2D
{
    public enum GameState
    {
        LANDING,
        PLAY,
        TAKEOFF
    }

    public GameState currentGameState = GameState.LANDING;

    private static readonly Vector2Int[] directions4 = new Vector2Int[4]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

    private const int MAX_MAX_QUEEN_COUNT = 3;
    private int maxQueenCount = 0;
    private int killedQueenCount = 0;

    [SerializeField]
    private ItemCounter itemCounter;

    // dirt
    [SerializeField]
    public Tilemap dirtMap;

    [SerializeField]
    private TileBase[] dirtTiles;

    [SerializeField]
    private Tilemap shadowMap;

    [SerializeField]
    private TileBase shadowTile;

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
    [SerializeField]
    private TileBase groundTile; // unused

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


    [SerializeField]
    private GameObject IronPrefab;

    [SerializeField]
    private GameObject CopperPrefab;

    [SerializeField]
    private GameObject GoldPrefab;

    public LayerMask layerMask;


    public GameObject EndScreen;

    public const int MAP_WIDTH = 75; // 100
    public const int MAP_HEIGHT = 120; // 150
    public const int SKY_HEIGHT = 10;
    public const int MIN_ENEMY_COUNT = 4;
    public const int MAX_ENEMY_COUNT = 7;
    public const int MIN_QUEEN_ENEMY_COUNT = 6;
    public const int MAX_QUEEN_ENEMY_COUNT = 11;
    public const float DEFAULT_DURABILITY = 4.0f;

    static List<Vector2> pointsAroundTile;

    public static float GetEnemyCountDepthCoef(int absoluteDepth)
    {
        float relativeDepth = absoluteDepth / (float)MAP_HEIGHT;
        float bonus = relativeDepth * 1.25f;

        return 1.0f + bonus;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        pointsAroundTile = new List<Vector2>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                pointsAroundTile.Add(new Vector2(i * 0.5f, j * 0.5f));
            }
        }

        layerMask = LayerMask.GetMask("TileMap");

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
                shadowMap.SetTile(RowCol2GridPosition(i, j), shadowTile);
                // tiles
                switch (typeGrid[i, j].type)
                {
                    case TempTile.TileType.EMPTY:
                        break;
                    case TempTile.TileType.DIRT:
                        AddTile(i, j, TileData.TileType.DIRT);
                        break;
                    case TempTile.TileType.ENTRANCE:
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
                    case TempTile.TileType.GROUND:
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
                        Debug.Log("Peperino Bombardino");
                        break;
                }
            }
        }

        maxQueenCount = Mathf.Min(maxQueenCount, MAX_MAX_QUEEN_COUNT);

        this.RemoveTile(new Vector3Int(MAP_WIDTH / 2, 0));
        itemCounter.SetSamples(killedQueenCount, maxQueenCount);
    }

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

        // choose dirt type
        float relativeDepth = row / (float)MAP_HEIGHT;
        int dirtIdx = (int)(Mathf.Clamp(relativeDepth * Random.Range(0.92f, 1.08f) * dirtTiles.Length, 0.1f, dirtTiles.Length - 0.1f));

        dirtMap.SetTile(gridPosition, dirtTiles[dirtIdx]);

        // kazdej tile ma svoje TileData
        float durability = Mathf.Pow(DEFAULT_DURABILITY, (float)(dirtIdx + 1));

        // materialovej overlay
        switch (tileType)
        {
            case TileData.TileType.DIRT:
                shadowMap.SetTile(gridPosition, shadowTile);
                break;
            case TileData.TileType.IRON:
                tileTypeMap.SetTile(gridPosition, ironTile);
                shadowMap.SetTile(gridPosition, shadowTile);
                break;
            case TileData.TileType.COPPER:
                tileTypeMap.SetTile(gridPosition, copperTile);
                shadowMap.SetTile(gridPosition, shadowTile);
                break;
            case TileData.TileType.GOLD:
                tileTypeMap.SetTile(gridPosition, goldTile);
                shadowMap.SetTile(gridPosition, shadowTile);
                break;
            case TileData.TileType.BEDROCK:
                tileTypeMap.SetTile(gridPosition, bedrockTile);
                shadowMap.SetTile(gridPosition, null);
                durability = float.PositiveInfinity;
                break;
            case TileData.TileType.GROUND: // unused
                tileTypeMap.SetTile(gridPosition, groundTile);
                shadowMap.SetTile(gridPosition, null);
                break;
            case TileData.TileType.NEST_BORDER:
                tileTypeMap.SetTile(gridPosition, nestBorderTile);
                shadowMap.SetTile(gridPosition, shadowTile);
                break;
            default:
                break;
        }


        int materialCount = Random.Range(1, 4);
        tileDatas[gridPosition] = new TileData(this, row, col, tileType, durability, materialCount);
    }

    public void RemoveTile(int row, int col)
    {
        Vector3Int gridPosition = RowCol2GridPosition(row, col);

        RemoveTile(gridPosition);
    }

    public void RemoveTile(Vector3Int gridPosition)
    {
        dirtMap.SetTile(gridPosition, null);
        tileTypeMap.SetTile(gridPosition, null);
        crackMap.SetTile(gridPosition, null);

        //odstranim shadow plus sousedni
        shadowMap.SetTile(gridPosition, null);

        //nejak vyresit odstraneni stinu z celeho otevreneho hnizda
        TileData? tile = GetTile(gridPosition);
        if (tile != null && tile.type == TileData.TileType.NEST_BORDER)
        {
            this.RemoveShadowFromEmptyNeighbours(gridPosition);
        }


        List<Vector3Int> neighbours = GetNeighbours(gridPosition);
        foreach (Vector3Int neighbour in neighbours)
            shadowMap.SetTile(neighbour, null);

        tileDatas.Remove(gridPosition);
    }

    public void RemoveShadowFromEmptyNeighbours(Vector3Int gridPosition)
    {
        List<Vector3Int> toRemoveShadow = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        //BFS - Be fast, shithead!

        List<Vector3Int> neighbours = GetNeighbours(gridPosition);
        foreach (Vector3Int neighbour in neighbours)
            queue.Enqueue(neighbour);

        while (queue.Count > 0)
        {
            Vector3Int position = queue.Dequeue();
            if (visited.Contains(position))
                continue;

            visited.Add(position);

            TileData? tile = GetTile(position);
            TileBase tileBase = shadowMap.GetTile(position);

            if ((tile != null && tile.type != TileData.TileType.NEST_BORDER) || tileBase != shadowTile)
            {
                continue;
            }

            toRemoveShadow.Add(position);

            if (tile != null && tile.type == TileData.TileType.NEST_BORDER)
            {
                continue;
            }
            neighbours = GetNeighbours(position);
            foreach (Vector3Int neighbour in neighbours)
                if (!visited.Contains(neighbour))
                    queue.Enqueue(neighbour);
        }


        RemoveShadow(toRemoveShadow);
    }

    /*
    private IEnumerator RemoveShadowCoroutine(List<Vector3Int> positions)
    {
        foreach (Vector3Int position in positions)
        {
            shadowMap.SetTile(position, null);
            yield return new WaitForSeconds(0.003f);
        }
    }*/

    private void RemoveShadow(List<Vector3Int> positions)
    {
        foreach (Vector3Int position in positions)
        {
            shadowMap.SetTile(position, null);
        }
    }

    public List<Vector3Int> GetNeighbours(Vector3Int gridPosition)
    {
        List<Vector3Int> neigbours = new List<Vector3Int>();

        neigbours.Add(new Vector3Int(gridPosition.x-1, gridPosition.y));
        neigbours.Add(new Vector3Int(gridPosition.x+1, gridPosition.y));
        neigbours.Add(new Vector3Int(gridPosition.x, gridPosition.y-1));
        neigbours.Add(new Vector3Int(gridPosition.x, gridPosition.y+1));

        return neigbours;
    }

    public void SpawnEnemies(int row, int col, int count)
    {
        Vector3 GridPositon = RowCol2GridPosition(row, col) + dirtMap.cellSize / 2.0f;

        for (int i = 0; i < count; i++)
        {
            float roll = Random.Range(0.0f, 1.0f);
            if (roll < 0.5f)
            {
                GameObject enemy = Instantiate(RangeEnemyPrefab, position: randomOffsettedPosition(GridPositon, 0.05f), rotation: Quaternion.identity);
                enemy.GetComponent<Enemy>().ScaleByDepth(row);
                optimalization.enemies.Add(enemy);
            }
            else
            {
                GameObject enemy = Instantiate(MeleeEnemyPrefab, position: randomOffsettedPosition(GridPositon, 0.05f), rotation: Quaternion.identity);
                enemy.GetComponent<Enemy>().ScaleByDepth(row);
                
                optimalization.enemies.Add(enemy);
            }
        }
    }

    public void SpawnNestEnemies(int row, int col)
    {
        float coef = GetEnemyCountDepthCoef(row);
        int enemyCount = (int)Random.Range(MIN_ENEMY_COUNT * coef, MAX_ENEMY_COUNT * coef);

        SpawnEnemies(row, col, enemyCount);
    }

    public void SpawnQueenNestEnemies(int row, int col)
    {
        GameObject queen = Instantiate(QueenPrefab, position: RowCol2GridPosition(row, col) + dirtMap.cellSize / 2.0f, rotation: Quaternion.identity);
        queen.GetComponent<Queen>().ScaleByDepth(row);
        optimalization.enemies.Add(queen);

        float coef = GetEnemyCountDepthCoef(row);
        int enemyCount = (int)Random.Range(MIN_QUEEN_ENEMY_COUNT * coef, MAX_QUEEN_ENEMY_COUNT * coef);

        SpawnEnemies(row, col, enemyCount);
    }

    public TileData? GetTile(Vector2 worldPosition)
    {
        Vector3Int gridPosition = dirtMap.WorldToCell(worldPosition);

        if(!tileDatas.ContainsKey(gridPosition))
            return null;

        return tileDatas[gridPosition];
    }

    public TileData? GetTile(Vector3Int gridPosition)
    {
        if (!tileDatas.ContainsKey(gridPosition))
            return null;

        return tileDatas[gridPosition];
    }

    public List<TileData> GetTilesNear(Vector2 worldPosition, float radius)
    {
        /*
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wordPosition, radius, layerMask);
         
        Debug.Log(colliders.Length); // TODO: smazat

        foreach (Collider2D collider in colliders)
        {
            Debug.Log(collider.attachedRigidbody.position);
            Vector2 position = collider.attachedRigidbody.position;
            Vector3Int gridPos = dirtMap.WorldToCell(position);
            tilesInRadius.Add(GetTile(gridPos));
        }

        return tilesInRadius;
        */

        List<TileData> tilesInRadius = new List<TileData>();

        for (int y = (int)(worldPosition.y - Mathf.Ceil(radius * 1.1f)); y <= (int)(worldPosition.y + Mathf.Ceil(radius * 1.1f)); y++)
        {
            for (int x = (int)(worldPosition.x - Mathf.Ceil(radius * 1.1f)); x <= (int)(worldPosition.x + Mathf.Ceil(radius * 1.1f)); x++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);
                if (!tileDatas.ContainsKey(gridPosition))
                    continue;

                TileData tile = GetTile(gridPosition);

                foreach (Vector2 p in pointsAroundTile)
                {
                    Vector2 pointPosition = new Vector2(gridPosition.x, gridPosition.y) + p;

                    if (Vector2.Distance(worldPosition, pointPosition) <= radius)
                    {
                        tilesInRadius.Add(tile);
                        break;
                    }
                }
            }
        }

        return tilesInRadius;

        /*
        for (int y = gridPosition.y - (int)System.Math.Ceiling(radius); y <= gridPosition.y + (int)System.Math.Ceiling(radius); y++)
        {
            for (int x = gridPosition.x - (int)System.Math.Ceiling(radius); x <= gridPosition.x + (int)System.Math.Ceiling(radius); x++)
            {
                Vector3Int gridNearPosition = new Vector3Int(x, y, gridPosition.z);
                if (!tileDatas.ContainsKey(gridNearPosition))
                    continue;

                TileData nearTile = tileDatas[gridNearPosition];
                if (Vector2.Distance(dirtMap.CellToWorld(gridNearPosition) + dirtMap.cellSize / 2.0f, worldPosition) <= radius)
                {
                    tilesInRadius.Add(nearTile);
                }
            }
        }

        return tilesInRadius;*/

        /*
        if (tileDatas.ContainsKey(gridPosition)) {
            TileData center = tileDatas[gridPosition];
            tilesInRadius.Add(center);
            wordPosition = dirtMap.CellToWorld(new Vector3Int(center.col, -center.row));
        }


        for (int y = gridPosition.y - (int)System.Math.Ceiling(radius); y <= gridPosition.y + (int)System.Math.Ceiling(radius); y++)
        {
            for (int x = gridPosition.x - (int)System.Math.Ceiling(radius); x <= gridPosition.x + (int)System.Math.Ceiling(radius); x++)
            {
                Vector3Int gridNearPosition = new Vector3Int(x, y, gridPosition.z);
                if (!tileDatas.ContainsKey(gridNearPosition))
                    continue;

                TileData nearTile = tileDatas[gridNearPosition];
                if(Vector2.Distance(dirtMap.CellToWorld(gridNearPosition) + dirtMap.cellSize / 2.0f, wordPosition) <= radius)
                {
                    tilesInRadius.Add(nearTile);
                }
            }
        }

        return tilesInRadius;*/
    }

    //vraci typ tilu, ktery byl vytezen, jestlize nebyl vytezen, tak vraci null
    public TileData? HitTile(Vector2 position, float damage)
    {
        Vector3Int gridPosition = dirtMap.WorldToCell(position);

        if (!tileDatas.ContainsKey(gridPosition))
            return null;

        TileData tile = tileDatas[gridPosition];

        return HitTile(tile, damage);
    }


    public TileData? HitTile(TileData tileData, float damage)
    { 

        float remainingDurability = tileData.Damage(damage);
        float maxDurability = tileData.maxDurability;
        float relativeDurability = remainingDurability / maxDurability;

        Vector3Int gridPosition = new Vector3Int(tileData.col, -tileData.row);
        Vector2 position = dirtMap.CellToWorld(gridPosition) + dirtMap.cellSize / 2.0f;

        if (relativeDurability <= 0.0f)
        {
            TileData tile = tileData;
            RemoveTile(gridPosition);

            //spawne prefaby
            switch (tile.type)
            {
                case TileData.TileType.IRON:
                    Instantiate(IronPrefab, position, Quaternion.identity);
                    break;
                case TileData.TileType.COPPER:
                    Instantiate(CopperPrefab, position, Quaternion.identity);
                    break;
                case TileData.TileType.GOLD:
                    Instantiate(GoldPrefab, position, Quaternion.identity);
                    break;
            }

            return tile;
        }
        else if (relativeDurability < 0.25f)
        {
            crackMap.SetTile(gridPosition, crackTile3);
        }
        else if (relativeDurability < 0.5f)
        {
            crackMap.SetTile(gridPosition, crackTile2);
        }
        else if (relativeDurability < 0.75f)
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
        currentGameState = GameState.TAKEOFF;
        GameObject.FindGameObjectWithTag("Rocket").GetComponent<Rocket>().TakeOff();

        player.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<Playah>().enabled = false;
    }

    public void GoToWinScreen()
    {
        EndScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        // TODO: smazat
        if (Input.GetKeyDown(KeyCode.H))
        {
            Win();
        }
    }
}
