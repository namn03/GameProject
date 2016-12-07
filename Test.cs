using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	public float f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		HingeJoint joint = GetComponent<HingeJoint> ();
		JointMotor motor = new JointMotor ();
		motor.force = f;
		joint.motor = motor;
	}
}
