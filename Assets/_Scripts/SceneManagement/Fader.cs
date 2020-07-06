using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup canvas;

        [SerializeField] float fadeInTime = 2.5f;
        [SerializeField] float fadeOutTime = 2.5f;
        [SerializeField] float fadeWaitTime = 1.0f;

        void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOut()
        {
            canvas = GetComponent<CanvasGroup>();

            while (canvas.alpha > 0) //alpha is not 0 
            {
                //moving alpha toward 0
                canvas.alpha -= Time.deltaTime / fadeOutTime;

                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator FadeIn()
        {
            canvas = GetComponent<CanvasGroup>();

            while(canvas.alpha < 1)
            {
                canvas.alpha += Time.deltaTime / fadeInTime;

                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator FadeWait()
        {
            yield return new WaitForSeconds(fadeWaitTime);
        }
    }
}

