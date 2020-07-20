using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemScripts
{
    public class GameManager : MonoBehaviour
    {
        public PlayerController player;
        public GameStatusController gameStatusController;
        public List<EnemyController> enemyControllers;
        public List<GameObject> enemyGameObjects;
        public GameObject invisibleBrick;
        public GameObject invisiblePowerUp;

        public float time = 400;
        public float finalTime;

        private void Awake()
        {
            if (gameStatusController != null)
            {
                gameStatusController.SetTime(time);
            }
        }

        private void Update()
        {
            if (player != null)
            {
                StopEnemiesFromMovingWhenPlayerDie();
                SetActiveEnemiesWhenSeePlayer();
                UpdateTime();
                if (player.isStopTime)
                {
                    finalTime = time;
                    player.isStopTime = false;
                }
            }
        }

        private void StopEnemiesFromMovingWhenPlayerDie()
        {
            if (!GameStatusController.IsDead) return;
            for (var i = 0; i < enemyControllers.Count; i++)
            {
                if (enemyControllers[i] == null)
                {
                    enemyControllers.Remove(enemyControllers[i]);
                }
                else
                {
                    enemyControllers[i].speed = 0;
                    enemyControllers[i].gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    enemyControllers[i].gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                }
            }
        }

        private void SetActiveEnemiesWhenSeePlayer()
        {
            for (var i = 0; i < enemyGameObjects.Count; i++)
            {
                if (enemyGameObjects[i] != null)
                {
                    if (enemyGameObjects[i].transform.position.x - player.transform.position.x < 12)
                    {
                        enemyGameObjects[i].SetActive(true);
                    }
                }
                else
                {
                    enemyGameObjects.Remove(enemyGameObjects[i]);
                }
            }
        }

        private void UpdateTime()
        {
            if (!GameStatusController.IsDead && !player.isWalkingToCastle && !player.isInCastle)
            {
                gameStatusController.SetTime(time -= Time.deltaTime * 2);
                if (time < 0)
                {
                    time = 0;
                    GameStatusController.IsDead = true;
                }
            }
            else if (player.isInCastle)
            {
                gameStatusController.SetTime(time -= Time.deltaTime * 60);

                if (time < 0)
                {
                    time = 0;
                    StartCoroutine(NextLevel());
                }
                else
                {
                    if (finalTime - time >= 1f)
                    {
                        GameStatusController.Score += 50;
                        finalTime = time;
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
            {
                invisibleBrick.SetActive(true);
                invisibleBrick.GetComponent<BoxCollider2D>().isTrigger = false;
                invisiblePowerUp.SetActive(true);
                invisiblePowerUp.GetComponent<BoxCollider2D>().isTrigger = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private static IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(1);
        }
    }
}