using RPG.Control;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SceneTarget : MonoBehaviour, IInteractive
    {
        [SerializeField] private string sceneName = null;
        [SerializeField] public Transform spawnPoint;
        [SerializeField] private GameObject uiGroup = null;

        bool disableInteraction = false;

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
                StartCoroutine("Transition");
            }
            else
            {
                //Logs a error in the event that the name is null
                Debug.LogError("[Scene Target]: " + gameObject.name + " scene target component does not have a name set");
            }
        }

        private IEnumerator Transition()
        {
            Fader fader = FindObjectOfType<Fader>();

            DontDestroyOnLoad(gameObject);

            //remove control
            FindObjectOfType<PlayerController>().enabled = false;

            yield return StartCoroutine(fader.FadeIn());


            //Destroys the serialized image 
            Destroy(uiGroup);

            //Save current level
            SavingWrapper saveComponent = FindObjectOfType<SavingWrapper>();
            saveComponent.Save();

            yield return SceneManager.LoadSceneAsync(sceneName);

            //remove control from new player
            FindObjectOfType<PlayerController>().enabled = false;

            //Load current level
            saveComponent.Load();


            SceneTarget target = GetOtherSceneTarget();
            UpdatePlayer(target);
            saveComponent.Save();

            yield return StartCoroutine(fader.FadeWait());

            //Restore control to player 
            FindObjectOfType<PlayerController>().enabled = true;

            yield return StartCoroutine(fader.FadeOut());

            Destroy(gameObject);
        }

        //This method moves the player to the spawnpoint of the new portal
        private void UpdatePlayer(SceneTarget target)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.parent.position = target.spawnPoint.position;
            player.GetComponent<NavMeshAgent>().Warp(target.spawnPoint.position);
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


        //IInteractive interface implementation
        public void Interact()
        {
            if (disableInteraction) return;


            if (Input.GetKeyDown(KeyCode.E))
            {
                disableInteraction = true;
                TransitionTo();
                GetComponent<Collider>().enabled = false;
            }
        }


        public void ShowUI(bool isActive)
        {
            if (uiGroup == null) return;

            uiGroup.SetActive(isActive);
        }
    }
}

