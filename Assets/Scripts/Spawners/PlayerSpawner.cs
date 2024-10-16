using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private MenuSwitcher menuSwitcher;
    [SerializeField] private Transform content;

    private PlayerContoller playerScript;
    private float deathTimer;
    public static List<Transform> players { get; private set; } = new();

    private void Start()
    {
        if (PhotonNetwork.IsConnected == true)
        {
            Respawn();
        }
    }

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

    public void PlayerRespawn()
    {
        deathTimer = deathTimer + 5;
        Invoke(nameof(Respawn), deathTimer);
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

    private void Respawn()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity);
        playerScript = player.GetComponent<PlayerContoller>();
        menuSwitcher.SetPlayerController(playerScript);
        playerScript.Initialization(MainCamera, this, content);
        virtualCamera.Follow = player.transform;
        players.Add(player.transform);
        AudioManager audioManager = AudioManager.instance;
        audioManager.OnNewAudiosAppeared();
    }
}