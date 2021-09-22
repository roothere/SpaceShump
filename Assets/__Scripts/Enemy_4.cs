using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ётот класс - сериализуемый класс подобно WeaponDefinition, предназначенный дл€ хранени€ данных
/// </summary>
[System.Serializable]
public class Part {
    public string           name;
    public float            health;
    public string[]         protectedBy;

    //[HideInInspector]
    public GameObject       go;
    [HideInInspector]
    public Material         mat;
}

/// <summary>
/// ¬раг создаетс€ за верхней границей, выбирает случайную точку на экране и перемещаетс€ к ней.
/// ƒобравшись до места, выбирает другую случайную точку и продолжает двигатьс€,
/// пока игрок не уничтожит его.
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[]           parts;

    private Vector3         p0, p1;
    private float           timeStart;
    private float           duration = 4;

    void Start() {
        p0 = p1 = pos;

        InitMovement();

        Transform t;
        foreach (Part prt in parts) {
            t = transform.Find(prt.name);
            if (t != null) {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement() {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeigth - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;
    }

    public override void Move() {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1) {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    Part FindPart(string n) { // а
        foreach (Part prt in parts) {
            if (prt.name == n) {
                return (prt);
            }
        }
        return (null);
    }
    Part FindPart(GameObject go) { // b
        foreach (Part prt in parts) {
            if (prt.go == go) {
                return (prt);
            }
        }
        return (null);
    }
    // Ёти функции возвращают true, если данна€ часть уничтожена
    bool Destroyed(GameObject go) { // с
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n) {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt) {
        if (prt == null) { // ≈сли ссылка на часть не была передана
            return (true); // ¬ернуть true (то есть: да, была уничтожена)
        }
        // ¬ернуть результат сравнени€: prt.health <= 0
        // ≈сли prt.health <= 0, вернуть true (да, была уничтожена)
        return (prt.health <= 0);
    }
    // ќкрашивает в красный только одну часть, а не весь корабль.
    void ShowLocalizedDamage(Material m) { // d
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }


    void OnCollisionEnter(Collision coll) {
        GameObject other = coll.gameObject;
        switch (other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();

                if (!bndCheck.isOnScreen) {
                    Destroy(other);
                    break;
                }

                GameObject goHit = coll.GetContact(0).thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null) {
                    goHit = coll.GetContact(0).otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                if (prtHit.protectedBy != null) {
                    foreach (string s in prtHit.protectedBy) {
                        if (!Destroyed(s)) {
                            Destroy(other);
                            return;
                        }
                    }
                }
                if (p.type == WeaponType.laser) {
                    prtHit.health -= (Time.time - p.birthTime) * Main.GetWeaponDefinition(p.type).continuousDamage;
                    ShowLocalizedDamage(prtHit.mat);
                }
                else {
                    prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                    ShowLocalizedDamage(prtHit.mat);
                }

                if (prtHit.health <= 0) { 
                    prtHit.go.SetActive(false);
                }
                
                bool allDestroyed = true; 
                foreach (Part prt in parts) {
                    if (!Destroyed(prt)) { 
                        allDestroyed = false; 
                        break; 
                    }
                }
                if (allDestroyed) { 
                                    
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }
}
