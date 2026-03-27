using UnityEngine;

public static class ScenePostProcessor
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Run()
    {
        ForceStateOnLoad[] markers = Object.FindObjectsByType<ForceStateOnLoad>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < markers.Length; i++)
        {
            GameObject go = markers[i].gameObject;

            go.SetActiveSmart(markers[i].TargetState);
        }
    }
}