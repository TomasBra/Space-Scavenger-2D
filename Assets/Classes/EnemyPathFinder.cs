using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class EnemyPathFinder
{
    public static Stack<Vector3> FindPath(Tilemap tilemap, Vector3 startPos, Vector3 endPos)
    {
        PriorityQueue<Vector3Int> open = new PriorityQueue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int?> predecessors = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, float> costs = new Dictionary<Vector3Int, float>();

        open.Enqueue(tilemap.WorldToCell(startPos), 0);
        predecessors[tilemap.WorldToCell(startPos)] = null;
        costs[tilemap.WorldToCell(startPos)] = 0.0f;
        Vector3Int curr;

        while (open.Count > 0)
        { 
            curr = open.Dequeue();

            if (curr == tilemap.WorldToCell(endPos))
            {
                break;
            }

            foreach (Vector3Int neighbor in GetAccessibleNeighbors(tilemap, curr))
            {
                float newCost = costs[curr] + Vector3Int.Distance(curr, neighbor);

                if (!costs.ContainsKey(neighbor) || newCost < costs[neighbor])
                {
                    costs[neighbor] = newCost;
                    int priority = (int)((newCost + Vector3Int.Distance(curr, tilemap.WorldToCell(endPos))) * 10000.0f);
                    open.Enqueue(neighbor, priority);
                    predecessors[neighbor] = curr;
                }
            }
        }

        return ReconstructPath(tilemap, predecessors, endPos);
    }

    private static Stack<Vector3> ReconstructPath(Tilemap tilemap, Dictionary<Vector3Int, Vector3Int?> predecessors, Vector3 endPos)
    {
        Stack<Vector3> path = new Stack<Vector3>();

        if (!predecessors.ContainsKey(tilemap.WorldToCell(endPos)))
        {
            return path;
        }

        Vector3Int? curr = tilemap.WorldToCell(endPos);

        while (curr != null)
        {
            path.Push(tilemap.CellToWorld((Vector3Int)curr) + tilemap.cellSize / 2.0f);
            curr = predecessors[(Vector3Int)curr];
        }

        // to je totiz policko, na kterym prave stoji
        path.Pop();

        return path;
    }

    static private readonly Vector3Int[] directions = new Vector3Int[8]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, -1, 0)
        };

    private static List<Vector3Int> GetAccessibleNeighbors(Tilemap tilemap, Vector3Int pos)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // vertical and horizontal
        for (int i = 0; i < directions.Length; i += 2)
        {
            Vector3Int direction = directions[i];

            Vector3Int neighborPos = pos + direction;

            if (tilemap.GetTile(neighborPos) == null)
            { 
                result.Add(neighborPos);
            }
        }

        // diagonal
        for (int i = 1; i < directions.Length; i += 2)
        {
            Vector3Int direction = directions[i];

            Vector3Int neighborPos = pos + direction;
            Vector3Int blockingPos1 = pos + directions[i - 1];
            Vector3Int blockingPos2 = pos + directions[(i + 1) % directions.Length];

            if (tilemap.GetTile(neighborPos) == null
                && tilemap.GetTile(blockingPos1) == null
                && tilemap.GetTile(blockingPos2) == null)
            {
                result.Add(neighborPos);
            }
        }

        return result;
    }
}
