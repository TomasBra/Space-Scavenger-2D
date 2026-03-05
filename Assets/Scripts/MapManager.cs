using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private Tilemap overlayMap;

    [SerializeField]
    private TileBase tile;

    [SerializeField]
    private TileBase overlayTile;
    [SerializeField]
    private TileBase overlayTile2;
    [SerializeField]
    private TileBase overlayTile3;

    private Dictionary<Vector3Int, TileData> tileDatas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        const int WIDTH = 4;
        const int HEIGHT = 8;

        tileDatas = new Dictionary<Vector3Int, TileData>();

        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                Vector3Int gridPosition = new Vector3Int(i, j, 0);
                map.SetTile(gridPosition, tile);
                tileDatas[gridPosition] = new TileData(map, gridPosition, TileData.TileType.DIRT);
            }
        }
    }

    public void HitTile(Vector2 position, float damage)
    {
        Vector3Int gridPosition = map.WorldToCell(position);
        Debug.Log(gridPosition);

        float remainingDuration = tileDatas[gridPosition].Damage(damage);

        if (remainingDuration <= 0)
        {
            overlayMap.SetTile(gridPosition, null);
        }
        else if (remainingDuration < 2)
        {
            overlayMap.SetTile(gridPosition, overlayTile3);
        }
        else if (remainingDuration < 3)
        {
            overlayMap.SetTile(gridPosition, overlayTile2);
        }
        else if (remainingDuration < 4)
        {
            overlayMap.SetTile(gridPosition, overlayTile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector3Int gridPosition = map.WorldToCell(mousePosition);

        //    float remainingDuration = tileDatas[gridPosition].Damage(1);

        //    if (remainingDuration <= 0) {
        //        overlayMap.SetTile(gridPosition, null);
        //    }
        //    else if (remainingDuration < 2)
        //    {
        //        overlayMap.SetTile(gridPosition, overlayTile3);
        //    }else if(remainingDuration < 3)
        //    {
        //        overlayMap.SetTile(gridPosition, overlayTile2);
        //    }else if (remainingDuration < 4)
        //    {
        //        overlayMap.SetTile(gridPosition, overlayTile);
        //    }
        //}
    }
}
