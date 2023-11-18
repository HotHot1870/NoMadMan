using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private ParticleSystem m_Smoke;
    [SerializeField] private Transform m_Sphere;
    [SerializeField] private AnimationCurve m_SphereSizeCurve;
    [SerializeField] private AudioSource m_AudioSource;


    public void Init(float damage , float radius){
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioSource);
        StartCoroutine(PlayExplosion(radius));
        // Damage
        var allEnemy = BaseDefenceManager.GetInstance().GetAllEnemyTrans();
        for(int i =0 ; i <allEnemy.Count;i++)
        {
            if(allEnemy[i] == null)
                continue;

            var distance = Vector3.Distance(allEnemy[i].position, this.transform.position);
            if(distance <= radius/2f){
                // in range 
                var enemyController = allEnemy[i].GetComponent<EnemyControllerBase>();
                enemyController.ChangeHp(damage * enemyController.GetScriptable().ExplodeDamageMod * -1);
            }
        }
    }

    private IEnumerator PlayExplosion(float radius){
        m_Smoke.Play();
        float passTime=0;
        float duration=0.5f;
        while (duration>passTime)
        {
            passTime += Time.deltaTime;
            float scale = Mathf.Lerp( 0f,radius/2f,  m_SphereSizeCurve.Evaluate( (duration-passTime)/duration) );
            m_Sphere.localScale= new Vector3( scale,scale,scale );
            yield return null;
        }

        Destroy(m_Self,5f);

    }
}
