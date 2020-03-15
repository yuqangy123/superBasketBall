using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Com.KhuongDuy.Basketball
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_startMenu;
        [SerializeField]
        private GameObject m_playBtn;  // belong start menu
        [SerializeField]
        private GameObject m_gameModesMenu;
        [SerializeField]
        private GameObject m_customizeMenu;
        [SerializeField]
        private GameObject m_getNewItemsMenu;
        [SerializeField]
        private GameObject m_pauseMenu;
        [SerializeField]
        private GameObject m_luckNextTime;
        [SerializeField]
        private GameObject m_newNotify;
        [SerializeField]
        private GameObject m_playButton;  // belong get new items menu

        private GameObject[] checkedsArray;

        [SerializeField]
        private Text m_endlessScoreText;
        [SerializeField]
        private Text m_timeScoreText;
        [SerializeField]
        private Text m_bounceScoreText;
        [SerializeField]
        private Text m_multiHoopScoreText;
        [SerializeField]
        private Text m_bestScoreText;
        [SerializeField]
        private Text m_lastScoreText;
        [SerializeField]
        private Text m_starAmount;
        [SerializeField]
        private Text m_starAmountInCustomize;
        [SerializeField]
        private Text m_starAmountInGetNewItems;

        private Animator m_startMenuAnimator;

        private Vector3[] m_ballsRaffleOriginalPos;

        [SerializeField]
        private float m_speedRunBallRaffle;

        private int
            m_startMenuHash,
            m_raffleAmount,
            m_ballIndexTemp;

        private bool
            m_startRaffle,
            m_nextRaffle;

        private PointerEventData pointer;
        private List<RaycastResult> raycastResult;

        public static UIManager _instance = null;

        public GameObject restartBtn;
        public GameObject ScoreSection;

        public Sprite[] ballsSprite;

        public Image[] ballRaffle;

        public Animator starRaffle;

        public bool StartMenuShowing { get; set; }

        // Constructor
        private UIManager() { }

		//for rate game

		public string RateUrl;

        // Behaviour messages
        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != null)
            {
                Destroy(this.gameObject);
            }

            SetUpComponents();
            SetCheckedBall();
        }

        private void SetUpComponents()
        {
            m_raffleAmount = 0;
            StartMenuShowing = true;

            m_startMenuAnimator = m_startMenu.GetComponent<Animator>();
            m_startMenuHash = Animator.StringToHash("FadeOut");

            m_ballsRaffleOriginalPos = new Vector3[4];
            for (int i = 0; i < ballRaffle.Length; i++)
            {
                m_ballsRaffleOriginalPos[i] = ballRaffle[i].transform.localPosition;
            }

            GetAllBallSprite();
        }

        private void GetAllBallSprite()
        {
            ballsSprite = new Sprite[68];

            for (int i = 66; i >= 0; i--)
            {
                string path = "Textures/shop/balls/ball_" + i;
                ballsSprite[i] = Resources.Load<Sprite>(path);
            }

            ballsSprite[67] = Resources.Load<Sprite>("Textures/social ball unlock/question_ball");
        }

        private void SetCheckedBall()
        {
            m_customizeMenu.SetActive(true);
            checkedsArray = GameObject.FindGameObjectsWithTag("Checked");
            m_customizeMenu.SetActive(false);

            int ballIndex = PlayerPrefs.GetInt(Constants.LAST_BALL, 0);

            for (int i = checkedsArray.Length - 1; i >= 0; i--)
            {
                if (i == ballIndex)
                {
                    checkedsArray[i].SetActive(true);
                }
                else
                {
                    checkedsArray[i].SetActive(false);
                }
            }
        }

        // Behaviour messages
        void Start()
        {
            pointer = new PointerEventData(EventSystem.current);
            raycastResult = new List<RaycastResult>();

            SetStarAmount();
            SetScore();
            GetBallsState();
        }

        public void SetStarAmount()
        {
            m_starAmount.text = PlayerPrefs.GetInt(Constants.STAR_AMOUNT, 0) + "";
            m_starAmountInCustomize.text = m_starAmountInGetNewItems.text = m_starAmount.text;
        }

        public void AddStar()
        {
            int currentStar = PlayerPrefs.GetInt(Constants.STAR_AMOUNT, 0);
            currentStar++;
            PlayerPrefs.SetInt(Constants.STAR_AMOUNT, currentStar);
        }

        public void SetScore()
        {
            m_endlessScoreText.text = PlayerPrefs.GetInt(Constants.ENDLESS_SCORE, 0) + "";
            m_timeScoreText.text = PlayerPrefs.GetInt(Constants.TIME_SCORE, 0) + "";
            m_bounceScoreText.text = PlayerPrefs.GetInt(Constants.BOUNCE_SCORE, 0) + "";
            m_multiHoopScoreText.text = PlayerPrefs.GetInt(Constants.MULTI_HOOP_SCORE, 0) + "";
        }

        public void AddScore(int score)
        {
            if (GameController._instance.LastMode == (int)GAMEMODE.ENDLESS)
            {
                int currentScore = PlayerPrefs.GetInt(Constants.ENDLESS_SCORE, 0);
                if (currentScore < score)
                {
                    PlayerPrefs.SetInt(Constants.ENDLESS_SCORE, score);
					GameCenter.Instance.ReportScore (score);
                }
            }
            else if (GameController._instance.LastMode == (int)GAMEMODE.TIME)
            {
                int currentScore = PlayerPrefs.GetInt(Constants.TIME_SCORE, 0);
                if (currentScore < score)
                {
                    PlayerPrefs.SetInt(Constants.TIME_SCORE, score);
                }
            }
            else if (GameController._instance.LastMode == (int)GAMEMODE.BOUNCE)
            {
                int currentScore = PlayerPrefs.GetInt(Constants.BOUNCE_SCORE, 0);
                if (currentScore < score)
                {
                    PlayerPrefs.SetInt(Constants.BOUNCE_SCORE, score);
                }
            }
            else if (GameController._instance.LastMode == (int)GAMEMODE.MULTI)
            {
                int currentScore = PlayerPrefs.GetInt(Constants.MULTI_HOOP_SCORE, 0);
                if (currentScore < score)
                {
                    PlayerPrefs.SetInt(Constants.MULTI_HOOP_SCORE, score);
                }
            }
        }

        private void GetBallsState()
        {
            string stateStr = "StateBall";
            for (int i = ballsSprite.Length - 2; i >= 0; i--)
            {
                string ballState = stateStr + i;
                if (PlayerPrefs.GetInt(ballState, 0) == 1)
                {
                    checkedsArray[i].transform.parent.GetChild(0).GetComponent<Image>().sprite = ballsSprite[i];
                }
            }
        }

        // Behaviour messages
        void Update()
        {
            if (!m_startRaffle)
            {
                return;
            }
            else
            {
                if (!starRaffle.enabled)
                {
                    starRaffle.enabled = true;

                    SoundManager._instance.PlaySound(Constants.SLOTS_SPINNING_SOUND);
                }

                if (m_raffleAmount != 10)
                {
                    if (Time.timeScale == 0.0f)
                    {
                        Time.timeScale = 0.00001f;
                        m_speedRunBallRaffle = m_speedRunBallRaffle / Time.timeScale;
                    }

                    ballRaffle[0].transform.position -= new Vector3(0.0f, m_speedRunBallRaffle * Time.deltaTime, 0.0f);
                    ballRaffle[1].transform.position -= new Vector3(0.0f, m_speedRunBallRaffle * Time.deltaTime, 0.0f);
                    ballRaffle[2].transform.position -= new Vector3(0.0f, m_speedRunBallRaffle * Time.deltaTime, 0.0f);
                    ballRaffle[3].transform.position -= new Vector3(0.0f, m_speedRunBallRaffle * Time.deltaTime, 0.0f);

                    if (ballRaffle[0].transform.localPosition.y <= -117.0f)
                    {
                        ballRaffle[0].enabled = false;
                    }
                    if (ballRaffle[1].transform.localPosition.y <= -117.0f)
                    {
                        ballRaffle[1].transform.localPosition = new Vector3(
                            ballRaffle[1].transform.localPosition.x,
                            ballRaffle[2].transform.localPosition.y + 133.0f,
                            0.0f);

                        ballRaffle[1].sprite = ballsSprite[(int)Mathf.Round(Random.Range(0.0f, ballsSprite.Length - 1))];
                    }
                    if (ballRaffle[2].transform.localPosition.y <= -117.0f)
                    {
                        ballRaffle[2].transform.localPosition = new Vector3(
                            ballRaffle[2].transform.localPosition.x,
                            ballRaffle[3].transform.localPosition.y + 133.0f,
                            0.0f);

                        ballRaffle[2].sprite = ballsSprite[(int)Mathf.Round(Random.Range(0.0f, ballsSprite.Length - 1))];
                    }
                    if (ballRaffle[3].transform.localPosition.y <= -117.0f)
                    {
                        ballRaffle[3].transform.localPosition = new Vector3(
                            ballRaffle[3].transform.localPosition.x,
                            ballRaffle[1].transform.localPosition.y + 133.0f,
                            0.0f);

                        ballRaffle[3].sprite = ballsSprite[(int)Mathf.Round(Random.Range(0.0f, ballsSprite.Length - 1))];

                        m_raffleAmount++;
                    }
                }
                else
                {
                    if (starRaffle.enabled)
                    {
                        starRaffle.enabled = false;

                        starRaffle.gameObject.transform.GetChild(0).rotation = Quaternion.identity;
                        starRaffle.gameObject.transform.GetChild(1).rotation = Quaternion.identity;

                        m_speedRunBallRaffle = m_speedRunBallRaffle * Time.timeScale;
                        Time.timeScale = 0.0f;
                    }

                    for (int i = ballRaffle.Length - 1; i >= 0; i--)
                    {
                        ballRaffle[i].transform.localPosition = m_ballsRaffleOriginalPos[i];
                    }

                    if (!ballRaffle[0].enabled)
                    {
                        SoundManager._instance.PlaySound(Constants.SLOTS_DONE_SOUND);

                        m_startRaffle = false;
                        m_nextRaffle = true;

                        ballRaffle[0].sprite = ballsSprite[(int)Mathf.Round(Random.Range(0.0f, ballsSprite.Length - 1))];
                        ballRaffle[0].enabled = true;

                        CheckBallUnlock();
                    }
                }
            }
        }

        private void CheckBallUnlock()
        {
            for (int i = ballsSprite.Length - 2; i >= 0; i--)
            {
                if (ballsSprite[i] == ballRaffle[0].sprite)
                {
                    m_ballIndexTemp = i;
                    break;
                }
            }

            Image ballImage = checkedsArray[m_ballIndexTemp].transform.parent.GetChild(0).GetComponent<Image>();
            // Fail
            if (ballImage.sprite == ballRaffle[0].sprite)
            {
                SoundManager._instance.PlaySound(Constants.SPIN_LOSE_SOUND);

                m_luckNextTime.SetActive(true);
            }
            // Succesfull
            else if (ballImage.sprite == ballsSprite[67])
            {
                SoundManager._instance.PlaySound(Constants.SPIN_WIN_SOUND);

                ballImage.sprite = ballRaffle[0].sprite;

                string state = "StateBall" + m_ballIndexTemp;
                PlayerPrefs.SetInt(state, 1);

                m_newNotify.SetActive(true);
                m_playButton.SetActive(true);
            }
        }

        public void SetBestScore()
        {
            if (GameController._instance.LastMode == (int)GAMEMODE.ENDLESS)
            {
                m_bestScoreText.text = PlayerPrefs.GetInt(Constants.ENDLESS_SCORE, 0) + "";
            }
            else if (GameController._instance.LastMode == (int)GAMEMODE.TIME)
            {
                m_bestScoreText.text = PlayerPrefs.GetInt(Constants.TIME_SCORE, 0) + "";
            }
            else if (GameController._instance.LastMode == (int)GAMEMODE.BOUNCE)
            {
                m_bestScoreText.text = PlayerPrefs.GetInt(Constants.BOUNCE_SCORE, 0) + "";
            }
            else if (GameController._instance.LastMode == (int)GAMEMODE.MULTI)
            {
                m_bestScoreText.text = PlayerPrefs.GetInt(Constants.MULTI_HOOP_SCORE, 0) + "";
            }
        }

        public void SetLastScore(int score)
        {
            m_lastScoreText.text = score + "";
        }

        // Play Button is clicked
        public void PlayBtn_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            FadeOutStartMenu();
            GameController._instance.CurrentScoreTextMesh.gameObject.SetActive(true);
        }

        // Game modes button is clicked
        public void GameModesBtn_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            GameController._instance.CanStartGame = false;
            m_gameModesMenu.SetActive(true);
        }

        // Customize button is clicked
        public void CustomizeBtn_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            GameController._instance.CanStartGame = false;
            m_customizeMenu.SetActive(true);
        }

        // Get new items button is clicked
        public void GetNewItemsBtn_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            m_customizeMenu.SetActive(false);

            ResetRaffle();
            ballRaffle[0].sprite = ballsSprite[67];
            m_getNewItemsMenu.SetActive(true);

            m_nextRaffle = true;
        }

        private void ResetRaffle()
        {
            m_luckNextTime.SetActive(false);
            m_newNotify.SetActive(false);
            m_playButton.SetActive(false);
            m_raffleAmount = 0;
        }

        // Handle button is clicked
        public void HandleBtn_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.SLOTS_START_SOUND);

            if (PlayerPrefs.GetInt(Constants.STAR_AMOUNT, 0) >= 100 && m_nextRaffle)
            {
                m_nextRaffle = false;

                int currentStar = PlayerPrefs.GetInt(Constants.STAR_AMOUNT, 0);
                currentStar -= 100;
                PlayerPrefs.SetInt(Constants.STAR_AMOUNT, currentStar);

                m_starAmount.text = currentStar + "";
                m_starAmountInCustomize.text = m_starAmountInGetNewItems.text = m_starAmount.text;

                m_startRaffle = true;
                ResetRaffle();
            }
        }

        // Play button is clicked (belong get new items menu)
        public void PlayBtn2_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            if (Time.timeScale == 0.0f)
            {
                Time.timeScale = 1.0f;
            }

            if (!GameController._instance.CanStartGame)
            {
                GameController._instance.CanStartGame = true;
            }

            m_getNewItemsMenu.SetActive(false);

            checkedsArray[PlayerPrefs.GetInt(Constants.LAST_BALL, 0)].SetActive(false);

            PlayerPrefs.SetInt(Constants.LAST_BALL, m_ballIndexTemp);
            checkedsArray[m_ballIndexTemp].SetActive(true);

            for (int i = GameController._instance.BallsArray.Length - 1; i >= 0; i--)
            {
                GameController._instance.BallsArray[i].GetComponent<SpriteRenderer>().sprite = ballsSprite[m_ballIndexTemp];
            }
        }

        // Back button is clicked (belong game modes menu)
        public void BackBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            if (Time.timeScale != 0.0f)
            {
                TurnOffGameModeMenu();
            }
            else
            {
                m_gameModesMenu.SetActive(false);
                m_pauseMenu.SetActive(true);
            }
        }

        // Back button is clicked (belong customize menu)
        public void BackBtn2_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            if (Time.timeScale != 0.0f)
            {
                GameController._instance.CanStartGame = true;
                m_customizeMenu.SetActive(false);
            }
            else
            {
                m_customizeMenu.SetActive(false);
                m_pauseMenu.SetActive(true);
            }
        }

        private void TurnOffGameModeMenu()
        {
            GameController._instance.CanStartGame = true;
            m_gameModesMenu.SetActive(false);
        }

        // Leader board button is clicked
        public void LeaderBoardBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            // Do something...
        }

        // Rate button is clicked
        public void RateBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);
			Application.OpenURL (RateUrl);

            // Do something...
        }

        // Share button is clicked
        public void ShareBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            // Do something...
        }

        // Gift button is clicked
        public void GiftBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            int currentStar = PlayerPrefs.GetInt(Constants.STAR_AMOUNT, 0);
            currentStar += 10;
            PlayerPrefs.SetInt(Constants.STAR_AMOUNT, currentStar);

            m_starAmount.text = currentStar + "";
            m_starAmountInCustomize.text = m_starAmountInGetNewItems.text = m_starAmount.text;
        }

		public void ShowRewardVideo()
		{
			if (AdsControl.Instance.GetRewardAvailable ())
				AdsControl.Instance.ShowRewardVideo ();
		}

        // Endless button is clicked
        public void EndlessBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            if (Time.timeScale == 0.0f)
            {
                Time.timeScale = 1.0f;
            }

            if (PlayerPrefs.GetInt(Constants.LAST_MODE, 0) != (int)GAMEMODE.ENDLESS)
            {
                ResetGame((int)GAMEMODE.ENDLESS);
            }

            TurnOffGameModeMenu();

            if (m_startMenu)
            {
                FadeOutStartMenu();

                GameController._instance.CurrentScoreTextMesh.gameObject.SetActive(true);

                if (ScoreSection.activeInHierarchy)
                {
                    ScoreSection.SetActive(false);
                }
            }
        }

        // Time challenge button is clicked
        public void TimeBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            if (Time.timeScale == 0.0f)
            {
                Time.timeScale = 1.0f;
            }

            if (PlayerPrefs.GetInt(Constants.LAST_MODE, 0) != (int)GAMEMODE.TIME)
            {
                ResetGame((int)GAMEMODE.TIME);
            }

            TurnOffGameModeMenu();

            if (m_startMenu)
            {
                FadeOutStartMenu();

                GameController._instance.CurrentScoreTextMesh.gameObject.SetActive(true);

                if (ScoreSection.activeInHierarchy)
                {
                    ScoreSection.SetActive(false);
                }
            }
        }

        // Bounce button is clicked
        public void BounceBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            if (Time.timeScale == 0.0f)
            {
                Time.timeScale = 1.0f;
            }

            if (PlayerPrefs.GetInt(Constants.LAST_MODE, 0) != (int)GAMEMODE.BOUNCE)
            {
                ResetGame((int)GAMEMODE.BOUNCE);
            }

            TurnOffGameModeMenu();

            if (m_startMenu)
            {
                FadeOutStartMenu();

                GameController._instance.CurrentScoreTextMesh.gameObject.SetActive(true);

                if (ScoreSection.activeInHierarchy)
                {
                    ScoreSection.SetActive(false);
                }
            }
        }

        // Multi hoops button is clicked
        public void MultiBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            if (Time.timeScale == 0.0f)
            {
                Time.timeScale = 1.0f;
            }

            if (PlayerPrefs.GetInt(Constants.LAST_MODE, 0) != (int)GAMEMODE.MULTI)
            {
                ResetGame((int)GAMEMODE.MULTI);
            }

            TurnOffGameModeMenu();

            if (m_startMenu)
            {
                FadeOutStartMenu();

                GameController._instance.CurrentScoreTextMesh.gameObject.SetActive(true);

                if (ScoreSection.activeInHierarchy)
                {
                    ScoreSection.SetActive(false);
                }
            }
        }

        private void ResetGame(int mode)
        {
            GameController._instance.LastMode = mode;
            PlayerPrefs.SetInt(Constants.LAST_MODE, mode);

            GameController._instance.BallWasThrown = false;
            GameController._instance.StartLerpBoard = false;
            GameController._instance.LerpDone = false;
            GameController._instance.BoardSpecialMove = false;
            GameController._instance.NeedReplaceBall = false;

            GameController._instance.SetUpMode();

            GameController._instance.CurrentBall.SetActive(false);
            GameController._instance.CurrentBall.SetActive(true);
            GameController._instance.CurrentBall.GetComponent<Animation>().Play("ball_appear");
        }

        // Restart button is clicked (in time mode)
        public void RestartBtn_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            ResetGame((int)GAMEMODE.TIME);
            GameController._instance.OverTime();
            GameController._instance.ResetGame();
        }

        // Pause button is clicked
        public void PauseBtn_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            GameController._instance.CanStartGame = false;
            m_pauseMenu.SetActive(true);

            Time.timeScale = 0.0f;
        }

        // Back button is clicked (belong pause menu)
        public void BackBtn3_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            GameController._instance.CanStartGame = true;
            m_pauseMenu.SetActive(false);

            Time.timeScale = 1.0f;
        }

        // Back button is clicked (belong get new items menu)
        public void BackBtn4_Onlick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            m_getNewItemsMenu.SetActive(false);
            m_customizeMenu.SetActive(true);
        }

        // Game mode button is clicked (belong pause menu)
        public void GameModes2Btn_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            m_pauseMenu.SetActive(false);
            GameModesBtn_Onclick();
        }

        // Sound button is clicked (belong pause menu)
        public void SoundBtn_Onclick()
        {
            SoundManager._instance.clickAudio.Play();

            pointer.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            EventSystem.current.RaycastAll(pointer, raycastResult);

            if (raycastResult.Count != 0)
            {
                if (PlayerPrefs.GetInt(Constants.SOUND_STATE, 1) == 1)
                {
                    PlayerPrefs.SetInt(Constants.SOUND_STATE, 0);
                    raycastResult[0].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/sound_off");
                }
                else
                {
                    PlayerPrefs.SetInt(Constants.SOUND_STATE, 1);
                    raycastResult[0].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/sound_on");
                }
            }
        }

        // Customize button is clicked (belong pause menu)
        public void Customize2Btn_Onclick()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            m_pauseMenu.SetActive(false);
            CustomizeBtn_Onclick();
        }

        // Ball button is clicked
        public void SetBall()
        {
            SoundManager._instance.PlaySound(Constants.CLICK_SOUND);

            pointer.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            EventSystem.current.RaycastAll(pointer, raycastResult);

            if (raycastResult.Count != 0)
            {
                Sprite ballSprite = raycastResult[0].gameObject.GetComponent<Image>().sprite;
                if (ballSprite != ballsSprite[ballsSprite.Length - 1])
                {
                    if (GameController._instance.BallsArray[0].GetComponent<SpriteRenderer>().sprite == ballSprite)
                    {
                        m_customizeMenu.SetActive(false);

                        if (!GameController._instance.CanStartGame)
                        {
                            GameController._instance.CanStartGame = true;
                        }
                        if (Time.timeScale == 0.0f)
                        {
                            Time.timeScale = 1.0f;
                        }
                    }
                    else
                    {
                        for (int i = GameController._instance.BallsArray.Length - 1; i >= 0; i--)
                        {
                            GameController._instance.BallsArray[i].GetComponent<SpriteRenderer>().sprite = ballSprite;
                        }

                        for (int i = ballsSprite.Length - 1; i >= 0; i--)
                        {
                            if (ballsSprite[i] == ballSprite)
                            {
                                PlayerPrefs.SetInt(Constants.LAST_BALL, i);
                                break;
                            }
                        }

                        GameObject.FindGameObjectWithTag("Checked").SetActive(false);

                        raycastResult[0].gameObject.transform.parent.GetChild(1).gameObject.SetActive(true);
                    }
                }
            }
        }

        public void FadeOutStartMenu()
        {
            m_playBtn.SetActive(false);
            m_startMenuAnimator.SetBool(m_startMenuHash, true);
            StartMenuShowing = false;
        }

        public void FadeInStartMenu()
        {
            m_startMenuAnimator.SetBool(m_startMenuHash, false);
            StartMenuShowing = true;
        }
    }
}
