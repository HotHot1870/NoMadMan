using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XinBodyPart : EnemyBodyPart
{
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private Color m_HealColor;
    [SerializeField] private Color m_Orange;

    protected override IEnumerator Start()
    {
        
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
        yield break;
    }

    public void SetXinBodyPart(bool isHideden){
        if(isHideden){
            foreach (var item in m_MeshRenderer.materials)
            {
                item.SetColor("_MainColor",m_Orange);
                ChangeBodyType(EnemyBodyPartEnum.Heal);
                StartCoroutine(HiddenEffect());
                m_Collider.enabled = false;
            }
        }else{
            foreach (var item in m_MeshRenderer.materials)
            {
                item.SetColor("_MainColor",m_Orange);
                SetDamageMod(1);
                ChangeBodyType(EnemyBodyPartEnum.Crit);
                StartCoroutine(ShowEffect());
                m_Collider.enabled = true;
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
