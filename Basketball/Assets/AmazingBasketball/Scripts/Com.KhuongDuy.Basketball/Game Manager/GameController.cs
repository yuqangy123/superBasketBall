using UnityEngine;
using System.Collections;

namespace Com.KhuongDuy.Basketball
{
    /// <summary>
    /// 
    /// Manage logic of game
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_board;
        [SerializeField]
        private GameObject m_multiBoard;
        [SerializeField]
        private GameObject m_timeBoard;
        [SerializeField]
        private GameObject m_bars;
        [SerializeField]
        private GameObject m_barsUI;
        [SerializeField]
        private GameObject[] m_ballsArray;
        [SerializeField]
        private GameObject[] m_scoresArray;
        [SerializeField]
        private GameObject m_ballShadow;
        [SerializeField]
        private GameObject m_star;

        private GameObject
            m_currentBall,
            m_multiBoardTopChild,
            m_multiBoardBottomChild,
            m_leftBar,
            m_rightBar;

        [SerializeField]
        private Renderer m_timeRenderer;

        private TextMesh m_timeTextMesh;

        private Rigidbody2D m_currentBallRigid2D;

        private Animation
            m_currentBallAnimation,
            m_boardAnimation;

        private CircleCollider2D m_currentBallCollider;

        [SerializeField]
        private Vector3 m_boardRightPositionToLerp;

        private Vector3
            m_boardOriginalPos,
            m_boardStartPointBeforeLerp;

        private Vector2
            m_firstTouchPos,
            m_secondTouchPos,
            m_currentSwipe;

        [SerializeField]
        private float m_forceThrowBall;
        [SerializeField]
        private float m_forceTorqueBall;
        [SerializeField]
        private float m_maxTimeToSwipe;
        [SerializeField]
        private float m_timeToLerpBoard;

        private float
            m_timeStartSwipe,
            m_timeStartLerpBoard,
            m_second,
            m_milisecond,
            m_throwsCount;

        private int
            m_lastMode,
            m_currentScore;

        private bool
            m_isSwiping,
            m_ballWasThrown,
            m_scored,
            m_canStartGame,
            m_startLerpBoard,
            m_lerpDone,
            m_runTimer,
            m_waitToStartTimeMode,
            m_addStar;

        public static GameController _instance = null;

        public TextMesh CurrentScoreTextMesh;

        public float speedMoveForMultiBoardMode;

        public GameObject[] BallsArray
        {
            get { return m_ballsArray; }
            set { m_ballsArray = value; }
        }

        public GameObject CurrentBall
        {
            get { return m_currentBall; }
            set { m_currentBall = value; }
        }

        public GameObject BallShadow
        {
            get { return m_ballShadow; }
            set { m_ballShadow = value; }
        }

        public GAMESTATE GameState { get; set; }

        public int LastMode
        {
            get { return m_lastMode; }
            set { m_lastMode = value; }
        }

        public bool BallWasThrown
        {
            get { return m_ballWasThrown; }
            set { m_ballWasThrown = value; }
        }

        public bool Scored
        {
            get { return m_scored; }
            set { m_scored = value; }
        }

        public bool CanStartGame
        {
            get { return m_canStartGame; }
            set { m_canStartGame = value; }
        }

        public bool StartLerpBoard
        {
            get { return m_startLerpBoard; }
            set { m_startLerpBoard = value; }
        }

        public bool LerpDone
        {
            get { return m_lerpDone; }
            set { m_lerpDone = value; }
        }

        public bool NeedReplaceBall { get; set; }
        public bool BoardSpecialMove { get; set; }
        public bool MultiBoardStartRun { get; set; }

        // Constructor
        private GameController() { }

        // Behaviour messages
        void Awake()
        {
			Application.targetFrameRate = 60;
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != null)
            {
                Destroy(this.gameObject);
            }
        }

        // Behaviour messages
        void Start()
        {
            SetUp();
        }

        private void SetUp()
        {
            m_canStartGame = true;
            m_currentScore = 0;
            m_currentBall = m_ballsArray[0];

            m_boardOriginalPos = m_board.transform.localPosition;
            m_boardAnimation = m_board.GetComponent<Animation>();

            m_leftBar = m_bars.transform.GetChild(0).gameObject;
            m_rightBar = m_bars.transform.GetChild(1).gameObject;

            m_second = 20.0f;
            m_milisecond = 0.0f;
            m_timeTextMesh = m_timeRenderer.GetComponent<TextMesh>();

            m_multiBoardTopChild = m_multiBoard.transform.GetChild(0).gameObject;
            m_multiBoardBottomChild = m_multiBoard.transform.GetChild(1).gameObject;

            m_lastMode = PlayerPrefs.GetInt(Constants.LAST_MODE, 0);

            SetUpMode();
            SetUpLastBall();
            SetUpCurrentBall();
        }

        public void SetUpMode()
        {
            ResetGame();
            for (int i = m_ballsArray.Length - 1; i >= 0; i--)
            {
                m_ballsArray[i].SetActive(false);
            }

            if (m_lastMode == (int)GAMEMODE.TIME)
            {
                m_board.transform.position = m_boardOriginalPos;
                if (!m_board.activeInHierarchy)
                {
                    m_board.SetActive(true);
                }

                m_timeBoard.SetActive(true);
                m_timeRenderer.sortingOrder = 2;
                UIManager._instance.restartBtn.SetActive(true);

                m_bars.SetActive(false);
                m_barsUI.SetActive(false);

                if (m_multiBoard.activeInHierarchy)
                {
                    m_multiBoard.SetActive(false);
                }

                OverTime();
            }
            else if (m_lastMode == (int)GAMEMODE.ENDLESS)
            {
                m_board.transform.position = m_boardOriginalPos;
                if (!m_board.activeInHierarchy)
                {
                    m_board.SetActive(true);
                }

                DisableTimeModeComponents();

                m_bars.SetActive(false);
                m_barsUI.SetActive(false);

                if (m_multiBoard.activeInHierarchy)
                {
                    m_multiBoard.SetActive(false);
                }
            }
            else if (m_lastMode == (int)GAMEMODE.BOUNCE)
            {
                m_board.transform.position = m_boardOriginalPos;
                if (!m_board.activeInHierarchy)
                {
                    m_board.SetActive(true);
                }

                DisableTimeModeComponents();

                m_bars.SetActive(true);
                m_barsUI.SetActive(true);

                if (m_multiBoard.activeInHierarchy)
                {
                    m_multiBoard.SetActive(false);
                }
            }
            else if (m_lastMode == (int)GAMEMODE.MULTI)
            {
                m_board.SetActive(false);

                DisableTimeModeComponents();

                m_bars.SetActive(false);
                m_barsUI.SetActive(false);

                m_multiBoard.SetActive(true);
            }
            MultiBoardStartRun = false;
        }

        private void DisableTimeModeComponents()
        {
            m_timeBoard.SetActive(false);
            UIManager._instance.restartBtn.SetActive(false);
        }

        private void SetUpLastBall()
        {
            int ballIndex = PlayerPrefs.GetInt(Constants.LAST_BALL, 0);
            for (int i = m_ballsArray.Length - 1; i >= 0; i--)
            {
                m_ballsArray[i].GetComponent<SpriteRenderer>().sprite = UIManager._instance.ballsSprite[ballIndex];
            }
        }

        private void SetUpCurrentBall()
        {
            m_currentBall.SetActive(true);
            m_currentBallRigid2D = m_currentBall.GetComponent<Rigidbody2D>();
            m_currentBallAnimation = m_currentBall.GetComponent<Animation>();
            m_currentBallCollider = m_currentBall.GetComponent<CircleCollider2D>();
        }

        // Behaviour messages
        void Update()
        {
            ThrowTheBall();

            // The ball was thrown
            CheckBallWasThrow();

            ControlBoard();
            RunTime();

            if (m_lastMode != (int)GAMEMODE.TIME)
            {
                GetStar(5, 8);
            }
            else
            {
                GetStar(12, 17);
            }
        }

        private void ThrowTheBall()
        {
            if (m_canStartGame)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 touchPoint = new Vector2(worldPoint.x, worldPoint.y);
                    

                    if (m_lastMode == (int)GAMEMODE.TIME)
                    {
                        if (!m_waitToStartTimeMode)
                        {
                            CheckIfPointerOverBall(ref touchPoint);
                        }
                    }
                    else
                    {
                        CheckIfPointerOverBall(ref touchPoint);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (m_isSwiping)
                {
                    m_isSwiping = false;

                    CheckDirection();
                }
            }

            if (m_isSwiping)
            {
                CheckSwipe();
            }
        }

        private void CheckIfPointerOverBall(ref Vector2 touchPoint)
        {
            Collider2D hit = Physics2D.OverlapPoint(touchPoint);
            if (hit != null)
            {
                if (hit.tag == "Ball")
                {
                    m_isSwiping = true;
                    m_timeStartSwipe = Time.time;
                    m_firstTouchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
            }
        }

        private void CheckSwipe()
        {
            float percentageComplete = (Time.time - m_timeStartSwipe) / m_maxTimeToSwipe;

            if (percentageComplete >= 1.0f)
            {
                m_isSwiping = false;

                CheckDirection();
            }
        }

        private void CheckDirection()
        {
            m_secondTouchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            m_secondTouchPos.x = m_firstTouchPos.x-2.5f;//test code
            m_currentSwipe = new Vector2(m_secondTouchPos.x - m_firstTouchPos.x, m_secondTouchPos.y - m_firstTouchPos.y);
            m_currentSwipe.Normalize();

            if (m_currentSwipe.y > 0.0f)
            {
                float angle = Vector2.Angle(m_currentSwipe, transform.right * -1);
                Debug.Log("angle:"+angle);
                if (30.0f <= angle && angle <= 150.0f)
                {
                    if (m_lastMode != (int)GAMEMODE.BOUNCE)
                    {
                        m_currentBallRigid2D.AddForceAtPosition(
                            m_currentSwipe * m_forceThrowBall, m_currentBall.transform.position, ForceMode2D.Impulse);

                        m_currentBallCollider.enabled = false;
                    }
                    else
                    {
                        Vector2 direction = Vector2.zero;
                        float bonusForce = 0.0f;

                        SetDirection(ref direction, ref angle, ref bonusForce);

                        if (direction == Vector2.zero)
                        {
                            m_currentBallRigid2D.AddForceAtPosition(
                            m_currentSwipe * m_forceThrowBall, m_currentBall.transform.position, ForceMode2D.Impulse);
                        }
                        else
                        {
                            m_currentBallRigid2D.AddForceAtPosition(
                                    direction * (m_forceThrowBall + bonusForce), m_currentBall.transform.position, ForceMode2D.Impulse);
                        }

                        Physics2D.IgnoreLayerCollision(8, 9);
                    }

                    if (m_currentSwipe.x >= 0.0f)
                    {
                        m_forceTorqueBall *= -1.0f;
                    }
                    m_currentBallRigid2D.AddTorque(m_forceTorqueBall, ForceMode2D.Impulse);

                    // Disappear ball shadow
                    m_ballShadow.SetActive(false);

                    m_currentBall.GetComponent<BallScript>().WasThrow = true;
                    m_currentBallAnimation.Play("ball_away");

                    m_scored = false;
                    m_ballWasThrown = true;
                    GameState = GAMESTATE.START;

                    if (UIManager._instance.StartMenuShowing)
                    {
                        UIManager._instance.FadeOutStartMenu();
                        UIManager._instance.ScoreSection.SetActive(false);
                    }

                    CurrentScoreTextMesh.gameObject.SetActive(true);

                    if (m_lastMode == (int)GAMEMODE.TIME)
                    {
                        if (!m_runTimer)
                        {
                            m_runTimer = true;
                        }
                    }

                    m_addStar = false;
                    m_throwsCount++;

                    SoundManager._instance.PlaySound(Constants.THROW_SOUND);
                }
            }
        }

        private void SetDirection(ref Vector2 direction, ref float angle, ref float bonusForce)
        {
            if (81.0f <= angle && angle <= 99.0f)
            {
                direction = new Vector2(0.0f, 1.0f);
            }
            // Left side
            else if (30.0f <= angle && angle <= 45.0f)
            {
                direction = new Vector2(-0.5f, 0.5f);
            }
            else if (46.0f <= angle && angle <= 64.0f)
            {
                direction = new Vector2(-0.3f, 0.8f);
                bonusForce += 0.5f;
            }
            else if (65.0f <= angle && angle <= 74.0f)
            {
                direction = new Vector2(-0.25f, 1.0f);
                bonusForce += 0.7f;
            }
            else if (75.0f <= angle && angle <= 80.0f)
            {
                direction = new Vector2(-0.3f, 1.0f);
                bonusForce += 1.5f;
            }
            // Right side
            else if (135.0f <= angle && angle <= 150.0f)
            {
                direction = new Vector2(0.5f, 0.5f);
            }
            else if (116.0f <= angle && angle <= 134.0f)
            {
                direction = new Vector2(0.3f, 0.8f);
                bonusForce += 0.5f;
            }
            else if (106.0f <= angle && angle <= 115.0f)
            {
                direction = new Vector2(0.25f, 1.0f);
                bonusForce += 0.7f;
            }
            else if (100.0f <= angle && angle <= 105.0f)
            {
                direction = new Vector2(0.3f, 1.0f);
                bonusForce += 1.5f;
            }
        }

        private void CheckBallWasThrow()
        {
            if (m_ballWasThrown)
            {
                if (m_lastMode != (int)GAMEMODE.TIME)
                {
                    if (NeedReplaceBall)
                    {
                        NeedReplaceBall = false;
                        ReplaceNewBall();

                        // If throw the ball slip
                        if (!m_scored)
                        {
                            GameOver();
                        }
                    }
                }
                else
                {
                    ReplaceNewBall();
                }
            }
        }

        private void ReplaceNewBall()
        {
            for (int i = m_ballsArray.Length - 1; i >= 0; i--)
            {
                if (!m_ballsArray[i].activeInHierarchy)
                {
                    m_currentBall = m_ballsArray[i];
                    SetUpCurrentBall();
                    m_currentBallAnimation.Play("ball_appear");

                    m_ballWasThrown = false;
                    break;
                }
            }
        }

        private void ControlBoard()
        {
            if (m_lastMode == (int)GAMEMODE.ENDLESS || m_lastMode == (int)GAMEMODE.TIME)
            {
                if (m_currentScore >= 10 && m_currentScore < 27)
                {
                    if (!m_startLerpBoard)
                    {
                        m_startLerpBoard = true;
                        m_timeStartLerpBoard = Time.time;
                    }

                    if (!m_lerpDone)
                    {
                        float percentage = (Time.time - m_timeStartLerpBoard) / m_timeToLerpBoard;
                        m_board.transform.position = Vector3.Lerp(m_boardOriginalPos, m_boardRightPositionToLerp, percentage);

                        if (percentage >= 1.0f)
                        {
                            m_lerpDone = true;
                            m_boardAnimation.Play("board_run_to_left");
                        }
                    }
                }
                else if (m_currentScore >= 27)
                {
                    BoardSpecialMove = true;
                }
            }
        }

        private void RunTime()
        {
            if (m_runTimer)
            {
                if (m_milisecond >= 60.0f)
                {
                    m_milisecond = 0.0f;
                    m_second -= 1.0f;
                }
                if (m_second == 0.0f)
                {
                    OverTime();
                    GameOver();
                    m_waitToStartTimeMode = true;
                    StartCoroutine(ContinueTimeMode());
                    return;
                }
                m_milisecond += (Time.deltaTime * 60.0f);
                float temp = Mathf.Round(m_milisecond);
                m_timeTextMesh.text = m_second + ":" + (temp >= 10.0f ? temp + "" : ("0" + temp));
            }
        }

        public void OverTime()
        {
            m_timeTextMesh.text = "00:00";
            m_second = 20.0f;
            m_milisecond = 0.0f;
            m_runTimer = false;
        }

        private void GetStar(float minTime, float maxTime)
        {

            if (m_throwsCount != 0)
            {
                if (m_throwsCount == minTime)
                {
                    if (!m_addStar)
                    {
                        m_addStar = true;

                        if (Random.value <= 0.5f)
                        {
                            m_star.SetActive(true);
                            m_throwsCount = 0;

                            SoundManager._instance.PlaySound(Constants.STAR_SOUND);
                        }
                    }
                }
                else if (m_throwsCount == maxTime)
                {
                    if (!m_addStar)
                    {
                        m_addStar = true;
                        m_star.SetActive(true);
                        m_throwsCount = 0;

                        SoundManager._instance.PlaySound(Constants.STAR_SOUND);
                    }
                }
            }
        }

        private void GameOver()
        {
            UIManager._instance.AddScore(m_currentScore);
            UIManager._instance.SetScore();
            UIManager._instance.SetBestScore();
            UIManager._instance.SetLastScore(m_currentScore);

            UIManager._instance.FadeInStartMenu();
            UIManager._instance.ScoreSection.SetActive(true);
			AdsControl.Instance.showAds ();

            CurrentScoreTextMesh.gameObject.SetActive(false);

            if (m_lastMode == (int)GAMEMODE.MULTI)
            {
                m_multiBoardTopChild.SetActive(false);
                m_multiBoardTopChild.SetActive(true);
                m_multiBoardBottomChild.SetActive(false);
                m_multiBoardBottomChild.SetActive(true);
            }
            else if (m_lastMode == (int)GAMEMODE.BOUNCE)
            {
                m_leftBar.SetActive(false);
                m_leftBar.SetActive(true);
                m_rightBar.SetActive(false);
                m_rightBar.SetActive(true);
            }

            ResetGame();
        }

        public void ResetGame()
        {
            m_currentScore = 0;

            CurrentScoreTextMesh.text = "0";

            if (m_boardAnimation.isPlaying)
            {
                m_boardAnimation.Stop();
            }
            m_board.transform.position = m_boardOriginalPos;

            m_startLerpBoard = false;
            m_lerpDone = false;
            BoardSpecialMove = false;
            MultiBoardStartRun = false;

            GameState = GAMESTATE.OVER;
        }

        private IEnumerator ContinueTimeMode()
        {
            yield return new WaitForSeconds(2.0f);
            m_waitToStartTimeMode = false;
        }

        public void Score(GameObject board, bool collision)
        {
            if (m_lastMode == (int)GAMEMODE.MULTI)
            {
                MultiBoardStartRun = true;
            }

            m_scored = true;

            GameObject scoreObj = null;
            if (m_lastMode == (int)GAMEMODE.MULTI)
            {
                scoreObj = board.transform.GetChild(0).gameObject;
            }
            else
            {
                for (int i = m_scoresArray.Length - 1; i >= 0; i--)
                {
                    if (!m_scoresArray[i].activeInHierarchy)
                    {
                        scoreObj = m_scoresArray[i];
                    }
                }
            }

            TextMesh scoreTextMesh = scoreObj.GetComponent<TextMesh>();

            if (collision)
            {
                scoreTextMesh.text = "+1";
                m_currentScore += 1;

                SoundManager._instance.PlaySound(Constants.BOUNCE_HIT_SOUND);
            }
            else
            {
                scoreTextMesh.text = "+2";
                m_currentScore += 2;

                SoundManager._instance.PlaySound(Constants.HIT_BASKET_SOUND);
            }
            scoreObj.GetComponent<Renderer>().sortingOrder = 2;
            scoreObj.SetActive(true);

            CurrentScoreTextMesh.text = m_currentScore + "";
        }

        public void ScoreInBounceMode(GameObject board, int score, Vector2 contactPoint)
        {
            m_scored = true;

            if (score != 0)
            {
                GameObject scoreObj = board.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
                scoreObj.transform.localPosition = new Vector3(scoreObj.transform.localPosition.x, contactPoint.y, 0.0f);

                TextMesh scoreTextMesh = scoreObj.GetComponent<TextMesh>();

                scoreTextMesh.text = "+" + score;
                m_currentScore += score;

                scoreObj.GetComponent<Renderer>().sortingOrder = 2;
                scoreObj.SetActive(true);

                CurrentScoreTextMesh.text = m_currentScore + "";

                SoundManager._instance.PlaySound(Constants.BOUNCE_HIT_SOUND);
            }
        }
    }

    public enum GAMEMODE
    {
        ENDLESS, TIME, BOUNCE, MULTI
    }

    public enum GAMESTATE
    {
        START, OVER
    }
}
