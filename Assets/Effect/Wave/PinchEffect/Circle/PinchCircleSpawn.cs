using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchCircleSpawn : MonoBehaviour
{

    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField][Range(0f,1f)] private float m_Normalized;
    [SerializeField][Range(0f,1f)] private float m_Size;
    [SerializeField] private AnimationCurve m_PinchCurve;
    [SerializeField] private AnimationCurve m_DarkenCurve;
    [SerializeField] private AnimationCurve m_SaturationCurve;
    [SerializeField] private AnimationCurve m_SizeCurve;
    private Material m_Mat = null;

    void Start()
    {
        m_Mat = m_SpriteRenderer.material;
        transform.localScale = new Vector3(1,1,1);
        
        float width = m_SpriteRenderer.sprite.bounds.size.x;
        float height = m_SpriteRenderer.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        float maxLength = Mathf.Max(worldScreenWidth / width, worldScreenHeight / height);
        
        transform.localScale = new Vector2( maxLength , maxLength );
        m_Mat.SetFloat("_ScreenRatio", (float)Screen.width/(float)Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        m_Normalized+=Time.deltaTime;
        m_Normalized = m_Normalized>=1?0:m_Normalized;
        
        m_Mat.SetFloat("_PinchStrength", m_PinchCurve.Evaluate(m_Normalized));
        m_Mat.SetFloat("_Saturation", m_SaturationCurve.Evaluate(m_Normalized));
        m_Mat.SetFloat("_ColorDarken", Mathf.Lerp(1f,5f ,m_DarkenCurve.Evaluate(m_Normalized) ));
        m_Mat.SetFloat("_Size", m_SizeCurve.Evaluate(m_Normalized) * m_Size);
        
    }
}
