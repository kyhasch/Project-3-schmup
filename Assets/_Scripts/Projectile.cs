using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer    rend;

    [Header("Set Dynamically")]
    public Rigidbody    rigid;
    [SerializeField]
    private eWeaponType _type;

    private void Awake() 
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update() 
    {
        if(bndCheck.offUp) Destroy(gameObject);    
    }

    /// <summary>
    ///     Sets the _type private field and colors this projectile 
    ///     to match the WeaponDefinition.
    /// </summary>
    /// <param name="etype">
    ///     The eWeaponType to use.    
    /// </param>
    public void SetType(eWeaponType etype) {
        _type = etype;
        WeaponDefinition def = ShootEmUp.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }

    public eWeaponType type {
        get {
            return _type;
        }

        set {
            SetType(value);
        }
    }
}
