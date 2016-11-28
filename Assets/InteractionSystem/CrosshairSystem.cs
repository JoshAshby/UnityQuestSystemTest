using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrosshairSystem : MonoBehaviour {
	[Header("Input")]
	[SerializeField]
	public Canvas m_HudCanvas = null;

	[SerializeField]
	public string m_InteractButton = "Fire1";

	[Header("Raycast")]
	[SerializeField]
	public float m_ActiveDistance = 1.2f;

	private Transform m_TargetObject = null;
	private Animator m_CrosshairAnimation = null;
	private Camera m_TargetCamera = null;

	private void Start () {
		m_CrosshairAnimation = m_HudCanvas.transform.Find ("Reticle").GetComponent<Animator> ();
		m_TargetCamera       = transform.GetComponentInChildren<Camera> ();
	}

	private void Update () {
		Vector3 center = new Vector3 (
			Screen.width / 2,
			Screen.height / 2
		);

		RaycastHit hit;
		Ray ray = m_TargetCamera.ScreenPointToRay (center);

		if (Physics.Raycast (ray, out hit, m_ActiveDistance)) {
			Transform objectHit = hit.transform;

			InteractiveObject interObj = objectHit.GetComponentInChildren<InteractiveObject> ();

			if (interObj != null) {
				if (objectHit != m_TargetObject) {
					if (m_TargetObject != null) {
						m_TargetObject.GetComponentInChildren<InteractiveObject> ().OnLookExit ();
					}

					objectHit.GetComponentInChildren<InteractiveObject> ().OnLookEnter ();

					m_TargetObject = objectHit;
				} else if (objectHit == m_TargetObject)
					objectHit.GetComponentInChildren<InteractiveObject> ().OnLookStay ();
			}
		} else {
			if (m_TargetObject != null) {
				m_TargetObject.GetComponentInChildren<InteractiveObject> ().OnLookExit ();
				m_TargetObject = null;
			}
		}

		if (Input.GetButtonDown (m_InteractButton) && m_TargetObject != null) {
			m_CrosshairAnimation.SetTrigger ("Interact");
			m_TargetObject.GetComponentInChildren<InteractiveObject> ().OnInteract ();
		}
	}

	private void OnDrawGizmosSelected () {
		Vector3 center = new Vector3 (
			Screen.width / 2,
			Screen.height / 2
		);

		RaycastHit hit;
		Ray ray = m_TargetCamera.ScreenPointToRay (center);

		if (Physics.Raycast (ray, out hit, m_ActiveDistance)) {
			Gizmos.color = Color.red;
		} else {
			Gizmos.color = Color.cyan;
		}

		Gizmos.DrawRay (ray.origin, ray.direction * m_ActiveDistance);
	}
}