using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    PlayerController playerController;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID});
        controller.TryGetComponent<PlayerController>(out playerController);
    }

    public void Die()
    {
        int color = playerController.colorIndex;
        PhotonNetwork.Destroy(controller);
        CreateController();
        playerController.colorIndex = color;
        playerController.SetColorSelf(color);
    }
}
