using UnityEngine;
using System.Collections;

namespace Com.KhuongDuy.Basketball
{
    public class StarScript : MonoBehaviour
    {
        // Constructor
        private StarScript() { }

        // End Animation of the star
        public void AddStar()
        {
            UIManager._instance.AddStar();
            UIManager._instance.SetStarAmount();
            this.gameObject.SetActive(false);
        }
    }
}
