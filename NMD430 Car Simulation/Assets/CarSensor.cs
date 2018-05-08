using UnityEngine;

//for documentation, see CarSensorController
public class CarSensor : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Wall") {
            transform.parent.GetComponent<CarSensorController>().OnWallEnter();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Wall") {
            transform.parent.GetComponent<CarSensorController>().OnWallExit();
        }
    }
}
