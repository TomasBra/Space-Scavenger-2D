using UnityEngine;

public class Health : GameObject2D
{
    /*
    Pøidá potomkovi životy a celkovì funkènost s nimi.
    Jedná se o potomka herního objektu 2D
    */

    [SerializeField]
    public float maxHP = 3f;

    //[HideInInspector]
    public float HP;


    public void Start()
    {
        base.Start();   
        HP = maxHP;
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
    }

    //vrací true pokud objekt má <= 0 životù
    public virtual bool TakeDamage(float damage, Vector2? knockbackDirection = null, bool destroyable = true)
    {
        HP -= damage;
        if (knockbackDirection != null && rigidbody != null)
        {
            knockbackDirection.Value.Normalize();
            
            rigidbody.AddForce(knockbackDirection.Value * 10f);
        }

        if (HP <= 0)
        {
            if(destroyable)
                Destroy(this.gameObject);
            return true;
        }

        return false;
    }
}
