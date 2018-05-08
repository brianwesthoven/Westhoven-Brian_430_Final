using UnityEngine;

// each CarSensor is built of three rectangluar colliders, when entering a wall the value is incremented, after the sensor has left the wall the sensors value is decremented.
// the value begins at zero.
public class CarSensorController : MonoBehaviour {

    public uint id; 
    private uint value;
   
	void Start () {
        value = 0;
	}

    public void OnWallEnter() {
        value++;
        transform.parent.GetComponent<CarController>().UpdateSensor(id, value);
    }

    public void OnWallExit() {
        value--;
        transform.parent.GetComponent<CarController>().UpdateSensor(id, value);
    }
}
