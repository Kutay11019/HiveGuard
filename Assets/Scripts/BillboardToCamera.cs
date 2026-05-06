using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    [SerializeField] private bool lockXRotation = false;
    [SerializeField] private bool lockZRotation = false;

    private void LateUpdate()
    {
        if (Camera.main == null)
        {
            return;
        }

        Vector3 directionToCamera = transform.position - Camera.main.transform.position;

        if (directionToCamera.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        Vector3 eulerAngles = targetRotation.eulerAngles;

        if (lockXRotation)
        {
            eulerAngles.x = 0f;
        }

        if (lockZRotation)
        {
            eulerAngles.z = 0f;
        }

        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}