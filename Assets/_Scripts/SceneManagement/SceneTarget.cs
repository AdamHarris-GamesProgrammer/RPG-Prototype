using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SceneTarget : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] public Transform spawnPoint;


        public enum DestinationIdentifier
        {
            A, B, C, D, E, F, G
        }

        [SerializeField] public DestinationIdentifier destination;
        [SerializeField] public DestinationIdentifier destinationToGo;



        public void TransitionTo()
        {
            //Safety Check
            if (sceneName.Length != 0)
            {
                //if (Debug.isDebugBuild)
                //{
                //    //Only check the scene name is valid in debug mode to save load times in the built game
                //    if (ValidSceneName())
                //    {
                //        StartCoroutine("Transition");
                //    }
                //    else
                //    {
                //        Debug.LogError("[Scene Target]: " + gameObject.name + " scene target component has a invalid name set");
                //    }
                //}
                //else
                //{
                //    StartCoroutine("Transition");
                //}

                StartCoroutine("Transition");

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
            for (int i = 0; i <= SceneManager.sceneCount; i++)
            {
                Scene validation = SceneManager.GetSceneByBuildIndex(i);

                if (validation.name == sceneName)
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator Transition()
        {
            Fader fader = FindObjectOfType<Fader>();

            DontDestroyOnLoad(gameObject);

            yield return StartCoroutine(fader.FadeIn());
            yield return SceneManager.LoadSceneAsync(sceneName);

            SceneTarget target = GetOtherSceneTarget();
            UpdatePlayer(target);

            yield return StartCoroutine(fader.FadeWait());

            yield return StartCoroutine(fader.FadeOut());


            Destroy(gameObject);
        }

        private void UpdatePlayer(SceneTarget target)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(target.spawnPoint.position);
            //player.transform.position = target.spawnPoint.position;
            player.transform.rotation = target.spawnPoint.rotation;


        }

        private SceneTarget GetOtherSceneTarget()
        {
            foreach (SceneTarget target in FindObjectsOfType<SceneTarget>())
            {
                if (target == this) continue;
                if (target.destination != destinationToGo) continue;

                return target;
            }
            return null;
        }

    }
}

