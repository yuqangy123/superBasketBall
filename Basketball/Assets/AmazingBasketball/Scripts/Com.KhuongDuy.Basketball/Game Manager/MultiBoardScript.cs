using UnityEngine;
using System.Collections;

namespace Com.KhuongDuy.Basketball
{
    /// <summary>
    /// Manage single board for multi hoops mode
    /// </summary>
    public class MultiBoardScript : MonoBehaviour
    {
        private GameObject m_lastBoard;

        private Vector3[] m_boardsOriginalPos;

        private float m_speed;

        public GameObject[] boards;

        public bool runToRight;

        // Constructor
        private MultiBoardScript() { }

        //Behaviour messages
        void Start()
        {
            m_boardsOriginalPos = new Vector3[boards.Length];
            for (int i = m_boardsOriginalPos.Length - 1; i >= 0; i--)
            {
                m_boardsOriginalPos[i] = boards[i].transform.localPosition;
            }

            SetLastBoard();

            m_speed = GameController._instance.speedMoveForMultiBoardMode;
        }

        private void SetLastBoard()
        {
            m_lastBoard = boards[0];
            for (int i = boards.Length - 1; i >= 0; i--)
            {
                if (runToRight)
                {
                    if (boards[i].transform.position.x < m_lastBoard.transform.position.x)
                    {
                        m_lastBoard = boards[i];
                    }
                }
                else
                {
                    if (boards[i].transform.position.x > m_lastBoard.transform.position.x)
                    {
                        m_lastBoard = boards[i];
                    }
                }
            }
        }

        //Behaviour messages
        void Update()
        {
            if (GameController._instance.MultiBoardStartRun)
            {
                for (int i = boards.Length - 1; i >= 0; i--)
                {
                    if (runToRight)
                    {
                        if (boards[i].transform.position.x >= 2.5f)
                        {
                            boards[i].transform.position = new Vector3(
                                m_lastBoard.transform.position.x - 2.0f,
                                boards[i].transform.position.y,
                                0.0f);

                            m_lastBoard = boards[i];
                        }

                        boards[i].transform.position += new Vector3(Time.deltaTime * m_speed, 0.0f, 0.0f);
                    }
                    else
                    {
                        if (boards[i].transform.position.x <= -2.5f)
                        {
                            boards[i].transform.position = new Vector3(
                                m_lastBoard.transform.position.x + 2.0f,
                                boards[i].transform.position.y,
                                0.0f);

                            m_lastBoard = boards[i];
                        }

                        boards[i].transform.position -= new Vector3(Time.deltaTime * m_speed, 0.0f, 0.0f);
                    }
                }
            }
        }

        // Behaviour messages
        void OnDisable()
        {
            if (m_boardsOriginalPos != null)
            {
                for (int i = m_boardsOriginalPos.Length - 1; i >= 0; i--)
                {
                    boards[i].transform.localPosition = m_boardsOriginalPos[i];
                }
            }

            SetLastBoard();
        }
    }
}
