using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity;
    [SerializeField] float sprintSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float smoothTime;

    [SerializeField] Material[] colors;

    [SerializeField] List<Item> items;
    [SerializeField] List<Item> restrictedItems;

    bool isRotationAllowed = true;

    public int colorIndex = 0;
    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;
    PhotonView PV;
    PlayerManager playerManager;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    private void Start()
    {
        if (PV.Owner.NickName == "flipt0")
        {
            for (int i = 0; i < restrictedItems.Count; i++)
            {
                items.Add(restrictedItems[i]);
            }
        }
        if (PV.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            transform.position += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        HandleLooking();
        HandleMoving();
        HandleJumping();
        HandleItems();
        HandleColors();
        if(transform.position.y < -10f)
        {
            Die();
        }
    }

    void HandleItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Count - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Count - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }
    }

    void HandleLooking()
    {
        if (Input.GetKeyDown("p"))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                isRotationAllowed = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                isRotationAllowed = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if(isRotationAllowed)
        {
            transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
            verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
            cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        }

    }
    
    void HandleMoving()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void EquipItem(int _index)
    {
        if(_index == previousItemIndex)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            return;
        }

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        previousItemIndex = itemIndex;

        if(PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    void HandleColors()
    {
        if (Input.GetKeyDown("c"))
        {
            if(PV.IsMine)
            {
                colorIndex = (colorIndex + 1) % colors.Length;
                SetColorSelf(colorIndex);
            }
        }
    }

    public void SetColorSelf(int _index)
    {
        GetComponent<MeshRenderer>().material = colors[_index];
        Hashtable hash = new Hashtable();
        hash.Add("colorIndex", colorIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        SetColor(colorIndex);
    }

    public void SetColor(int _index)
    {
        GetComponent<MeshRenderer>().material = colors[_index];
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer == PV.Owner )
        {
            if (changedProps.ContainsKey("itemIndex"))
            {
                EquipItem((int)changedProps["itemIndex"]);
            }
            if (changedProps.ContainsKey("colorIndex"))
            {
                SetColor((int)changedProps["colorIndex"]);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("player entered room");
        SetColorSelf(colorIndex);
        EquipItem(itemIndex);
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if(!PV.IsMine)
        {
            return;
        }
        currentHealth -= damage;
        healthbarImage.fillAmount = currentHealth / maxHealth;
        if(currentHealth<=0)
        {
            Die();
        }
    }


    void Die()
    {
        playerManager.Die();
    }

    public void GetKicked()
    {
        PV.RPC("RPC_GetKicked", RpcTarget.All);
    }

    [PunRPC]
    void RPC_GetKicked()
    {
        if(!PV.IsMine)
        {
            return;
        }
        Application.Quit();
    }

}
