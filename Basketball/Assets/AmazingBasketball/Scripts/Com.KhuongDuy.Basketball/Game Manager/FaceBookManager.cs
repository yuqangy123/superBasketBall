using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
#if FACEBOOK_SDK
using Facebook.Unity;
using Facebook.MiniJSON;
#endif
public class FaceBookManager : MonoBehaviour
{

	List<string> readPermission = new List<string> () { "public_profile", "user_friends", "user_games_activity" },
		publishPermission = new List<string> () { "publish_actions" };


	public string androidLinkShare,iosLinkShare;

	public string rateLink;

	public enum STATE
	{
		SHARE,
		INVITE}

	;

	private STATE currentState;
	private static FaceBookManager _instance;
	public static FaceBookManager Instance { get { return _instance; } }
	void Awake()
	{
		if (FindObjectsOfType (typeof(AdsControl)).Length > 1) {
			Destroy (gameObject);
			return;
		}
		_instance = this;
		DontDestroyOnLoad (gameObject);
	}
	// Use this for initialization
	void Start ()
	{
		InitFB ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	private void InitFB ()
	{
		Debug.Log("FaceBookManager InitFB");
		#if FACEBOOK_SDK
		if (!FB.IsInitialized) {
			FB.Init (InitCallback, onHideUnity);
		} else {
			FB.ActivateApp ();
		}
		#endif
	}

	private void InitCallback ()
	{
		Debug.Log("FaceBookManager InitCallback");
		#if FACEBOOK_SDK
		if (FB.IsInitialized) {
			//PrintLog("Initialized !");
			Debug.Log("<color=yellow>FaceBookManager Initialized!</color>");
		} else {
			Debug.Log("<color=yellow>FaceBookManager Failed to Initialize the Facebook SDK!</color>");
		}
		#endif
	}

	// Perform Unity Tasks When App is Connecting To Facebook
	private void onHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void ShareOnFB ()
	{
		currentState = STATE.SHARE;

		#if UNITY_IOS
		string shareLink = iosLinkShare;
		#endif

		#if UNITY_ANDROID
		string shareLink = androidLinkShare;
		#endif

		#if FACEBOOK_SDK
		if (FB.IsLoggedIn) {
			FB.ShareLink (
				contentURL: new Uri (shareLink),
				callback: delegate (IShareResult shareRes) {
					if (string.IsNullOrEmpty (shareRes.Error) && !shareRes.Cancelled) {
						Debug.Log ("Posting Successful!");
					} else
						Debug.Log ("Posting Unsuccessful!");
				}
			);
		} else
			LoginFB ();
		#endif
		
	}

	void LoginFB ()
	{
		Debug.Log("FaceBookManager LoginFB");
		#if FACEBOOK_SDK
		if (FB.IsLoggedIn) {
			Debug.Log ("FaceBookManager Logged In !");
		} else {
			FB.ActivateApp ();
			FB.LogInWithReadPermissions (readPermission, LoginCallback);
		}
		#endif
	}

	#if FACEBOOK_SDK

	//Callback method of login
	void LoginCallback (ILoginResult result)
	{
		if (FB.IsLoggedIn) {
			Debug.Log ("FaceBookManager Logged In Successfully!");
			if (currentState == STATE.SHARE)
				ShareOnFB ();
			else if (currentState == STATE.INVITE)
				NativeInviteFriendsFB ();
		} else {
			Debug.Log ("FaceBookManager User cancelled login");
		}
	}
	#endif
	// Native Invite!
	public void NativeInviteFriendsFB ()
	{
		Debug.Log ("FaceBookManager NativeInviteFriendsFB");
		currentState = STATE.SHARE;
		#if FACEBOOK_SDK
		if (FB.IsLoggedIn) {
			FB.AppRequest (
				"Let's play zombie street trsigger", null, null, null, null, null, "Let's play",
				callback: delegate (IAppRequestResult result) {
					if (result.RawResult != null) {
						if (DeserializeJSONFriends (result.RawResult) != "")
						if (GetFriendList (DeserializeJSONFriends (result.RawResult)).Count > 0) {
							if (PlayerPrefs.GetInt ("Invite") == 0) {
								
							}
						}
					}
				});
		} else
			LoginFB ();
		#endif
	}

	public void Rate ()
	{
		Application.OpenURL (rateLink);
	}

	public string DeserializeJSONFriends (string response)
	{
		
		string friendID = "";
		#if FACEBOOK_SDK
		var dict = Json.Deserialize (response) as IDictionary;
		friendID = dict ["to"].ToString ();
		#endif
		return friendID;
	}

	private List<string> GetFriendList (string _friend)
	{
		List<string> friendList = new List<string> ();
		if (_friend.Contains (",")) {
			string[] _str = _friend.Split (',');
			for (int i = 0; i < _str.Length; i++) {
				friendList.Add (_str [i]);
			}
		} else
			friendList.Add (_friend);
		return friendList;
	}

}
