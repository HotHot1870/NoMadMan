using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeLineController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_Self;
    [SerializeField][Range(0f,360f)] private float m_Rotation = 0;
    [SerializeField][Range(0f,1f)] private float m_Normalized = 1f;

    void Start()
    {
        transform.localScale = new Vector3(1,1,1);
        
        float width = m_Self.sprite.bounds.size.x;
        float height = m_Self.sprite.bounds.size.y;

        float maxObjectLine = Mathf.Max(width, height);

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        float maxScreenLine = Mathf.Max(worldScreenHeight, worldScreenWidth);

        float targetScale = Mathf.Max(worldScreenWidth / width , worldScreenHeight / height);
        
        transform.localScale = new Vector2( targetScale, targetScale);
        
    }

    private void Update() {

        m_Self.material.SetFloat("_Normalized",m_Normalized);
        m_Self.material.SetFloat("_Rotation",m_Rotation);
    }
}
