using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText=null;
    [SerializeField] private Image _cookieImage=null;
    [SerializeField] private Upgrades _upgrades=null;

    private Upgrades.Upgrade _currentUpgrade=null;
    private long _points = 0;

    private static GameManager _instance;
    public static GameManager Instance {  get { return _instance; } }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            UpdateScore();
            _currentUpgrade = _upgrades.upgradeList[0];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateScore()
    {
        _scoreText.text="Score: "+_points.ToString();
    }

    public void AddScore()
    {
        _points += _currentUpgrade.clickAmount;
        UpdateScore();
    }

    public bool UsePoints(long amount)
    {
        if(amount <= _points)
        {
            _points -= amount;
            return true;
        }
        return false;
    }

    public long GetPoints()
    {
        return _points;
    }

    public List<Upgrades.Upgrade> GetUpgrades()
    {
        return _upgrades.upgradeList;
    }

    public bool PurchaseUpgrade(Upgrades.Upgrade upgrade)
    {
        if(UsePoints(upgrade.cost))
        {
            _currentUpgrade =upgrade;
            _cookieImage.sprite = upgrade.sprite;
            UpdateScore();
            return true;
        }
        return false;
    }
}
