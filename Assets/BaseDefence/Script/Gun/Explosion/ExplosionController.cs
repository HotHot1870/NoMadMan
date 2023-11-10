using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private ParticleSystem m_Smoke;
    [SerializeField] private Transform m_Sphere;
    [SerializeField] private AnimationCurve m_SphereSizeCurve;


    public void Init(float damage , float radius){
        //m_Sphere.localScale = Vector3.zero;
        StartCoroutine(PlayExplosion(radius));
        // TODO : Damage
    }

    private IEnumerator PlayExplosion(float radius){
        m_Smoke.Play();
        float passTime=0;
        float duration=0.5f;
        while (duration>passTime)
        {
            passTime += Time.deltaTime;
            float scale = Mathf.Lerp( 0f,radius,  m_SphereSizeCurve.Evaluate( (duration-passTime)/duration) );
            m_Sphere.localScale= new Vector3( scale,scale,scale );
            yield return null;
        }

        yield return null;
        Destroy(m_Self);

    }
}
