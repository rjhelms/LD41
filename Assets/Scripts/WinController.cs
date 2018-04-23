using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class WinController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image CoverPanel;
    public Color CoverPanelBlack;
    public Color CoverPanelClear;
    public Text ScoreText;
    public float FadeTime;
    private float fadeTimeLeft;
    public GameState State;
    public Camera WorldCamera;
    public int TargetX = 320;
    public int TargetY = 200;
    public Material RenderTexture;
    public GameObject MusicPlayer;
    private float pixelRatioAdjustment;

    // Use this for initialization
    void Start()
    {
        pixelRatioAdjustment = (float)TargetX / (float)TargetY;
        if (pixelRatioAdjustment <= 1)
        {
            RenderTexture.mainTextureScale = new Vector2(pixelRatioAdjustment, 1);
            RenderTexture.mainTextureOffset = new Vector2((1 - pixelRatioAdjustment) / 2, 0);
            WorldCamera.orthographicSize = TargetY / 2;
        }
        else
        {
            pixelRatioAdjustment = 1f / pixelRatioAdjustment;
            RenderTexture.mainTextureScale = new Vector2(1, pixelRatioAdjustment);
            RenderTexture.mainTextureOffset = new Vector2(0, (1 - pixelRatioAdjustment) / 2);
            WorldCamera.orthographicSize = TargetX / 2;
        }
        fadeTimeLeft = FadeTime;
        CoverPanel.color = CoverPanelBlack;
        GameObject musicPlayer = GameObject.FindGameObjectWithTag("Music");
        if (!musicPlayer)
        {
            musicPlayer = Instantiate(MusicPlayer);
            DontDestroyOnLoad(musicPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        ScoreText.text = string.Format("{0}", ScoreManager.Instance.Score);
        switch (State)
        {
            case GameState.STARTING:
                fadeTimeLeft -= Time.deltaTime;
                CoverPanel.color = Color.Lerp(CoverPanelBlack, CoverPanelClear, (1 - (fadeTimeLeft / FadeTime)));
                if (fadeTimeLeft <= 0)
                {
                    CoverPanel.color = CoverPanelClear;
                    Debug.Log("Starting!");
                    State = GameState.RUNNING;
                }
                break;
            case GameState.WON:
                fadeTimeLeft -= Time.deltaTime;
                CoverPanel.color = Color.Lerp(CoverPanelClear, CoverPanelBlack, (1 - (fadeTimeLeft / FadeTime)));
                if (fadeTimeLeft <= 0)
                {
                    ScoreManager.Instance.Reset();
                    SceneManager.LoadScene("title");
                }
                break;
            case GameState.RUNNING:
                if (Input.anyKeyDown)
                {
                    fadeTimeLeft = FadeTime;
                    State = GameState.WON;
                }
                break;
        }
    }
}
