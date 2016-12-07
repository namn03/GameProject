using UnityEngine;
using System.Collections;

public class Policy {

	public TextAsset mJsonText;
	private JsonAction mAction;

	public Policy() {
	}

	public void updatePolicy() {
	}

	public JsonAction getAction(TextAsset jasonText) {
		return JsonUtility.FromJson<JsonAction> (jasonText.text);
	}
}
