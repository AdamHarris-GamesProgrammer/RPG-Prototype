using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SceneTarget : MonoBehaviour
    {
        [SerializeField] private string sceneName;


        public void TransitionTo()
        {
            //Safety Check
            if(sceneName.Length != 0)
            {
                if (Debug.isDebugBuild)
                {
                    //Only check the scene name is valid in debug mode to save load times in the built game
                    if (ValidSceneName())
                    {
                        SceneManager.LoadScene(sceneName);
                    }
                    else
                    {
                        Debug.LogError("[Scene Target]: " + gameObject.name + " scene target component has a invalid name set");
                    }
                }
                else
                {
                    SceneManager.LoadScene(sceneName);
                }

            }
            else
            {
                //Logs a error in the event that the name is null
                Debug.LogError("[Scene Target]: " + gameObject.name + " scene target component does not have a name set");
            }
        }

        //This method cycles through all the scene names in the game to check that the sceneName variable has a correct name
        private bool ValidSceneName()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene validation = SceneManager.GetSceneByBuildIndex(i);

                if (validation.name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

