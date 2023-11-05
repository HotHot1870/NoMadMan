using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBodyPartEnum
{
    Body = 0,
    Shield = 1,
    Crit = 2
}


public class EnemyBodyPart : MonoBehaviour
{
    [SerializeField] private EnemyControllerBase m_EnemyController;
    [SerializeField] private ParticleSystem m_OnHitEffect=null;
    [SerializeField] private Collider m_Collider;
    [SerializeField][Range(0f, 3f)] private float m_DamageMod = 1;
    [SerializeField] private EnemyBodyPartEnum m_BodyType;
    [SerializeField] private MeshRenderer m_Renderer = null;
    [SerializeField] private SkinnedMeshRenderer m_SkinRenderer = null;
    private float m_EmissionDelay = 0;

    private IEnumerator Start()
    {
        
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

    public void OnHit(float damage)
    {
        m_EnemyController.ChangeHp(damage * m_DamageMod * -1);

        // hit effect
        if(m_OnHitEffect != null && m_EmissionDelay<=0){
            var hitEffect = Instantiate(m_OnHitEffect,this.transform);
            hitEffect.Play();
            hitEffect.transform.position = this.transform.position;
            Destroy(hitEffect.gameObject,3f);
            // prevent shotgun emit too much 
            m_EmissionDelay = 0.1f;
            StartCoroutine(EmitHitEffectDelay());
        }
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
        // prevent blocking bullet after dead
        m_Collider.enabled = false;
        // burn effect
        StartCoroutine(OnDeadEffect());
    }

    private IEnumerator OnDeadEffect()
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
            if(m_Renderer != null)
                m_Renderer.material.SetFloat("_Normalized",  (fadeTimeNeeded- passedTime) / fadeTimeNeeded);

            if(m_SkinRenderer != null)
                m_SkinRenderer.material.SetFloat("_Normalized",  (fadeTimeNeeded-passedTime) / fadeTimeNeeded);

        }
        
        if(m_Renderer != null)
            m_Renderer.material.SetFloat("_Normalized",  0);

        if(m_SkinRenderer != null)
            m_SkinRenderer.material.SetFloat("_Normalized",  0);

    }

}
