using UnityEngine;
using System.Collections;

namespace Com.KhuongDuy.Basketball
{
    public class BallScript : MonoBehaviour
    {
        private Rigidbody2D rg2D;

        private CircleCollider2D m_ballCircleColliders;

        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        private Vector3 m_ballOriginalPos;

        private Quaternion m_ballOriginalRotation;

        private string m_typeBoard;

        private bool
            m_isImpactWithHoop,
            m_isImpactWithBars,
            m_throughBasket;

        public bool WasThrow { get; set; }

        // Constructor
        private BallScript() { }

        // Behaviour messages
        void Awake()
        {
            rg2D = GetComponent<Rigidbody2D>();
            m_ballCircleColliders = GetComponent<CircleCollider2D>();
        }

        // Behaviour messages
        void Start()
        {
            m_ballOriginalPos = transform.localPosition;
            m_ballOriginalRotation = transform.localRotation;
        }

        // Behaviour messages
        void Update()
        {
            if (WasThrow)
            {
                if (rg2D.velocity.y <= 0.0f)
                {
                    if (GameController._instance.LastMode == (int)GAMEMODE.BOUNCE)
                    {
                        Physics2D.IgnoreLayerCollision(8, 9, false);
                        m_spriteRenderer.sortingOrder = -1;
                    }

                    m_ballCircleColliders.enabled = true;
                }

                if (transform.position.y <= -1.0f)
                {
                    if (GameController._instance.LastMode != (int)GAMEMODE.BOUNCE)
                    {
                        m_ballCircleColliders.enabled = false;
                    }
                    else
                    {
                        if (!m_throughBasket)
                        {
                            GameController._instance.Scored = false;
                        }

                        Physics2D.IgnoreLayerCollision(8, 10);
                    }
                }
            }

            if (transform.position.y <= -3.0f)
            {
                if (GameController._instance.LastMode != (int)GAMEMODE.TIME)
                {
                    SoundManager._instance.PlaySound(Constants.END_SOUND);
                }

                GameController._instance.NeedReplaceBall = true;
                this.gameObject.SetActive(false);
            }
            else
            {
                return;
            }
        }

        // Behaviour messages
        void OnDisable()
        {
            Reset();
        }

        private void Reset()
        {
            transform.position = m_ballOriginalPos;
            transform.rotation = m_ballOriginalRotation;

            m_spriteRenderer.sortingOrder = 2;
            m_ballCircleColliders.enabled = true;
            m_typeBoard = "";

            m_isImpactWithHoop = false;
            m_isImpactWithBars = false;
            m_throughBasket = false;
            WasThrow = false;

            Physics2D.IgnoreLayerCollision(8, 10, false);
        }

        // Behaviour messages
        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.tag == "Hoop")
            {
                m_isImpactWithHoop = true;

                SoundManager._instance.PlaySound(Constants.BOUNCE_SOUND);
            }

            if (col.gameObject.tag == "Bar")
            {
                m_isImpactWithBars = true;

                BarScript barScript = col.gameObject.GetComponent<BarScript>();
                barScript.Impact = true;
                GameController._instance.ScoreInBounceMode(col.transform.gameObject, barScript.GetScore(), col.contacts[0].point);
            }
        }

        // Behaviour messages
        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Snare 1")
            {
                m_typeBoard = col.name;
            }

            if (col.tag == "Snare 2")
            {
                if (GameController._instance.GameState != GAMESTATE.OVER)
                {
                    if (m_typeBoard == col.name)
                    {
                        if (GameController._instance.LastMode != (int)GAMEMODE.BOUNCE)
                        {
                            GameController._instance.Score(col.transform.parent.parent.gameObject, m_isImpactWithHoop);
                        }
                        else
                        {
                            if (!m_isImpactWithBars)
                            {
                                GameController._instance.ScoreInBounceMode(col.transform.gameObject, 0, Vector2.zero);
                                m_throughBasket = true;
                            }
                            else
                            {
                                m_throughBasket = true;
                            }

                            SoundManager._instance.PlaySound(Constants.HIT_BASKET_SOUND);
                        }
                    }
                }
            }
        }

        // Make effects the ball enters the basket
        public void BallIsHighestPoint()
        {
            m_spriteRenderer.sortingOrder = -1;
        }

        // Make effects the ball appear
        public void BallAppear()
        {
            GameController._instance.BallShadow.SetActive(true);
        }
    }
}
