using Cinemachine;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using System.Linq;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private MenuSwitcher menuSwitcher;

    private PlayerContoller playerScript;
    public static List<Transform> players { get; private set; } = new();

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Invoke(nameof(GetPlayers), 1);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("Player disconnected");
        foreach (Transform player in players)
        {
            if (player == null)
            {
                players.Remove(player);
            }
        }
    }

    private void GetPlayers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            print("Player Conected");
            players = FindObjectsByType<PlayerContoller>(FindObjectsSortMode.None)
                .Select(i => i.GetComponent<Transform>())
                .ToList();
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected == true)
        {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity);
            playerScript = player.GetComponent<PlayerContoller>();
            menuSwitcher.SetPlayerController(playerScript);
            playerScript.Initialization(MainCamera);
            virtualCamera.Follow = player.transform;
            players.Add(player.transform);
        }
    }
}