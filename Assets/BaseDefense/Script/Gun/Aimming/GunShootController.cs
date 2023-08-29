using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BaseDefenseNameSpace;
using UnityEngine.Rendering.Universal;

public class GunShootController : MonoBehaviour
{
    private GunScriptable m_SelectedGun = null;

    [Header("Ammo")]
    private float m_CurrentAmmo = 0;
    private int m_CurrentWeaponSlotIndex = 0;
    private Dictionary<int, float> m_GunsClipAmmo = new Dictionary<int, float>(); // how many ammo left on gun when switching

    [Header("Shooting")]
    [SerializeField] private GameObject m_ShotPointPrefab; // indicate where the shot land 
    [SerializeField] private Transform m_ShotDotParent;
    [SerializeField] private AudioSource m_ShootAudioSource;
    private float m_CurrentShootCoolDown = 0; // must be 0 or less to shoot 
    private Coroutine m_SemiAutoShootCoroutine = null;




    private void Start()
    {
        BaseDefenseManager.GetInstance().m_UpdateAction += ShootCoolDown;
        MainGameManager.GetInstance().AddNewAudioSource(m_ShootAudioSource);

        


        m_SemiAutoShootCoroutine = null;





        ChangeAmmoCount(0, true);
        //m_FPSImage.sprite = m_SelectedGun.FPSSprite;
        //var crossHairworldPos = Camera.main.ScreenToWorldPoint(m_CrossHair.position);
        var screenCenter = new Vector3(Screen.width/2f,Screen.height/2f,0);
        //MoveCrossHair( screenCenter );
    }


    public void OnShootBtnDown(){
        if (m_CurrentAmmo <= 0)
        {
            m_ShootAudioSource.PlayOneShot(m_SelectedGun.OutOfAmmoSound);
        }
        else
        {
            m_SemiAutoShootCoroutine = null;
            if (m_SemiAutoShootCoroutine == null && m_SelectedGun.IsSemiAuto)
            {
                m_SemiAutoShootCoroutine = StartCoroutine(SemiAutoShoot());
                return;
            }
            Shoot();
        }
    }

    public void OnShootBtnUp(){
        if (m_SemiAutoShootCoroutine != null)
        {
            StopCoroutine(m_SemiAutoShootCoroutine);
            m_SemiAutoShootCoroutine = null;
        }
    }

    public void OnClickReload(){
        if (IsFullClipAmmo())
            return;

        GunReloadControllerConfig gunReloadConfig = new GunReloadControllerConfig
        {
            GunScriptable = m_SelectedGun,
            GainAmmo = GainAmmo,
            SetAmmoToFull = SetClipAmmoToFull,
            SetAmmoToZero = SetClipAmmoToZero,
            IsFullClipAmmo = IsFullClipAmmo,
        };
        BaseDefenseManager.GetInstance().StartReload(gunReloadConfig);
    }
/*

    gun model controller

    private void HideWeaponModel()
    {
        m_FPSImage.gameObject.SetActive(false);
    }

    private void ShowWeaponModel()
    {
        m_FPSImage.gameObject.SetActive(true);
    }*/

    private void ShootCoolDown()
    {
        // fire rate
        m_CurrentShootCoolDown -= Time.deltaTime;
    }

    public void SetSelectedGun(GunScriptable gun, int slotIndex)
    {
        
        if (m_SelectedGun != null)
            m_GunsClipAmmo[m_CurrentWeaponSlotIndex] = m_CurrentAmmo;

        m_SelectedGun = gun;
         m_CurrentWeaponSlotIndex = slotIndex;

        BaseDefenseManager.GetInstance().SetAccruacy(m_SelectedGun.Accuracy);
        m_SemiAutoShootCoroutine = null;
        ChangeAmmoCount(m_GunsClipAmmo[slotIndex], true);
    }
    private void GainAmmo(int changes)
    {
        ChangeAmmoCount(changes, false);
    }

    private bool IsFullClipAmmo()
    {
        return m_CurrentAmmo >= m_SelectedGun.ClipSize;
    }

    private void SetClipAmmoToZero()
    {
        ChangeAmmoCount(0, true);
    }

    private void SetClipAmmoToFull()
    {
        ChangeAmmoCount(m_SelectedGun.ClipSize, true);
    }
    private IEnumerator SemiAutoShoot()
    {
        while (m_CurrentAmmo > 0)
        {
            Shoot();
            yield return null;
        }

        m_ShootAudioSource.PlayOneShot(m_SelectedGun.OutOfAmmoSound);
    }



    private void Shoot()
    {
        if (m_CurrentShootCoolDown > 0)
            return;

        // shoot sound
        m_ShootAudioSource.PlayOneShot(m_SelectedGun.ShootSound);

        for (int j = 0; j < m_SelectedGun.PelletPerShot; j++)
        {
            // random center to point distance
            
            float randomDistance = Random.Range(0, 
                BaseDefenseManager.GetInstance().GetCrosshairController().m_MaxAccuracyLose * 
                ( 1 - Mathf.InverseLerp(0f,100f, BaseDefenseManager.GetInstance().GetAccruacy() ))) ;
            float randomAngle = Random.Range(0, 360f);
            Vector3 accuracyOffset = new Vector3(
                Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance,
                Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
                0
            );


            // spawn dot for player to see
            var shotPoint = Instantiate(m_ShotPointPrefab,m_ShotDotParent);
            shotPoint.GetComponent<RectTransform>().position = accuracyOffset + BaseDefenseManager.GetInstance().GetCrosshairPos();

            
            // acc lose on shoot            
            BaseDefenseManager.GetInstance().SetAccruacy(
                BaseDefenseManager.GetInstance().GetAccruacy()-m_SelectedGun.Recoil
            );
            Destroy(shotPoint, 1);
        }

        m_CurrentShootCoolDown = 1 / m_SelectedGun.FireRate;
        ChangeAmmoCount(-1, false);
    }


    public float GetShootCoolDown(){
        return m_CurrentShootCoolDown;
    }

    private void ChangeAmmoCount(float num, bool isSetAmmoCount = false)
    {
        /*
        if (isSetAmmoCount)
        {
            m_CurrentAmmo = num;
        }
        else
        {
            m_CurrentAmmo += num;
        }
        if (m_CurrentAmmo > m_SelectedGun.ClipSize)
        {
            m_CurrentAmmo = m_SelectedGun.ClipSize;
        }

        m_AmmoText.text = $"{m_CurrentAmmo} / {m_SelectedGun.ClipSize}";*/
    }
    public GunScriptable GetSelectedGun(){
        return m_SelectedGun;
    }


    private void OnDestroy()
    {

    }
}
