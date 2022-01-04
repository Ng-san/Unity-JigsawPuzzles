using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using GoogleMobileAds.Api;

public class Admob : MonoBehaviour
{
    public float fulltime =0;
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private static Admob _instance;
    public static Admob instance
    {
        get{
            if(_instance == null){
                _instance = FindObjectOfType<Admob>();
            }
            return _instance;
        }
    }
    public  float TimeShowAdmob;
    void Update()
    {
        // if(RemoteConfig.instance.checkFetchData){
                // fulltime += Time.deltaTime;
                if(fulltime < TimeShowAdmob)
                {
                    fulltime += Time.deltaTime;
                }
        // }
    }
     public void RequestBanner()
    {

        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        this.bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);

        
    }
    
    public void RequestInterstitial()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
            string adUnitId = "unexpected_platform";
        #endif
        if(this.interstitial != null)
        {
            this.interstitial.Destroy();
        }
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);

        this.interstitial.OnAdLoaded += Interstitial_OnAdLoaded;
    }

    private void Interstitial_OnAdLoaded(object sender, System.EventArgs e)
    {
        ShowInterstitial();
    }
    
    public void ShowInterstitial()
    {
        if(this.interstitial.IsLoaded())
        {
            interstitial.Show();
            
        }
        else{
            Debug.Log("Interstitial Ad is not ready yet");
        }
    }
}
