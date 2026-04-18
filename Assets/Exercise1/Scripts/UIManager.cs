using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameHUDPanel;
    public GameObject endScreenPanel;

    [Header("Menu UI")]
    public TMP_InputField ipInput;
    public TMP_InputField portInput; // 如果需要自定义端口，可以通过 Transport 组件修改
    public TMP_Text connectionInfoText;
    public TMP_Text timeoutMsgText;

    [Header("Game UI")]
    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;
    public TMP_Text statusText;
    public TMP_Text endScreenMsgText;

    private string lastIP = "localhost";

    void Awake()
    {
        if (Instance == null) Instance = this;
        // 获取本地IP并显示 (简单获取IPv4)
        connectionInfoText.text = $"Local IP: {NetworkManager.singleton.networkAddress}";
    }

    public void HostGame()
    {
        NetworkManager.singleton.StartHost();
        SwitchToHUD();
    }

    public void JoinGame()
    {
        lastIP = string.IsNullOrEmpty(ipInput.text) ? "localhost" : ipInput.text;
        NetworkManager.singleton.networkAddress = lastIP;
        NetworkManager.singleton.StartClient();
        StartCoroutine(CheckConnectionTimeout(5f)); // 5秒超时
    }

    private IEnumerator CheckConnectionTimeout(float timeoutTime)
    {
        timeoutMsgText.text = "Connecting...";
        float timer = 0;
        while (timer < timeoutTime)
        {
            if (NetworkClient.isConnected)
            {
                SwitchToHUD();
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // 超时处理
        NetworkManager.singleton.StopClient();
        timeoutMsgText.text = "Connection Timed Out!";
    }

    public void PlayAgain()
    {
        endScreenPanel.SetActive(false);
        // 如果是原Host（服务器端依然活跃或通过某种标识记录），重新启动Host
        // 这里做一个简单判断，如果你之前是Host端，按Play Again就再开Host
        // 否则就用缓存的 lastIP 重新 Join
        // 实际开发中可以引入 PlayerPrefs 缓存上次身份
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame() => Application.Quit();

    // -- 供其他模块调用的更新方法 --
    public void UpdateP1Score(int score) => p1ScoreText.text = score.ToString();
    public void UpdateP2Score(int score) => p2ScoreText.text = score.ToString();
    public void UpdateStatus(string msg) => statusText.text = msg;

    public void ShowEndScreen(bool isWinner)
    {
        gameHUDPanel.SetActive(false);
        endScreenPanel.SetActive(true);
        endScreenMsgText.text = isWinner ? "VICTORY" : "DEFEAT";
    }

    public void PlaySound(string soundType)
    {
        // 在这里播放音效，比如 AudioManager.Play(soundType)
    }

    private void SwitchToHUD()
    {
        mainMenuPanel.SetActive(false);
        gameHUDPanel.SetActive(true);
        timeoutMsgText.text = "";
    }
}