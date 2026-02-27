using Unity.Mathematics;
using UnityEngine;

public class TextureNoise : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int textureSizeX = 1024;
    [SerializeField] private int textureSizeY = 1024;

    [SerializeField] private NOISE_TYPE noiseType;
    [SerializeField] private float2 noiseOffset;
    [SerializeField] private float noiseScale = 100f;
    [SerializeField] private float resultPow = 1f;

    private Texture2D texture;

    private void Start()
    {
        GenerateTextureNoise();
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            GenerateTextureNoise();
        }
    }

    public void Generate()
    {
        GenerateTextureNoise();
    }

    private void GenerateTextureNoise()
    {
        Debug.Log("Noise scale : " + noiseScale);
        Debug.Log(noise.cellular(new float2(10, 20)));

        texture = new Texture2D(textureSizeX, textureSizeY);
        targetRenderer.material.mainTexture = texture;

        Color c = new Color(1f, 0f, 0f);

        for (int x = 0; x < textureSizeX; x++)
        {
            for(int y = 0; y < textureSizeY; y++)
            {
                float2 coordsOffseted = new float2(x + noiseOffset.x, y + noiseOffset.y);
                float2 coords = coordsOffseted / noiseScale;

                switch(noiseType)
                {
                    case NOISE_TYPE.CELLULAR:
                        float noiseX = noise.cellular(coords).x;
                        c = new Color(1f * noiseX, 0f * noiseX, 0f * noiseX);
                        texture.SetPixel(x, y, c);
                        break;

                    case NOISE_TYPE.PERLIN:
                        float noiseValue = noise.cnoise(coords);
                        c = new Color(1f * noiseValue, 0f * noiseValue, 0f * noiseValue);
                        texture.SetPixel(x, y, c);
                        break;

                    case NOISE_TYPE.SIMPLEX:
                        noiseValue = noise.snoise(coords);
                        c = new Color(1f * noiseValue, 0f * noiseValue, 0f * noiseValue);
                        texture.SetPixel(x, y, c);
                        break;

                }
            }
        }

        texture.Apply();
    }
}
