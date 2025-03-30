using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types.
/// it also includes a "shield" type to allow a shield power-up.
/// Items marked [NI] below are Not Implemented in the IGDPD book.
/// </summary>
public enum eWeaponType{
    none, // The default / no weapon
    blaster, // A simple blaster
    spread, // Two shots simultaneously
    phaser, // [NI] Shots that move in waves
    missile, // [NI] Homing missiles
    laser, // [NI] Damage over time
    shield, //Raises shieldLevel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties
///     of a specific weapon in the Inspector. The Main class has
///     an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition{
    public eWeaponType type = eWeaponType.none;
    public string      letter;
    public Color       color = Color.white;
    public GameObject  prefabProjectile;
    public Color       projectileColor = Color.white;
    public float       damageOnHit = 0;
    public float       continuousDamage = 0;
    public float       delayBetweenShots = 0;
    public float       velocity = 20;
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private eWeaponType     _type = eWeaponType.none;
    public WeaponDefinition def;
    public GameObject       collar;
    public float            lastShotTime;
    
    private Renderer        collarRend;

    private void Start() {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        SetType(_type);

        if(PROJECTILE_ANCHOR == null) {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
    }

    public void Fire(){
        if(!gameObject.activeInHierarchy) return;

        if(Time.time -lastShotTime < def.delayBetweenShots) return;

        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;

        if(transform.up.y < 0) vel.y = -vel.y;
        
        switch (type)
        {
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.rigid.linearVelocity = vel;
                break;

            case eWeaponType.spread:
                p = MakeProjectile();
                p.rigid.linearVelocity = vel;

                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.linearVelocity = p.transform.rotation * vel;

                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.linearVelocity = p.transform.rotation * vel;

                break;
        }
    }

    public Projectile MakeProjectile(){
        GameObject go = Instantiate<GameObject>(def.prefabProjectile);

        if(transform.parent.gameObject.tag == "Hero"){
            go.tag = "HeroProjectile";
            go.layer = LayerMask.NameToLayer("HeroProjectile");
        }
        else{
            go.tag = "EnemyProjectile";
            go.layer = LayerMask.NameToLayer("EnemyProjectile");
        }

        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;

        return p;
    }

    public eWeaponType type {
        get { return _type; }
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt){
        _type = wt;

        if(type == eWeaponType.none) {
            gameObject.SetActive(false);
            return;
        }
        else{
            gameObject.SetActive(true);
        }

        def = ShootEmUp.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0;
    }


}
