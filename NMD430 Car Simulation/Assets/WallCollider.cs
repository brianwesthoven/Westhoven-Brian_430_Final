using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour {

    void OnCollisionEnter(Collision c) {
        Debug.Log(gameObject.name + "has collided with " + c.gameObject.name);
    }
}
