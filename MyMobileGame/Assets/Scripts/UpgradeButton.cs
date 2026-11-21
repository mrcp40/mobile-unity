using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _costText = null;
    [SerializeField] private Image _upgradeImage = null;
    [SerializeField] private Button _button = null;

    private Upgrades.Upgrade _upgradeData = null;
    private long _prevPoints = 0;

    public void SetData(Upgrades.Upgrade upgrade)
    {
        _upgradeData = upgrade;
        _prevPoints = GameManager.Instance.GetPoints();
        UpdateDisplay();
    }
    private void Update()
    {
        if(_prevPoints!=GameManager.Instance.GetPoints())
        {
            UpdateDisplay();
            _prevPoints = GameManager.Instance.GetPoints();
        }
    }
    public void UpdateDisplay()
    {
        _costText.text=_upgradeData.cost.ToString();
        _upgradeImage.sprite=_upgradeData.sprite;
        _button.interactable = _upgradeData.cost <= GameManager.Instance.GetPoints();
    }

    public void OnClick()
    {
        if(GameManager.Instance.PurchaseUpgrade(_upgradeData))
        {
            UpdateDisplay();
        }
    }
}
