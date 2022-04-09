using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO
//test game with 3 players
//defines and player settings
//death cause
//gamemode scriptables
//player class
//menu list interface

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
        HandleGameRestart();
    }
    private void Update()
    {
        HandlePhase();
    }
    #endregion
    #region GameRestart
    public void RestartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pview.RPC("HandleGameRestart", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    void HandleGameRestart()
    {
        InitializePlayer();
        InitializeRoom();
    }
    #endregion
    #region GamePhase
    public enum GamePhase
    {
        Loading = 0,
        PreGame = 5,
        Day = 1,
        Defense = 6,
        Voting = 2,
        Night = 3,
        PostGame = 4,
    }
    float NextGamePhaseTime = 0;
    float NextNightTime = 0;
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
                    if (NextNightTime < 0)
                    {
                        int playerCount = GetAllLivingPlayers().Length;
                        BeginNewDay(playerCount > 2 ? 300 : 10);
                    }
                    ResetPlayerVotes();
                }
                break;
            case GamePhase.Night:
                BeginNewDay(-1);
                RunPhaseCountdown(60);
                break;
            case GamePhase.Defense:
                ResetPlayerVotes();
                ChangePhase(GamePhase.Voting);
                break;
            case GamePhase.Voting:
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
            case GamePhase.Night:
                HandleAntagCoordinatedAttack();
                ChangePhase(GamePhase.Day);
                break;
            case GamePhase.Defense:
                ChangePhase(GamePhase.Voting);
                break;
            case GamePhase.Voting:
                if (NextGamePhaseTime > 0)
                    ChangePhase(GamePhase.Day);
                else
                {
                    HandlePlayerLynching(false);
                    ChangePhase(GamePhase.Night);
                }
                break;
            default:
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
    void BeginNewDay(float daytime)
    {
        Debug.Log("[WerewolfGame] Run countdown timer " + daytime);
        NextNightTime = Time.time + daytime;
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "NextNightTime", NextNightTime } });
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
            { "CountDown", 0 },
            { "NextNightTime", 0 }
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
        for (int antag = 0; antag < Mathf.Ceil(GetPlayerCount() * .25f); antag++)
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
    void HandlePlayerAccusations()
    {
        if (RecountPlayerAccusations(out string votedPlayer))
        {
            Debug.Log(votedPlayer + " was lynched");
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "VotedPlayer", votedPlayer } });
            ChangePhase(GamePhase.Voting);
        }
    }
    bool RecountPlayerAccusations(out string votedPlayer)
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
        return totVotes>1;
    }
    #endregion
    #region Lynching Phase
    bool RecountPlayerLynchings(out bool EveryoneVoted)
    {
        EveryoneVoted = true;
        Player[] importantPlayers = GetAllLivingPlayers();
        int votes = 0;
        foreach (Player p in importantPlayers)
        {
            if (p.CustomProperties.TryGetValue("VoteTarget", out var playervote))
            {
                string pVote = (string)playervote;
                if (pVote == "")
                {
                    EveryoneVoted = false;
                }
                else if (pVote == "Y")
                {
                    votes++;
                }
            }
        }
        return votes < Mathf.CeilToInt(importantPlayers.Length / 2);
    }
    void HandlePlayerLynching(bool forwardPhase)
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("VotedPlayer", out object value))
            return;

        string votedPlayer = (string)value;
        bool votedKill = RecountPlayerLynchings(out bool everyoneVoted);
        if (votedKill || everyoneVoted)
        {
            if (votedKill)
            {
                Debug.Log(votedPlayer + " was lynched");
                KillPlayer(GetPlayerByName(votedPlayer));
                if (forwardPhase)
                    ChangePhase(GamePhase.Night);
            }
            else if (everyoneVoted)
            {
                Debug.Log(votedPlayer + " was pardoned");
                if (forwardPhase)
                    GoToNextPhase();
            }
        }
    }
    #endregion
    #region Antag Coordinated Attack
    bool RecountAntagCoordinatedAttack(out string votedPlayer)
    {
        Dictionary<string, int> votesCount = new Dictionary<string, int>();
        Player[] antags = GetLivingAntags();
        foreach (Player p in antags)
        {
            if (p.CustomProperties.TryGetValue("VoteTarget", out var playervote))
            {
                string sVote = (string)playervote;
                if (IsPlayerAntagonist(GetPlayerByName(sVote)))
                    continue;
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
        return totVotes == antags.Length;
    }
    void HandleAntagOrganization()
    {
        if (RecountAntagCoordinatedAttack(out string votedPlayer) && NextGamePhaseTime - Time.time > 5)
        {
            Debug.Log("Antags have decided to kill " + votedPlayer);
            RunPhaseCountdown(5);
        }
    }
    void HandleAntagCoordinatedAttack()
    {
        if (RecountAntagCoordinatedAttack(out string votedPlayer))
        {
            Debug.Log("Antags have killed " + votedPlayer);
            Player murderTarget = GetPlayerByName(votedPlayer);
            if (murderTarget != null && !IsPlayerAntagonist(murderTarget) && IsPlayerAlive(murderTarget))
            {
                KillPlayer(murderTarget);
                ChangePhase(GamePhase.Day);
            }
        }
    }
    #endregion
    #region Player Properties Update
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (changedProps.ContainsKey("FinishedLoading"))
            CheckGameStarted();

        if (changedProps.ContainsKey("PlayerVote"))
        {
            if (CurrentPhase == GamePhase.Day)
                HandlePlayerAccusations();
            else if (CurrentPhase == GamePhase.Voting)
                HandlePlayerLynching(true);
            else if (CurrentPhase == GamePhase.Night)
            HandleAntagOrganization();
        }
    }
    #endregion
    #region Variable Sync
    void UpdateVarsFromServer()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("GamePhase", out object newGamePhase))
        {
            try
            {
                CurrentPhase = (GamePhase)newGamePhase;
            }
            catch
            {
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
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("NextNightTime", out object DayTime))
        {
            try
            {
                NextNightTime = (float)DayTime;
            }
            catch
            {
                NextNightTime = 0;
            }
        }
    }

    public override void OnJoinedRoom()
    {
        UpdateVarsFromServer();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            UpdateVarsFromServer();
        }
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //CheckEndOfGame();
    }

    #endregion
    #region Player Stuff

    void InitializePlayer()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {"FinishedLoading", true},
                {"CustomPortrait", "Player"},
                {"PlayerClass", "Spectator"},
                {"VoteTarget", -1},
                {"WasKilled", false},
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public bool IsPlayerAlive(Player p)
    {

        if (p.CustomProperties.TryGetValue("WasKilled", out var isDead))
        {
            return !(bool)isDead;

        }
        return false;
    }
    public Player[] GetAllLivingPlayers()
    {
        List<Player> players = new List<Player>();
        foreach (Player p in PhotonNetwork.PlayerList)
        {
                if (IsPlayerAlive(p))
                {
                    players.Add(p);
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
    public bool IsPlayerAntagonist(Player targetplayer)
    {
        if (targetplayer.CustomProperties.TryGetValue("PlayerClass", out var playerclass))
            return (string)playerclass == "Werewolf";
        return false;
    }
    void KillPlayer(Player p)
    {
        p.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "WasKilled", true } });
    }
    public bool IsPlayerAntagonist(string className)
    {
        return className == "Werewolf";
    }
    Player GetPlayerByName(string playerName)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.NickName == playerName)
            {
                return p;
            }
        }
        return null;
    }
    #endregion
    #region End Of The Game
    void EndTheGame()
    {

    }
    #endregion
}
