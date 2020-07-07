using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFile = "save";

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

        private void Save()
        {
            Debug.Log(Application.persistentDataPath);
            GetComponent<SavingSystem>().Save(saveFile);
        }

        private void Load()
        {
            GetComponent<SavingSystem>().Load(saveFile);
        }

    }
}

