using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOff : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> m_Renderer = new List<MeshRenderer>();
    [SerializeField] private List<SkinnedMeshRenderer> m_SkinRenderer = new List<SkinnedMeshRenderer>();
    private bool m_IsIncrease = true;
     float passedTime = 0;
    void Update()
    {
        float fadeTimeNeeded = 2f;
        if(m_IsIncrease){
            passedTime += Time.deltaTime;
        }else{
            passedTime -= Time.deltaTime;
        }
        for (int i = 0; i < Mathf.Max(m_SkinRenderer.Count,m_Renderer.Count); i++)
        {
            if(m_Renderer.Count > i ){
                if( m_Renderer[i] != null)
                    m_Renderer[i].material.SetFloat("_Normalized",  (fadeTimeNeeded- passedTime) / fadeTimeNeeded);
            }

            if(m_SkinRenderer.Count > i){
                if( m_SkinRenderer != null)
                m_SkinRenderer[i].material.SetFloat("_Normalized",  (fadeTimeNeeded-passedTime) / fadeTimeNeeded);

            }
            
        }

        if(passedTime>fadeTimeNeeded||passedTime<0){
            m_IsIncrease = !m_IsIncrease;
            passedTime = Mathf.Clamp( fadeTimeNeeded,0,passedTime);
            
            for (int i = 0; i < Mathf.Max(m_SkinRenderer.Count,m_Renderer.Count); i++)
            {
                if(m_Renderer.Count > i ){
                    if( m_Renderer[i] != null)
                        m_Renderer[i].material.SetFloat("_Seed",  Random.Range(0f,1f));
                }

                if(m_SkinRenderer.Count > i){
                    if( m_SkinRenderer != null)
                    m_SkinRenderer[i].material.SetFloat("_Seed",  Random.Range(0f,1f));

                }
                
            }
        }
        

        
    }
}
