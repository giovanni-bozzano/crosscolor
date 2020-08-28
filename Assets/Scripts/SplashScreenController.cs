using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class SplashScreenController : MonoBehaviour
{
    void Start()
    {
        Input.multiTouchEnabled = false;
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate(success => {
            if (success)
            {
                ILeaderboard leaderboard = PlayGamesPlatform.Instance.CreateLeaderboard();
                leaderboard.id = GPGSIds.leaderboard_leaderboard;
                string[] userFilter = { Social.localUser.id };
                leaderboard.SetUserFilter(userFilter);
                leaderboard.LoadScores(loadSuccess =>
                {
                    if (loadSuccess && leaderboard.localUserScore != null && leaderboard.localUserScore.date.Ticks > 621355968000000000)
                    {
                        GameController.RecordValue = leaderboard.localUserScore.value / 1000;
                    }
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                });
            }
            else
            {
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
        });
    }
}
