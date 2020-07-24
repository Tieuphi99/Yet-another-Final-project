using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemScripts
{
    public class LoadingSceneToLevelScene : MonoBehaviour
    {
        public GameObject liveStat;
        public GameObject gameOverPopup;
        private AudioSource _loadSceneAudio;
        public AudioClip gameOverSound;

        private void Start()
        {
            _loadSceneAudio = GetComponent<AudioSource>();
            if (GameStatusController.Live < 1)
            {
                GameStatusController.IsGameOver = true;
            }

            if (!GameStatusController.IsGameOver)
            {
                if (GameStatusController.IsDead)
                {
                    StartCoroutine(RepeatLevelScene());
                }
                else
                {
                    StartCoroutine(LevelScene());
                }
            }
            else
            {
                liveStat.SetActive(false);
                gameOverPopup.SetActive(true);
                _loadSceneAudio.PlayOneShot(gameOverSound);
                StartCoroutine(StartingScene());
            }
        }

        private static IEnumerator LevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(GameStatusController.CurrentLevel);
            GameStatusController.CurrentLevel += 1;
        }

        private static IEnumerator RepeatLevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(GameStatusController.CurrentLevel - 1);
            GameStatusController.IsDead = false;
        }

        private static IEnumerator StartingScene()
        {
            yield return new WaitForSeconds(4.5f);
            SceneManager.LoadScene(0);
            GameStatusController.Live = 3;
            GameStatusController.Score = 0;
            GameStatusController.CollectedCoin = 0;
            GameStatusController.IsGameOver = false;
            GameStatusController.IsDead = false;
        }
    }
}