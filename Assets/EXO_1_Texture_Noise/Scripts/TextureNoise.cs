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

    [SerializeField] private Color color = new Color(1f, 0f, 0f);

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
        texture = new Texture2D(textureSizeX, textureSizeY);
        targetRenderer.material.mainTexture = texture;

        float noiseValue = 0f;

        for (int x = 0; x < textureSizeX; x++)
        {
            for(int y = 0; y < textureSizeY; y++)
            {
                float2 coordsOffseted = new float2(x + noiseOffset.x, y + noiseOffset.y);
                float2 coords = coordsOffseted * noiseScale;

                switch(noiseType)
                {
                    case NOISE_TYPE.CELLULAR:
                        noiseValue = noise.cellular(coords).x;
                        texture.SetPixel(x, y, GetColor(noiseValue));
                        break;

                    case NOISE_TYPE.PERLIN:
                        noiseValue = noise.cnoise(coords);
                        texture.SetPixel(x, y, GetColor(noiseValue));
                        break;

                    case NOISE_TYPE.SIMPLEX:
                        noiseValue = noise.snoise(coords);
                        texture.SetPixel(x, y, GetColor(noiseValue));
                        break;

                }
            }
        }

        texture.Apply();
    }

    private Color GetColor(float noise)
    {
        return new Color(color.r * noise * resultPow,
                         color.g * noise * resultPow,
                         color.b * noise * resultPow);
    }
}
