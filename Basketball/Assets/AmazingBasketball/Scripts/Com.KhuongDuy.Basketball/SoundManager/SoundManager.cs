using UnityEngine;
using System.Collections;

namespace Com.KhuongDuy.Basketball
{
    /// <summary>
    /// Manage sounds of game
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager _instance = null;

        public AudioClip[] bounceAudioClips;

        public AudioSource 
            throwAudio,
            endAudio,
            bounceAudio,
            bounceHitAudio,
            hitTheBasketAudio,
            starAudio,
            clickAudio,
            slotsStartAudio,
            slotsSpinningAudio,
            slotsDoneAudio,
            spinLoseAudio,
            spinWinAudio;

        // Constructor
        private SoundManager() { }

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
        }

        public void PlaySound(string name)
        {
            if (PlayerPrefs.GetInt(Constants.SOUND_STATE, 1) == 1)
            {
                switch (name)
                {
                    case Constants.THROW_SOUND:
                        throwAudio.Play();
                        break;
                    case Constants.END_SOUND:
                        endAudio.Play();
                        break;
                    case Constants.BOUNCE_SOUND:
                        if (Random.value <= 0.3f)
                        {
                            bounceAudio.clip = bounceAudioClips[0];
                        }
                        else if (Random.value > 0.3f && Random.value <= 0.7f)
                        {
                            bounceAudio.clip = bounceAudioClips[1];
                        }
                        else
                        {
                            bounceAudio.clip = bounceAudioClips[2];
                        }
                        bounceAudio.Play();
                        break;
                    case Constants.BOUNCE_HIT_SOUND:
                        bounceHitAudio.Play();
                        break;
                    case Constants.HIT_BASKET_SOUND:
                        hitTheBasketAudio.Play();
                        break;
                    case Constants.STAR_SOUND:
                        starAudio.Play();
                        break;
                    case Constants.CLICK_SOUND:
                        clickAudio.Play();
                        break;
                    case Constants.SLOTS_START_SOUND:
                        slotsStartAudio.Play();
                        break;
                    case Constants.SLOTS_SPINNING_SOUND:
                        slotsSpinningAudio.Play();
                        break;
                    case Constants.SLOTS_DONE_SOUND:
                        slotsDoneAudio.Play();
                        break;
                    case Constants.SPIN_LOSE_SOUND:
                        spinLoseAudio.Play();
                        break;
                    case Constants.SPIN_WIN_SOUND:
                        spinWinAudio.Play();
                        break;
                }
            }
        }
    }
}
