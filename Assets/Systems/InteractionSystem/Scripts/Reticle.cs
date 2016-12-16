using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Reticle : MonoBehaviour
{
    private Animator _reticleAnimator = null;

    private void Awake()
    {
        _reticleAnimator = GetComponent<Animator>();
    }

    public void LookAt(Transform obj)
    {
        _reticleAnimator.SetBool("Looking", obj != null);
    }

    public void InteractWith(Transform obj)
    {
        _reticleAnimator.SetTrigger("Interact");
    }
}