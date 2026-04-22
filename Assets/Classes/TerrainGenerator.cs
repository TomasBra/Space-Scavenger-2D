using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;

public class TempTile
{
    public enum TileType
    {
        EMPTY,
        DIRT,
        BEDROCK,
        GROUND, // unused
        IRON,
        COPPER,
        GOLD,
        NEST_BORDER,
        NEST_CENTER,
        QUEEN,
    }
    
    public int row;
    public int col;
    public TileType type;

    public TempTile(int row, int col, TileType type)
    {
        this.row = row;
        this.col = col;
        this.type = type;
    }
}

public class TerrainGenerator
{
    void Shuffle(List<TempTile> tiles)
    {
        tiles.Sort((x, y) => 
        {
            float roll = (float)random.NextDouble();
            if (roll < 0.5)
            {
                return 1;
            }

            return -1;
        });
    }

    readonly int WIDTH;
    readonly int HEIGHT;

    const int NEST_COUNT = 15;
    const int QUEEN_NEST_COUNT = 5; 

    const int MIN_NEST_SIZE = 120; //50
    const int MAX_NEST_SIZE = 200; // 100
    const int MIN_QUEEN_NEST_SIZE = 230;
    const int MAX_QUEEN_NEST_SIZE = 350;

    const float MIN_NEST_DEPTH = 0.08f;
    const float MIN_QUEEN_NEST_DEPTH = 0.24f;

    char[] representations = { ' ', 'o', '#', '@', '$', '%', '*' };

    const int DEF_MIN_NEST_DIST = 20;

    const float NEST_CORE_RATIO = 0.4f;

    System.Random random = new System.Random();

    List<Tuple<int, int>> nestPositions;

    TempTile[,] tiles;

    public TerrainGenerator(int w, int h)
    {
        WIDTH = w;
        HEIGHT = h;

        tiles = new TempTile[HEIGHT, WIDTH];
    }


    // (0, 1)
    float IronDepthFunc(float depth)
    {
        return depth;
    }

    float GoldDepthFunc(float depth)
    {
        return (float)Math.Cbrt(depth);
    }

    float CopperDepthFunc(float depth)
    {
        return depth * depth * depth;
    }

    float NestDepthFunc(float depth)
    {
        return MIN_NEST_DEPTH + depth * (1 - MIN_NEST_DEPTH);
    }

    float QueenNestDepthFunc(float depth)
    {
        return (float)Math.Cbrt(MIN_QUEEN_NEST_DEPTH + depth * (1 - MIN_QUEEN_NEST_DEPTH));
    }

    /*
    void Print()
    {
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                TempTile.TileType type = tiles[i, j].type;

                Console.Write(representations[type]);
            }
            Console.WriteLine();
        }

        for (int i = 0; i < WIDTH; i++)
        {
            Console.Write("=");
        }
        Console.WriteLine();

        /*
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                int type = tiles[i, j].type;

                Console.Write(GetNeighbors_3x3(i, j, [BORDER]).Count);
            }
            Console.WriteLine();
        }
    }
    */

    Tuple<int, int>[] directions = {
            new Tuple<int, int>(0, 1),
            new Tuple<int, int>(0, -1),
            new Tuple<int, int>(1, 0),
            new Tuple<int, int>(-1, 0),
        };

    List<TempTile> GetNeighbors_Cross(int row, int col, TempTile.TileType[] tileTypes)
    {
        List<TempTile> result = new List<TempTile>();

        foreach (var dir in directions)
        {
            int i = row + dir.Item1;
            int j = col + dir.Item2;

            if (i < 0 || i >= HEIGHT || j < 0 || j >= WIDTH)
            {
                continue;
            }

            if ((tileTypes.Length == 0 && tiles[i, j].type != TempTile.TileType.EMPTY) || tileTypes.Contains(tiles[i, j].type))
            {
                result.Add(tiles[i, j]);
            }
        }

        return result;
    }

    List<TempTile> GetNeighbors_3x3(int row, int col, TempTile.TileType[] tileTypes)
    {
        List<TempTile> result = new List<TempTile>();

        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = col - 1; j <= col + 1; j++)
            {

                if (i < 0 || i >= HEIGHT || j < 0 || j >= WIDTH)
                {
                    continue;
                }

                if (i == row && j == col)
                {
                    continue;
                }

                if ((tileTypes.Length == 0 && tiles[i, j].type != TempTile.TileType.EMPTY) || tileTypes.Contains(tiles[i, j].type))
                {
                    result.Add(tiles[i, j]);
                }
            }
        }

        return result;
    }

    float DistanceFromNearestNest(int startRow, int startCol)
    {
        float minDist = float.PositiveInfinity;

        foreach (var rowCol in nestPositions)
        {
            float dist = (float)Math.Sqrt(Math.Pow(startRow - rowCol.Item1, 2) + Math.Pow(startCol - rowCol.Item2, 2));
            if (dist < minDist)
            {
                minDist = dist;
            }
        }

        return minDist;
    }

    int GetRandomRow(Func<float, float> depthFunc)
    {
        return (int)(depthFunc((float)random.NextDouble()) * HEIGHT);
    }

    bool GenerateNest(bool isQueen)
    {
        int startRow;
        int startCol;
        float distFromNearest;
        bool willRepeat;
        int minNestDist = DEF_MIN_NEST_DIST;
        do
        {
            if (minNestDist < 0)
            {
                return false;
            }

            if (isQueen)
            {
                startRow = GetRandomRow(QueenNestDepthFunc);
            }
            else
            {
                startRow = GetRandomRow(NestDepthFunc);
            }
            startCol = random.Next(0, WIDTH);
            distFromNearest = DistanceFromNearestNest(startRow, startCol);
            if (distFromNearest <= minNestDist)
            {
                minNestDist--;
            }

            // cannot spawn nest right next to another nest or bedrock
            bool isNeighborBad = false;
            List<TempTile> neighbors = GetNeighbors_3x3(startRow, startCol, new TempTile.TileType[2] { TempTile.TileType.BEDROCK, TempTile.TileType.NEST_BORDER });
            if (neighbors.Count > 0)
            {
                isNeighborBad = true;
            }

            // to successfully start a nest:
                // dont neighbor another nest tile or bedrock tile
                // must start in dirt
                // distance from another nest must be enough
            willRepeat = isNeighborBad
                || tiles[startRow, startCol].type != TempTile.TileType.DIRT
                || distFromNearest <= minNestDist;

        } while (willRepeat);

        TempTile[,] tilesBackup = new TempTile[HEIGHT, WIDTH];

        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                // tiles[i, j].visited = false;
                tilesBackup[i, j] = new TempTile(i, j, tiles[i, j].type);
            }
        }

        int count;
        if (isQueen)
        {
            count = random.Next(MIN_QUEEN_NEST_SIZE, MAX_QUEEN_NEST_SIZE);
        }
        else
        {
            count = random.Next(MIN_NEST_SIZE, MAX_NEST_SIZE);
        }

        HashSet<TempTile> visited = new HashSet<TempTile>();

        int firstCount = (int)(count * (1.0f - NEST_CORE_RATIO));
        Queue<TempTile> q = new Queue<TempTile>();
        q.Enqueue(tiles[startRow, startCol]);
        //tiles[startRow, startCol].visited = true;

        while (q.Count > 0 && count > firstCount)
        {
            TempTile curr = q.Dequeue();
            int row = curr.row;
            int col = curr.col;
            curr.type = TempTile.TileType.EMPTY;

            List<TempTile> neighbors = GetNeighbors_Cross(row, col, new TempTile.TileType[1] { TempTile.TileType.DIRT });
            Shuffle(neighbors);

            foreach (var neighbor in neighbors)
            {
                //neighbor.visited = true;
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    q.Enqueue(neighbor);//random.NextSingle());
                }
            }

            count--;
        }

        PriorityQueue<TempTile> pq = new PriorityQueue<TempTile>();
        while (q.Count > 0)
        {
            pq.Enqueue(q.Dequeue(), random.Next());
        }

        while (pq.Count > 0 && count > 0)
        {
            TempTile curr = pq.Dequeue();
            int row = curr.row;
            int col = curr.col;
            curr.type = TempTile.TileType.EMPTY;

            List<TempTile> neighbors = GetNeighbors_Cross(row, col, new TempTile.TileType[1] { TempTile.TileType.DIRT });

            foreach (var neighbor in neighbors)
            {
                // neighbor.visited = true;
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    pq.Enqueue(neighbor, random.Next());
                }
            }

            //Print();
            //Console.ReadLine();

            count--;
        }

        if (count > 0)
        {
            tiles = tilesBackup;
            return false;
        }

        while (pq.Count > 0)
        {
            TempTile curr = pq.Dequeue();
            List<TempTile> neighbors = GetNeighbors_3x3(curr.row, curr.col, new TempTile.TileType[0] { });
            bool willEmpty = true;
            foreach (TempTile neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    curr.type = TempTile.TileType.NEST_BORDER;
                    willEmpty = false;
                }
            }

            if (willEmpty)
            {
                curr.type = TempTile.TileType.EMPTY;
            }
        }

        nestPositions.Add(new Tuple<int, int>(startRow, startCol));

        if (isQueen)
        {
            tiles[startRow, startCol].type = TempTile.TileType.QUEEN;
        }
        else
        {
            tiles[startRow, startCol].type = TempTile.TileType.NEST_CENTER;
        }
            return true;
    }

    void Init()
    {
        nestPositions = new List<Tuple<int, int>>();

        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (i == 0 || i == HEIGHT - 1 || j == 0 || j == WIDTH - 1)
                {
                    tiles[i, j] = new TempTile(i, j, TempTile.TileType.NEST_BORDER);
                }
                else
                {
                    tiles[i, j] = new TempTile(i, j, TempTile.TileType.DIRT);
                }
            }
        }
    }

    void GenerateBedrockBorders()
    {
        for (int col = 0; col < WIDTH; col++)
        {
            tiles[0, col] = new TempTile(0, col, TempTile.TileType.BEDROCK);
            tiles[HEIGHT - 1,col] = new TempTile(HEIGHT - 1, col, TempTile.TileType.BEDROCK);
        }

        for (int row = 0; row < HEIGHT; row++)
        {
            tiles[row, 0] = new TempTile(row, 0, TempTile.TileType.BEDROCK);
            tiles[row, WIDTH - 1] = new TempTile(row, WIDTH - 1, TempTile.TileType.BEDROCK);
        }
    }

    void GenerateNests(int nestCount, bool isQueen)
    {
        for (int i = 0; i < nestCount; i++)
        {
            GenerateNest(isQueen);
        }
    }

    Tuple<int, int> FindTile(TempTile.TileType tileType, Func<float, float> depthFunc)
    {
        int startRow;
        int startCol;
        do
        {
            float depth = depthFunc((float)random.NextDouble());
            startRow = (int)(depth * HEIGHT);
            startCol = random.Next(0, WIDTH);

        } while (tiles[startRow, startCol].type != tileType);

        return new Tuple<int, int>(startRow, startCol);
    }

    void GenerateOre(TempTile.TileType tileType, int oreCount, int minCluster, int maxCluster, Func<float, float> depthFunc)
    {
        int countLeft = oreCount;

        while (countLeft > 0)
        {
            int currCluster = random.Next(minCluster, maxCluster + 1);

            int tryCount = 5;
            Tuple<int, int> startRowCol;
            do
            {
                startRowCol = FindTile(TempTile.TileType.DIRT, depthFunc);

                tryCount--;
            }
            while (tryCount > 0
                && GetNeighbors_Cross(startRowCol.Item1, startRowCol.Item2, new TempTile.TileType[1] { tileType }).Count > 0);

            HashSet < TempTile > visited = new HashSet<TempTile>();
            PriorityQueue<TempTile> pq = new PriorityQueue<TempTile>();
            pq.Enqueue(tiles[startRowCol.Item1, startRowCol.Item2], 0);

            while (pq.Count > 0 && currCluster > 0)
            {
                TempTile curr = pq.Dequeue();
                int row = curr.row;
                int col = curr.col;
                curr.type = tileType;

                List<TempTile> neighbors = GetNeighbors_Cross(row, col, new TempTile.TileType[1] { TempTile.TileType.DIRT });

                foreach (var neighbor in neighbors)
                {
                    //neighbor.visited = true;
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        pq.Enqueue(neighbor, random.Next());
                    }
                }

                currCluster--;
                countLeft--;
            }
        }
    }

    void PolishBorders()
    {
        int polishCount = 0;
        for (int i = 1; i < HEIGHT - 1; i++)
        {
            for (int j = 1; j < WIDTH - 1; j++)
            {
                TempTile curr = tiles[i, j];
                if (curr.type != TempTile.TileType.NEST_BORDER)
                {
                    continue;
                }

                for (int k = 0; k < polishCount; k++)
                {
                    List<TempTile> borderNeighbors = GetNeighbors_3x3(i, j, new TempTile.TileType[1] { TempTile.TileType.NEST_BORDER });
                    List<TempTile> universalNeighbors = GetNeighbors_3x3(i, j, new TempTile.TileType[0] { });

                    if (borderNeighbors.Count <= 2 &&
                        universalNeighbors.Count - borderNeighbors.Count < 1)
                    {
                        curr.type = TempTile.TileType.EMPTY;
                    }
                }
            }
        }
    }

    public TempTile[,] GenerateTerrain()
    {
        Init();

        //Nemìnit poøadí
        GenerateNests(QUEEN_NEST_COUNT, true);
        GenerateNests(NEST_COUNT, false);
        //PolishBorders();

        GenerateOre(TempTile.TileType.IRON, 250, 2, 3, IronDepthFunc);
        GenerateOre(TempTile.TileType.COPPER, 225, 1, 4, CopperDepthFunc);
        GenerateOre(TempTile.TileType.GOLD, 200, 1, 3, GoldDepthFunc);

        GenerateBedrockBorders();

        //Print();

        return tiles;
    }
}
