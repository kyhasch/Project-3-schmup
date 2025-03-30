using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    [Header("Set in Inspector")]
    public float       speed = 30f;
    public float       rollMult = -45f;
    public float       pitchMult = 30f;

    public Weapon[]    weapons;

    [Header("Set Dynamically")]
    [SerializeField]
    private float       _shieldLevel = 1;
    private GameObject  lastTriggerGo = null;
    public GameObject   prefabProjectile;
    
    private void Start()
    {
        if(S == null) S = this;
        else Debug.LogError("Here.Awake() - Attempted to assign second Hero.S");

        ClearWeapons();
        weapons[0].SetType(eWeaponType.blaster);
    }

    private void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if(Input.GetAxis("Jump") == 1 && fireDelegate != null)
            fireDelegate();
    }

    void OnTriggerEnter(Collider other) {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) return;

        lastTriggerGo = go;

        if(go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp") 
            AbsorbPowerUp(go);
        else 
            print("Triggered by non-Enemy: " + go.name);
    }

    public float shieldLevel
    {
        get
        {
            return _shieldLevel;
        }
        set
        {
            _shieldLevel = Mathf.Min(value,4);

            if(value < 0)
            {
                Destroy(gameObject);
                ShootEmUp.SEU.DelayedRestart();
            }

        }
    }

    Weapon GetEmptyWeaponSlot(){
        for (int i = 0; i < weapons.Length; i++){
            if(weapons[i].type == eWeaponType.none)
                return weapons[i];
        }

        return null;
    }

    void ClearWeapons(){
        foreach (Weapon w in weapons)
            w.SetType(eWeaponType.none);
    }

    public void AbsorbPowerUp(GameObject go){
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case eWeaponType.shield:
                shieldLevel++;
                break;
            
            default:
                if(pu.type == weapons[0].type){
                    Weapon w = GetEmptyWeaponSlot();
                    if(w != null)
                        w.SetType(pu.type);
                }
                else{
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }

        pu.AbsorbedBy(gameObject);
    }
}
