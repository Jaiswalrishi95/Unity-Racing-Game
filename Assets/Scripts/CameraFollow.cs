using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    public Transform followTarget;

    [SerializeField]
    private Vector3 offsetPosition;

    [SerializeField]
    private Space offsetPositionSpace = Space.Self;

    [SerializeField]
    private bool lookAt = true;

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (followTarget == null)
        {
            Debug.LogWarning("Missing target ref !", this);

            return;
        }

        // compute position
        if (offsetPositionSpace == Space.Self)
        {
            transform.position = followTarget.TransformPoint(offsetPosition);
        }
        else
        {
            transform.position = followTarget.position + offsetPosition;
        }

        // compute rotation
        if (lookAt)
        {
            transform.LookAt(followTarget);
        }
        else
        {
            transform.rotation = followTarget.rotation;
        }
    }
}