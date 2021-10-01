using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    private BoundsCheck     bndCheck;
    private Renderer        rend;

    [Header("Set Dynamically")]
    public Rigidbody        rigid;
    public int              leftRight;

    public float            birthTime;
    private Vector2         xStart;

    [SerializeField]
    private WeaponType      _type;

    public Vector3 pos {
        get {
            return (this.transform.position);
        }
        set {
            this.transform.position = value;
        }
    }
    public WeaponType type {
        get {
            return (_type);
        }
        set {
            SetType(value);
        }
    }
    private void Awake() {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (type == WeaponType.phaser || type == WeaponType.laser) {
            birthTime = Time.time;
            xStart.x = transform.position.x;
        }
    }

    private void Update() {
        if (bndCheck.offUp) {
            Destroy(gameObject);
        }
        if (type == WeaponType.laser) ChangeLaserBeam(xStart, birthTime);
        if (type == WeaponType.phaser) MoveSinProjectile(xStart, birthTime);
    }

    public void MoveSinProjectile(Vector2 xStart, float birthTime) {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * age;
        float sin = Mathf.Sin(theta) * leftRight;
        tempPos.x = xStart.x + 10*sin;
        pos = tempPos;
    }

    public void ChangeLaserBeam(Vector2 xStart, float birthTime) {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        pos = tempPos;

    }
    /// <summary>
    /// Изменяет скрытое поле _type и устанавливает цвет этого снаряда,
    /// как определено в WeaponDefinition.
    /// </summary>
    /// 
    /// <param name="eType">
    /// Тип WeaponType используемого оружия.
    /// </param>
    public void SetType(WeaponType eType) {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}
