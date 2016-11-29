using UnityEngine;
using System.Collections;

public class OutlineAdjust : InteractiveBehaviour {
	public Color change_to;

	private Color old_color;
	private Material material;

	void Start() {
		material = GetComponent<Renderer> ().material;
	}

	override public void OnLookEnter() {
		old_color = material.GetColor ("_OutlineColor");
		material.SetColor("_OutlineColor", change_to);
	}

	override public void OnLookExit() {
		material.SetColor("_OutlineColor", old_color);
	}
}