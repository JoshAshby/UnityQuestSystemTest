using UnityEngine;
using Zenject;

/// <summary>
/// This script handles raycasting out from the center of the screen to the
/// distance specificed in `ActiveDistance` and triggers any InteractiveBehaviour
/// Scripts it finds on a collision.
/// </summary>
public class CrosshairSystem : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    public string InteractButton = "Fire1";

    [Header("Raycast")]
    [SerializeField]
    public float ActiveDistance = 1.2f;

    [SerializeField]
    public string[] LayersToHit = { };

    [HideInInspector]
    public Transform CurrentTransform = null;

    public delegate void OnLookChangeHandler(Transform obj);
    public event OnLookChangeHandler OnLookChange;

    public delegate void OnInteractHandler(Transform obj);
    public event OnInteractHandler OnInteract;

    private Camera TargetCamera = null;

    private int CalculatedLayerMask;

    private Vector3 ScreenCenter;
    private RaycastHit RaycastHitTarget;

    private Transform PreviousTransform = null;
    private InteractiveBehaviour[] PreviousBehaviours = { };

    private InteractiveBehaviour[] CurrentBehaviours = { };
    private InteractiveSettings CurrentSettings = null;

    [Inject]
    private IGameManager _gameManager;

    private void Start()
    {
        TargetCamera = transform.GetComponentInChildren<Camera>();

        CalculatedLayerMask = LayerMask.GetMask(LayersToHit);
    }

    private void Update()
    {
        if (_gameManager.fsm.CurrentState == GameStates.Menu)
        {
            return;
        }

        SetCenter();
        RaycastForward();
        HandleInput();
    }

    private void SetCenter()
    {
        ScreenCenter = new Vector3(
            Screen.width / 2,
            Screen.height / 2
        );
    }

    private void RaycastForward()
    {
        Ray ray = TargetCamera.ScreenPointToRay(ScreenCenter);

        if (PreviousTransform != null)
            PreviousBehaviours = PreviousTransform.GetComponentsInChildren<InteractiveBehaviour>();

        if (Physics.Raycast(ray, out RaycastHitTarget, ActiveDistance, CalculatedLayerMask))
        {
            CurrentTransform = RaycastHitTarget.transform;
            CurrentBehaviours = CurrentTransform.GetComponentsInChildren<InteractiveBehaviour>();

            if (CurrentTransform != PreviousTransform)
            {
                if (PreviousTransform != null)
                {
                    foreach (InteractiveBehaviour Behaviour in PreviousBehaviours)
                    {
                        Behaviour.OnLookExit();
                    }
                }

                foreach (InteractiveBehaviour Behaviour in CurrentBehaviours)
                {
                    Behaviour.OnLookEnter();
                }

                PreviousTransform = CurrentTransform;

                if (OnLookChange != null)
                    OnLookChange(CurrentTransform);
            }
            else
            {
                foreach (InteractiveBehaviour Behaviour in CurrentBehaviours)
                {
                    Behaviour.OnLookStay();
                }
            }
        }
        else
        {
            CurrentTransform = null;

            if (PreviousTransform != null)
            {
                foreach (InteractiveBehaviour Behaviour in PreviousBehaviours)
                {
                    Behaviour.OnLookExit();
                }

                PreviousTransform = null;

                if (OnLookChange != null)
                    OnLookChange(null);
            }
        }
    }

    private void HandleInput()
    {
        if (CurrentTransform == null)
            return;

        if (Input.GetButtonDown(InteractButton))
        {
            foreach (InteractiveBehaviour Behaviour in CurrentBehaviours)
            {
                Behaviour.OnInteract();
            }

            if (OnInteract != null)
                OnInteract(CurrentTransform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (TargetCamera == null)
            return;

        Ray ray = TargetCamera.ScreenPointToRay(ScreenCenter);

        if (Physics.Raycast(ray, out RaycastHitTarget, ActiveDistance, CalculatedLayerMask))
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.cyan;
        }

        Gizmos.DrawRay(ray.origin, ray.direction * ActiveDistance);
    }
}