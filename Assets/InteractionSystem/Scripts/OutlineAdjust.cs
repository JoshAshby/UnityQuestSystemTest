using UnityEngine;

public class OutlineAdjust : InteractiveBehaviour {
    [HeaderAttribute("Material Color")]
    [SerializeField]
    public Color ChangeToColor;

    [SerializeField]
    public string MaterialColorName;

    private Color old_color;
    private Material material;

    void Start() {
        material = GetComponent<Renderer> ().material;
    }

    override public void OnLookEnter () {
        Debug.LogFormat ("Changing color to {0}", ChangeToColor.ToString());

        old_color = material.GetColor (MaterialColorName);
        material.SetColor(MaterialColorName, ChangeToColor);
    }

    override public void OnLookExit () {
        Debug.LogFormat ("Reverting color from {0}", ChangeToColor.ToString());
        material.SetColor(MaterialColorName, old_color);
    }
}