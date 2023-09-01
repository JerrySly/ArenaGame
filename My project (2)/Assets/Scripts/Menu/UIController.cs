using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayersParams: EventArgs
{
    public List<Player> players { get; set; }
}

public class UIController : MonoBehaviour
{
    [SerializeField] private Button createLobby;
    [SerializeField] private Button quickJoin;
    [SerializeField] private Button joinBycode;
    [SerializeField] private TextMeshProUGUI lobbyCode;
    [SerializeField] private Canvas startCanvas;
    [SerializeField] private Canvas lobbyCavas;
    [SerializeField] private Button exitLobby;
    [SerializeField] private GameObject playersPanel;
    [SerializeField] private GameObject textPrefub;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    private float nextOffset = 10f;


    void Awake()
    {
        quickJoin.onClick.AddListener(() =>
        {
            ActiveLobby();
            Debug.Log("Join");
            LobbyController.JoinLobby();


        });
        joinBycode.onClick.AddListener(() =>
        {
            ActiveLobby();
            Debug.Log(lobbyCode.text);
            LobbyController.JoinLobbyByCode(lobbyCode.text);
        });
        createLobby.onClick.AddListener(() =>
        {
            ActiveLobby();
            LobbyController.CreateLobby("test", 4);

        });
        exitLobby.onClick.AddListener(() =>
        {
            LobbyController.ExitFromLobby();
            ActiveStart();
        });
        Debug.Log("Set event");
        LobbyController.UpdatePlayers += UpdatePanelContent;
        LobbyController.SetCodeEvent += SetLobbyCode;
    }

    void ActiveStart()
    {
        startCanvas.gameObject.SetActive(true);
        lobbyCavas.gameObject.SetActive(false);
    }

    void ActiveLobby()
    {
        lobbyCavas.gameObject.SetActive(true);
        startCanvas.gameObject.SetActive(false);
    }

    public void SetLobbyCode(object sender, EventArgs e)
    {
        lobbyCodeText.text += LobbyController.GetLobbyCode();
    }

    public void UpdatePanelContent(object sender, UpdatePlayersParams e)
    {
        Debug.Log($"Update players {e.players.Count}");
        this.nextOffset = 15f;
        foreach (Transform child in playersPanel.transform)
        {
            if(child.CompareTag("DynamicText")) Destroy(child.gameObject);
        }
        foreach (var player in e.players)
        {
            var playerText = Instantiate(textPrefub);
            Debug.Log($"{player.Id} {playerText}");
            playerText.transform.SetParent(playersPanel.transform);
            playerText.transform.localScale = new Vector3(0.2f, 0.3f, 0.3f);
            playerText.GetComponent<TextMeshProUGUI>().text = player.Id;
            playerText.GetComponent<TextMeshProUGUI>().fontSize = 12;
            playerText.transform.SetLocalPositionAndRotation(new Vector3(0, nextOffset, 0 ), Quaternion.identity);
            this.nextOffset += 15f;

        }
    }
}
