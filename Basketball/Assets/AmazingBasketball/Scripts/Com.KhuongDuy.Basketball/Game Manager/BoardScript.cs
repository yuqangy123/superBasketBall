using UnityEngine;
using System.Collections;

namespace Com.KhuongDuy.Basketball
{
    /// <summary>
    /// Manage single board for Endless, Timer and Bounce modes
    /// </summary>
    public class BoardScript : MonoBehaviour
    {
        private Animation m_anim;

        // Constructor
        private BoardScript() { }

        // Behaviour messages
        void Awake()
        {
            m_anim = GetComponent<Animation>();
        }

        // Select the direction to move after reach left side
        public void ReachLeftSide()
        {
            if (!GameController._instance.BoardSpecialMove)
            {
                m_anim.Play("board_run_to_right");
            }
            else
            {
                if (Random.value <= 0.5f)
                {
                    m_anim.Play("board_run_to_right");
                }
                else
                {
                    m_anim.Play("board_run_to_down_right");
                }
            }
        }

        // Select the direction to move after reach right side
        public void ReachRightSide()
        {
            if (!GameController._instance.BoardSpecialMove)
            {
                m_anim.Play("board_run_to_left");
            }
            else
            {
                if (Random.value <= 0.5f)
                {
                    m_anim.Play("board_run_to_left");
                }
                else
                {
                    m_anim.Play("board_run_to_down_left");
                }
            }
        }
    }
}
