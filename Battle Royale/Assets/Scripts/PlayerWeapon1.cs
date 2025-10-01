using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerWeapon : MonoBehaviourPun
{
    [Header("Guns")]
    public GameObject basicGun;
    public GameObject sniperGun;
    public GameObject shotGun;


    [Header("Used Stats")]
    private int damage;
    public int curAmmo;
    public int maxAmmo;
    private float bulletSpeed;
    private float shootRate;

    [Header("Basic Stats")]
    public int basicDamage;
    public int basicMaxAmmo;
    public float basicBulletSpeed;
    public float basicShootRate;

    [Header("Shotgun Stats")]
    public int shotgunDamage;
    public int shotgunMaxAmmo;
    public float shotgunBulletSpeed;
    public float shotgunShootRate;
    public int shotgunBulletSpread;

    [Header("Sniper Stats")]
    public int sniperDamage;
    public int sniperMaxAmmo;
    public float sniperBulletSpeed;
    public float sniperShootRate;
    public Transform sniperBulletSpawnPos;


    [Header("Bullet")]
    private float lastShootTime;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;
    private PlayerController player;
    void Awake()
    {
        // get required components
        player = GetComponent<PlayerController>();
        GiveBasic();
    }

    public void TryShoot()
    {
        // can we shoot?
        if (curAmmo <= 0 || Time.time - lastShootTime < shootRate)
        {
        return;
        }
        curAmmo--;
        lastShootTime = Time.time;
        // update the ammo UI
        // spawn the bullet
        if(sniperGun.activeSelf)
            player.photonView.RPC("SpawnBullet", RpcTarget.All, sniperBulletSpawnPos.transform.position, sniperBulletSpawnPos.transform.forward);
        else if(shotGun.activeSelf)
        {
            for(float x = -1;x<2;x++)
            {
                for (float y = -1; y < 2; y++)
                {
                    player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.transform.position, bulletSpawnPos.transform.forward
                                                                                                           + x/shotgunBulletSpeed*(bulletSpawnPos.transform.right)
                                                                                                           + y/shotgunBulletSpeed*(bulletSpawnPos.transform.up));
                }
            }
        }

        else
            player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.transform.position, bulletSpawnPos.transform.forward);
        GameUI.instance.UpdateAmmoText();
    }

    [PunRPC]
    void SpawnBullet(Vector3 pos, Vector3 dir)
    {
        Debug.Log("Spawning");
        // spawn and orientate it
        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;
        // get bullet script
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        // initialize it and set the velocity
        bulletScript.Initialize(damage, player.id, player.photonView.IsMine);
        bulletScript.rig.linearVelocity = dir * bulletSpeed;

    }

    [PunRPC]
    public void GiveAmmo(int ammoToGive)
    {
        curAmmo = Mathf.Clamp(curAmmo + ammoToGive, 0, maxAmmo);
        GameUI.instance.UpdateAmmoText();
        // update the ammo text
    }

    [PunRPC]
    public void GiveBasic()
    {
        basicGun.SetActive(true);
        sniperGun.SetActive(false);
        shotGun.SetActive(false);
        damage= basicDamage;
        curAmmo= basicMaxAmmo;
        maxAmmo= basicMaxAmmo;
        bulletSpeed= basicBulletSpeed;
        shootRate= basicShootRate;
        GameUI.instance.UpdateAmmoText();
    }

    [PunRPC]
    public void GiveShotgun()
    {
        shotGun.SetActive(true);
        basicGun.SetActive(false);
        sniperGun.SetActive(false);
        damage = shotgunDamage;
        curAmmo = shotgunMaxAmmo;
        maxAmmo = shotgunMaxAmmo;
        bulletSpeed = shotgunBulletSpeed;
        shootRate = shotgunShootRate;
        GameUI.instance.UpdateAmmoText();
    }

    [PunRPC]
    public void GiveSniper()
    {
        sniperGun.SetActive(true);
        shotGun.SetActive(false);
        basicGun.SetActive(false);
        damage = sniperDamage;
        curAmmo = sniperMaxAmmo;
        maxAmmo = sniperMaxAmmo;
        bulletSpeed = sniperBulletSpeed;
        shootRate = sniperShootRate;
        GameUI.instance.UpdateAmmoText();
    }
}
