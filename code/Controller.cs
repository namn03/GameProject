using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Controller {
	private enum State {BACKSTANCE, EXTEND, FRONTSTANCE, GATHER };
	private State mState = State.EXTEND;

	private string[] CONTROL_PARAMS = {"shoulder", "elbow", "hip", "knee", "ankle"};

	private float[] mPastTau;

	public Controller(Character character) {
		List<JsonBodyDef> jsonBody = character.mjsonObject.BodyDefs;
		List<JsonJoint> jsonJoint = character.mjsonObject.Skeleton.Joints;

		int n = jsonBody.Count;
		mPastTau = new float[n];
	}

	public float[] getTau(JsonAction action, Character character) {
		List<JsonPDController> jsonPDs = character.mjsonObject.PDControllers;
		List<GameObject> bodies = character.mBodies;

		int numJoint = jsonPDs.Count;
		float[] targetTau = new float[numJoint];
		float[] tauErr = new float[numJoint];

		for (int i = 1; i < numJoint; i++) {
			targetTau [i] = 0;
			JsonPDController pd = jsonPDs [i];
			GameObject body = bodies [i];

			HingeJoint joint = bodies [i].GetComponent<HingeJoint> ();
			float currTheta = Mathf.Deg2Rad * joint.angle;
			float currVel = Mathf.Deg2Rad * joint.velocity;
			float targetTheta;

			// if current joint is controlled by action, use that theta
			if (pd.Name.Contains ("spine")) {
				targetTheta = getTargetTheta (action.StateParams, "SpineCurve");

			} else if (CONTROL_PARAMS.Contains (pd.Name)){
				targetTheta = getTargetTheta (action.StateParams, pd.Name);
			} else {
				targetTheta = pd.TargetTheta;
			}

			JointSpring spring = joint.spring;
			spring.targetPosition = Mathf.Rad2Deg*targetTheta;
			joint.spring = spring;

			//targetTau[i] += calcSPD(currTheta, targetTheta, currVel, pd);
			int parent = character.mjsonObject.Skeleton.Joints[i].Parent;
			float mass = character.mjsonObject.BodyDefs [i].Mass;
			calcGravity (ref targetTau, body, joint, mass, i, parent);
			// targetTau[i] += calcVirtual

			tauErr [i] = targetTau [i] -  mPastTau [i];
			mPastTau [i] = targetTau [i];
		}

		return tauErr;
	}

	public void advanceState() {
		if (mState == State.GATHER) {
			mState = State.BACKSTANCE;
		} else {
			mState++;
		}
	}

	private float calcSPD(float currTheta, float targetTheta, float currVel, JsonPDController pd) {
		float thetaErr = targetTheta - currTheta;
		float result = pd.Kp * thetaErr - pd.Kd*currVel;

		return result;
	}

	private void calcGravity(ref float[] tau, GameObject body, HingeJoint joint, float m, int i, int parent) {
		tau [i] -= Vector3.Cross (body.transform.TransformVector(joint.anchor), m*Physics.gravity).z;
		tau [parent] += tau [i];
	}

	private float getTargetTheta(JsonStateParam param, string className) {
		float targetTheta = -1;
		JsonState stateParam = null;
		switch(mState) {
		case State.BACKSTANCE:
			stateParam = param.BackStance;
			break;
		case State.EXTEND:
			stateParam = param.Extend;
			break;
		case State.FRONTSTANCE:
			stateParam = param.FrontStance;
			break;
		case State.GATHER:
			stateParam = param.Gather;
			break;
		}

		switch (className) {
		case "SpineCurve":
			targetTheta = stateParam.SpineCurve;
			break;
		case "shoulder":
			targetTheta = stateParam.Shoulder;
			break;
		case "elbow":
			targetTheta = stateParam.Elbow;
			break;
		case "hip":
			targetTheta = stateParam.Hip;
			break;
		case "knee":
			targetTheta = stateParam.Knee;
			break;
		case "ankle":
			targetTheta = stateParam.Ankle;
			break;
		}

		return targetTheta;
	}
}