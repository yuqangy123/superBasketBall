using UnityEngine;
using System.Collections;

namespace Com.KhuongDuy.Basketball
{
    public class BlurScore : MonoBehaviour
    {
        // Constructor
        private BlurScore() { }

        // Event after show score
        public void TurnOff()
        {
            this.gameObject.SetActive(false);
        }
    }
}
