using System.Collections;

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.25f;
    public float enemyDefaultPadding = 1.5f;

    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {
           WeaponType.blaster, WeaponType.blaster,
           WeaponType.spread, WeaponType.shield };

    private BoundsCheck bndCheck;

    public void ShipDestroyed(Enemy e) {
        if (Random.value <= e.powerUpDropChance) {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);
            pu.transform.position = e.transform.position;
        }
    }

/*    public void SpawnCheatPowerUp(WeaponType wT)
    {
        WeaponType puType = wT;

        GameObject go = Instantiate(prefabPowerUp) as GameObject;
        PowerUp pu = go.GetComponent<PowerUp>();

        pu.SetType(puType);
        pu.transform.position = Hero.S.transform.position;
    }*/

    void Awake() {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions) {
            WEAP_DICT[def.type] = def;
        }
    }
    
    public void SpawnEnemy() {

        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeigth + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

/*    public void SpawnCheatEnemy() {

        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeigth + enemyPadding;
        go.transform.position = pos;

    }*/

    public void DelayedRestart(float delay) {
        Invoke("Restart", delay);
    }

    public void Restart() {
        SceneManager.LoadScene("_Scene_0");
    }
/// <summary>
/// —татическа€ функци€, возвращающа€ WeaponDefinition из статического
/// защищенного пол€ WEAP_DICT класса Main.
/// </summary>
/// 
/// <param name="wt">
/// “ип оружи€ WeaponType, дл€ которого требуетс€ получить 
/// WeaponDefinition.
/// </param>
/// 
/// <returns>
/// Ёкземпл€р WeaponDefinition или, если нет такого определени€ дл€ указанного
/// WeaponType, возвращает новый экземпл€р WeaponDefinition с типом none.
/// </returns>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt) {
        if (WEAP_DICT.ContainsKey(wt)) {
            return (WEAP_DICT[wt]);
        }
        return (new WeaponDefinition());
    }
}
