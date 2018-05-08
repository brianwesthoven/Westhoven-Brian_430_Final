using UnityEngine;

public class CarCollider : MonoBehaviour {
    //The car collider is just an invisible box to check when the car has "failed" by hitting a wall, after a collsion the car is reset to its origin.
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Wall") {
            GetComponent<CarController>().ResetCar();
        }
    }
}
