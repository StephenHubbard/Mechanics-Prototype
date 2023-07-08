using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

// Not gonna use this class atm
public class Leaderboard : MonoBehaviour
{
    [SerializeField] private bool _activateLeaderBoard = false;
    [SerializeField] private GameObject _leaderboardContainer;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private TextMeshProUGUI _playerNames;
    [SerializeField] private TextMeshProUGUI _playerScores;
    [SerializeField] private GameObject _loadingText;
    [SerializeField] private Score _score;

    // for lootlocker api
    const string LEADERBOARD_ID = "mechanicsPrototype";

    private void Start()
    {
        if (_activateLeaderBoard) {
            StartCoroutine(GuestLoginRoutine());
            StartCoroutine(SetupRoutine());
        }
    }

    public void SubmitButton() {
        StartCoroutine(SubmitButtonRoutine());
    }

    private IEnumerator GuestLoginRoutine()
    {
        bool gotResponse = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                gotResponse = true;
            }
            else
            {
                gotResponse = true;
            }
        });

        yield return new WaitWhile(() => gotResponse == false);
    }

    private IEnumerator SubmitButtonRoutine() {
        _playerNameInputField.gameObject.SetActive(false);
        yield return SetPlayerName();
        yield return SubmitScoreRoutine(_score.CurrentScore);
        yield return FetchTopHighScoresRoutine();
    } 

    private IEnumerator SetupRoutine()
    {
        yield return LoginRoutine();
        yield return FetchTopHighScoresRoutine();
    }

    private IEnumerator SetPlayerName()
    {
        bool done = false;

        LootLockerSDKManager.SetPlayerName(_playerNameInputField.text, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully set player name");
                done = true;
            }
            else
            {
                Debug.Log("Could not set player name" + response.Error);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator SubmitScoreRoutine(int scoreToUpload) {
        bool done = false;
        string playerID = _playerNameInputField.text;

        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, LEADERBOARD_ID, (response) => 
        {
            if (response.success) {
                Debug.Log("Successfully uploaded score");
                done = true;
            } else {
                Debug.Log("Failed" + response.Error);
                done = true;
            }
        });
        
        _playerNameInputField.text = "";

        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator FetchTopHighScoresRoutine() {
        bool done = false;
        LootLockerSDKManager.GetScoreList(LEADERBOARD_ID, 10, 0, (response) => {
            if (response.success) {
                string tempPlayerNames = "Names\n";
                string tempPlayerScores = "Scores\n";

                LootLockerLeaderboardMember[] members = response.items;

                for (int i = 0; i < members.Length; i++)
                {
                    tempPlayerNames += members[i].rank + ". ";

                    if (members[i].member_id != "") {
                        tempPlayerNames += members[i].member_id;
                    } 

                    tempPlayerScores += members[i].score + "\n";
                    tempPlayerNames += "\n";
                }
                done = true;
                _playerNames.text = tempPlayerNames;
                _playerScores.text = tempPlayerScores;
                Debug.Log("Successfully loaded player high scores.");
                _loadingText.SetActive(false);
            } else {
                Debug.LogErrorFormat("Failed {0}", response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    private IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player was logged in");
                done = true;
            }
            else
            {
                Debug.Log("Could not start session");
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }
}
