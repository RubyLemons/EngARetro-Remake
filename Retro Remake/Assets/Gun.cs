using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{

    [SerializeField] Animator animator;

    [SerializeField] Freelook freelook;
    [SerializeField] Freeroam freeroam;
    [SerializeField] Health health;

    [SerializeField] float walkSmooth = 0.15f;

    [Header("Raycast")]

    [SerializeField] float maxDist = 100;
    [SerializeField] LayerMask layer;

    [SerializeField] Transform bulletScar;

    [Header("Shooting")]

    [SerializeField] Light flair;
    [SerializeField] float flairIntensity = 7.5f;

    [Space(10)]

    float elaspedTime;
    [SerializeField] float[] debTime = new float[3] { 0.315f, 0.23625f, 0.75f };

    [SerializeField] int enemyLayer = 6;

    [Space(10)]

    [SerializeField] Transform slide;

    public int[] ammoCapacity = new int[2] { 28, 112 };
    public bool reloadAction;
    public bool fireAction;

    [SerializeField] TextMeshProUGUI label;

    [Header("Upgrade Properties")]

    //ENCHANTMENT
    [Range(0, 2)] public static int enchantment = 0;

    [Space(10)]
    
    string[] colorMat = new string[3] { "#3A3A3A", "#FFB11E", "#00C0FF" };
    [SerializeField] Material mat;

    [SerializeField] UnityEngine.UI.Image ico;
    [SerializeField] UnityEngine.UI.Image icoShop;
    [SerializeField] Sprite[] icoSrc = new Sprite[3];


    [Space(10)]

    //EFFICIENCY
    [Range(0, 1)] [SerializeField] float[] efficiency = new float[4] { 0.25f, 0.3f, 0.375f, 0.5f };
    [SerializeField] float[] damageMultipler = new float[3] { 1.25f, 1.375f, 2f };

    //RECOIL SUPPRESSOR
    [SerializeField] float[] recoil = new float[3] { 3f, 1.5f, 1f };
    [Range(0, 2)] public static int recoilLevel = 0;
    [SerializeField] GameObject suppressor;

    [Space(10)]

    //STOCK EXTENSION
    [SerializeField] float[] spread = new float[3] { 0.0375f, 0.025f, 0.0125f };
    [Range(0, 2)] public static int spreadLevel = 0;
    [SerializeField] GameObject stock;

    [Space(10)]

    //MAGAZINE EXTENSION
    [SerializeField] int[] magSize = new int[3] { 8, 16, 28 };
    [Range(0, 2)] public static int magLevel = 0;
    [SerializeField] GameObject[] mag = new GameObject[3];

    [Space(10)]

    //RESERVE AMMUNATION
    [SerializeField] int[] ammunation = new int[3] { 56, 84, 112 };

    void Update()
    {
        //forbidden
        if (Token.forbidden) {
            Token.forbidden = false;
            animator.Play("Forbidden", 0, 0);
        }

        bool forbidden = animator.GetCurrentAnimatorStateInfo(0).IsName("Forbidden");

        Visuals();

        //flair light
        flair.intensity = Mathf.Lerp(flair.intensity, 0, 0.325f);

        //Shooting

        elaspedTime += Time.deltaTime;
        fireAction = elaspedTime < ((Token.ammo[0] < 4) ? debTime[Mathf.Clamp(enchantment >= 2 ? enchantment - 1 : enchantment, 0, 2)] : debTime[enchantment]) || forbidden;

        if (Input.GetMouseButton(0) && !reloadAction)
        {
            if (Token.ammo[0] > 0 && !fireAction && !Token.inShop)
            {
                animator.Play("Fire level " + ((Token.ammo[0] < 4) ? Mathf.Clamp(enchantment - 1, 0, 2) : enchantment), 0, 0.0f);
                elaspedTime = 0;
            }
        }

        //reloading
        reloadAction = (animator.GetCurrentAnimatorStateInfo(0).IsName("Reload 0") || animator.GetCurrentAnimatorStateInfo(0).IsName("Reload 1") || forbidden);

        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0) && !(Token.ammo[0] > 0))
            Reload();

        //Ammo
        
        slide.localPosition = Vector3.up * ((Token.ammo[0] == 0 && (!reloadAction || forbidden)) ? 0.00135f : 0);

        for (int i = 0; i < Token.ammo.Length; i++)
            Token.ammo[i] = Mathf.Clamp(Token.ammo[i], 0, ammoCapacity[i]);

        label.text = $"{Token.ammo[0]}/{Token.ammo[1]}";

        //walk animation
        float blend = (freeroam.moving && freeroam.ground) ? (!freeroam.fast ? 0.5f : 1) : 0;

        float walkTransition = Mathf.Lerp(animator.GetFloat("Walk"), blend, walkSmooth);

        animator.SetFloat("Walk", walkTransition);
    }


    void Visuals()
    {
        //enchantment
        if (mat.GetColor("_BaseColor") != ColorConvert.FromHex(colorMat[enchantment]))
            mat.SetColor("_BaseColor", ColorConvert.FromHex(colorMat[enchantment]));

        ico.sprite = icoSrc[enchantment];
        icoShop.sprite = ico.sprite;

        //suppressor
        suppressor.SetActive(recoilLevel > 0);
        
        //stock
        stock.SetActive(spreadLevel > 0);

        //mag
        ammoCapacity[0] = magSize[magLevel];
        ammoCapacity[1] = ammunation[enchantment];

        for (int i = 0; i < mag.Length; i++)
            mag[i].SetActive(i == magLevel);
    }

    GameObject BulletScar(bool active = true) {
        GameObject newBulletScar = Instantiate(bulletScar.gameObject);
        newBulletScar.SetActive(true);

        newBulletScar.GetComponent<MeshRenderer>().enabled = active;

        StartCoroutine(RemoveThis());

        IEnumerator RemoveThis()
        {
            yield return new WaitForSeconds(2.5f);
            LeanTween.scale(newBulletScar, Vector3.zero, 0.25f)
                .setOnComplete(() => Destroy(newBulletScar));
        }

        return newBulletScar;
    }

    public void Fire()
    {
        freelook.mouseDelta.y += recoil[recoilLevel] + (enchantment / 8) * Random.Range(1f, 2f);

        flair.intensity = flairIntensity;
        Token.ammo[0] -= 1;

        Vector3 misSpread = (transform.right * Random.Range(-spread[spreadLevel], spread[spreadLevel])) + Vector3.up * Random.Range(-spread[spreadLevel], spread[spreadLevel]);

        RaycastHit hit;
        bool ray = Physics.Raycast(freelook.cam.transform.position, freelook.cam.transform.forward + misSpread, out hit, maxDist, layer);

        bool enemy = hit.collider?.gameObject.layer == enemyLayer;
        bool head = hit.collider?.gameObject.tag == "Enemy";

        if (!ray) return;
        Transform bulletScar = BulletScar(!enemy).transform;

        if (enemy)
        {
            Enemy enemyInst = hit.collider.transform.parent.parent.parent.parent.GetComponent<Enemy>();

            enemyInst.health -= efficiency[enchantment + ((Token.maxUpgrades) ? 1 : 0)] * (head ? damageMultipler[enchantment] : 1);
        }

        bulletScar.position = hit.point + (hit.normal * 0.01f);
        bulletScar.rotation = ((hit.normal != Vector3.zero) ? Quaternion.LookRotation(hit.normal) : Quaternion.identity) * Quaternion.Euler(Vector3.forward * Random.Range(0, 46));
    }

    void Reload()
    {
        if (reloadAction || fireAction || Token.ammo[0] == ammoCapacity[0] || Token.ammo[1] == 0 || Token.inShop) return;
        bool emptyed = Token.ammo[0] == 0;

        animator.Play("Reload " + (emptyed ? 0 : 1));
    }

    public void ReplenishAmmo()
    {
        int moreAmmo = 0;

        for (int i = 0; i < ammoCapacity[0] - Token.ammo[0]; i++)
        {
            if (Token.ammo[1] > 0)
            {
                Token.ammo[1] -= 1;
                moreAmmo += 1;
            }
        }

        Token.ammo[0] += moreAmmo;
    }

    public void KYS()
    {
        if (Token.ammo[0] > 0)
            health.Damage(1);
    }
}
