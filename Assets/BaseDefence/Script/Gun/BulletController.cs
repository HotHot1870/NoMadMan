using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField] private Transform m_Self;
    [SerializeField] private float m_Speed = 10;
    private Vector3 m_Destination;
    private Vector3 m_StartPos ;

    private float m_StartDistince = 1;
    private float m_TimeNeedToReach = 1;

    public void Init(Vector3 destination){
        m_StartPos = m_Self.position;
        m_Destination = destination;
        m_StartDistince = Vector3.Distance(m_Self.position, m_Destination);
        m_TimeNeedToReach = m_StartDistince / m_Speed;
        StartCoroutine(Move());
        m_Self.LookAt(destination);
    }
    

    private IEnumerator Move(){
        float passedTime = 0;

        while(passedTime<m_TimeNeedToReach){
            m_Self.position = Vector3.Lerp(m_StartPos, m_Destination, passedTime/m_TimeNeedToReach);
            passedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
