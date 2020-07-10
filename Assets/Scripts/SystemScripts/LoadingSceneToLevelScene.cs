using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemScripts
{
    public class LoadingSceneToLevelScene : MonoBehaviour
    {
        public GameObject liveStat;
        public GameObject gameOverPopup;
        
        // Start is called before the first frame update
        private void Start()
        {
            if (!GameStatusController.IsGameOver)
            {
                StartCoroutine(GameStatusController.IsDead ? RepeatLevelScene() : LevelScene());
            }
            else
            {
                liveStat.SetActive(false);
                gameOverPopup.SetActive(true);
                StartCoroutine(StartingScene());
                GameStatusController.Live = 3;
                GameStatusController.Score = 0;
                GameStatusController.CollectedCoin = 0;
                GameStatusController.IsGameOver = false;
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
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(0);
            GameStatusController.IsGameOver = false;
        }
    }
}