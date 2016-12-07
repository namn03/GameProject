/*
 * Define a class for character JSON file parsing 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class JsonObject {
	public JsonSkeleton Skeleton;
	public List<JsonBodyDef> BodyDefs;
	public List<JsonPDController> PDControllers;
}

[System.Serializable]
public class JsonSkeleton {
	public List<JsonJoint> Joints;
}

[System.Serializable]
public class JsonBodyDef {
	public string Name;
	public string Shape;
	public float Mass;
	public float AttachX;
	public float AttachY;
	public float AttachZ;
	public float Theta;
	public float Param0;
	public float Param1;
	public float Param2;
	public float ColorR;
	public float ColorG;
	public float ColorB;
	public float ColorA;
}

[System.Serializable]
public class JsonPDController {
	public string Name;
	public float Kp;
	public float Kd;
	public float TorqueLim;
	public float TargetTheta;
	public bool UseWorldCoord;
}


[System.Serializable]
public class JsonJoint {
	public string Name;
	public int Type;
	public int Parent;
	public float AttachX;
	public float AttachY;
	public float AttachZ;
	public int LinkStiffness;
	public float LimLow;
	public float LimHigh;
}