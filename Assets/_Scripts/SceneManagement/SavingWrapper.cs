﻿using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFile = "save";

        IEnumerator Start()
        {
            Fader fade = FindObjectOfType<Fader>();
            fade.FadeOutImmediate();
            
            yield return GetComponent<SavingSystem>().LoadLastScene(saveFile);
            yield return fade.FadeOut();
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

    }
}

