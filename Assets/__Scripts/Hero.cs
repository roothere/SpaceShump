using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public static Hero S;

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;

    private GameObject lastTriggerGo = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private void Start() {
        if (S == null) {
            S = this;
        } else {
            Debug.LogError("Hero.Awake() - Попытка создать второй экземпляр Hero.S!");
        }
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    private void Update() {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
            fireDelegate();
        }

/*      if (Input.GetKeyDown(KeyCode.Alpha1)) Main.S.SpawnCheatPowerUp(WeaponType.blaster);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Main.S.SpawnCheatPowerUp(WeaponType.spread);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Main.S.SpawnCheatPowerUp(WeaponType.fan);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Main.S.SpawnCheatPowerUp(WeaponType.laser);
        if (Input.GetKeyDown(KeyCode.Alpha5)) Main.S.SpawnCheatPowerUp(WeaponType.phaser);
        if (Input.GetKeyDown(KeyCode.Alpha0)) Main.S.SpawnCheatPowerUp(WeaponType.shield);
        if (Input.GetKeyDown(KeyCode.R)) Main.S.Restart();
        if (Input.GetKeyDown(KeyCode.T)) Main.S.SpawnCheatEnemy();*/
    }

    private void OnTriggerEnter(Collider other) {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        if (go == lastTriggerGo) {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "Enemy") {
            shieldLevel--;
            Destroy(go);
        } else if (go.tag == "PowerUp") {
            AbsorbPowerUp(go);
        } else {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type) {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pu.type == weapons[0].type) {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null) {
                        w.SetType(pu.type);
                    }
                } else {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel {
        get {
            return (_shieldLevel);
        }
        set {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0) {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        int maxWeapons = Main.GetWeaponDefinition(weapons[0].type).maxWeapons;
        for (int i = 0; i < weapons.Length; i++) {
            if (weapons[i].type == WeaponType.none && maxWeapons > i) {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons() {
        foreach (Weapon w in weapons) {
            w.SetType(WeaponType.none);
        }
    }
}
