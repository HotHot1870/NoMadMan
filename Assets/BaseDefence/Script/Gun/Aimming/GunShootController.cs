using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseDefenceNameSpace;

[System.Serializable]
public class ProjectileDetail
{
    public BulletType BulletType;
    public GameObject Prefab;
}

public enum BulletType
{
    BasicBullet = 0,
    Puncture ,
    PillBomb ,
    Rocket ,
    BowActionArrow,
    BoomArrow


}
public class GunShootController : MonoBehaviour
{
    private GunScriptable m_SelectedGun = null;
    [SerializeField] private AudioClip m_OutOfAmmoSound;

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
    [Header("Projectile")]
    [SerializeField] private List<ProjectileDetail> m_ListProjectile = new List<ProjectileDetail>();
    [SerializeField] private Dictionary<BulletType,ProjectileDetail> m_AllProjectile = new Dictionary<BulletType,ProjectileDetail>();




    private void Start()
    {
        foreach (var item in m_ListProjectile)
        {
            m_AllProjectile.Add(item.BulletType,item);
        }
        BaseDefenceManager.GetInstance().m_UpdateAction += ShootCoolDown;
        MainGameManager.GetInstance().AddNewAudioSource(m_ShootAudioSource);

        m_SemiAutoShootCoroutine = null;

        //ChangeAmmoCount(0, true);
    }


    public void OnShootBtnDown(){
        if (m_CurrentAmmo <= 0)
        {
            // do not shoot if out of ammo
            m_ShootAudioSource.PlayOneShot(m_OutOfAmmoSound);
        }
        else
        {
            m_SemiAutoShootCoroutine = null;
            if (m_SemiAutoShootCoroutine == null && m_SelectedGun.GetStatValue("IsSemiAuto").ToString() == "Yes")
            {
                m_SemiAutoShootCoroutine = StartCoroutine(SemiAutoShoot());
                // semi auto 
                return;
            }
            //single shot
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
        BaseDefenceManager.GetInstance().StartReload(gunReloadConfig);
    }

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

        BaseDefenceManager.GetInstance().SetAccruacy((float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("Accuracy")));
        m_SemiAutoShootCoroutine = null;
        ChangeAmmoCount(m_GunsClipAmmo[slotIndex], true);
    }

    // on start ammo set up 
    public void SetUpGun(int slotIndex , GunScriptable gun){
        m_GunsClipAmmo.Add(slotIndex, (float) System.Convert.ToSingle(gun.GetStatValue("ClipSize")));
        if(m_SelectedGun == null){
            BaseDefenceManager.GetInstance().SwitchSelectedWeapon(slotIndex);

        }
    }

    private void GainAmmo(int changes)
    {
        ChangeAmmoCount(changes, false);
    }

    private bool IsFullClipAmmo()
    {
        return m_CurrentAmmo >= (float) System.Convert.ToSingle(m_SelectedGun.GetStatValue("ClipSize") );
    }

    private void SetClipAmmoToZero()
    {
        ChangeAmmoCount(0, true);
    }

    private void SetClipAmmoToFull()
    {
        ChangeAmmoCount((float) System.Convert.ToSingle(m_SelectedGun.GetStatValue("ClipSize")), true);
    }
    private IEnumerator SemiAutoShoot()
    {
        while (m_CurrentAmmo > 0)
        {
            Shoot();
            yield return null;
        }

        m_ShootAudioSource.PlayOneShot(m_OutOfAmmoSound);
    }



    private void Shoot()
    {
        if (m_CurrentShootCoolDown > 0)
            return;

        // shoot sound
        m_ShootAudioSource.PlayOneShot(m_SelectedGun.ShootSound);

        // muzzel
        var muzzleFlash = BaseDefenceManager.GetInstance().GetCurrentGunMuzzelPartical();
        if(muzzleFlash != null){
            muzzleFlash.Play();
        }

        var gunModelAnimator = BaseDefenceManager.GetInstance().GetCurrentGunAnimator();
        if(gunModelAnimator!=null){
            gunModelAnimator.Play("Shoot");
        }

        // shot all pellet
        for (int j = 0; j < (float) System.Convert.ToSingle(m_SelectedGun.GetStatValue("Pellet")); j++)
        {
            // random center to point distance
            
            float randomDistance = Random.Range(0, 
                BaseDefenceManager.GetInstance().GetCrosshairController().m_MaxAccuracyLose * 
                ( 1 - Mathf.InverseLerp(0f,100f, BaseDefenceManager.GetInstance().GetAccruacy() ))) ;
            float randomAngle = Random.Range(0, 360f);
            Vector3 accuracyOffset = new Vector3(
                Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance,
                Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
                0
            );

            // spawn dot for player to see
            var shotPoint = Instantiate(m_ShotPointPrefab,m_ShotDotParent);
            var dotPos = accuracyOffset + BaseDefenceManager.GetInstance().GetCrosshairPos();
            shotPoint.GetComponent<RectTransform>().position = dotPos;

            CaseRayWithShootDot(dotPos, shotPoint.GetComponent<ShootDotController>() );
            Destroy(shotPoint, 0.3f);
        }
        
            // acc lose on shoot            
            BaseDefenceManager.GetInstance().SetAccruacy(
                BaseDefenceManager.GetInstance().GetAccruacy()- (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("Recoil"))
            );

        m_CurrentShootCoolDown = 1 / (float) System.Convert.ToSingle(m_SelectedGun.GetStatValue("FireRate"));
        ChangeAmmoCount(-1, false);
    }

    private void CaseRayWithShootDot(Vector3 dotPos, ShootDotController dotController){
        Ray ray = Camera.main.ScreenPointToRay(dotPos);
        RaycastHit hitEnemy;
        RaycastHit hitEnvironmentAndEnemy;
        RaycastHit hitEnvironment;
        Physics.Raycast(ray,out hitEnemy,500, 1<<12);
        Physics.Raycast(ray,out hitEnvironmentAndEnemy,500, 1<<12|1<<10);
        Physics.Raycast(ray,out hitEnvironment,500, 1<<12|1<<10);
        

        //TODO : bullet type
        switch (m_SelectedGun.GunStats.BulletType)
        {
            case BulletType.BasicBullet:
                // normal 
                BasicShootHandler(hitEnemy,dotController);
            break;
            case BulletType.Puncture:
                // TODO : puncture
            break;
            case BulletType.PillBomb:
                var pillBomb = Instantiate(m_AllProjectile[BulletType.PillBomb].Prefab);
                // set spawn point to gun point
                pillBomb.transform.position = BaseDefenceManager.GetInstance().GetGunModelController().GetGunPoint();

                pillBomb.GetComponent<ProjectileController>().Init(hitEnvironmentAndEnemy.point,(float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("Damage")),m_SelectedGun.ExplodeRadius);

                break;
            case BulletType.Rocket:
                var rocket = Instantiate(m_AllProjectile[BulletType.Rocket].Prefab);
                // set spawn point to gun point
                rocket.transform.position = BaseDefenceManager.GetInstance().GetGunModelController().GetGunPoint();

                rocket.GetComponent<ProjectileController>().Init(hitEnvironmentAndEnemy.point,(float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("Damage")),m_SelectedGun.ExplodeRadius);
                break;
            case BulletType.BowActionArrow:
                var bowActionExplosion = Instantiate(m_AllProjectile[BulletType.BowActionArrow].Prefab);
                bowActionExplosion.transform.position = hitEnvironmentAndEnemy.point;
                bowActionExplosion.GetComponent<ExplosionController>().Init((float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("Damage")), m_SelectedGun.ExplodeRadius);
                break;
            default:
                break;
        }

    }

    private void BasicShootHandler(RaycastHit hit , ShootDotController dotController){
        if (hit.transform != null)//Physics.Raycast(ray, out hit, 500, 1<<12))
        {
            if(hit.transform.TryGetComponent<EnemyBodyPart>(out var bodyPart)){
                if(!bodyPart.IsDead()){
                    switch (bodyPart.GetBodyType())
                    {
                        case EnemyBodyPartEnum.Body:
                            dotController.OnHit();
                        break;
                        case EnemyBodyPartEnum.Shield:
                            dotController.OnHitShield();
                        break;
                        case EnemyBodyPartEnum.Crit:
                            dotController.OnCrit();
                        break;
                        default:
                        break;
                    }
                    bodyPart.OnHit((float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("Damage")) , Camera.main.WorldToScreenPoint(hit.point));
                }else{
                    dotController.OnMiss();
                }
            }
        }else{
            dotController.OnMiss();
        }
    }


    public float GetShootCoolDown(){
        return m_CurrentShootCoolDown;
    }

    private void ChangeAmmoCount(float num, bool isSetAmmoCount = false)
    {
        
        if (isSetAmmoCount)
        {
            m_CurrentAmmo = num;
        }
        else
        {
            m_CurrentAmmo += num;
        }
        if (m_CurrentAmmo > (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("ClipSize")))
        {
            m_CurrentAmmo = (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("ClipSize"));
        }

        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetAmmoText( $"{m_CurrentAmmo} / {(float)System.Convert.ToSingle(m_SelectedGun.GetStatValue("ClipSize"))}" );
    }
    public GunScriptable GetSelectedGun(){
        return m_SelectedGun;
    }


    private void OnDestroy()
    {

    }
}
