using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int   score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f;

    [Header("Set Dynamically: Enamy")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;

    protected BoundsCheck bndCheck;

    void Awake() 
    {
        bndCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
            originalColors[i] = materials[i].color;
    }

    void Update() 
    {
        Move();

        if(showingDamage && Time.time > damageDoneTime) UnShowDamage();

        if (bndCheck != null && bndCheck.offDown) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other) {
        GameObject otherGO = other.gameObject;
        
        switch (otherGO.tag)
        {
            case "HeroProjectile":
                Projectile p = otherGO.GetComponent<Projectile>();
                if(!bndCheck.isOnScreen){
                    Destroy(otherGO);
                    break;
                }

                ShowDamage();
                health -= ShootEmUp.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    if(!notifiedOfDestruction) ShootEmUp.SEU.ShipDestroyed(this);
                    
                    notifiedOfDestruction = true;
                    
                    Destroy(gameObject);
                }

                Destroy(otherGO);
                break;

            default:
                print("Enemy hit by non-HeroProjectile: " + otherGO.name);
                break;
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void ShowDamage() {
        foreach (Material m in materials)
            m.color = Color.red;

        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage() {
        for (int i = 0; i < materials.Length; i++)
            materials[i].color = originalColors[i];

        showingDamage = false;
    }

    public Vector3 pos 
    {
        get 
        {
            return transform.position;
        }

        set 
        {
            transform.position = value;
        }
    }
}
