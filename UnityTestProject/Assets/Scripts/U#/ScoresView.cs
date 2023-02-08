namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;
    using TMPro;

    /// <summary>
    /// Keeps track of and displays player scores
    /// </summary>
    [AddComponentMenu("OpsLib/ScoresView")]
    public class ScoresView : UdonSharpBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI TMPField;
        [SerializeField]
        private string PlayerListText;
        [SerializeField]
        [UdonSynced]
        private string[] _playerNames = new string[10];
        [SerializeField]
        [UdonSynced]
        private int[] _playerScores = new int[10];

        void Start()
        {
            UpdateView();
        }

        public override void OnDeserialization()
        {
            UpdateView();
        }

        public void AddPlayerScore(string playerName, int score)
        {
            //Take ownership of the object so we can modify its state
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);

            //First, look to see if the player's name is already in the list
            for (int i = 0; i < _playerNames.Length; i++)
            {
                if (_playerNames[i] == playerName)
                {
                    _playerScores[i] += score;
                    SortArrays();
                    UpdateView();
                    RequestSerialization();
                    return;
                }
            }

            //Else, If the player's name was not found in the list, we need to add it to the first available spot
            for (int j = 0; j < _playerNames.Length; j++)
            {
                if (string.IsNullOrEmpty(_playerNames[j]))
                {
                    _playerNames[j] = playerName;
                    _playerScores[j] = score;
                    SortArrays();
                    UpdateView();
                    RequestSerialization();
                    return;
                }
            }
            
            //Else, If our new player's score is higher than an existing player's score, we need to bump them off the list
            //NOTE: If a player gets bumped off the list, we will lose all their score information, so we may want to increase
            //      the PlayerNames/Scores array sizes if we expect many more players and then truncate the data on the UI side
            if(score > _playerScores[_playerScores.Length - 1])
            {
                _playerNames[_playerScores.Length - 1] = playerName;
                _playerScores[_playerScores.Length - 1] = score;
                SortArrays();
                UpdateView();
                RequestSerialization();
            }
        }

        public void Test()
        {
            string playerName = $"TestPlayer {Random.Range(0, 20)}";
            int playerScore = Random.Range(0, 100);
            Debug.Log($"Adding {playerScore} points for player {playerName}");
            AddPlayerScore(playerName, playerScore);
        }

        /// <summary>
        /// Just a simple bubble sort
        /// </summary>
        private void SortArrays()
        {
            for (int i = 0; i < _playerScores.Length - 1; i++)
            {
                for (int j = 0; j < _playerScores.Length - i - 1; j++)
                {
                    if (_playerScores[j] < _playerScores[j + 1])
                    {
                        int tempScore = _playerScores[j];
                        string tempName = _playerNames[j];
                        _playerScores[j] = _playerScores[j + 1];
                        _playerScores[j + 1] = tempScore;
                        _playerNames[j] = _playerNames[j + 1];
                        _playerNames[j + 1] = tempName;
                    }
                }
            }
        }

        private void UpdateView()
        {
            if (TMPField != null)
            {
                PlayerListText = "";
                for (int i = 0; i < _playerNames.Length; i++)
                {
                    if (!string.IsNullOrEmpty(_playerNames[i]))
                        PlayerListText += $"{i + 1}. {_playerNames[i]} \t Wins: {_playerScores[i]}\n";
                }

                TMPField.text = PlayerListText;
            }
        }
    }
}