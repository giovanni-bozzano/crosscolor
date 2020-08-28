using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class MenuController : MonoBehaviour
{ 
    public void ChangeScene(string scene) => SceneManager.LoadScene(scene, LoadSceneMode.Single);

    public void SignIn()
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_leaderboard);
        }
        else
        {
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
                    });
                }
            });
        }
    }
}
