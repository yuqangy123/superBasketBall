using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms;
//using UnityEngine.SocialPlatforms.GameCenter;

public class GameCenter : MonoBehaviour
{

	public string leaderboardName = "leaderboard01";

	bool gameOver = false;

	protected GameCenter ()
	{
	}

	private static GameCenter _instance;


	public static GameCenter Instance { get { return _instance; } }

	void Awake ()
	{

		if (FindObjectsOfType (typeof(AdsControl)).Length > 1) {
			Destroy (gameObject);
			return;
		}

		_instance = this;
	


		DontDestroyOnLoad (gameObject); 

	
	}

	void Start ()
	{

		#if UNITY_IPHONE
		Social.localUser.Authenticate (success => {
			if (success) {
				Debug.Log ("Authentication successful");
			} else {
				Debug.Log ("Authentication failed");
			}
		});
		#endif


	}

	public void ReportScore (long score)
	{
		#if UNITY_IPHONE
		//Debug.Log("Reporting score " + score + " on leaderboard " + leaderboardID);
		Social.ReportScore (score, leaderboardName, success => {
			if (success) {
				Debug.Log ("Reported score successfully");
			} else {
				Debug.Log ("Failed to report score");
			}

			Debug.Log (success ? "Reported score successfully" : "Failed to report score");
			Debug.Log ("New Score:" + score);  
		});
		#endif
	}

	public  void ShowLeaderboard ()
	{
		#if UNITY_IPHONE
		Social.ShowLeaderboardUI ();
		#endif
	}

}

