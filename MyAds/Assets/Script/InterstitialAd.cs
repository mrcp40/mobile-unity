using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener,IUnityAdsShowListener
{
    [SerializeField] private Button _button=null;

    [SerializeField] private string _androidAdId = "";
    [SerializeField] private string _iosAdId = "";

    private string _activeAdId = "";
    private bool _isAdLoaded=false;

    private void Awake()
    {
        _activeAdId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iosAdId : _androidAdId;
    }
    private void Start()
    {
        
    }

    public void ShowAd()
    {
        if(!_isAdLoaded)
        {
            Advertisement.Load(_activeAdId, this);
            _button.interactable = false;
        }
        Advertisement.Show(_activeAdId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    { 
        _button.interactable=true;
        _isAdLoaded=true;
    }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    { 
    
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
    
    }

    public void OnUnityAdsShowStart(string placementId)
    { 
    
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
    
    }
}
