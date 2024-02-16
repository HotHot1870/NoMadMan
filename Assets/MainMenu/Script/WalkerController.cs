using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerController : MonoBehaviour
{
    private Animator m_Animator;
    [SerializeField] private GameObject m_Self;
    [SerializeField] private List<RemnantModleAndAnimation> m_AllModleAndAnimation = new List<RemnantModleAndAnimation>();
    private Vector3 m_Destination;
    private float m_Speed = 2f;

    void Start(){
        // set random modle
        int randomIndex = UnityEngine.Random.Range(0,m_AllModleAndAnimation.Count);
        for (int i = 0; i < m_AllModleAndAnimation.Count; i++)
        {
            if(i==randomIndex){
                m_AllModleAndAnimation[i].m_Self.SetActive(true);
                m_Animator = m_AllModleAndAnimation[i].m_Animtor;
            }else{
                Destroy(m_AllModleAndAnimation[i].m_Self);
                m_AllModleAndAnimation.RemoveAt(i);
                i--;
                randomIndex--;
            }
        }
        m_Animator.Play("Move");
    }

    public void Init(Vector3 destination){
        m_Destination = destination;
        m_Speed = Random.Range(1f,3f);
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetFloat("Speed",m_Speed);
        float moveDistance = m_Speed * Time.deltaTime ;
        m_Self.transform.position = Vector3.MoveTowards(
            m_Self.transform.position, m_Destination, moveDistance);

        if(Vector3.Distance(m_Self.transform.position,m_Destination)<2f){
            Destroy(m_Self);
        }
    }
}
