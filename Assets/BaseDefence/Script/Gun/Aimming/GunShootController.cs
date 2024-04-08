using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseDefenceNameSpace;
using Cinemachine;

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
    [SerializeField] private CinemachineVirtualCamera m_ShootCamera;
    private Coroutine m_CameraShakeCoroutine = null;

    [Header("Ammo")]
    private float m_CurrentAmmo = 0;
    private int m_CurrentWeaponSlotIndex = 0;
    private Dictionary<int, float> m_GunsClipAmmo = new Dictionary<int, float>(); // how many ammo left on gun when switching

    [Header("Shooting")]
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

    public void SetGunIdle(){
        var gunModelAnimator = BaseDefenceManager.GetInstance().GetCurrentGunAnimator();
        if(!gunModelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            gunModelAnimator.Play("Idle");
    }

    public void OnShootBtnDown(){
        if(BaseDefenceManager.GetInstance().GetCurHp() <=0){
            return;
        }
        if (m_CurrentAmmo <= 0)
        {
            // do not shoot if out of ammo
            m_ShootAudioSource.PlayOneShot(m_OutOfAmmoSound);
        }
        else
        {
            m_SemiAutoShootCoroutine = null;
            if (m_SemiAutoShootCoroutine == null && m_SelectedGun.GetStatValue(GunScriptableStatEnum.IsSemiAuto).ToString() == "Yes")
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

        BaseDefenceManager.GetInstance().SetAccruacy((float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Accuracy)));
        m_SemiAutoShootCoroutine = null;
        ChangeAmmoCount(m_GunsClipAmmo[slotIndex], true);
    }

    // on start ammo set up 
    public void SetUpGun(int slotIndex , GunScriptable gun){
        m_GunsClipAmmo.Add(slotIndex, (float) System.Convert.ToSingle(gun.GetStatValue(GunScriptableStatEnum.ClipSize)));
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
        return m_CurrentAmmo >= (float) System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.ClipSize) );
    }

    private void SetClipAmmoToZero()
    {
        ChangeAmmoCount(0, true);
    }

    public void SetClipAmmoToFull()
    {
        ChangeAmmoCount((float) System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.ClipSize)), true);
    }
    private IEnumerator SemiAutoShoot()
    {
        while (m_CurrentAmmo > 0 )
        {
            if(BaseDefenceManager.GetInstance().GetCurHp() <=0 || BaseDefenceManager.GetInstance().GameStage == BaseDefenceStage.Result)
                yield break;
            
            Shoot();
            yield return null;
        }

        m_ShootAudioSource.PlayOneShot(m_OutOfAmmoSound);
    }



    private IEnumerator ShakeCamera(){
        m_ShootCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 1f;
        m_ShootCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 5f;
        float passTime = 0;
        float duration =0.15f;
        while (passTime <duration)
        {
            passTime += Time.deltaTime;
            yield return null;
        }
        m_ShootCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0f;
        m_ShootCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;

    }

    private void Shoot()
    {
        if (m_CurrentShootCoolDown > 0 || BaseDefenceManager.GetInstance().GameStage == BaseDefenceStage.Result)
            return;

        if(BaseDefenceManager.GetInstance().GetCurHp() <=0)
            return;
        // shoot sound
        m_ShootAudioSource.PlayOneShot(m_SelectedGun.ShootSound);
        if(m_CameraShakeCoroutine!= null){
            StopCoroutine(m_CameraShakeCoroutine);
        }
        m_CameraShakeCoroutine = StartCoroutine(ShakeCamera());

        // muzzel
        var muzzleFlash = BaseDefenceManager.GetInstance().GetCurrentGunMuzzelPartical();
        if(muzzleFlash != null){
            muzzleFlash.Play();
        }

        var gunModelAnimator = BaseDefenceManager.GetInstance().GetCurrentGunAnimator();
        if(gunModelAnimator!=null && m_SelectedGun.DisplayName != "Minigun"){
            gunModelAnimator.Play("Shoot");
        }else if(m_SelectedGun.DisplayName == "Minigun" && !gunModelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot")){
            // play shoot animation only when shoot animation is not already playing for minigun
            gunModelAnimator.Play("Shoot");
        }

        // shot all pellet
        for (int j = 0; j < (float) System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Pellet)); j++)
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
            /*
            var shotPoint = Instantiate(m_ShotPointPrefab,m_ShotDotParent);
            shotPoint.GetComponent<RectTransform>().position = dotPos;

            Destroy(shotPoint, 0.3f);*/
            var dotPos = accuracyOffset + BaseDefenceManager.GetInstance().GetCrosshairPos();
            CaseRayWithShootDot(dotPos);

        }
        
            // acc lose on shoot            
            BaseDefenceManager.GetInstance().SetAccruacy(
                BaseDefenceManager.GetInstance().GetAccruacy()- (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Recoil))
            );

        m_CurrentShootCoolDown = 1 / (float) System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.FireRate));
        ChangeAmmoCount(-1, false);
    }

    private void CaseRayWithShootDot(Vector3 dotPos/*, ShootDotController dotController*/){
        Ray ray = Camera.main.ScreenPointToRay(dotPos);
        RaycastHit hitEnemy;
        RaycastHit hitEnvironmentAndEnemy;
        RaycastHit hitEnvironment;
        Physics.Raycast(ray,out hitEnemy,500, 1<<12);
        Physics.Raycast(ray,out hitEnvironmentAndEnemy,500, 1<<12|1<<10);
        Physics.Raycast(ray,out hitEnvironment,500, 1<<12|1<<10);
        
        switch ((BulletType)System.Enum.Parse( typeof(BulletType),m_SelectedGun.GetStatValue(GunScriptableStatEnum.BulletType).ToString().Trim()) )
        {
            case BulletType.BasicBullet:
                // normal 
                BasicShootHandler(hitEnvironmentAndEnemy/*,dotController*/);

                var bullet = Instantiate(m_AllProjectile[BulletType.BasicBullet].Prefab);
                // set spawn point to gun point
                bullet.transform.position = BaseDefenceManager.GetInstance().GetGunModelController().GetGunPoint();
                bullet.GetComponent<BulletController>().Init(hitEnvironment.point);
            break;
            case BulletType.Puncture:
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray, 500f, 1<<12|10);
                List<int> hitedEnemySpawnId = new List<int>();
                foreach (var item in hits)
                {
                    hitedEnemySpawnId.Add(BasicShootHandler(item,hitedEnemySpawnId)); 
                }
            break;
            case BulletType.PillBomb:
                var pillBomb = Instantiate(m_AllProjectile[BulletType.PillBomb].Prefab);
                // set spawn point to gun point
                pillBomb.transform.position = BaseDefenceManager.GetInstance().GetGunModelController().GetGunPoint();

                pillBomb.GetComponent<ProjectileController>().Init(hitEnvironmentAndEnemy.point,
                    (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Damage)),m_SelectedGun.ExplodeRadius);

                break;
            case BulletType.Rocket:
                var rocket = Instantiate(m_AllProjectile[BulletType.Rocket].Prefab);
                // set spawn point to gun point
                rocket.transform.position = BaseDefenceManager.GetInstance().GetGunModelController().GetGunPoint();

                rocket.GetComponent<ProjectileController>().Init(hitEnvironmentAndEnemy.point,
                    (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Damage)),m_SelectedGun.ExplodeRadius);
                break;
            case BulletType.BowActionArrow:
                var bowActionExplosion = Instantiate(m_AllProjectile[BulletType.BowActionArrow].Prefab);
                bowActionExplosion.transform.position = hitEnvironmentAndEnemy.point;
                bowActionExplosion.GetComponent<ExplosionController>().Init(
                    (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Damage)), m_SelectedGun.ExplodeRadius);
                break;
            default:
                break;
        }

    }

    private int BasicShootHandler(RaycastHit hit /*, ShootDotController dotController */,List<int> allHittedSpawnId = null){
        if (hit.transform != null)//Physics.Raycast(ray, out hit, 500, 1<<12))
        {
            if(hit.transform.TryGetComponent<EnemyBodyPart>(out var bodyPart)){
                
                int punctureCount = 0;
                // puncture bullet type
                if(allHittedSpawnId !=null){
                    punctureCount = allHittedSpawnId.Count;
                    // check if already hitted if list provide
                    if(allHittedSpawnId.Contains(bodyPart.GetEnemySpawnId())){
                        // already hitted , skip
                        return bodyPart.GetEnemySpawnId();
                    }
                }
                if(!bodyPart.IsDead()){
                    /*
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
                    }*/

                    // foreach puncture count , reduce damage by 50%
                    bodyPart.OnHit((float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.Damage) ) * 
                        (1 * Mathf.Pow( 0.5f ,punctureCount)), 
                        Camera.main.WorldToScreenPoint(hit.point),
                        hit.point);
                }/*else{
                    dotController.OnMiss();
                }*/
                
                return bodyPart.GetEnemySpawnId();
            }else if(hit.transform.TryGetComponent<GroundController>(out var groundController)){
                // if hit ground , if so , emit mud
                groundController.EmitMud(hit.point);
            }
            //dotController.OnMiss();
        }else{
        }
        return -1;
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
        if (m_CurrentAmmo > (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.ClipSize)))
        {
            m_CurrentAmmo = (float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.ClipSize));
        }

        BaseDefenceManager.GetInstance().GetBaseDefenceUIController().SetAmmoText(
             $"{m_CurrentAmmo} / {(float)System.Convert.ToSingle(m_SelectedGun.GetStatValue(GunScriptableStatEnum.ClipSize))}" );
    }
    public GunScriptable GetSelectedGun(){
        return m_SelectedGun;
    }


    private void OnDestroy()
    {

    }
}
