using UnityEngine;

public class MainScreen : MonoBehaviour
{
    public void OpenScreen(string screenName)
    {
        ScreenManager.Instance.OpenScreen(screenName);
    }
}
