using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour,IUnityAdsInitializationListener
{
    [SerializeField] private bool _testMode = true;
    [SerializeField] private string _androidGameId = "";
    [SerializeField] private string _iosGameId = "";

    private string _activeGameId = "";

    static AdManager _instance;
    public static AdManager Instance {  get { return _instance; } }

    private void Awake()
    {
        if(_instance == null)
        {

        _activeGameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iosGameId : _androidGameId;
        Advertisement.Initialize(_activeGameId, _testMode, this);
        _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Ad initialized");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error,string message)
    {
        Debug.Log($"Ad failed to initialized:{error.ToString()} - {message}");

    }
}
