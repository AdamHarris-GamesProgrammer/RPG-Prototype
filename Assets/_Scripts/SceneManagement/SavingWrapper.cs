﻿using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFile = "save";
        [SerializeField] bool loadLastScene = true;
        IEnumerator Start()
        {
            if (loadLastScene)
            {
                Fader fade = FindObjectOfType<Fader>();
                fade.FadeOutImmediate();

                yield return GetComponent<SavingSystem>().LoadLastScene(saveFile);
                yield return fade.FadeOut();
            }

        }

        void Update()
        {
            //L Key loads the save file
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            //O Key saves the game
            if (Input.GetKeyDown(KeyCode.O))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Save()
        {
            Debug.Log(Application.persistentDataPath);
            GetComponent<SavingSystem>().Save(saveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(saveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(saveFile);
        }

    }
}

