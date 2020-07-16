using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool hasPlayed = false;


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!hasPlayed)
                {
                    GetComponent<PlayableDirector>().Play();
                    hasPlayed = true;
                }
            }
        }
    }
}

