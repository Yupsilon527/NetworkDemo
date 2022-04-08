using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfGameController : MonoBehaviourPunCallbacks
{
    public static WerewolfGameController main = null;
    public PhotonView pview;
    #region Unity
    public void Awake()
    {
        main = this;
    }

    private void Start()
    {
        InitializeRoom();
    }
    private void Update()
    {
        HandlePhase();
    }
    #endregion
    #region GamePhase
    public enum GamePhase
    {
        Loading = 0,
        PreGame = 5,
        Day = 1,
        Voting = 2,
        Night = 3,
        PostGame = 4,
    }
    float NextGamePhaseTime = 0;
    public GamePhase CurrentPhase = GamePhase.Loading;
    void ChangePhase(GamePhase nPhase)
    {
        Debug.Log("[WerewolfGame] Change phase to " + nPhase);
        if (PhotonNetwork.IsMasterClient)
        {
            pview.RPC("OnGamephaseChange", RpcTarget.AllViaServer, nPhase);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "GamePhase", (int)CurrentPhase } });
        }
    }
    [PunRPC]
    void OnGamephaseChange(GamePhase nPhase, PhotonMessageInfo info)
    {
        NextGamePhaseTime = -1;
        switch (nPhase)
        {
            case GamePhase.Loading:
                break;
            case GamePhase.PreGame:
                RunPhaseCountdown(3);
                break;
            case GamePhase.Day:
                if (GetLivingAntags().Length == 0)
                {
                    Debug.Log("Village won!");
                }
                else if (GetLivingVillagers().Length == 0)
                {
                    Debug.Log("Antags won!");
                }
                else
                {
                    int playerCount = GetAllLivingPlayers().Length;
                    RunPhaseCountdown(playerCount > 2 ? 300 : 10);
                    ResetPlayerVotes();
                }
                break;
            case GamePhase.Voting:
                ResetPlayerVotes();
                break;
        }
        CurrentPhase = nPhase;
    }
    void HandlePhase()
    {
        //Debug.Log("[WerewolfGame] Coutdown time " + CurrentPhase +" "+(NextGamePhaseTime - Time.time));
        if (NextGamePhaseTime > 0 && NextGamePhaseTime < Time.time)
            GoToNextPhase();
    }
    void GoToNextPhase()
    {
        switch (CurrentPhase)
        {
            case GamePhase.Day:
                ChangePhase(GamePhase.Night);
                break;
            case GamePhase.Voting:
                if (NextGamePhaseTime > 0)
                    ChangePhase(GamePhase.Day);
                else
                    ChangePhase(GamePhase.Night);
                break;
            default:
                ChangePhase(GamePhase.Day);
                break;
        }
    }
    void RunPhaseCountdown(float time)
    {
        Debug.Log("[WerewolfGame] Run countdown timer " + time);
        NextGamePhaseTime = Time.time + time;
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CountDown", NextGamePhaseTime } });
    }
    #endregion
    #region Start The Game

    void InitializeRoom()
    {
        CurrentPhase = GamePhase.Loading;
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "GamePhase", CurrentPhase },
            { "VotedPlayer", "" },
            { "CountDown", 0 }
        });
    }

    void StartTheGame()
    {
        ChangePhase(GamePhase.PreGame);
        InitializePlayerRoles();
    }
    public int GetPlayerCount()
    {
        return PhotonNetwork.PlayerList.Length;
    }
    void InitializePlayerRoles()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        List<Player> PunPlayers = new List<Player>();
        PunPlayers.AddRange(PhotonNetwork.PlayerList);

        List<string> Roles = new List<string>();
        for (int antag = 0; antag < Mathf.Max(1, GetPlayerCount() * .25f); antag++)
        {
            Roles.Add("Werewolf");
        }
        for (int civ = Roles.Count; civ < PunPlayers.Count; civ++)
        {
            Roles.Add("Villager");
        }

        for (int player = 0; PunPlayers.Count>0;player++)
        {
            Player p = PunPlayers[Mathf.FloorToInt((PunPlayers.Count - 1) * Random.value)];
            p.SetCustomProperties( new  ExitGames.Client.Photon.Hashtable{ {"PlayerClass", Roles[player] } });
            PunPlayers.Remove(p);
        }

    }

    #endregion
    #region Loading Phase
    protected void CheckGameStarted()
    {
        if (CheckAllPlayerLoadedLevel())
        {
            StartTheGame();
        }
    }
    protected bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue("FinishedLoading", out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }

    #endregion
    #region Voting Phase
    void ResetPlayerVotes()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            p.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VoteTarget", "" } });
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {{ "VotedPlayer", "" }});
    }
    bool RecountPlayerVotes(out string votedPlayer)
    {
        Dictionary<string, int> votesCount = new Dictionary<string, int>();
        foreach (Player p in GetAllLivingPlayers())
        {
            if (p.CustomProperties.TryGetValue("VoteTarget", out var playervote))
            {
                string sVote = (string)playervote;
                if (votesCount.ContainsKey(sVote))
                {
                    votesCount[sVote]++;
                }
                else
                {
                    votesCount.Add(sVote, 1);
                }
            }
        }
        votedPlayer = "";
        int totVotes = 0;
        foreach (KeyValuePair<string, int> votes in votesCount)
        {
            if (votes.Value > totVotes)
            {
                votedPlayer = votes.Key;
                totVotes = votes.Value;
            }
        }
        return totVotes>2;
    }
    #endregion
    #region PuN
    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Werewolf Lobby Scene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("GamePhase", out object newGamePhase))
            {
                try {
                    CurrentPhase = (GamePhase)newGamePhase;
                }
                catch                 {
                    CurrentPhase = GamePhase.Loading;
                }
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("CountDown", out object CountDown))
            {
                try
                {
                    NextGamePhaseTime = (float)CountDown;
                }
                catch
                {
                    NextGamePhaseTime = 0;
                }
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //CheckEndOfGame();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (changedProps.ContainsKey("FinishedLoading"))
        {
            CheckGameStarted();
        }

        if (CurrentPhase == GamePhase.Day && changedProps.ContainsKey("PlayerVote"))
        {
            if (RecountPlayerVotes(out string votedPlayer))
            {
                Debug.Log(targetPlayer.NickName + " has accused " + votedPlayer);
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VotedPlayer", votedPlayer } });
                ChangePhase(GamePhase.Voting);
            }
        }
    }

    #endregion
    #region End Of The Game
    public Player[] GetAllLivingPlayers()
    {
        List<Player> players = new List<Player>();
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("PlayerClass", out var playerclass) && p.CustomProperties.TryGetValue("WasKilled", out var isDead))
            {
                if (!(bool)isDead)
                {
                    players.Add(p);
                }
            }
        }
        return players.ToArray();
    }
    public Player[] GetLivingAntags()
    {
        List<Player> antags = new List<Player>();
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("PlayerClass", out var playerclass) && p.CustomProperties.TryGetValue("WasKilled", out var isDead))
            {
                if (!(bool)isDead && IsPlayerAntagonist((string)playerclass))
                {
                    antags.Add(p);
                }
            }
        }
        return antags.ToArray();
    }
    public Player[] GetLivingVillagers()
    {
        List<Player> villagers = new List<Player>();
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("PlayerClass", out var playerclass) && p.CustomProperties.TryGetValue("WasKilled", out var isDead))
            {
                if (!(bool)isDead && !IsPlayerAntagonist((string)playerclass))
                    {
                        villagers.Add(p);
                    }
            }
        }
        return villagers.ToArray() ;
    }
    bool IsPlayerAntagonist(string className)
    {
        return className == "Werewolf";
    }
    void EndTheGame()
    {

    }
    #endregion
}
