using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistantObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject FaderObjectPrefab;

        static bool hasSpawned = false;

        void Awake()
        {
            if (hasSpawned) return;

            hasSpawned = true;

            SpawnObjects();
        }

        private void SpawnObjects()
        {
            GameObject fader = Instantiate(FaderObjectPrefab);
            DontDestroyOnLoad(fader);
        }

    }
}

