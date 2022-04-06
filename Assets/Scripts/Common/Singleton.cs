using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.Common
{
    /// <summary>
    /// Singleton
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<T>();

                    if (_Instance == null)
                    {
                        GameObject obj = new GameObject($"Singleton({typeof(T).Name})");
                        _Instance = obj.AddComponent<T>();
                    }
                }
                return _Instance;
            }
        }
        private static T _Instance;

        public static void Release()
        {
            if (_Instance == null)
                return;
            Destroy(_Instance.gameObject);
        }

        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this as T;
            DontDestroyOnLoad(this);
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }
    }
}