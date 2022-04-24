using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO
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
        InitializePlayer();
        InitializeRoom();
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
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
        InitializeRoom();
        CheckGameStarted();
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
    public float NextGamePhaseTime = 0;
    public float NextNightTime = 0;
    public GamePhase CurrentPhase = GamePhase.Loading;
    void ChangePhase(GamePhase nPhase)
    {
        Debug.Log("[WerewolfGame] Change phase to " + nPhase);
        if (PhotonNetwork.IsMasterClient)
        {
            pview.RPC("OnGamephaseChange", RpcTarget.AllBufferedViaServer, nPhase);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { WerewolfGameDefines.GamePhase, (int)CurrentPhase } });
        }
    }
    [PunRPC]
    void OnGamephaseChange(GamePhase nPhase, PhotonMessageInfo info)
    {
        Debug.Log("[WerewolfGame] Server change phase to " + nPhase);
        RunPhaseCountdown( -1);
        if (GetLivingAntags().Length == 0 || GetLivingVillagers().Length == 0)
        {
            Debug.Log("[WerewolfGame] Game Over! One side won!");
            ChangePhase(GamePhase.PostGame);
        }
        switch (nPhase)
        {
            case GamePhase.Loading:
                break;
            case GamePhase.PreGame:
                RunPhaseCountdown(WerewolfGameDefines.PregameTime);
                break;
            case GamePhase.Day:
                
                        int playerCount = GetAllLivingPlayers().Length;
                if (playerCount > 2)
                {
                    if (NextNightTime <= 0)
                    {
                        if (IsFirstDay())
                            BeginNewDay(WerewolfGameDefines.FirstDayDuration);
                        else 
                            BeginNewDay(WerewolfGameDefines.DayTimeDuration);
                    }
                    ResetPlayerVotes();
                }
                else
                {
                    Debug.Log("[WerewolfGame] Game Over; Not Enough Players");
                    ChangePhase(GamePhase.PostGame);
                }
                break;
            case GamePhase.Night:
                ResetPlayerVotes();
                BeginNewDay(-1);
                RunPhaseCountdown(WerewolfGameDefines.NightTimeDuration);
                break;
            case GamePhase.Defense:
                ResetPlayerVotes();
                ChangePhase(GamePhase.Voting);
                break;
            case GamePhase.Voting:
                break;
            case GamePhase.PostGame:
                OnGameEnd();
                break;
        }
        CurrentPhase = nPhase;
    }
    void HandlePhase()
    {
        //Debug.Log("[WerewolfGame] Coutdown time " + CurrentPhase +" "+(NextGamePhaseTime - Time.time));
        if ((NextGamePhaseTime > 0 && NextGamePhaseTime < Time.time) || (CurrentPhase ==  GamePhase.Day && NextNightTime < Time.time))
            GoToNextPhase();
    }
    void GoToNextPhase()
    {
        Debug.Log("[WerewolfGame] Go to next phase from "+ CurrentPhase);
        switch (CurrentPhase)
        {
            case GamePhase.Day:
                ChangePhase(GamePhase.Night);
                break;
            case GamePhase.Night:
                HandleAntagCoordinatedAttack();
                ChangePhase(GamePhase.Day);
                break;
            case GamePhase.Defense:
                ChangePhase(GamePhase.Voting);
                break;
            case GamePhase.Voting:
                if (HandleEndOfVoting() || NextNightTime<Time.time)
                {
                    ChangePhase(GamePhase.Night);
                }
                else
                {
                    ChangePhase(GamePhase.Day);
                }
                ResetPlayerVotes();
                break;
            default:
                    ChangePhase(GamePhase.Day);
                break;
        }
    }
    void RunPhaseCountdown(float time)
    {
        Debug.Log("[WerewolfGame] Run countdown timer " + time);
        NextGamePhaseTime = time < 0 ? time : (Time.time + time);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { WerewolfGameDefines.PhaseCountdown, NextGamePhaseTime } });
    }
    void BeginNewDay(float daytime)
    {
        Debug.Log("[WerewolfGame] Run countdown timer " + daytime);
        NextNightTime = daytime < 0 ? daytime : (Time.time + daytime);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { WerewolfGameDefines.DayCycle, NextNightTime } });
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
            { WerewolfGameDefines.GamePhase, CurrentPhase },
            { WerewolfGameDefines.VotedPlayer, "" },
            { WerewolfGameDefines.PhaseCountdown, 0 },
            { WerewolfGameDefines.DayCycle, 0 },
            { WerewolfGameDefines.FirstDay, true }
        });
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    void StartTheGame()
    {
        Debug.Log("[WerewolfGame] Start New Server Game");
        ResetPlayerProps();
        InitializePlayerRoles();
        ChangePhase(GamePhase.PreGame);
    }

    void OnGameEnd()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }
    public int GetPlayerCount()
    {
        return PhotonNetwork.PlayerList.Length;
    }
    void InitializePlayerRoles()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        Debug.Log("[WerewolfGame] Initialize Game Roles");
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
            p.SetCustomProperties( new  ExitGames.Client.Photon.Hashtable{ {WerewolfGameDefines.PlayerClass, Roles[player] } });
            PunPlayers.Remove(p);
        }
    }

    #endregion
    #region Loading Phase
    protected void CheckGameStarted()
    {
        Debug.Log("[WerewolfGame] Check Game Started");
        if (PhotonNetwork.IsMasterClient && CheckAllPlayerLoadedLevel())
        {
            StartTheGame();
        }
    }
    protected bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerLoading, out playerLoadedLevel))
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
    public bool IsFirstDay()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WerewolfGameDefines.FirstDay, out var FirstDay))
        {
            return (bool)FirstDay;
        }
        return false;
    }
    void ResetPlayerVotes()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            p.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { WerewolfGameDefines.PlayerVote, "" } });
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {{ WerewolfGameDefines.VotedPlayer, "" }});
    }
    void HandlePlayerAccusations()
    {
        if (RecountPlayerAccusations(out string votedPlayer))
        {
            Debug.Log(votedPlayer + " was lynched");
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { WerewolfGameDefines.VotedPlayer, votedPlayer } });
            ChangePhase(GamePhase.Voting);
        }
    }
    bool RecountPlayerAccusations(out string votedPlayer)
    {
        Debug.Log("[WerewolfGame] Recount lynch vote for votedPlayer");
        Dictionary<string, int> votesCount = new Dictionary<string, int>();
        foreach (Player p in GetAllLivingPlayers())
        {
            if (p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerVote, out var playervote))
            {
                if (playervote as string == null)
                    continue;
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
            if (votes.Key!="" && votes.Value > totVotes)
            {
                votedPlayer = votes.Key;
                totVotes = votes.Value;
            }
        }
        Debug.Log("[WerewolfGame] "+votedPlayer+" was lynched with "+ totVotes + " votes");
        return votedPlayer!= "" && totVotes > 1;
    }
    #endregion
    #region Lynching Phase
    bool RecountPlayerLynchings(out bool VotedKill)
    {
        VotedKill = false;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WerewolfGameDefines.VotedPlayer, out object value))
            return false;
        string votedPlayer = (string)value;

        bool EveryoneVoted = true;
        Player[] VotingPlayers = GetAllLivingPlayers();
        int votes = 0;
        foreach (Player p in VotingPlayers)
        {
            if (p.NickName == votedPlayer)
                continue;
            if (p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerVote, out var playervote))
            {
                string pVote = (string)playervote;
                 if (pVote == WerewolfGameDefines.PlayerVoteYes)
                {
                    votes++;
                }
                else if (pVote != WerewolfGameDefines.PlayerVoteNo)
                {
                    EveryoneVoted = false;
                }
            }
        }
        VotedKill = votes >= Mathf.CeilToInt((VotingPlayers.Length) / 2);
        return EveryoneVoted;
    }
    void HandlePlayerLynching()
    {
        Debug.Log("[WerewolfGame] Handle player lyncing...");
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WerewolfGameDefines.VotedPlayer, out object value))
            return;

        bool everyoneVoted = RecountPlayerLynchings(out bool votedKill);
        if (everyoneVoted)
        {
            GoToNextPhase();
        }
    }
    bool HandleEndOfVoting()
    {
        Debug.Log("[WerewolfGame] Handle player lyncing...");
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WerewolfGameDefines.VotedPlayer, out object value))
            return false;

        string votedPlayer = (string)value;
        bool everyoneVoted = RecountPlayerLynchings(out bool votedKill);

        if (votedKill)
        {
            Debug.Log(votedPlayer + " was lynched");
            KillPlayer(GetPlayerByName(votedPlayer), WerewolfGameDefines.PlayerAliveState.lynched);
            return true;
        }
        else if (everyoneVoted)
        {
            Debug.Log(votedPlayer + " was pardoned");

        }
        return false;
    }
    #endregion
    #region Antag Coordinated Attack
    bool RecountAntagCoordinatedAttack(out string votedPlayer)
    {
        Dictionary<string, int> votesCount = new Dictionary<string, int>();
        Player[] antags = GetLivingAntags();
        foreach (Player p in antags)
        {
            if (p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerVote, out var playervote))
            {
                string sVote = (string)playervote;
                if (sVote == "" || IsPlayerAntagonist(GetPlayerByName(sVote)))
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
        return votedPlayer!="" && totVotes == antags.Length;
    }
    void HandleAntagOrganization()
    {
        if (RecountAntagCoordinatedAttack(out string votedPlayer) && GetPlayerByName(votedPlayer)!=null && NextGamePhaseTime - Time.time > 5)
        {
            Debug.Log("Antags have decided to kill " + votedPlayer);
            RunPhaseCountdown(WerewolfGameDefines.PhaseTimeShort);
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
                KillPlayer(murderTarget, WerewolfGameDefines.PlayerAliveState.mauled);
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { WerewolfGameDefines.FirstDay, false } });
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

        if (changedProps.ContainsKey(WerewolfGameDefines.PlayerLoading))
            CheckGameStarted();

        if (changedProps.ContainsKey(WerewolfGameDefines.PlayerVote))
        {
            if (CurrentPhase == GamePhase.Day && !IsFirstDay())
                HandlePlayerAccusations();
            else if (CurrentPhase == GamePhase.Voting)
                HandlePlayerLynching();
            else if (CurrentPhase == GamePhase.Night)
                HandleAntagOrganization();
        }
    }
    #endregion
    #region Variable Sync
    void UpdateVarsFromServer()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WerewolfGameDefines.GamePhase, out object newGamePhase))
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
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WerewolfGameDefines.PhaseCountdown, out object CountDown))
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
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WerewolfGameDefines.DayCycle, out object DayTime))
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
                {WerewolfGameDefines.PlayerLoading, true},
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    void ResetPlayerProps()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {WerewolfGameDefines.PlayerPortrait, "Player"},
                {WerewolfGameDefines.PlayerClass, "Spectator"},
                {WerewolfGameDefines.PlayerVote, ""},
                {WerewolfGameDefines.PlayerState, (int)WerewolfGameDefines.PlayerAliveState.alive},
            };
        foreach (Player p in PhotonNetwork.PlayerList)
            p.SetCustomProperties(props);
    }
    public bool IsPlayerAlive(Player p)
    {

        if (p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerState, out var isDead))
        {
            return (int)isDead == 0;

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
            if (p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerClass, out var playerclass) && p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerState, out var aliveState))
            {
                if ((int)aliveState==0 && IsPlayerAntagonist((string)playerclass))
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
            if (p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerClass, out var playerclass) && p.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerState, out var aliveState))
            {
                if ((int)aliveState == 0 && !IsPlayerAntagonist((string)playerclass))
                    {
                        villagers.Add(p);
                    }
            }
        }
        return villagers.ToArray() ;
    }
    public bool IsPlayerAntagonist(Player targetplayer)
    {
        if (targetplayer == null)
            return false;
        if (targetplayer.CustomProperties.TryGetValue(WerewolfGameDefines.PlayerClass, out var playerclass))
            return (string)playerclass == "Werewolf";
        return false;
    }
    void KillPlayer(Player p, WerewolfGameDefines.PlayerAliveState deathCause)
    {
        Debug.Log(p.NickName + " was killed on server!");
        p.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { WerewolfGameDefines.PlayerState, (int)deathCause } });
    }
    public bool IsPlayerAntagonist(string className)
    {
        return className == "Werewolf";
    }
    Player GetPlayerByName(string playerName)
    {
        Debug.Log("[WerewolfGame] Get Player by name " + playerName);
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
        NextGamePhaseTime = -1;
        NextNightTime = -1;
        ChangePhase(GamePhase.PostGame);
    }
    #endregion
}
