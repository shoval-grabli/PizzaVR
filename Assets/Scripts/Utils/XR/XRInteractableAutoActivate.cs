using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRInteractableAutoActivate : MonoBehaviour
{
    [SerializeField] XRGrabInteractable interactable;
    private void Start()
    {
        StartCoroutine(EnableInteractor());
    }
    IEnumerator EnableInteractor()
    {
        yield return new WaitForFixedUpdate();
        if (interactable == null)
            GetComponent<XRGrabInteractable>().enabled = true;
        else
            interactable.enabled = true;

    }
}
