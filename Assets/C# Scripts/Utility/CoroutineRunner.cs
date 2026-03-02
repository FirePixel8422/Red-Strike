using System.Collections;
using UnityEngine;


namespace Fire_Pixel.Utility
{
    /// <summary>
    /// Handle Update Callbacks and batch them for every script by an event based register system
    /// </summary>
    public static class CoroutineRunner
    {
        private static CoroutineRunnerInstance runnerInstance;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            runnerInstance = new GameObject(">>CoroutineRunner<<").AddComponent<CoroutineRunnerInstance>();
            GameObject.DontDestroyOnLoad(runnerInstance.gameObject);
        }

        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return runnerInstance.StartCoroutine(coroutine);
        }
        public static void StopCoroutine(Coroutine coroutine)
        {
            runnerInstance.StopCoroutine(coroutine);
        }

        private class CoroutineRunnerInstance : MonoBehaviour { }
    }
}