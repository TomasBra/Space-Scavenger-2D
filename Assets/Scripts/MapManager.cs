using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private TileBase tile;

    private Dictionary<Vector3Int, TileData> tileDatas;

    /*[SerializeField]
    private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }   
        }
    }*/

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            tileDatas[gridPosition].Damage(1);
            
            /*TileBase clickedTile = map.GetTile(gridPosition);

            dataFromTiles[clickedTile].durability += -1;

            print("Durability: " + dataFromTiles[clickedTile].durability);

            if (dataFromTiles[clickedTile].durability <= 0)
            {
                map.SetTile(gridPosition, null);
            }*/
        }
    }
}
