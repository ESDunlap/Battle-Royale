using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Health,
    Ammo,
    Sniper,
    Shotgun,
    Shield,
    Basic
}

public class Pickup : MonoBehaviourPun
{
    public PickupType type;
    public int value;

    [PunRPC]
    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.CompareTag("Player"))
        {
            // get the player
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);
            if (type == PickupType.Health)
                player.photonView.RPC("Heal", player.photonPlayer, value);
            else if (type == PickupType.Ammo)
                player.photonView.RPC("GiveAmmo", player.photonPlayer, value);
            else if (type == PickupType.Basic)
                player.photonView.RPC("GiveBasic", player.photonPlayer, RpcTarget.All);
            else if (type == PickupType.Sniper)
                player.photonView.RPC("GiveSniper", player.photonPlayer, RpcTarget.All);
            else if (type == PickupType.Shotgun)
                player.photonView.RPC("GiveShotgun", player.photonPlayer, RpcTarget.All);
            else if (type == PickupType.Shield)
                player.photonView.RPC("GiveShield", player.photonPlayer, RpcTarget.All);
            // destroy the object
            //PhotonNetwork.Destroy(gameObject);
            // BUG: pickups don't get removed from game and throw error:
            // "Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view"
            // https://forum.photonengine.com/discussion/15373/failed-to-network-remove-gameobject-because-it-is-missing-a-valid-instantiationid-on-view
            photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void DestroyPickup()
    {
        Destroy(gameObject);
    }
}
