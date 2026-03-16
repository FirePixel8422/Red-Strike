using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmartBillboard : UpdateMonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool freezeX, freezeY, freezeZ;

    protected override void OnUpdate() => UpdateParticle();

    private Camera GetCamera()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                return SceneView.lastActiveSceneView.camera;
            }
        }
#endif
        return Camera.main;
    }

    private void UpdateParticle()
    {
        Camera cam = GetCamera();

        if (cam == null || target == null)
        {
            return;
        }

        Vector3 faceDir = transform.position - cam.transform.position;
        Quaternion lookRot = Quaternion.LookRotation(faceDir);

        Vector3 euler = lookRot.eulerAngles;
        Vector3 targetEuler = target.eulerAngles;

        if (freezeX) { euler.x = targetEuler.x; }
        if (freezeY) { euler.y = targetEuler.y; }
        if (freezeZ) { euler.z = targetEuler.z; }

        transform.eulerAngles = euler;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying || target == null) return;

        UpdateParticle();
    }
}