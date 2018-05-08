/**
 * Title: NMD430 Final
 * Name: Brian Westhoven
 * Date: 2018/05/01 (yyyy,mm,dd)
 * Description: Simulation of a self driving car. The user can switch between a computer and themself using the wasd keys.
 * "Traits" for the car simulation can be randomized, or be a set that works for must of the track.
 */

using UnityEngine;

public class CarController : MonoBehaviour {
    // the cars default position
    private static readonly Vector3 origin = new Vector3(0, 1, 0);

    // constants for manual control
    private const float MAX_ANGLE  = 30;
    private const float MAX_TORQUE = 300;

    // the range a trait value can be: from n to -n
    private const float TORQUE_ANGLE_WEIGHT = 20.0f;
    private const float TORQUE_ANGLE_MULTIPLER = 2.0f;
    private const float TORQUE_CENTER_WEIGHT = 15.0f;
    private const float TORQUE_CENTER_MULTIPLER = 10.0f;

    // the default trait values used when random values are not being used
    private const float DEFUALT_TORQUE_ANGLE_WEIGHT = 10.0f;
    private const float DEFUALT_TORQUE_ANGLE_MULTIPLER = 1.5f;
    private const float DEFUALT_TORQUE_CENTER_WEIGHT = 5.0f;
    private const float DEFUALT_TORQUE_CENTER_MULTIPLER = 8.0f;

    // the trait values
    private float torqueAngleWeight;
    private float torqueAngleMultipler;
    private float torqueCenterWeight;
    private float torqueCenterMultipler;
    // unfortunatly I couldn't find a method of resizing the sensors each round
    private float sensorAngleLength;
    private float sensorCenterLength;

    // used to keep score of the best car
    private float score;
    private float highScore;

    private uint[] sensors;

    public bool manualDrive;
    public bool randomTraits;

	public GameObject wheelShape;
    private WheelCollider[] wheels;

    // here we find all the WheelColliders down in the hierarchy
    public void Start() {
        SetTraits();

        sensors = new uint[] { 0, 0, 0 }; //Left, Middle, Right

        wheels = GetComponentsInChildren<WheelCollider>();

		for (int i = 0; i < wheels.Length; ++i) {
			var wheel = wheels [i];

			// create wheel shapes only when needed
			if (wheelShape != null) {
				var ws = GameObject.Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;
			}
		}
	}

	// this is a really simple approach to updating wheels
	// here we simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero
	// this helps us to figure our which wheels are front ones and which are rear
	public void Update()
	{
        float angle, torque;

        //if its manual drive angle and torque are based on the wasd keys
        if (manualDrive) { 
            angle = MAX_ANGLE * Input.GetAxis("Horizontal");
            torque = MAX_TORQUE * Input.GetAxis("Vertical");
        //if its not manual drive, angle and torque are based on the sensors + the traits
        } else {
            angle = torqueAngleWeight * ((int)sensors[0] - (int)sensors[2]) * torqueAngleMultipler;
            torque = MAX_TORQUE / torqueCenterWeight + (float)sensors[1] * torqueCenterMultipler;
        }

        foreach (WheelCollider wheel in wheels) {
            // a simple car where front wheels steer while rear ones drive
            if (wheel.transform.localPosition.z > 0)
                wheel.steerAngle = angle;

            if (wheel.transform.localPosition.z < 0)
                wheel.motorTorque = torque;

            // update visual wheels if any
            if (wheelShape) {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                // assume that the only child of the wheelcollider is the wheel shape
                Transform shapeTransform = wheel.transform.GetChild(0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
        }
	}

    public void OnGUI() {
        GUILayout.BeginVertical();
        GUILayout.Label("Sensors: ");
        GUILayout.Label("Left:   " + sensors[0].ToString());
        GUILayout.Label("Center: " + sensors[1].ToString());
        GUILayout.Label("Right:  " + sensors[2].ToString());
        GUILayout.Label("Velocity: ");
        GUILayout.Label(GetComponent<Rigidbody>().velocity.ToString());
        manualDrive = GUILayout.Toggle(manualDrive, "Manual Drive");

        if (manualDrive == false) {
            randomTraits = GUILayout.Toggle(randomTraits, "Random Traits");
            GUILayout.Label("Traits:");
            GUILayout.Label("angle weight:     " + torqueAngleWeight.ToString());
            GUILayout.Label("angle multipler:  " + torqueAngleMultipler.ToString());
            GUILayout.Label("center weight:    " + torqueCenterWeight.ToString());
            GUILayout.Label("center multipler: " + torqueCenterWeight.ToString());
        }

        GUILayout.EndVertical();
    }

    public void ResetCar() {
        //return the car to the origin, reset its angle
        gameObject.transform.position = origin;
        gameObject.transform.rotation = Quaternion.identity;

        //set the velocity of the rigidbody back to 0
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();

        //reset the traits, if the should change
        SetTraits();
    }

    private void SetTraits() {
        if (randomTraits == true) {
            torqueAngleWeight = Random.Range(-TORQUE_ANGLE_WEIGHT, TORQUE_ANGLE_WEIGHT);
            torqueAngleMultipler = Random.Range(-TORQUE_ANGLE_MULTIPLER, TORQUE_ANGLE_MULTIPLER);
            torqueCenterWeight = Random.Range(-TORQUE_CENTER_WEIGHT, TORQUE_CENTER_WEIGHT);
            torqueCenterMultipler = Random.Range(-TORQUE_CENTER_MULTIPLER, TORQUE_CENTER_MULTIPLER);
        }
        else {
            torqueAngleWeight     = DEFUALT_TORQUE_ANGLE_WEIGHT;
            torqueAngleMultipler  = DEFUALT_TORQUE_ANGLE_MULTIPLER;
            torqueCenterWeight    = DEFUALT_TORQUE_CENTER_WEIGHT;
            torqueCenterMultipler = DEFUALT_TORQUE_CENTER_MULTIPLER;
        }
    }

    //the values are updated when sensors collider with the walls
    public void UpdateSensor(uint id, uint value) {
        sensors[id] = value; 
    }
}
