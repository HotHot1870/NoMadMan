using System.Collections;
using System.Collections.Generic;
using ullScreenShockWaveControllerNameSpace;
using UnityEngine;

namespace ullScreenShockWaveControllerNameSpace
{
    [System.Serializable]
    public class FullScreenShockWaveConfig
    {
        public float WaveThiccness = 0.1f;
        public float WaveSize = 0.5f;
        public float WaveStrength = 5f;
        public float WaveSpeed = 0.1f;
        public float XPosNormalized = 0.5f;
        public float YPosNormalized = 0.5f;
        public int WaveCount = 1;

    }
}

public class FullScreenShockWaveController : MonoBehaviour
{
    private const string HOW_TO_USE = " - FullScreenEffectCamera with the SAME camera setting as main camera is required\n" +
        " - Set FullScreenEffectCamera as the chile of main camera with position and rotation of zero\n" +
        " - Add new layer FullScreenEffect \n" +
        " - In FullScreenEffectCamera >> Rendering >> Culling Mask , remove FullScreenEffect\n" +
        " - In FullScreenEffectCamera >> OutPut >> Output Texture , Create and add new render texture\n" +
        " - In FullScreenShockWave , change layer to FullScreenEffect\n" +
        " - In FullScreenShockWave >> Additional Setting >> Sorting Layer , Change to FullScreenEffect\n" +
        " - Sorting Layer FullScreenEffect MUST be the highest layer which cover ALL other layer\n" +
        " - In ShockWaveFullScreen(material) , add the render texture in \"RenderTexture\"\n";
    [Header(HOW_TO_USE)]
    [SerializeField] private SpriteRenderer m_FullScreenSpriteRenderer;
    [SerializeField] private AnimationCurve m_StrengthReduceOverTimeCurve;
    /*
    [SerializeField][Range(0.01f, 1f)] private float m_WaveThiccness = 0.5f;
    [SerializeField][Range(0.01f, 1f)] private float m_WaveSize = 0.5f;
    [SerializeField][Range(-5f, 5f)] private float m_WaveStrength = 5f;
    [SerializeField][Range(0f, 3f)] private float m_WaveSpeed = 0.1f;*/
    private FullScreenShockWaveConfig m_Config = null;
    private Material m_FullScreenShockWave = null;
    private float m_WaveCount = 0;

    private void Start()
    {
        m_FullScreenShockWave = m_FullScreenSpriteRenderer.material;
        //public float ScreenRatio = 9f/16f;
        
        m_FullScreenShockWave.SetFloat("_NormailzedTime", 0);
        m_FullScreenShockWave.SetFloat("_ScreenRatio", (float)Screen.width/(float)Screen.height);
            
        transform.localScale = new Vector3(1,1,1);
        
        float width = m_FullScreenSpriteRenderer.sprite.bounds.size.x;
        float height = m_FullScreenSpriteRenderer.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        
        transform.localScale = new Vector2( worldScreenWidth / width, worldScreenHeight / height);

    }
    private void Update()
    {
        if (m_Config == null || m_WaveCount>=m_Config.WaveCount)
        {
            return;
        }
        if (m_FullScreenShockWave)
        {
            if (Mathf.Abs(m_FullScreenShockWave.GetFloat("_NormailzedTime")) > m_Config.WaveSize)
            {
                m_WaveCount++;
                m_FullScreenShockWave.SetFloat("_Thiccness", m_Config.WaveThiccness);
                m_FullScreenShockWave.SetFloat("_Strength", m_Config.WaveStrength);
                m_FullScreenShockWave.SetFloat("_NormailzedTime", 0);
            }
            else
            {
                var normailzedTime = m_FullScreenShockWave.GetFloat("_NormailzedTime");
                m_FullScreenShockWave.SetFloat("_NormailzedTime",
                    normailzedTime + m_Config.WaveSpeed * Time.deltaTime);

                float strengthReduction = m_FullScreenShockWave.GetFloat("_Strength") >= 0 ? -1 : 1;
                m_FullScreenShockWave.SetFloat("_Strength",
                    Mathf.Lerp(m_Config.WaveStrength, 0, m_StrengthReduceOverTimeCurve.Evaluate(normailzedTime / m_Config.WaveSize)));

            }

        }
    }

    public void Init(FullScreenShockWaveConfig config)
    {
        m_Config = config;
        if (m_FullScreenShockWave)
        {
            m_WaveCount = 0;
            m_FullScreenShockWave.SetFloat("_Thiccness", config.WaveThiccness);
            m_FullScreenShockWave.SetFloat("_Strength", config.WaveStrength);
            m_FullScreenShockWave.SetFloat("_NormailzedTime", 0);
            m_FullScreenShockWave.SetVector("_CenterPosition", new Vector4(config.XPosNormalized , config.YPosNormalized,0,0));
        }
    }

}
