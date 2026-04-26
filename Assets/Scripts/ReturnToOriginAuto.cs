using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnToOriginAuto : MonoBehaviour
{
    private Vector3 origin;
    private Quaternion originRotation;
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    bool overriden = false;
    public bool Override {  get { return overriden; } set { overriden = value; print("return to origin auto for " + gameObject.name + " overriden"); } }

    void Start()
    {
        origin = transform.position;
        originRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null)
            grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        ReturnHome();
        ReturnHome();
    }

    public void ReturnHome()
    {
        if(overriden) return;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        transform.position = origin;
        transform.rotation = originRotation;
        // השאר isKinematic = true כדי שלא יפול
        // יחזור ל-false רק כשמרימים אותו
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (rb != null) rb.isKinematic = false;
        // הסר את ה-listener כדי לא לצבור
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }
}