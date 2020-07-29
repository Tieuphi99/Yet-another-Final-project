using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemScripts
{
    public class GameStatusController : MonoBehaviour
    {
        public TextMeshProUGUI playerScoreText;
        public TextMeshProUGUI playerHighScoreText;
        public TextMeshProUGUI collectedCoinText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI secondsText;
        public TextMeshProUGUI livesText;
        public GameObject score200Prefab;
        public GameObject score1000Prefab;
        public GameObject pausePopup;
        public GameObject instructionPopup;
        public GameObject creditPopup;
        public GameObject firstMessagePopup;
        public GameObject secondMessagePopup;
        public Transform scoreParent;
        private AudioSource _gameStatusAudio;

        public AudioClip pauseSound;
        public AudioClip stageClearSound;

        private bool _pauseTrigger;

        public static int CollectedCoin;
        public static int Score;
        private static int _highScore;
        public static int Live;
        public static int CurrentLevel;
        public static bool IsDead;
        public static bool IsGameOver;
        public static bool IsStageClear;
        public static bool IsBigPlayer;
        public static bool IsFirePlayer;
        public static bool IsBossBattle;
        public static bool IsGameFinish;
        public static bool IsEnemyDieOrCoinEat;
        public static bool IsPowerUpEat;
        public static bool IsShowMessage;
        public static string PlayerTag;
        private float _second;

        private void Awake()
        {
            SetScore(playerHighScoreText, _highScore);
            _gameStatusAudio = GetComponent<AudioSource>();
            _pauseTrigger = false;
        }

        private void Update()
        {
            if (IsStageClear)
            {
                _gameStatusAudio.PlayOneShot(stageClearSound);
                IsStageClear = false;
            }

            if (IsShowMessage)
            {
                StartCoroutine(DisplayFirstMessage());
            }

            if (IsEnemyDieOrCoinEat)
            {
                IsEnemyDieOrCoinEat = false;
                UpdateScorePopup(score200Prefab);
            }

            if (IsPowerUpEat)
            {
                IsPowerUpEat = false;
                UpdateScorePopup(score1000Prefab);
            }

            if (Score > _highScore)
            {
                _highScore = Score;
            }

            SetCoin();
            SetLevel();
            SetScore(playerScoreText, Score);
            SetLive();
            Pause();
        }

        private void SetScore(TextMeshProUGUI scoreText, int score)
        {
            switch (score.ToString().Length)
            {
                case 0:
                    scoreText.SetText("000000");
                    break;
                case 3:
                    scoreText.SetText($"000{score}");
                    break;
                case 4:
                    scoreText.SetText($"00{score}");
                    break;
                case 5:
                    scoreText.SetText($"0{score}");
                    break;
                case 6:
                    scoreText.SetText($"{score}");
                    break;
            }
        }

        private void SetCoin()
        {
            if (CollectedCoin > 0)
            {
                collectedCoinText.SetText($"x0{CollectedCoin}");
                if (CollectedCoin <= 9) return;
                collectedCoinText.SetText($"x{CollectedCoin}");
                if (CollectedCoin > 99)
                {
                    collectedCoinText.SetText("x00");
                }
            }
            else
            {
                collectedCoinText.SetText("x00");
            }
        }

        public void SetTime(float second)
        {
            _second = second;
            if (_second > 0)
            {
                if (_second > 99.5f)
                {
                    secondsText.SetText(Mathf.RoundToInt(_second).ToString());
                }
                else if (_second > 9.5f)
                {
                    secondsText.SetText($"0{Mathf.RoundToInt(_second).ToString()}");
                }
                else
                {
                    secondsText.SetText($"00{Mathf.RoundToInt(_second).ToString()}");
                }
            }
            else
            {
                secondsText.SetText("000");
            }
        }

        private void SetLevel()
        {
            levelText.SetText(SceneManager.GetActiveScene().name);
        }

        private void SetLive()
        {
            livesText.SetText($"x {Live.ToString()}");
        }

        private void Pause()
        {
            if (SceneManager.GetActiveScene().buildIndex > 1)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    _gameStatusAudio.PlayOneShot(pauseSound);
                    _pauseTrigger = !_pauseTrigger;
                    pausePopup.SetActive(_pauseTrigger);
                    Time.timeScale = _pauseTrigger ? 0 : 1;
                }
            }
        }

        public void StartGame()
        {
            SceneManager.LoadScene(1);
            CurrentLevel = 2;
            Live = 3;
            Score = 0;
            CollectedCoin = 0;
            PlayerTag = "Player";
        }

        public void OpenInstructionPopup()
        {
            instructionPopup.SetActive(true);
        }

        public void OpenCreditPopup()
        {
            creditPopup.SetActive(true);
        }

        public void CloseInstructionPopup()
        {
            instructionPopup.SetActive(false);
        }

        public void CloseCreditPopup()
        {
            creditPopup.SetActive(false);
        }

        public void ExitGame()
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }

        private void UpdateScorePopup(GameObject scorePrefab)
        {
            GameObject score = Instantiate(scorePrefab, scoreParent);
            StartCoroutine(DestroyScorePrefab(score));
        }

        private IEnumerator DestroyScorePrefab(GameObject prefab)
        {
            yield return new WaitForSeconds(1);
            Destroy(prefab);
        }

        private IEnumerator DisplayFirstMessage()
        {
            yield return new WaitForSeconds(1);
            firstMessagePopup.SetActive(true);
            StartCoroutine(DisplaySecondMessage());
        }

        private IEnumerator DisplaySecondMessage()
        {
            yield return new WaitForSeconds(1.5f);
            secondMessagePopup.SetActive(true);
        }
    }
}