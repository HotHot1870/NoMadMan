using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    [SerializeField] private EnemyController m_EnemyController;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private Collider m_Collider;
    [SerializeField][Range(0f, 3f)] private float m_DamageMod = 1;
    [SerializeField] private bool m_IsShield = false;

    private void Start()
    {
        m_EnemyController.m_OnDead += OnDead;
    }

    public void OnHit(float damage)
    {
        m_EnemyController.ChangeHp(damage * m_DamageMod * -1);
    }

    public bool IsShield(){
        return m_IsShield;
    }

    public bool IsDead(){
        return m_EnemyController.IsDead();
    }

    public void OnDead()
    {
        // prevent blocking bullet after dead
        m_Collider.enabled = false;
        // burn effect
        StartCoroutine(BurnOut());
    }

    private IEnumerator BurnOut()
    {
        float passedTime = 0;
        float fadeTimeNeeded = 0.5f;
        while (passedTime < fadeTimeNeeded)
        {
            passedTime += Time.deltaTime;
            yield return null;
            m_Renderer.material.SetFloat("_Normalized",  passedTime / fadeTimeNeeded);

        }
        m_Renderer.material.SetFloat("_Normalized", 1);

    }

}
