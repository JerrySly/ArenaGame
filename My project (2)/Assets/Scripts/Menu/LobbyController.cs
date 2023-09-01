using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;



public class LobbyController : MonoBehaviour
{
    private static Lobby currentLobby;
    private static float lobbyUpdateTimer;
    public static event EventHandler<UpdatePlayersParams> UpdatePlayers;
    public static event EventHandler SetCodeEvent;




    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void Update()
    {
        HandleLobbyPollForUpdates();
    }

    public static async void CreateLobby(string lobbyName, int maxPlayers)
    {
        try
        {
            currentLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            UpdatePlayers.Invoke(null, new UpdatePlayersParams
            {
                players = await GetLobbyPlayersList(currentLobby.Id)
            });
            SetCodeEvent.Invoke(null, EventArgs.Empty);

        }
        catch (LobbyServiceException err)
        {
            Debug.Log(err.Message);
        }
    }

    public static async Task<QueryResponse> GetLobbiesList(int count = 25)
    {
        try
        {
            return await Lobbies.Instance.QueryLobbiesAsync(new QueryLobbiesOptions()
            {
                Count = count,
            });
        }
        catch (LobbyServiceException err)
        {
            Debug.Log(err.Message);
        }

        return null;
    }

    public static async void JoinLobby()
    {
        try
        {
            var list = await GetLobbiesList(25);
            Debug.Log(list.Results.Count);
            if (list.Results.Count > 0)
            {
                currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(list.Results[0].Id);
                Debug.Log(currentLobby.Players.Count);
                UpdatePlayers.Invoke(null, new UpdatePlayersParams
                {
                    players = await GetLobbyPlayersList(currentLobby.Id)
                });
                SetCodeEvent.Invoke(null, EventArgs.Empty);

            }
        }
        catch (LobbyServiceException err)
        {
            Debug.Log(err.Message);
        }
    }
    public static async void JoinLobbyById(string lobbyId)
    {
        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
            UpdatePlayers.Invoke(null, new UpdatePlayersParams
            {
                players = await GetLobbyPlayersList(currentLobby.Id)
            });
            SetCodeEvent.Invoke(null, EventArgs.Empty);
        }
        catch (LobbyServiceException err)
        {
            Debug.Log(err.Message);
        }
    }

    public static async void JoinLobbyByCode(string code)
    {
        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code);
            UpdatePlayers.Invoke(null, new UpdatePlayersParams
            {
                players = await GetLobbyPlayersList(currentLobby.Id)
            });
            SetCodeEvent.Invoke(null, EventArgs.Empty);
        }
        catch (LobbyServiceException err)
        {
            Debug.Log(err.Message);
        }
    }

    public static async Task<List<Player>> GetLobbyPlayersList(string id)
    {
        try
        {
            if (String.IsNullOrEmpty(id))
            {
                return currentLobby.Players;
            }
            else
            {
                return (await Lobbies.Instance.GetLobbyAsync(id)).Players;
            }
        }
        catch (LobbyServiceException err)
        {
            Debug.Log(err.Message);
        }

        return null;
    }

    public static async void ExitFromLobby()
    {
        try
        {
            await Lobbies.Instance.ReconnectToLobbyAsync(currentLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    public static async void HandleLobbyPollForUpdates()
    {
        if (currentLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                float maxTimer = 15;
                lobbyUpdateTimer = maxTimer;

                currentLobby = await Lobbies.Instance.GetLobbyAsync(currentLobby.Id);
            }
        }
    }

    public static string GetLobbyCode()
    {
        return currentLobby.LobbyCode;
    }
}