using UnityEngine;
using System.Collections;

public class CreateCharacter : MonoBehaviour {

	public TextAsset mCharacterJson;
	public TextAsset mActionJson;
	public GameObject mRefCube;

	private Controller mController;
	private Character mCharacter;
	private Policy mPolicy;
	private float lastTime;

	void Start () {
		mCharacter = new Character (mCharacterJson, mRefCube);
		mPolicy = new Policy ();
		mController = new Controller(mCharacter);
	}

	void FixedUpdate () {
		float time = Time.time;
		/*
		mPolicy.updatePolicy ();
		*/
		JsonAction action = mPolicy.getAction (mActionJson);
		float[] t = mController.getTau(action, mCharacter);
		mCharacter.simulate (t);

		//if (Input.anyKeyDown) {
		if (lastTime + 0.125 < time) {
			mController.advanceState ();
			lastTime = time;
		}

	}
}
