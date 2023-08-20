using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavenSpawnController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private AnimationCurve m_RayAnimationCurve;
    [SerializeField][Range(0f,1f)] private float m_Normailzed = 0;
    private Material m_Material = null;

    private void Start() {
        m_Material = m_SpriteRenderer.material;
    }

    private void Update() {
        if(m_Material){
            m_Material.SetFloat("_MainNormalized",m_Normailzed);
            m_Material.SetFloat("_CloudNormalized", m_RayAnimationCurve.Evaluate(1-m_Normailzed));
            m_Material.SetFloat("_RayNormalized", m_RayAnimationCurve.Evaluate(1-m_Normailzed));
        }
    }
}
