using System;
using System.Collections;
using UnityEngine;

public class Health : GameObject2D
{
    [SerializeField]
    protected AnimationClip DeathAnimClip;

    [SerializeField]
    public float maxHP = 3f;

    [HideInInspector]
    public float HP;

    [HideInInspector]
    public bool dead;

    [HideInInspector]
    public Material material;

    [SerializeField]
    private float FLASH_TIME = 0.5f;

    private Coroutine _damageFlashCoroutine;
    private float _lastDamageTime;
    private bool _isFlashing;

    public float death_offset;

    public void Start()
    {
        base.Start();
        HP = maxHP;
        material = GetComponent<SpriteRenderer>().material;

        if(DeathAnimClip != null )
            death_offset = DeathAnimClip.length;
        else
            death_offset = 0;
    }

    public void Update()
    {
        base.Update();

        if (dead)
        {
            Destroy(this.GetComponent<BoxCollider2D>());
            Destroy(this.GetComponent<CircleCollider2D>());
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public virtual bool TakeDamage(float damage, Vector2? knockbackDirection = null, bool destroyable = true)
    {
        if (dead)
            return true;
        _lastDamageTime = Time.time;

        if (!_isFlashing)
        {
            _damageFlashCoroutine = StartCoroutine(DamageFlasherLoop());
        }

        HP -= damage;

        if (knockbackDirection != null && rigidbody != null)
        {
            Vector2 dir = knockbackDirection.Value.normalized;
            rigidbody.AddForce(dir * 10f);
        }

        if (HP <= 0 && !dead)
        {
            SetFlashAmount(0); // WARN: tohle jsem pripsal
            StopAllCoroutines();
            dead = true;

            if (destroyable)
            {
                SetAnimatorTrigger("Dead");
                this.Invoke(() => Destroy(transform.gameObject), death_offset);
            }
            return true;
        }

        return false;
    }

    private IEnumerator DamageFlasherLoop()
    {
        _isFlashing = true;

        while (true)
        {
            float cycleStartDamageTime = _lastDamageTime;

            float elapsedTime = 0f;
            while (elapsedTime < FLASH_TIME)
            {
                elapsedTime += Time.deltaTime;

                float currentFlashAmount = Mathf.Lerp(1f, 0f, elapsedTime / FLASH_TIME);
                SetFlashAmount(currentFlashAmount);

                yield return null;
            }

            SetFlashAmount(0f);

            // Pokud bìhem tohoto cyklu pøišel další damage,
            // pustíme další celý cyklus
            if (_lastDamageTime <= cycleStartDamageTime)
            {
                break;
            }
        }

        _isFlashing = false;
        _damageFlashCoroutine = null;
    }

    public void SetFlashAmount(float amount)
    {
        material.SetFloat("_FlashAmount", amount);
    }
}