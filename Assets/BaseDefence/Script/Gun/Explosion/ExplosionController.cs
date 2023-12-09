using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private ParticleSystem m_Smoke;
    [SerializeField] private Transform m_Sphere;
    [SerializeField] private AnimationCurve m_SphereSizeCurve;
    [SerializeField] private AudioSource m_AudioSource;
    private List<int> m_AllHittedEnemy = new List<int>();
    private float m_Damage;


    public void Init(float damage , float radius){
        MainGameManager.GetInstance().AddNewAudioSource(m_AudioSource);
        StartCoroutine(PlayExplosion(radius));
        m_Damage = damage;
        this.transform.localScale = Vector3.one * radius;
        var smokeMain = m_Smoke.main;
        smokeMain.startSize = new ParticleSystem.MinMaxCurve(radius*5f, radius*15f);
        StartCoroutine(RemoveCollider());
    }

    private IEnumerator RemoveCollider(){
        yield return new WaitForSeconds(0.15f);
        m_Self.GetComponent<SphereCollider>().enabled = false;
    }

    private IEnumerator PlayExplosion(float radius){
        m_Smoke.Play();
        float passTime=0;
        float duration=0.25f;
        while (duration>passTime)
        {
            passTime += Time.deltaTime;
            float scale = Mathf.Lerp( 0f,1f,  m_SphereSizeCurve.Evaluate( (duration-passTime)/duration) );
            m_Sphere.localScale= new Vector3( scale,scale,scale );
            yield return null;
        }

        Destroy(m_Self,3f);

    }

    private void OnTriggerEnter(Collider other)
    {
        var enemyBodyPart = other.GetComponent<EnemyBodyPart>();
        if(enemyBodyPart !=null){
            var enemySpawnId = enemyBodyPart.GetEnemySpawnId();
            if(!m_AllHittedEnemy.Contains(enemySpawnId)){
                // never hit this enemy , hit it
                m_AllHittedEnemy.Add(enemySpawnId);
                enemyBodyPart.ChangeHp(m_Damage * enemyBodyPart.GetExplosiveDamageMod() * -1);
                BaseDefenceManager.GetInstance().SetDamageText(
                    m_Damage * enemyBodyPart.GetExplosiveDamageMod(),
                    Color.yellow,
                    Camera.main.WorldToScreenPoint(other.transform.position));
                
            }

        }
    }
}
