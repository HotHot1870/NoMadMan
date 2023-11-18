using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private Transform m_Self;
    [SerializeField] private float m_Speed = 10;
    [SerializeField] private AnimationCurve m_Curve;
    [SerializeField] private GameObject m_Explosion;
    private Vector3 m_Destination;
    private Vector3 m_StartPos ;

    private float m_StartDistince = 1;
    private float m_TimeNeedToReach = 1;
    private float m_Damage;
    private float m_Radius;

    public void Init(Vector3 destination, float damage , float radius){
        m_StartPos = m_Self.position;
        m_Destination = destination;
        m_StartDistince = Vector3.Distance(m_Self.position, m_Destination);
        m_TimeNeedToReach = m_StartDistince / m_Speed;
        m_Radius = radius;
        m_Damage = damage;
        StartCoroutine(Move());
        m_Self.LookAt(destination);
    }

    private IEnumerator Move(){
        float passedTime = 0;

        while(passedTime<m_TimeNeedToReach){
            var xzPos= Vector3.Lerp(m_StartPos, m_Destination, passedTime/m_TimeNeedToReach);
            float yPos = Mathf.Lerp(m_StartPos.y,m_Destination.y, m_Curve.Evaluate(passedTime/m_TimeNeedToReach));
            m_Self.position = new Vector3(xzPos.x,yPos,xzPos.z);
            passedTime += Time.deltaTime;
            yield return null;
        }

        // Explode
        var explosion = Instantiate(m_Explosion);
        explosion.transform.position = m_Destination;
        explosion.GetComponent<ExplosionController>().Init(m_Damage , m_Radius);
        Destroy(this.gameObject);
    }
}
