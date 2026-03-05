using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    const string PlayerNameKey = "PlayerName";
    const string GameModeKey = "GameMode";

    [SerializeField] float heartbeatTimerMax = 15f; // cada 15 segundos
    [SerializeField] int lobbyMaxPlayers = 1;//4;
    [SerializeField] string lobbyName = "Mi Lobby Test";
    [SerializeField] string lobbyCode;
    [SerializeField] string lobbyId;
    [SerializeField] bool isPrivateLobby = false;
    [SerializeField] string playerName;
    [SerializeField] string gameMode = "CargandoRocas";
    [SerializeField] string newPlayerName;
    

    string PlayerId => AuthenticationService.Instance.PlayerId;


    Lobby joinedLobby;


    
    private void Awake()
    {
        playerName = "Pedro" + UnityEngine.Random.Range(10, 99);
    }

    async void Start()
    {
        await InitializeUnityAuthentication();
        StartCoroutine(HeartbeatLobby_CR());
    }

    private async Task InitializeUnityAuthentication()
    {
        //TODO: se puede pasar un argumento InitializationOptions para poder probar multiples build en la misma máquina.
        await UnityServices.InitializeAsync(); // await para que no se quede pegado esperando iniciar los servicios.
        AuthenticationService.Instance.SignedIn += SignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync(); // nos podemos autenticar de forma anónima.
    }

    IEnumerator HeartbeatLobby_CR()
    {
        WaitForSeconds wait = new WaitForSeconds(heartbeatTimerMax);
        while (enabled) 
        {
            yield return wait;
            print("PRE HandleLobbyHeartbeat");
            HandleLobbyHeartbeat();
        }
    }    

    private void SignedIn()
    {
        print($"Usuario conectado: {PlayerId}");
    }


    //TODO: pasar a public estas funciones para que se puedan llamar desde el script que controla la UI.
    // Argumentos: lobbyName, bool isPrivate. También lá máxima cantidad de jugadores: eso dependerá del ejercicio elegido
    [ContextMenu("CreateLobby")]
    async void CreateLobby()
    {
        //TODO: aquí se podrían gatillar algunos eventos:
        try
        {
            //TODO: OnCreateLobbyStarted
            int maxPlayers = lobbyMaxPlayers;

            // Opciones de lobby
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isPrivateLobby,
                Player = BuildPlayerData(playerName),
                Data = BuildLobbyData(gameMode)
            };

            joinedLobby = await Unity.Services.Lobbies.LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            print($"Lobby creado: {joinedLobby.Name}. Max Players: {joinedLobby.MaxPlayers}. Id: {joinedLobby.Id}. Code: {joinedLobby.LobbyCode}");
            //TODO: OnCreateLobbySucceded
        }
        catch (LobbyServiceException e)
        {
            print($"Error LobbyServiceException: {e}");
            //TODO: OnCreateLobbyFailed
        }
        catch (Exception e)
        {
            print($"CreateLobby.Exception: {e}");
            //TODO: OnCreateLobbyFailed
        }

    }

    private Dictionary<string, DataObject> BuildLobbyData(string gameMode)
    {
        return new Dictionary<string, DataObject>
        {
            { GameModeKey, new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
        };
    }

    private Player BuildPlayerData(string name)
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { PlayerNameKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, name) }
            }
        };
    }

    [ContextMenu("ListLobbies")]
    async void ListLobbies()
    {
        try
        {

            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions 
            {
                Count = 10,
                Filters = new List<QueryFilter>
                { 
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(asc: false, QueryOrder.FieldOptions.Created) // false: quiere decir que será en orden descendente.
                }

            };

            //TODO: se supone que en la nueva versión del SDK se debe usar LobbyService en vez de Lobbies.
            // ej: await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            print($"Lobbies encontrados: {queryResponse.Results.Count}");

            foreach (var lobby in queryResponse.Results)
            {
                print($"Lobby: {lobby.Name}. Max Players: {lobby.MaxPlayers}. GameMode: {lobby.Data?[GameModeKey].Value}");
            }
        }
        catch (Exception e)
        {
            print($"ListLobbies.Exception: {e}");
        }   
    }

    async void HandleLobbyHeartbeat()
    {
        if (joinedLobby == null)
            return;

        if (!IsLobbyHost())
            return;

        print("PRE SendHeartbeatPingAsync");
        await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        print("Ping OK");
    }

    [ContextMenu("JoinLobby")]
    async void JoinLobby()
    {
        //TODO eventos:   OnJoinStarted, OnJoinFailed, y un Succeded también podría ser. Y agregarlos en esta función y en la de QuickJoin.
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            if (queryResponse == null || queryResponse.Results == null || queryResponse.Results.Count == 0)
            {
                print("No hay lobbies disponibles.");
                return;
            }

            var firstLobby = queryResponse.Results[0];

            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = BuildPlayerData(playerName)
            };
            joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(firstLobby.Id, joinLobbyByIdOptions);

            print($"Join OK. Lobby: {joinedLobby.Name}. AvailableSlots: {joinedLobby.AvailableSlots}");
        }
        catch (Exception e)
        {
            print($"JoinLobby.Exception: {e}");
        }
    }

    [ContextMenu("JoinLobbyByCode")]
    async void JoinLobbyByCode()
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = BuildPlayerData(playerName)
            };

            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            print($"Join with Code OK. Lobby: {joinedLobby.Name}. AvailableSlots: {joinedLobby.AvailableSlots}");
        }
        catch (Exception e)
        {
            print($"JoinLobbyByCode.Exception: {e}");
        }
    }

    //TODO: crear función
    [ContextMenu("JoinLobbyById")]
    void JoinLobbyById()
    {
        JoinLobbyById(lobbyId);
    }

    async void JoinLobbyById(string lobbyId)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = BuildPlayerData(playerName)
            };

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

            print($"Join by Id: OK. Lobby: {joinedLobby.Name}. AvailableSlots: {joinedLobby.AvailableSlots}");
        }
        catch (Exception e)
        {
            print($"JoinLobbyById.Exception: {e}");
        }
    }

    [ContextMenu("QuickJoinLobby")]
    async void QuickJoinLobby()
    {
        try
        {
            joinedLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
            print($"QuickJoinLobby OK. Lobby: {joinedLobby.Name}. AvailableSlots: {joinedLobby.AvailableSlots}");

        }
        catch (Exception e)
        {
            print($"QuickJoinLobby.Exception: {e}");
        }
    }

    [ContextMenu("PrintPlayers")]
    void PrintPlayers()
    {
        print($"Players in lobby {joinedLobby.Name}. Count: {joinedLobby.Players.Count}. Slots: {joinedLobby.AvailableSlots}. GameMode: {joinedLobby.Data?[GameModeKey].Value}");
        foreach (var player in joinedLobby.Players)
        {
            print($"Id: {player.Id}. Nombre: {player.Data?[PlayerNameKey].Value}");
        }
    }

    [ContextMenu("RefreshHostLobby")]
    async void RefreshJoinedLobby() //TODO: esta función abría que agregarla a una corutina, que se ejecute cada 1.5 seg.
    {
        if (joinedLobby == null)
            return;
        //QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
        //hostLobby = queryResponse.Results[0];
        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
    }

    [ContextMenu("UpdateLobbyGameMode")]
    void UpdateLobbyGameMode()
    {
        UpdateLobbyGameMode(gameMode);
    }

    async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions
            {
                Data = BuildLobbyData(gameMode)
            };

            joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, updateLobbyOptions);
        }
        catch (Exception e)
        {
            print($"UpdateLobbyGameMode.Exception: {e}");
        }
    }

    [ContextMenu("UpdatePlayerName")]
    void UpdatePlayerName()
    {
        UpdatePlayerName(newPlayerName);
    }

    async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { PlayerNameKey, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                }
            };

            joinedLobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, PlayerId, updatePlayerOptions);
        }
        catch (Exception e)
        {
            print($"UpdatePlayerName.Exception: {e}");
        }
        
    }

    [ContextMenu("LeaveLobby")]
    async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, PlayerId);
            RefreshJoinedLobby(); // La función RemovePlayerAsync no me retorna el lobby para poder actualizarlo altiro.
        }
        catch (Exception e)
        {
            print($"UpdatePlayerName.Exception: {e}");
        }
    }

    [ContextMenu("KickPlayer")]
    async void KickPlayer()
    {
        try
        {
            //TODO: se podría validar que esta funcionalidad solo la ejecute el host lobby
            // if !IsLobbyHost return;
            var secondPlayerTest = joinedLobby.Players[1];
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, secondPlayerTest.Id);
            RefreshJoinedLobby();
        }
        catch (Exception e)
        {
            print($"UpdatePlayerName.Exception: {e}");
        }
    }

    [ContextMenu("StopCoroutines")]
    void StopCoroutines()
    {
        StopAllCoroutines();
    }

    [ContextMenu("MigrateLobbyHost")]
    async void MigrateLobbyHost()
    {
        try
        {
            joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
        }
        catch (Exception e)
        {
            print($"MigrateLobbyHost.Exception: {e}");
        }
    }

    [ContextMenu("DeleteLobby")]
    async void DeleteLobby()
    {
        try
        {
            if (joinedLobby == null)
                return;
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        }
        catch (Exception e)
        {
            print($"DeleteLobby.Exception: {e}");
        }
    }

    bool IsLobbyHost()
    {
        if (joinedLobby == null)
            return false;

        return joinedLobby.HostId == PlayerId;
    }


}
