using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XinBodyPart : EnemyBodyPart
{
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private Color m_HealColor;
    [SerializeField] private Color m_Orange;
    private Coroutine m_Spawning = null;

    protected void Awake(){
        m_Collider = this.GetComponent<BoxCollider>();
        ChangeBodyType(EnemyBodyPartEnum.Crit);
    }

    protected override IEnumerator Start()
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
        yield break;
    }

    public void SetXinBodyPart(bool isHideden){
        if(m_Spawning != null){
            StopCoroutine(m_Spawning);
        }
        if(isHideden){
            foreach (var item in m_MeshRenderer.materials)
            {
                item.SetColor("_MainColor",m_Orange);
                //ChangeBodyType(EnemyBodyPartEnum.Heal);
                m_Spawning = StartCoroutine(HiddenEffect());
                m_Collider.enabled = false;
            }
        }else{
            foreach (var item in m_MeshRenderer.materials)
            {
                item.SetColor("_MainColor",m_Orange);
                SetDamageMod(1);
                m_Spawning = StartCoroutine(ShowEffect());
                m_Collider.enabled = true;
            }
        }
    }

    public void SetNormalized(float normalized, bool hideOnLower = true){
        if(m_Renderer != null){
            foreach (var item in m_Renderer.materials)
            {
                if(!hideOnLower && normalized < item.GetFloat("_Normalized"))
                    item.SetFloat("_Normalized",  normalized);
            }
        }

        if(m_SkinRenderer != null){
            foreach (var item in m_SkinRenderer.materials)
            { 
                if(!hideOnLower && normalized < item.GetFloat("_Normalized"))
                    item.SetFloat("_Normalized",  normalized);
            }
        }
    }

    private IEnumerator HiddenEffect(){
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
                    if(item.GetFloat("_Normalized")<=0)
                        continue;

                    item.SetFloat("_Normalized",  (fadeTimeNeeded- passedTime) / fadeTimeNeeded);
                }
            }

            if(m_SkinRenderer != null){
                foreach (var item in m_SkinRenderer.materials)
                {
                    if(item.GetFloat("_Normalized")<=0)
                        continue;
                        
                    item.SetFloat("_Normalized",  (fadeTimeNeeded- passedTime) / fadeTimeNeeded);
                }
            }

        }
    }
/*
    public void DespawnImmediate(){
        if(m_Spawning != null){
            StopCoroutine(m_Spawning);
        }
        if(m_Renderer != null){
            foreach (var item in m_Renderer.materials)
            {
                item.SetFloat("_Normalized",  0);
            }
        }

        if(m_SkinRenderer != null){
            foreach (var item in m_SkinRenderer.materials)
            {
                item.SetFloat("_Normalized",  0);
            }
        }
    }*/

    private IEnumerator ShowEffect(){
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
                    item.SetFloat("_Normalized",  passedTime / fadeTimeNeeded);
                }
            }

            if(m_SkinRenderer != null){
                foreach (var item in m_SkinRenderer.materials)
                {
                    item.SetFloat("_Normalized",  passedTime / fadeTimeNeeded);
                }
            }

        }
    }
}
