using UnityEngine;

public class XRStartingPos : MonoBehaviour
{
    public GameObject cameraOffset;
    public Transform startingPosition;

    private void Start()
    {
        cameraOffset.transform.position = startingPosition.position;
    }
}
