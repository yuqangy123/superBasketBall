using UnityEngine;
using System.Collections;

namespace Com.KhuongDuy.Basketball
{
    public class BarScript : MonoBehaviour
    {
        [SerializeField]
        private float m_speedDown;

        public RectTransform barFilling;

        public bool Impact { get; set; }

        // Constructor
        private BarScript() { }

        // Behaviour messages
        void Update()
        {
            if (Impact)
            {
                Impact = false;

                barFilling.localScale += new Vector3(0.0f, 0.2f, 0.0f);
                barFilling.localScale = new Vector3(barFilling.localScale.x, Mathf.Clamp01(barFilling.localScale.y), 1.0f);
            }

            if (barFilling.localScale.y > 0.0f)
            {
                barFilling.localScale -= new Vector3(0.0f, Time.deltaTime * m_speedDown, 0.0f);
            }
        }

        public int GetScore()
        {
            if (0.1f <= barFilling.localScale.y && barFilling.localScale.y <= 0.2f)
            {
                return 1;
            }
            else if (0.2f < barFilling.localScale.y && barFilling.localScale.y <= 0.4f)
            {
                return 2;
            }
            else if (0.4f < barFilling.localScale.y && barFilling.localScale.y <= 0.6f)
            {
                return 3;
            }
            else if (0.6f < barFilling.localScale.y && barFilling.localScale.y <= 0.8f)
            {
                return 4;
            }
            else if (0.8f < barFilling.localScale.y && barFilling.localScale.y <= 1.0f)
            {
                return 5;
            }

            return 1;
        }

        // Behaviour messages
        void OnDisable()
        {
            barFilling.localScale = new Vector3(barFilling.localScale.x, 0.0f, 1.0f);
        }
    }
}

