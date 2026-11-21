using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _inputBlocker=null;
    [SerializeField] private List<GameObject> _screens = new List<GameObject>();
    [SerializeField] private string _initialScreen = "";
    [SerializeField] private float _transitionDuration = 1.0f;

    private GameObject _topScreen=null;
    private Coroutine _transitCoroutine=null;

    static private ScreenManager _instance=null;
    public static ScreenManager Instance { get { return _instance; } }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            foreach(var screen in _screens)
            {
                screen.SetActive(_initialScreen == screen.name);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenScreen(string screenName)
    {
        _topScreen = null;
        foreach(GameObject screen in _screens)
        {
            if(screen.name == screenName)
            {
                _topScreen= screen;
                break;
            }
        }

        if(_transitCoroutine != null)
        {
            StopCoroutine( _transitCoroutine );
        }
        _transitCoroutine = StartCoroutine(OpenScreen(_topScreen));
    }

    public void CloseScreen()
    {
        if (_topScreen == null)
        {
            return;
        }
        if (_transitCoroutine != null)
        {
            StopCoroutine(_transitCoroutine );
        }
        _transitCoroutine = StartCoroutine(CloseScreen(_topScreen));
    }

    IEnumerator OpenScreen(GameObject go)
    {
        _inputBlocker.SetActive(true);
        RectTransform rectTransform = go.transform as RectTransform;
        rectTransform.anchoredPosition=new Vector2(-Screen.width, rectTransform.anchoredPosition.y);
        go.SetActive(true);

        Tween screenTween = rectTransform.DOAnchorPosX(0.0f, _transitionDuration).SetEase(Ease.OutExpo);
        yield return screenTween.WaitForCompletion();

        screenTween.Kill();
        _inputBlocker.SetActive(false);
    }

    IEnumerator CloseScreen(GameObject go)
    {
        _inputBlocker.SetActive(true);
        RectTransform rectTransform = go.transform as RectTransform;

        Tween screenTween = rectTransform.DOAnchorPosX(-Screen.width, _transitionDuration).SetEase(Ease.OutExpo);
        yield return screenTween.WaitForCompletion();

        screenTween.Kill();
        go.SetActive(false);

        _inputBlocker.SetActive(false);
    }
}
