using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileWeaponUpgrade
{
    public abstract void Apply(List<GameObject> projectiles, Vector2 player_positon, Vector2 fire_point_position, Vector2 direction);
    
}
