using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new System.Exception("An instance of this singleton already exists.");
            }
            else
            {
                Instance = (T)this;
            }
        }
    }    