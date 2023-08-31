using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    [SerializeField] private EnemyController m_EnemyController;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField][Range(0f, 3f)] private float m_DamageMod = 1;

    private void Start()
    {
        m_EnemyController.m_OnDead += OnDead;
    }

    public void OnHit(float damage)
    {
        m_EnemyController.ChangeHp(damage * m_DamageMod * -1);
    }

    public void OnDead()
    {
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
            m_Renderer.material.SetFloat("_Normalized", (fadeTimeNeeded - passedTime) / fadeTimeNeeded);

        }
        m_Renderer.material.SetFloat("_Normalized", 0);

    }

}
