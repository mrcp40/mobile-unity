using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class JumpHud : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private string _scorePrefix = "";
    [SerializeField] private TMP_Text _bestText;
    [SerializeField] private string _bestPrefix = "BEST ";

    [SerializeField] private GameObject _gameOverRoot;
    [SerializeField] private GameObject _chargeRoot;
    [SerializeField] private Image _chargeFill;
    [SerializeField] private Slider _chargeSlider;

    private void Awake()
    {
        if (_gameOverRoot != null)
            _gameOverRoot.SetActive(false);

        GameObject chargeRoot = GetChargeRoot();
        if (chargeRoot != null)
            chargeRoot.SetActive(false);

    }

    public void SetScore(int score)
    {
        if (_scoreText != null)
            _scoreText.text = _scorePrefix + score;
    }

    public void SetBest(int best)
    {
        if (_bestText != null)
            _bestText.text = _bestPrefix + best;
    }

    public void SetGameOver(bool visible)
    {
        if (_gameOverRoot != null && _gameOverRoot.activeSelf != visible)
            _gameOverRoot.SetActive(visible);
    }

    public void SetCharge(float value01, bool visible)
    {
        GameObject chargeRoot = GetChargeRoot();
        if (chargeRoot != null && chargeRoot.activeSelf != visible)
            chargeRoot.SetActive(visible);

        float value = Mathf.Clamp01(value01);

        if (_chargeSlider != null)
        {
            _chargeSlider.value = value;
            return;
        }

        if (_chargeFill == null)
            return;

        if (_chargeFill.type == Image.Type.Filled)
        {
            _chargeFill.fillAmount = value;
        }
        else
        {
            
            _chargeFill.rectTransform.anchorMax =
                new Vector2(value, _chargeFill.rectTransform.anchorMax.y);
        }
    }

    private GameObject GetChargeRoot()
    {
        if (_chargeRoot != null)
            return _chargeRoot;

        if (_chargeSlider != null)
            return _chargeSlider.gameObject;

        if (_chargeFill != null)
            return _chargeFill.gameObject;

        return null;
    }
}
