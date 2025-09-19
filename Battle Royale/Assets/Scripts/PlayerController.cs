using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class PlayerController : MonoBehaviourPun
{
    private int curAttackerId;
    private bool flashingDamage;
    public MeshRenderer mr;
    [Header("Move Stats")]
    public float moveSpeed;
    public float jumpForce;
    [Header("Components")]
    public Rigidbody rig;
    public int id;
    public Player photonPlayer;
    [Header("Player Stats")]
    public int curHp;
    public int maxHp;
    public int kills;
    public bool dead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || dead)
            return;
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();
    }

    void Move()
    {
        // get the input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // calculate a direction relative to where we're facing
        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.linearVelocity.y;
        // set that as our velocity
        rig.linearVelocity = dir;
    }

    void TryJump()
    {
        // create a ray facing down
        Ray ray = new Ray(transform.position, Vector3.down);
        // shoot the raycast
        if (Physics.Raycast(ray, 1.5f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;
        // is this not our local player?
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {         
    if(dead)
        return; 
    curHp -= damage;
    curAttackerId = attackerId;
    // flash the player red
    photonView.RPC("DamageFlash", RpcTarget.Others);
    // update the health bar UI
    // die if no health left
    if(curHp <= 0)
        photonView.RPC("Die", RpcTarget.All);
    }


    [PunRPC]
    void DamageFlash()
    {
        if (flashingDamage)
            return;
        StartCoroutine(DamageFlashCoRoutine());
        IEnumerator DamageFlashCoRoutine()
        {
            flashingDamage = true;
            Color defaultColor = mr.material.color;
            mr.material.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            mr.material.color = defaultColor;
            flashingDamage = false;
        }
    }

    [PunRPC]
    void Die()
    {
    }
}
