using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace Fire_Pixel.Utility
{
#pragma warning disable UDR0002
#pragma warning disable UDR0004
    /// <summary>
    /// Uitlity class to have an optimized easy acces to Updte Callbacks by using an Action based callback system
    /// </summary>
    public static class UpdateScheduler
    {
        private static event Action Update;
        private static event Action LateUpdate;
        private static event Action FixedUpdate;

        private static event Action NetworkTick;

        private static event Action LateDestroy;
        private static event Action LateApplicationQuit;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            UpdateCallbackManager gameManager = new GameObject(">>UpdateScheduler<<").AddComponent<UpdateCallbackManager>();
            gameManager.Init();

            GameObject.DontDestroyOnLoad(gameManager.gameObject);
        }
        public static void EnableNetworkTickEvents()
        {
            UpdateCallbackManager.Instance.EnableNetworkTickEvent();
        }


        #region void Update

            /// <summary>
            /// Register a method to call every frame like Update()
            /// </summary>
        public static void RegisterUpdate(Action action)
        {
            Update += action;
        }
        /// <summary>
        /// Unregister a registerd method for Update()
        /// </summary>
        public static void UnRegisterUpdate(Action action)
        {
            Update -= action;
        }
        /// <summary>
        /// Register or Unregister a method for Update() based on bool <paramref name="register"/>
        /// </summary>
        public static void ManageUpdate(Action action, bool register)
        {
            if (register)
            {
                RegisterUpdate(action);
            }
            else
            {
                UnRegisterUpdate(action);
            }
        }

        #endregion


        #region void NetworkTick

        /// <summary>
        /// Register a method to call every frame like NetworkTick()
        /// </summary>
        public static void RegisterNetworkTick(Action action)
        {
            NetworkTick += action;
        }
        /// <summary>
        /// Unregister a registerd method for NetworkTick()
        /// </summary>
        public static void UnRegisterNetworkTick(Action action)
        {
            NetworkTick -= action;
        }
        /// <summary>
        /// Register or Unregister a method for NetworkTick() based on bool <paramref name="register"/>
        /// </summary>
        public static void ManageNetworkTick(Action action, bool register)
        {
            if (register)
            {
                RegisterNetworkTick(action);
            }
            else
            {
                UnRegisterNetworkTick(action);
            }
        }

        #endregion


        #region void LateUpdate

        /// <summary>
        /// Register a method to call after every frame like LateUpdate()
        /// </summary>
        public static void RegisterLateUpdate(Action action)
        {
            LateUpdate += action;
        }
        /// <summary>
        /// Unregister a registerd method for LateUpdate()
        /// </summary>
        public static void UnRegisterLateUpdate(Action action)
        {
            LateUpdate -= action;
        }
        /// <summary>
        /// Register or Unregister a method for LateUpdate() based on bool <paramref name="register"/>
        /// </summary>
        public static void ManageLateUpdate(Action action, bool register)
        {
            if (register)
            {
                RegisterLateUpdate(action);
            }
            else
            {
                UnRegisterLateUpdate(action);
            }
        }

        #endregion


        #region void FixedUpdate

        /// <summary>
        /// Register a method to call every fixed frame like FixedUpdate()
        /// </summary>
        public static void RegisterFixedUpdate(Action action)
        {
            FixedUpdate += action;
        }
        /// <summary>
        /// Unregister a registerd method for FixedUpdate()
        /// </summary>
        public static void UnRegisterFixedUpdate(Action action)
        {
            FixedUpdate -= action;
        }
        /// <summary>
        /// Register or Unregister a method for FixedUpdate() based on bool <paramref name="register"/>
        /// </summary>
        public static void ManageFixedUpdate(Action action, bool register)
        {
            if (register)
            {
                RegisterFixedUpdate(action);
            }
            else
            {
                UnRegisterFixedUpdate(action);
            }
        }

        #endregion


        public static void CreateLateDestroyCallback(Action action)
        {
            LateDestroy += action;
        }
        public static void CreateLateApplicationQuitCallback(Action action)
        {
            LateApplicationQuit += action;
        }


        /// <summary>
        /// Handle Update Callbacks and batch them for every script by an event based register system
        /// </summary>
        private class UpdateCallbackManager : MonoBehaviour
        {
            public static UpdateCallbackManager Instance { get; set; }

            public void Init()
            {
                Instance = this;
                gameObject.isStatic = true;

                StartCoroutine(UpdateLoop());
            }
            public void EnableNetworkTickEvent()
            {
                NetworkManager.Singleton.NetworkTickSystem.Tick += InvokeNetworkTick;
            }
            private void InvokeNetworkTick()
            {
                NetworkTick?.Invoke();
            }

            private IEnumerator UpdateLoop()
            {
                float fixedAccumulator = 0f;
                float fixedDelta = Time.fixedDeltaTime;

                while (true)
                {
                    // Update
                    Update?.Invoke();

                    // FixedUpdate
                    fixedAccumulator += Time.deltaTime;
                    while (fixedAccumulator >= fixedDelta)
                    {
                        FixedUpdate?.Invoke();
                        fixedAccumulator -= fixedDelta;
                    }

                    // LateUpdate
                    LateUpdate?.Invoke();

                    if (LateDestroy != null)
                    {
                        LateDestroy.Invoke();
                        LateDestroy = null;
                    }

                    yield return null;
                }
            }

            private void OnApplicationQuit()
            {
                LateUpdate += () =>
                {
                    if (LateApplicationQuit != null)
                    {
                        LateApplicationQuit.Invoke();
                        LateApplicationQuit = null;
                    }
                };
            }
            private void OnDestroy()
            {
                Update = null;
                LateUpdate = null;
                FixedUpdate = null;

                LateDestroy = null;
                LateApplicationQuit = null;

                StopAllCoroutines();
            }
        }
    }
#pragma warning restore UDR0002
#pragma warning restore UDR0004
}