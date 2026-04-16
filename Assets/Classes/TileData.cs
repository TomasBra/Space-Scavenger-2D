using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileData
{
    public enum TileType
    {
        DIRT,
        BEDROCK,
        GROUND,
        IRON,
        COPPER,
        GOLD,
        NEST_BORDER
    }

    public TileType type;
    public int materialAmount = 1;
    public readonly float maxDurability;
    private float durability;
    private int row, col;

    private MapManager mapManager;

    public TileData(MapManager mapManager, int row, int col, TileType type, float durability, int materialAmount)
    {
        this.mapManager = mapManager;
        this.row = row;
        this.col = col;
        this.type = type;
        this.materialAmount = materialAmount;
        this.maxDurability = durability;
        this.durability = durability;
    }

    public float Damage(float damage)
    {
        durability += -damage;
        if (durability <= 0)
        {
            mapManager.RemoveTile(row, col);
        }

        return durability;
        // TODO: jakoze spravnejsi by bylo ho jeste vymazat z MapManager.tileDatas,
        // aby tam jen tak nesmrdel :)
    }
}
