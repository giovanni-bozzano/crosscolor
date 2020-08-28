using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using GooglePlayGames;
using static GridButton;

public class GameController : MonoBehaviour
{
    public static long? RecordValue { get; set; } = null;
    private Text time;
    private Text record;
    private GameObject quitPopup;
    private GameObject victoryPopup;
    private Text victoryMessage;
    private Text victoryTime;
    private Text victoryRecord;
    private GameObject victoryQuit;
    private GameObject victoryLogin;
    private GameObject victoryQuitLarge;
    private float timer;
    private bool paused;
    private bool started;

    public void Start()
    {
        time = GameObject.Find("Time").GetComponent<Text>();
        record = GameObject.Find("Record").GetComponent<Text>();
        quitPopup = GameObject.Find("Quit Popup");
        victoryPopup = GameObject.Find("Victory Popup");
        victoryMessage = victoryPopup.transform.Find("Panel/Message").GetComponent<Text>();
        victoryTime = victoryPopup.transform.Find("Panel/Canvas/Canvas/Time").GetComponent<Text>();
        victoryRecord = victoryPopup.transform.Find("Panel/Canvas/Canvas/Record").GetComponent<Text>();
        victoryQuit = victoryPopup.transform.Find("Panel/Buttons/Quit").gameObject;
        victoryLogin = victoryPopup.transform.Find("Panel/Buttons/Login").gameObject;
        victoryQuitLarge = victoryPopup.transform.Find("Panel/Buttons/Quit Large").gameObject;
        quitPopup.SetActive(false);
        victoryPopup.SetActive(false);
        if (RecordValue.HasValue)
        {
            string minutes = Mathf.Floor(RecordValue.Value / 60).ToString("00");
            string seconds = (RecordValue.Value % 60).ToString("00");
            record.text = string.Format("{0}:{1}", minutes, seconds);
            victoryRecord.text = string.Format("{0}:{1}", minutes, seconds);
        }
        Initialize();
        InvokeRepeating("UpdateTimer", 1.0f, 1.0f);
    }

    public void Update()
    {
        if (!paused && started)
        {
            timer = timer + Time.deltaTime;
        }
    }

    public void Initialize()
    {
        started = false;
        timer = 0;
        UpdateTimer();
        GridButton[] buttons = GameObject.Find("Grid").GetComponentsInChildren<GridButton>();
        for (int counter = 0; counter < 100; counter++)
        {
            int random = Random.Range(0, buttons.Length - 1);
            buttons[random].SimulatePress();
        }
        foreach (GridButton button in buttons)
        {
            button.RenderNewColor();
        }
    }

    public void ChangeScene(string scene) => SceneManager.LoadScene(scene, LoadSceneMode.Single);

    public void UpdateTimer()
    {
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        time.text = string.Format("{0}:{1}", minutes, seconds);
    }

    public void CheckVictory()
    {
        started = true;
        GridButton[] buttons = GameObject.Find("Grid").GetComponentsInChildren<GridButton>();
        GridColor color = buttons[0].Color;
        foreach (GridButton button in buttons)
        {
            if (button.Color != color)
            {
                return;
            }
        }
        ShowVictoryPopup();
    }

    public void ShowQuitPopup()
    {
        paused = true;
        quitPopup.SetActive(true);
    }

    public void HideQuitPopup()
    {
        paused = false;
        GameObject.Find("No").GetComponent<AnimatedButton>().Reset();
        quitPopup.SetActive(false);
    }

    public void ShowVictoryPopup()
    {
        paused = true;
        if (Social.localUser.authenticated)
        {
            Social.ReportScore((long)timer * 1000, GPGSIds.leaderboard_leaderboard, success => { });
            if (!RecordValue.HasValue || timer < RecordValue)
            {
                RecordValue = (long)timer;
                victoryMessage.text = "YOU ALSO SCORED A NEW PERSONAL RECORD";
            } else
            {
                victoryMessage.text = "YOU DID NOT BEAT YOUR PREVIOUS PERSONAL RECORD";
            }
            victoryQuit.SetActive(false);
            victoryLogin.SetActive(false);
        } else
        {
            victoryMessage.text = "LOGIN TO VIEW YOUR PERSONAL RECORD";
            victoryQuitLarge.SetActive(false);
        }
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        victoryTime.text = string.Format("{0}:{1}", minutes, seconds);
        victoryPopup.SetActive(true);
    }

    public void SignIn() => Social.localUser.Authenticate(success =>
    {
        if (success)
        {
            ILeaderboard leaderboard = PlayGamesPlatform.Instance.CreateLeaderboard();
            leaderboard.id = GPGSIds.leaderboard_leaderboard;
            string[] userFilter = { Social.localUser.id };
            leaderboard.SetUserFilter(userFilter);
            leaderboard.LoadScores(loadSuccess => {
                if (loadSuccess && leaderboard.localUserScore != null && leaderboard.localUserScore.date.Ticks > 621355968000000000)
                {
                    RecordValue = leaderboard.localUserScore.value / 1000;
                    string minutes = Mathf.Floor(RecordValue.Value / 60).ToString("00");
                    string seconds = (RecordValue.Value % 60).ToString("00");
                    victoryRecord.text = string.Format("{0}:{1}", minutes, seconds);
                    if (timer < RecordValue)
                    {
                        RecordValue = (long)timer;
                        victoryMessage.text = "YOU ALSO SCORED A NEW PERSONAL RECORD";
                    }
                    else
                    {
                        victoryMessage.text = "YOU DID NOT BEAT YOUR PREVIOUS PERSONAL RECORD";
                    }
                }
                Social.ReportScore((long)timer * 1000, GPGSIds.leaderboard_leaderboard, reportSuccess => { });
                victoryQuit.SetActive(false);
                victoryLogin.SetActive(false);
                victoryQuitLarge.SetActive(true);
            });
        }
    });
}
