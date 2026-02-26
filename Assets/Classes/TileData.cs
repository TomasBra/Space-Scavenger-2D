using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData
{
    public enum TileType
    {
        DIRT
    }

    private TileType type;
    private float durability;
    private Vector3Int gridPosition;

    private Tilemap map;

    public TileData(Tilemap map, Vector3Int gridPosition, TileType type)
    {
        this.map = map;
        this.gridPosition = gridPosition;
        this.type = type;

        switch (type)
        {
            case TileType.DIRT:
                durability = 3;
                break;
        }
    }

    public void Damage(float damage)
    {
        durability += -damage;
        if (durability <= 0)
        {
            map.SetTile(gridPosition, null);
        }

        // TODO: jakoze spravnejsi by bylo ho jeste vymazat z MapManager.tileDatas,
        // aby tam jen tak nesmrdel :)
    }
}
