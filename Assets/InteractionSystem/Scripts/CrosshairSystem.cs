using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This script handles
/// </summary>
public class CrosshairSystem : MonoBehaviour {
    [Header("Display")]
    [SerializeField]
    public Canvas HudCanvas = null;

    [Header("Input")]
    [SerializeField]
    public string InteractButton = "Fire1";

    [Header("Raycast")]
    [SerializeField]
    public float ActiveDistance = 1.2f;

    [SerializeField]
    public string[] LayersToHit = { };

    private Animator CrosshairAnimation = null;
    private Transform CrosshairInfo = null;

    private Camera TargetCamera = null;

    private int CalculatedLayerMask;

    private Vector3 ScreenCenter;
    private RaycastHit RaycastHitTarget;

    private Transform PreviousTransform = null;
    private InteractiveBehaviour[] PreviousBehaviours = { };

    private Transform CurrentTransform = null;
    private InteractiveBehaviour[] CurrentBehaviours = { };
    private InteractiveSettings CurrentSettings = null;

    private void Start () {
        CrosshairAnimation  = HudCanvas.transform.Find ("Reticle").GetComponent<Animator> ();
        CrosshairInfo       = HudCanvas.transform.Find ("ReticleInfo");
        TargetCamera        = transform.GetComponentInChildren<Camera> ();

        CalculatedLayerMask = LayerMask.GetMask (LayersToHit);
    }

    private void Update () {
        SetCenter ();
        RaycastForward ();
        UpdateReticleInfo ();
        HandleInput ();
    }

    private void SetCenter () {
        ScreenCenter = new Vector3 (
            Screen.width  / 2,
            Screen.height / 2
        );
    }

    private void RaycastForward () {
        Ray ray = TargetCamera.ScreenPointToRay (ScreenCenter);

        if (PreviousTransform != null)
            PreviousBehaviours = PreviousTransform.GetComponentsInChildren<InteractiveBehaviour> ();

        if (Physics.Raycast (ray, out RaycastHitTarget, ActiveDistance, CalculatedLayerMask)) {
            CurrentTransform = RaycastHitTarget.transform;
            CurrentBehaviours = CurrentTransform.GetComponentsInChildren<InteractiveBehaviour> ();

            if (CurrentTransform != PreviousTransform) {
                if (PreviousTransform != null) {
                    foreach (InteractiveBehaviour Behaviour in PreviousBehaviours) {
                        Behaviour.OnLookExit ();
                    }
                }

                foreach (InteractiveBehaviour Behaviour in CurrentBehaviours) {
                    Behaviour.OnLookEnter ();
                }

                PreviousTransform = CurrentTransform;
            } else {
                foreach (InteractiveBehaviour Behaviour in CurrentBehaviours) {
                    Behaviour.OnLookStay ();
                }
            }
        } else {
            CurrentTransform = null;

            if (PreviousTransform != null) {
                foreach(InteractiveBehaviour Behaviour in PreviousBehaviours) {
                    Behaviour.OnLookExit ();
                }

                PreviousTransform = null;
            }
        }
    }

    private void UpdateReticleInfo () {
        if (CurrentTransform == null) {
            CrosshairInfo.gameObject.SetActive (false);
        } else {
            CrosshairInfo.gameObject.SetActive (true);

            CurrentSettings = CurrentTransform.GetComponentInParent<InteractiveSettings> ();
            CrosshairInfo.GetComponentInChildren<Text> ().text = CurrentSettings.Name;
        }
    }

    private void HandleInput () {
        if (CurrentTransform == null)
            return;

        if (Input.GetButtonDown (InteractButton)) {
            CrosshairAnimation.SetTrigger ("Interact");

            foreach(InteractiveBehaviour Behaviour in CurrentBehaviours) {
                Behaviour.OnInteract ();
            }
        }
    }

    private void OnDrawGizmosSelected () {
        if (TargetCamera == null)
            return;

        Ray ray = TargetCamera.ScreenPointToRay (ScreenCenter);

        if (Physics.Raycast (ray, out RaycastHitTarget, ActiveDistance, CalculatedLayerMask)) {
            Gizmos.color = Color.red;
        } else {
            Gizmos.color = Color.cyan;
        }

        Gizmos.DrawRay (ray.origin, ray.direction * ActiveDistance);
    }
}