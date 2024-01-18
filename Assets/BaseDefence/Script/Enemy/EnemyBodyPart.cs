using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public enum EnemyBodyPartEnum
{
    Body = 0,
    Shield = 1,
    Crit = 2,
    Heal = 3
}


public class EnemyBodyPart : MonoBehaviour
{
    [SerializeField] private EnemyControllerBase m_EnemyController;
    [SerializeField] private ParticleSystem m_OnHitEffect=null;
    [SerializeField] private float m_DamageMod = 1;
    [SerializeField] private EnemyBodyPartEnum m_BodyType;
    [SerializeField] private MeshRenderer m_Renderer = null;
    [SerializeField] private SkinnedMeshRenderer m_SkinRenderer = null;
    [SerializeField] private float m_BodyPartHpPresentage = 1f;
    private Collider m_Collider;
   // [SerializeField] private AudioSource m_AudioPlayer;
   // [SerializeField] private AudioClip m_OnHitSound;
    private bool m_CanPlayHitSound = true;
    private float m_EmissionDelay = 0;

    private IEnumerator Start()
    {
       // m_OnHitSound = Resources.Load<AudioClip>("Enemy/Audio/Hit");
       // m_AudioPlayer = this.AddComponent<AudioSource>();
        m_Collider = this.GetComponent<Collider>();
        if(m_Renderer != null)
            m_Renderer.material.SetFloat("_Normalized",  0);

        if(m_SkinRenderer != null)
            m_SkinRenderer.material.SetFloat("_Normalized",  0);
            
        m_EnemyController.OnDeadAction += OnDead;

        // seed value
        if(m_Renderer != null)
            m_Renderer.material.SetFloat("_Seed", Random.Range(0f,1f));

        if(m_SkinRenderer != null)
            m_SkinRenderer.material.SetFloat("_Seed", Random.Range(0f,1f));

        // Spawn effect
        if(m_Renderer == null && m_SkinRenderer == null)
            yield break;

        float passedTime = 0;
        float fadeTimeNeeded = 0.9f;
        while (passedTime < fadeTimeNeeded)
        {
            passedTime += Time.deltaTime;
            yield return null;
            if(m_Renderer != null)
                m_Renderer.material.SetFloat("_Normalized",  passedTime / fadeTimeNeeded);

            if(m_SkinRenderer != null)
                m_SkinRenderer.material.SetFloat("_Normalized",  passedTime/ fadeTimeNeeded);

        }
        if(m_Renderer != null)
            m_Renderer.material.SetFloat("_Normalized",  1);

        if(m_SkinRenderer != null)
            m_SkinRenderer.material.SetFloat("_Normalized",  1);
    }

    public void SetDamageMod(float damageMod){
        m_DamageMod = damageMod;
    }

    public void SetBackColor(Color color){
        if(m_Renderer != null)
            m_Renderer.material.SetColor("_BackColor", color);

        if(m_SkinRenderer != null)
            m_SkinRenderer.material.SetColor("_BackColor", color);

    }
    
    public void SetSpawnMeshNormalized(float setValue){
        if(m_Renderer != null)
            m_Renderer.material.SetFloat("_Normalized",  setValue);

        if(m_SkinRenderer != null)
            m_SkinRenderer.material.SetFloat("_Normalized",  setValue);
        
    }
    public float GetSpawnMeshNormalized(){
        if(m_Renderer != null)
            return m_Renderer.material.GetFloat("_Normalized");

        if(m_SkinRenderer != null)
            return m_SkinRenderer.material.GetFloat("_Normalized");

        return 0;

    }

    public void ChangeMaterialNoiseDensity(float changes){

        float baseValue = 0;
        if(m_Renderer != null){
            baseValue = m_Renderer.material.GetFloat("_NoiseDensity");
        }else if(m_SkinRenderer != null){
            baseValue = m_SkinRenderer.material.GetFloat("_NoiseDensity");
        }


        if(m_Renderer != null)
            m_Renderer.material.SetFloat("_NoiseDensity",  baseValue+changes);

        if(m_SkinRenderer != null)
            m_SkinRenderer.material.SetFloat("_NoiseDensity",  baseValue+changes);
    }


    public float GetExplosiveDamageMod(){
        return m_EnemyController.GetScriptable().ExplodeDamageMod;
    }

    public void ChangeHp(float changes){
        m_EnemyController.ChangeHp(changes);
    }

    public virtual void OnHit(float damage, Vector2 screenPos, Vector3 hitPos)
    {
        m_BodyPartHpPresentage -= ((damage * m_DamageMod) / m_EnemyController.GetMaxHp());
        m_EnemyController.ChangeHp(damage * m_DamageMod * -1);
        

        // hit effect
        if(m_OnHitEffect != null && m_EmissionDelay<=0){
            var hitEffect = Instantiate(m_OnHitEffect);
            hitEffect.transform.position = hitPos;
            hitEffect.Play();
            Destroy(hitEffect.gameObject,2f);
            // prevent shotgun emit too much 
            m_EmissionDelay = 0.2f;
            StartCoroutine(EmitHitEffectDelay());
        }

        // show damage
        Color color= new Color(1,0,0) ;
        switch (m_BodyType)
            {
                case EnemyBodyPartEnum.Body:
                    color = Color.white;
                break;
                case EnemyBodyPartEnum.Shield:
                    color = Color.blue;
                break;
                case EnemyBodyPartEnum.Crit:
                    color = Color.red;
                break;
                case EnemyBodyPartEnum.Heal:
                    color = Color.green;
                break;
                default:
                break;
            }

        BaseDefenceManager.GetInstance().SetDamageText(damage * m_DamageMod,color,screenPos);
        if(m_BodyPartHpPresentage <=0){
            // Destroy body part
            StartCoroutine(OnDeadEffect());
/*
            if(!m_EnemyController.IsDead()){
            //  play fall back animation if not dead
                m_EnemyController.FallBack();
            }*/
        }
/*
        // Hit sound
        if(m_CanPlayHitSound){
            m_AudioPlayer.PlayOneShot(m_OnHitSound);
            m_CanPlayHitSound = false;
            StartCoroutine(HitSoundBlocker());
        }*/
    }
    public void ChangeBodyType(EnemyBodyPartEnum bodytype){
        m_BodyType = bodytype;
    }
/*
    private IEnumerator HitSoundBlocker(){
        // prevent multi sound play on shot gun hit
        float duration = 0.05f;
        float passedTime = 0;
        while (passedTime < duration)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }
        m_CanPlayHitSound = true;
    }*/

    public int GetEnemySpawnId(){
        return m_EnemyController.GetId();
    }

    private IEnumerator EmitHitEffectDelay(){
        while (m_EmissionDelay>=0)
        {
            m_EmissionDelay -=Time.deltaTime;
            yield return null;
        }

    }

    public EnemyBodyPartEnum GetBodyType(){
        return m_BodyType;
    }

    public bool IsDead(){
        return m_EnemyController.IsDead();
    }

    public void OnDead()
    {
        if(this == null){
            return;
        }
        // prevent blocking bullet after dead
        if(m_Collider != null)
            m_Collider.enabled = false;
        // dead effect
        StartCoroutine(OnDeadEffect());
    }

    protected virtual IEnumerator OnDeadEffect()
    {
        if(m_Renderer == null && m_SkinRenderer == null)
            yield break;

        float fadeTimeNeeded = 0.9f;
        // to make it look more responsive
        float passedTime = fadeTimeNeeded*0.15f;
        while (passedTime < fadeTimeNeeded)
        {
            passedTime += Time.deltaTime;
            yield return null;
            if(m_Renderer != null){
                foreach (var item in m_Renderer.materials)
                {
                    item.SetFloat("_Normalized",  (fadeTimeNeeded- passedTime) / fadeTimeNeeded);
                }
            }

            if(m_SkinRenderer != null){
                foreach (var item in m_SkinRenderer.materials)
                {
                    item.SetFloat("_Normalized",  (fadeTimeNeeded- passedTime) / fadeTimeNeeded);
                }
            }

        }
        
        if(m_Renderer != null)
            m_Renderer.material.SetFloat("_Normalized",  0);

        if(m_SkinRenderer != null)
            m_SkinRenderer.material.SetFloat("_Normalized",  0);

        if(this.gameObject != null)
            Destroy(this.gameObject);

    }

}
