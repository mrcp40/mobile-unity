using System.Collections.Generic;
using UnityEngine;

public class StoreScreen : MonoBehaviour
{
    [SerializeField] private RectTransform _content=null;
    [SerializeField] private GameObject _upgradeButtonPrefab=null;

    private void OnEnable()
    {
        List<Upgrades.Upgrade> upgrades = GameManager.Instance.GetUpgrades();
        for(int i = 0; i < upgrades.Count; ++i)
        {
            GameObject button=Instantiate(_upgradeButtonPrefab,_content);
            UpgradeButton buttonData=button.GetComponent<UpgradeButton>();
            buttonData.SetData(upgrades[i]);
        }
    }

    private void OnDisable()
    {
        for(int i = 0;i<_content.childCount;++i)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
    }

    public void OnCloseScreen()
    {
        ScreenManager.Instance.CloseScreen();
    }
}
