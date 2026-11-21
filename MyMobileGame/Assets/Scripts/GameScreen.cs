using UnityEngine;

public class GameScreen : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.AddScore();
    }

    public void GoToStore()
    {
        ScreenManager.Instance.OpenScreen("StoreScreen");
    }
}
