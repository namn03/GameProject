using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character {
	public JsonObject mjsonObject;
	public List<GameObject> mBodies;

	public Character(TextAsset jsonText, GameObject refCube) {
		mjsonObject = JsonUtility.FromJson<JsonObject> (jsonText.text);
		mBodies = new List<GameObject> ();

		// make root
		JsonJoint joint = mjsonObject.Skeleton.Joints [0];
		JsonBodyDef body = mjsonObject.BodyDefs [0];

		GameObject root = GameObject.Instantiate (refCube);
		root.name = "root";
		Mesh mesh = root.GetComponent<MeshFilter> ().mesh;
		mesh.vertices = StretchMesh (mesh.vertices, body.Param0, body.Param1);
		mesh.RecalculateBounds();
		root.GetComponent<MeshCollider> ().sharedMesh = mesh;
		mBodies.Add (root);

		for (int i = 1; i < mjsonObject.Skeleton.Joints.Count; i++) {
			joint = mjsonObject.Skeleton.Joints [i];
			body = mjsonObject.BodyDefs [i];

			// info of parent body & joint
			int parentIdx = joint.Parent;
			JsonJoint parentJoint = mjsonObject.Skeleton.Joints [parentIdx];
			JsonBodyDef parentBody = mjsonObject.BodyDefs [parentIdx];
			GameObject parent = GameObject.Find (parentBody.Name);

			//Vector3 scale = new Vector3 (body.Param0, body.Param1, 0.1f);
			//refCube.transform.localScale = scale;
			GameObject obj = (GameObject) GameObject.Instantiate (refCube);
			obj.name = body.Name;

			// set position of body part
			Vector3 position = new Vector3 (body.AttachX, body.AttachY) - new Vector3(parentBody.AttachX, parentBody.AttachY);
			obj.transform.position = parent.transform.position + position + new Vector3(joint.AttachX, joint.AttachY);

			// set joint
			HingeJoint hingeJoint = obj.AddComponent<HingeJoint> ();
			hingeJoint.connectedBody = parent.GetComponent<Rigidbody>();
			hingeJoint.autoConfigureConnectedAnchor = false;
			hingeJoint.anchor = - new Vector3 (body.AttachX, body.AttachY);
			hingeJoint.connectedAnchor = parent.transform.InverseTransformPoint(obj.transform.TransformPoint(hingeJoint.anchor));
			hingeJoint.autoConfigureConnectedAnchor = true;
			hingeJoint.axis = new Vector3 (0, 0, 1);

			hingeJoint.useSpring = true;
			JointSpring spring = new JointSpring ();
			JsonPDController pd = mjsonObject.PDControllers [i];
			spring.targetPosition = Mathf.Rad2Deg * pd.TargetTheta;
			spring.spring = 20*pd.Kd;
			spring.damper = 0.1f*pd.Kp;
			hingeJoint.spring = spring;

			if(joint.LimLow < joint.LimHigh) {
				hingeJoint.useLimits = true;
				JointLimits limit = new JointLimits ();
				limit.max = Mathf.Rad2Deg*joint.LimHigh;
				limit.min = Mathf.Rad2Deg*joint.LimLow;
				hingeJoint.limits = limit;
			}

			// resize mesh
			mesh = obj.GetComponent<MeshFilter> ().mesh;
			mesh.vertices = StretchMesh (mesh.vertices, body.Param0, body.Param1);
			mesh.RecalculateBounds();
			obj.GetComponent<MeshCollider> ().sharedMesh = mesh;

			obj.GetComponent<Rigidbody> ().mass = body.Mass;

			mBodies.Add (obj);
		}

		for (int i = 0; i < mjsonObject.Skeleton.Joints.Count; i++) {
			for (int j = i; j < mjsonObject.Skeleton.Joints.Count; j++) {
				Physics.IgnoreCollision (mBodies [i].GetComponent<MeshCollider>(), mBodies [j].GetComponent<MeshCollider>());
			}
		}

		refCube.SetActive (false);
	}

	private Vector3[] StretchMesh(Vector3[] vertices, float stretchX, float stretchY) {
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = Vector3.Scale (new Vector3 (stretchX, stretchY, 0.1f), vertices [i]);
		}

		return vertices;
	}

	public void simulate(float[] tau) {
		for (int i = 1; i < mBodies.Count; i++) {
			/*
			string name = mjsonObject.BodyDefs [i].Name;
			if (name != "head") {
				continue;
			}
*/
			Vector3 tauVec = new Vector3 (0, 0, tau [i]);
			mBodies [i].GetComponent<Rigidbody> ().AddTorque (tauVec);
		}
	}
}