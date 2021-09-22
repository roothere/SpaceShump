using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Предотвращает выход игрового объекта за границы экрана
/// Работает только с ортографической камерой MainCamera в [0, 0, 0]
///</summary>

public class BoundsCheck : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Set Dynamically")]
    public bool isOnScreen = true;
    public float camWidth;
    public float camHeigth;

    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown;

    void Awake() {
        camHeigth = Camera.main.orthographicSize;
        camWidth = camHeigth * Camera.main.aspect;

        //print(camHeigth);
        //print(camWidth);
    }

    void LateUpdate() {
        Vector3 pos = transform.position;
        isOnScreen = true;
        offRight = offLeft = offUp = offDown = false;

        if (pos.x > camWidth - radius) {
            pos.x = camWidth - radius;
            offRight = true;
        }
        if (pos.x < -camWidth + radius) {
            pos.x = -camWidth + radius;
            offLeft = true;
        }

        if (pos.y > camHeigth - radius) {
            pos.y = camHeigth - radius;
            offUp = true;
        }
        if (pos.y < -camHeigth + radius) {
            pos.y = -camHeigth + radius;
            offDown = true;
        }

        isOnScreen = !(offDown || offLeft || offUp || offRight);
        if (keepOnScreen && !isOnScreen) {
            transform.position = pos;
            isOnScreen = true;
            offDown = offLeft = offUp = offRight = false;
        }
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(camWidth * 2, camHeigth * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
