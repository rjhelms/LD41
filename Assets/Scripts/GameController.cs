using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public enum GameState
{
    STARTING,
    RUNNING,
    SCROLLING,
    PAUSED,
    WON,
    LOST,
}

public class GameController : MonoBehaviour
{
    [Header("Spawn prefabs")]
    public GameObject[] SpawnPrefabs;
    public GameObject[] BossPrefabs;

    [Header("Resolution and Display")]
    public Camera WorldCamera;
    public int TargetX = 320;
    public int TargetY = 200;
    public Material RenderTexture;
    private float pixelRatioAdjustment;

    [Header("UI Elements")]
    public Image HealthImage;
    public Text BombsText;
    public Text LevelText;
    public Text ScoreText;
    public Image LivesImage;
    public Image CoverPanel;
    public GameObject MusicPlayer;

    public Color CoverPanelBlack;
    public Color CoverPanelClear;
    public float FadeTime;

    [Header("Sounds")]
    public AudioClip PlayerHit;
    public AudioClip EnemyHit;
    public AudioClip PlayerBomb;
    public AudioClip PlayerJump;
    public AudioClip PlayerDead;
    public AudioClip PowerUp;
    public AudioClip LevelClear;
    public AudioClip GameOver;
    public AudioSource Audio;

    [Header("Scroll properties")]
    public int ScreenWidth = 320;
    public int CurrentPosition = 160;
    public int TargetPosition = 0;
    public int ScrollAmount = 296;
    public int ScrollPerFrame = 2;
    public int NumScreens;
    public int CurScreen;


    public GameState State;

    private GameObject[] backgrounds;
    private float fadeTimeLeft;

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
        ActivateEnemies(0);
        backgrounds = GameObject.FindGameObjectsWithTag("Background");
        GenerateLevel();
        LevelText.text = string.Format("LEVEL {0}", ScoreManager.Instance.Level);
        CurScreen = 0;
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
        // TODO: remove debug code
        switch (State)
        {
            case GameState.STARTING:
                fadeTimeLeft -= Time.deltaTime;
                CoverPanel.color = Color.Lerp(CoverPanelBlack, CoverPanelClear, (1 - (fadeTimeLeft / FadeTime)));
                UpdateUI();
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
                    if (ScoreManager.Instance.Level < 5)
                    {
                        SceneManager.LoadScene("main");
                    }
                }
                break;
            case GameState.LOST:
                // TODO: game over state
                fadeTimeLeft -= Time.deltaTime;
                CoverPanel.color = Color.Lerp(CoverPanelClear, CoverPanelBlack, (1 - (fadeTimeLeft / FadeTime)));
                break;
            case GameState.SCROLLING:
            case GameState.RUNNING:
                GameObject toSpawn = null;
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    toSpawn = Instantiate(SpawnPrefabs[0]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    toSpawn = Instantiate(SpawnPrefabs[1]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    toSpawn = Instantiate(SpawnPrefabs[2]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    toSpawn = Instantiate(SpawnPrefabs[3]);
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    toSpawn = Instantiate(SpawnPrefabs[4]);
                }
                if (toSpawn)
                {
                    toSpawn.transform.position += new Vector3(CurrentPosition - 160, 0, 0);
                    toSpawn.GetComponent<BaseActor>().Active = true;
                    toSpawn.GetComponent<BaseActor>().LeftBound += CurrentPosition - 160;
                    toSpawn.GetComponent<BaseActor>().RightBound += CurrentPosition - 160;
                }
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    CanScroll();
                }
                if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    ScoreManager.Instance.Level++;
                    SceneManager.LoadScene("main");
                }
                if (ScoreManager.Instance.Score >= ScoreManager.Instance.NextLife)
                {
                    ScoreManager.Instance.Lives++;
                    ScoreManager.Instance.NextLife *= 2;
                    Audio.PlayOneShot(PowerUp);
                }
                UpdateUI();
                break;
        }
    }

    private void UpdateUI()
    {
        if (ScoreManager.Instance.HitPoints >= 0)
        {
            HealthImage.rectTransform.sizeDelta = new Vector3(ScoreManager.Instance.HitPoints * 16, 16, 0);
        }
        else
        {
            HealthImage.rectTransform.sizeDelta = new Vector3(0, 16, 0);
        }

        BombsText.text = string.Format("BOMBS:{0,3}", ScoreManager.Instance.Bombs);
        ScoreText.text = string.Format("{0}", ScoreManager.Instance.Score);
        LivesImage.rectTransform.sizeDelta = new Vector3(ScoreManager.Instance.Lives * 16, 16, 0);
    }

    private void FixedUpdate()
    {
        if (State == GameState.SCROLLING)
        {
            CurrentPosition += ScrollPerFrame;
            WorldCamera.transform.position = new Vector3(CurrentPosition, WorldCamera.transform.position.y, WorldCamera.transform.position.z);
            if (CurrentPosition == TargetPosition)
                State = GameState.RUNNING;
            foreach (GameObject background in backgrounds)
            {
                background.transform.position += new Vector3(1, 0, 0);
            }
        }
    }
    public bool CanScroll()
    {
        // find all active enemies
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        foreach (BaseEnemy enemy in enemies)
        {
            if (enemy.Active)
            {
                Debug.Log("Found active enemy, can't scroll");
                return false;
            }
        }
        // if none - can scroll
        Debug.Log("No active enemies");
        return true;
    }

    public bool StartScroll()
    {
        if (!CanScroll())
            return false;

        CurScreen++;

        if (CurScreen == NumScreens)
        {
            WinLevel();
            return true;
        }

        // destroy all projectiles
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile projectile in projectiles)
        {
            Destroy(projectile.gameObject);
        }

        // set state to scrolling
        State = GameState.SCROLLING;

        // calculate new position
        TargetPosition = CurrentPosition + ScrollAmount;
        Debug.Log(string.Format("New screen bounds:. {0}, {1}", TargetPosition - 160, TargetPosition + 160));
        // activate new enemies
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        foreach (BaseEnemy enemy in enemies)
        {
            enemy.LeftBound += ScrollAmount;
            enemy.RightBound += ScrollAmount;
            if (enemy.transform.position.x >= TargetPosition - 160 & enemy.transform.position.x <= TargetPosition - 160 + ScreenWidth)
            {
                enemy.Active = true;
            }
        }
        return true;
    }

    private void ActivateEnemies(int leftPosition)
    {
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        foreach (BaseEnemy enemy in enemies)
        {
            if (enemy.transform.position.x >= leftPosition & enemy.transform.position.x <= leftPosition + ScreenWidth)
            {
                enemy.Active = true;
            }
        }
    }

    private void GenerateLevel()
    {
        NumScreens = ScoreManager.Instance.Level + 3;
        int minY = 8;
        int maxY = 97;
        int maxBaddieIndex = ScoreManager.Instance.Level + 2;
        if (ScoreManager.Instance.Level == 4)
        {
            NumScreens = 1;
            maxBaddieIndex = 5;
        }
        for (int curScreen = 0; curScreen < NumScreens; curScreen++)
        {
            int minX = (curScreen * 268) + 160;
            int maxX = (curScreen * 268) + 296;
            Debug.Log(string.Format("Screen {0} bounds: {1} - {2}", curScreen, minX, maxX));
            int numBaddies = curScreen + ScoreManager.Instance.Level + 1;
            for (int curBaddie = 0; curBaddie < numBaddies; curBaddie++)
            {
                // go easy for the first two screens
                int curMaxBaddieIndex = maxBaddieIndex;
                if (curScreen == 0)
                {
                    curMaxBaddieIndex -= 2;
                } else if (curScreen == 1)
                {
                    curMaxBaddieIndex -= 1;
                }
                int xPos = Random.Range(minX, maxX);
                int yPos = Random.Range(minY, maxY);
                Vector3 position = new Vector3(xPos, yPos, yPos);
                int baddieType = Random.Range(0, curMaxBaddieIndex);
                GameObject newBaddie = Instantiate(SpawnPrefabs[baddieType], position, Quaternion.identity);
                if (curScreen == 0)
                {
                    newBaddie.GetComponent<BaseEnemy>().Active = true;
                }
            }
        }
        if (ScoreManager.Instance.Level == 4)
        {
            NumScreens = 2;
            foreach (GameObject Boss in BossPrefabs)
            {
                Instantiate(Boss, Boss.transform.position, Quaternion.identity);
            }
        }
    }

    public void WinLevel()
    {
        Debug.Log("Level won!");
        State = GameState.WON;
        fadeTimeLeft = FadeTime;
        Audio.PlayOneShot(LevelClear);
        ScoreManager.Instance.Level++;

    }

    public void LoseLevel()
    {
        Debug.Log("Game over!");
        Audio.PlayOneShot(GameOver);
        State = GameState.LOST;
        fadeTimeLeft = FadeTime;
    }
}
