using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms;
#if ADS_PLUGIN
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine.Advertisements;
using Com.KhuongDuy.Basketball;
public class AdsControl : MonoBehaviour
{
	
	
	protected AdsControl ()
	{
	}
	
	private static AdsControl _instance;
	#if ADS_PLUGIN
	ShowOptions options;
	InterstitialAd interstitial;
	#endif
	public string AdmobID, UnityID, UnityZoneID;
	public static AdsControl Instance { get {
			return _instance;
		} }
	
	void Awake ()
	{
		
		if (FindObjectsOfType (typeof(AdsControl)).Length > 1) {
			Destroy (gameObject);
			return;
		}
		
		_instance = this;
		MakeNewInterstial ();

		
		DontDestroyOnLoad (gameObject); //Already done by CBManager

		#if ADS_PLUGIN
		if (Advertisement.isSupported) { // If the platform is supported,
			Advertisement.Initialize(UnityID); // initialize Unity Ads.
		}
		options = new ShowOptions();
		//options.IUnityAdsListener;
		//options.resultCallback
		options.resultCallback = HandleShowResult;
		//IUnityAdsListener.AddListener(HandleShowResult);
		//IUnityAdsListener and call Advertisement.AddListener()
		#endif
	}


	public void HandleInterstialAdClosed (object sender, EventArgs args)
	{
		#if ADS_PLUGIN
		if (interstitial != null)
			interstitial.Destroy ();
		MakeNewInterstial ();
		#endif

		
	}
	void MakeNewInterstial ()
	{

#if ADS_PLUGIN
#if UNITY_ANDROID
		interstitial = new InterstitialAd (AdmobID);
#endif
#if UNITY_IPHONE
		interstitial = new InterstitialAd (AdmobID);
#endif
		interstitial.OnAdClosed += HandleInterstialAdClosed;
		AdRequest request = new AdRequest.Builder ().Build ();
		interstitial.LoadAd (request);
#endif

	}


	public void showAds ()
	{
		int random =UnityEngine.Random.Range (0, 3);
		if (random == 1) {
			#if ADS_PLUGIN
				interstitial.Show ();
			#endif
		}
	}
	

	public bool GetRewardAvailable ()
	{
		bool avaiable = false;
		return avaiable;
	}

	public void ShowRewardVideo ()
	{
		#if ADS_PLUGIN
		Advertisement.Show (UnityZoneID, options);
		#endif

		
	}
	public void HideBannerAds ()
	{
	}
	public void ShowBannerAds ()
	{
	}
	#if ADS_PLUGIN
	private void HandleShowResult (ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			FindObjectOfType<UIManager> ().GiftBtn_Onlick ();
			break;
		case ShowResult.Skipped:
			break;
		case ShowResult.Failed:
			break;
		}
	}
	#endif
}

