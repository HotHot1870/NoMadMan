using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndGameXinSpawner : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer m_XinRenderer;
    [SerializeField] private CreditController m_CreditController;
    [SerializeField] private PlayableDirector m_Director;
    private Material m_XinMat;

    void Start(){
        m_XinMat = m_XinRenderer.material;
        StartCoroutine(SpawnXin());
        m_Director.Play();
    }

    private IEnumerator SpawnXin(){
        float passTime = 0f;
        float duration = 15f;
        while (passTime<duration)
        {
            passTime+=Time.deltaTime;
            yield return null;
            m_XinMat.SetFloat("_Normalized",Mathf.Clamp01(passTime/duration) );
        }
        
        yield return null;
        while (m_Director.state == PlayState.Playing)
        {
            yield return null;
        }
        m_CreditController.Init();
    }
}
