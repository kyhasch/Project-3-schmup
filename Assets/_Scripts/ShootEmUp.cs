using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShootEmUp : MonoBehaviour
{
    static public ShootEmUp SEU;
    static protected Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[]       prefabEnemies;
    public float              enemySpawnPerSecod = 0.5f;
    public float              enemyDefaultPadding = 1.5f;
    public float              gameRestartDelay = 2f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject         prefabPowerUp;
    public eWeaponType[]      powerUpFrequency = new eWeaponType[] { eWeaponType.blaster,
                                                                     eWeaponType.blaster,
                                                                     eWeaponType.spread,
                                                                     eWeaponType.shield
                                                                   };

    private BoundsCheck       bndCheck;

    private void Awake() 
    {
        SEU = this;
        
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecod);

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition wpDef in weaponDefinitions)
            WEAP_DICT[wpDef.type] = wpDef;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        
        if (go.GetComponent<BoundsCheck>() != null) 
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f/enemySpawnPerSecod);
    }

    /// <summary>
    ///     Static function that gets a WeaponDefinition from the WEAP_DICT static
    ///     protected field of the ShootEmUp Class.
    /// </summary>
    /// <param name="wt">
    ///     The eWeaponType of the desired WeaponDefinition.
    /// </param>
    /// <returns> 
    ///     The WeaponDefinition or, if there is no WeaponDefinition with the WeaponType
    ///     passed in, return a new WeaponDefinition with a WeaponType of none.
    /// </returns>
    static public WeaponDefinition GetWeaponDefinition(eWeaponType wt)
    {
        if(WEAP_DICT.ContainsKey(wt)) return WEAP_DICT[wt];

        return new WeaponDefinition();
    }

    public void ShipDestroyed(Enemy e){
        if(true){
            int ndx = Random.Range(0, powerUpFrequency.Length);
            eWeaponType puType = powerUpFrequency[ndx];
            
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);
            pu.transform.position = e.transform.position;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }
    public void DelayedRestart()
    {
        Invoke("Restart", gameRestartDelay);
    }

}