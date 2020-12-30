using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicsControlRemover : MonoBehaviour
    {
        //STATE
        GameObject _player;

        //UNITY MESSAGES
        private void Start()
        {
            //Adds the Enable and Disable Control methods to the stopped and played events
            GetComponent<PlayableDirector>().stopped += EnableControl;
            GetComponent<PlayableDirector>().played += DisableControl;

            //Finds the Player
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        //EVENT LISTENERS
        void DisableControl(PlayableDirector director)
        {
            //Cancels the current action
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();

            //Stops the player from moving
            _player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector director)
        {
            //Allows the player to move
            _player.GetComponent<PlayerController>().enabled = true;
        }
    }
}

