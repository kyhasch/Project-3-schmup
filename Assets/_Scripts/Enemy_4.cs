using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Part is another serializable data storage class just like WeaponDefinition.
/// </summary>
[System.Serializable]
public class Part {
    public string   name;
    public float    health;
    public string[] protectedBy;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material   mat;
}
/// <summary>
///     Enemy_4 will start offscreen and then pick a random point on screen to
///     move to. Once it has arrived, it will pick another random point and 
///     continue until the player has shot it down.
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;
    private Vector3 p0, p1;
    private float   timeStart;
    private float   duration = 4f;

    private void Start() {
        p0 = p1 = pos;
        InitMovement();

        Transform t;
        foreach (Part p in parts)
        {
            t = transform.Find(p.name);
            if(t != null){
                p.go = t.gameObject;
                p.mat = p.go.GetComponent<Renderer>().material;
            }
        }
    }

    private void OnCollisionEnter(Collision coll) {
        GameObject other = coll.gameObject;

        switch(other.tag){
            case "HeroProjectile":
                Projectile p = other.GetComponent<Projectile>();

                if(!bndCheck.isOnScreen){
                    Destroy(other);
                    break;
                }

                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part partHit = FindPart(goHit);

                if(partHit == null){
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    partHit = FindPart(goHit);
                }

                if(partHit.protectedBy != null){
                    foreach (string s in partHit.protectedBy)
                    {
                        if(!Destroyed(s)){
                            Destroy(other);
                            return;
                        }
                    }
                }

                partHit.health -= ShootEmUp.GetWeaponDefinition(p.type).damageOnHit;

                ShowLocalizedFamage(partHit.mat);

                if(partHit.health <= 0) partHit.go.SetActive(false);

                bool allDestroyed = true;
                foreach (Part part in parts)
                {
                    if(!Destroyed(part)){
                        allDestroyed = false;
                        break;
                    }
                }

                if(allDestroyed){
                    ShootEmUp.SEU.ShipDestroyed(this);
                    Destroy(gameObject);
                }

                Destroy(other);
                break;
        }
    }

    void InitMovement() {
        p0 = p1;

        float widthMinRad = bndCheck.camWidth - bndCheck.radius;
        float heightMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widthMinRad, widthMinRad);
        p1.y = Random.Range(-heightMinRad, heightMinRad);

        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if(u >= 1){
            InitMovement();
            u = 0;
        }

        u = Utils.Ease(u, Utils.EasingType.easeOut);
        pos = (1 - u) * p0 + u * p1;
    }
    
    Part FindPart(string n){
        foreach (Part p in parts)
            if(p.name == n) return p;

        return null;
    }

    Part FindPart(GameObject go){
        foreach (Part p in parts)
            if(p.go == go) return p;

        return null;
    }

    bool Destroyed(GameObject go){
        return Destroyed(FindPart(go));
    }

    bool Destroyed(string n){
        return Destroyed(FindPart(n));
    }

    bool Destroyed(Part part){
        if(part == null) return true;

        return (part.health <= 0);
    }

    void ShowLocalizedFamage(Material m){
        m.color = Color.red;

        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }
}
