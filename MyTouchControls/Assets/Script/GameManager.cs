using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlatformSpawner _spawner;
    [SerializeField] private JumpPlayer _player;
    [SerializeField] private JumpCamera _cameraRig;
    [SerializeField] private JumpHud _hud;

    [SerializeField] private int _maxPoints = 10;
    [SerializeField] private int _minPoints = 1;

    [SerializeField] private float _restartDelay = 0.6f;

    [SerializeField] private bool _showHud = true;
    [SerializeField] private bool _showChargeBar = true;

    public int Score => _score;

    private const string BestKey = "JumpJump.Best";

    private int _score;
    private int _best;
    private bool _isGameOver;
    private float _canRestartAt;

    private void Start()
    {
        if (_spawner == null || _player == null)
        {
            enabled = false;
            return;
        }

        _player.Landed += OnLanded;
        _player.Fell += OnFell;
        _player.PressBegan += OnPressBegan;

        _best = PlayerPrefs.GetInt(BestKey, 0);

        if (_showHud)
        {
            if (_hud == null)
            { 
                _hud = FindAnyObjectByType<JumpHud>();
            }
            else
            {
                _hud.gameObject.SetActive(true);
            }
        }
        else if (_hud != null)
        {
            _hud.gameObject.SetActive(false);
        }

        Restart();
    }

    private void OnDestroy()
    {
        if (_player == null)
            return;

        _player.Landed -= OnLanded;
        _player.Fell -= OnFell;
        _player.PressBegan -= OnPressBegan;
    }

    private void Update()
    {
        if (!_showHud || _hud == null)
            return;

        bool charging = _showChargeBar && _player != null && _player.IsCharging;
        _hud.SetCharge(charging ? _player.Charge01 : 0f, charging);
    }

    public void Restart()
    {
        _score = 0;
        _isGameOver = false;

        _spawner.ResetGame();
        _player.ResetOn(_spawner.Current.transform.position);
        _player.AcceptInput = true;

        if (_cameraRig != null)
            _cameraRig.SnapToTarget();

        RefreshHud();
    }

    private void OnPressBegan()
    {
        if (_isGameOver && Time.time >= _canRestartAt)
            Restart();
    }

    private void OnLanded(JumpPlatform platform)
    {
        if (platform != _spawner.Next)
            return;

        float distance = platform.DistanceToCenter(_player.transform.position);
        float t = Mathf.Clamp01(distance / platform.Radius);

        int points = Mathf.RoundToInt(Mathf.Lerp(_maxPoints, _minPoints, t));
        _score += points;

        _spawner.Advance();
        RefreshHud();
    }

    private void OnFell()
    {
        _isGameOver = true;
        _player.AcceptInput = false;
        _canRestartAt = Time.time + _restartDelay;

        if (_score > _best)
        {
            _best = _score;
            PlayerPrefs.SetInt(BestKey, _best);
            PlayerPrefs.Save();
        }

        RefreshHud();
    }

    private void RefreshHud()
    {
        if (!_showHud || _hud == null)
            return;

        _hud.SetScore(_score);
        _hud.SetBest(_best);
        _hud.SetGameOver(_isGameOver);
        _hud.SetCharge(0f, false);
    }
}
