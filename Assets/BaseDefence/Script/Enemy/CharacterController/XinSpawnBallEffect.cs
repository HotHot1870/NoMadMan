using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XinSpawnBallEffect : MonoBehaviour
{    
    [SerializeField] private Transform m_Sphere;
    [SerializeField] private AudioSource m_AudioSourceStart;
    [SerializeField] private AudioSource m_AudioSourceWind;
    [SerializeField] private AudioClip m_SpawnSoundClip;
    [SerializeField] private AudioClip m_WindSoundClip;
    [SerializeField] private AnimationCurve m_SphereHeighCurve;
    // Start is called before the first frame update
    void Start()
    {
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioSourceStart);
        m_Sphere.localScale = Vector3.zero;
        StartCoroutine(SphereGrow());
        m_AudioSourceStart.PlayOneShot(m_SpawnSoundClip);
        m_AudioSourceWind.PlayOneShot(m_WindSoundClip);
    }

    private IEnumerator SphereGrow(){
        float passTime= 0f ;
        float duration = 1.5f;
        float maxSize = 125f;
        var mat = m_Sphere.GetComponent<MeshRenderer>().material;
        while (passTime<duration)
        {
            passTime += Time.deltaTime;
            mat.SetFloat("_Heigh",Mathf.Lerp(0.1f,0.5f, m_SphereHeighCurve.Evaluate(passTime / duration) ) );
            m_Sphere.localScale = Vector3.one * Mathf.Lerp(0f,maxSize, passTime / duration);
            yield return null;
        }
        
        Destroy(m_Sphere.gameObject,1);
        
    }
}
