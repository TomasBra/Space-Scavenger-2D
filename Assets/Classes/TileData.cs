using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData
{
    public enum TileType
    {
        DIRT,
        BEDROCK,
        IRON,
        COPPER,
        GOLD,
        NEST_BORDER
    }

    private TileType type;
    private float durability;
    private int row, col;

    private MapManager mapManager;

    public TileData(MapManager mapManager, int row, int col, TileType type)
    {
        this.mapManager = mapManager;
        this.row = row;
        this.col = col;
        this.type = type;

        /*smazat*/
        durability = 4;

        /*
        switch (type)
        {
            case TileType.DIRT:
                durability = 4;
                break;
            default:
                throw new System.Exception("PEPA UTOCI!!!");
                break;
        }*/


    }

    public float Damage(float damage)
    {
        durability += -damage*Time.deltaTime;
        if (durability <= 0)
        {
            mapManager.RemoveTile(row, col);
        }

        return durability;
        // TODO: jakoze spravnejsi by bylo ho jeste vymazat z MapManager.tileDatas,
        // aby tam jen tak nesmrdel :)
    }
}
