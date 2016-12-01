using UnityEngine;
using System.Collections;

public class OutlineAdjust : InteractiveBehaviour {
	public Color change_to;

	private Color old_color;
	private Material material;

	void Start() {
		material = GetComponent<Renderer> ().material;
	}

	override public void OnLookEnter () {
		Debug.LogFormat ("Changing color to {0}", change_to.ToString());

//		old_color = material.GetColor ("_OutlineColor");
//		material.SetColor("_OutlineColor", change_to);
	}

	override public void OnLookExit () {
		Debug.LogFormat ("Reverting color from {0}", change_to.ToString());
//		material.SetColor("_OutlineColor", old_color);
	}
}