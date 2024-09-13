using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform throwStartPoint;
    [SerializeField] private GameObject itemBox;

    private List<ItemBox> items = new();
    private PlayerContoller owner;
    private Transform content;
    private Transform inventoryObject;
    private PhotonView photon;

    private void Awake()
    {
        content = GameObject.FindGameObjectWithTag("Content").transform;
        inventoryObject = GameObject.FindGameObjectWithTag("Inventory").transform;
        owner = GetComponent<PlayerContoller>();
        photon = GetComponent<PhotonView>();
    }

    private void Start()
    {
        inventoryObject.gameObject.SetActive(false);
    }

    public void AddItem(Item item)
    {
        PhotonView takenObjectPhoton = item.GetComponent<PhotonView>();
        int id = takenObjectPhoton.ViewID;
        ItemBox itemBox = Instantiate(this.itemBox, content)
            .GetComponent<ItemBox>();
        itemBox.Init(item, owner);
        items.Add(itemBox);

        photon.RPC(nameof(SetTakenObjectParameters), RpcTarget.All, id);
    }

    [PunRPC]
    public void SetTakenObjectParameters(int id)
    {
        PhotonView photonObject = PhotonView.Find(id);
        Collider2D takingObject = photonObject.gameObject.GetComponent<Collider2D>();

        takingObject.isTrigger = false;
        takingObject.gameObject.SetActive(false);
        takingObject.transform.SetParent(throwStartPoint);
        takingObject.transform.localPosition = Vector3.zero;
    }

    public void DeleteItem(ItemBox itemBox)
    {
        items.Remove(itemBox);
        Destroy(itemBox.gameObject);
    }
}
