using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class JsonAction {
	public JsonMiscParam MiscParams;
	public JsonStateParam StateParams;
}

[System.Serializable]
public class JsonMiscParam {
	public float TransTime;
	public float Cv;
	public float BackForceX;
	public float BackForceY;
	public float FrontForceX;
	public float FrontForceY;
}

[System.Serializable]
public class JsonStateParam {
	public JsonState BackStance;
	public JsonState Extend;
	public JsonState FrontStance;
	public JsonState Gather;
}

[System.Serializable]
public class JsonState { 
	public float SpineCurve;
	public float Shoulder;
	public float Elbow;
	public float Hip;
	public float Knee;
	public float Ankle;
}