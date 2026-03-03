using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class TextureNoiseSeed : MonoBehaviour
{
    [Header("Noise Settings")]
    [Tooltip("Renderer component that will display the generated noise texture. The generated texture will be assigned to the material of this renderer. Changing this value will change where the noise texture is displayed in the scene.")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int textureSizeX = 1024;
    [SerializeField] private int textureSizeY = 1024;

    [SerializeField] private NOISE_TYPE noiseType;
    [SerializeField] private float2 noiseOffset;
    [SerializeField] private float noiseScale = 100f;
    [SerializeField] private float resultPow = 1f;

    [Header("Color")]
    [Tooltip("Base color for the noise texture. The final color of each pixel will be this base color multiplied by the noise value and the resultPow. Changing this value will affect the overall color of the generated noise texture.")]
    [SerializeField] private Color color = new Color(1f, 0f, 0f);

    [Header("Seed")]
    [Tooltip("Seed for the random generation of noise parameters. Changing this value will generate a different noise pattern.")]
    [SerializeField] private int seed = 0;
    [Tooltip("Minimum and maximum values for the random generation of noise offset. Changing these values will affect the range of possible noise patterns generated with different seeds.")]
    [SerializeField] private float2 offsetMinMax;
    [Tooltip("Minimum and maximum values for the random generation of noise scale. Changing these values will affect the range of possible noise patterns generated with different seeds.")]
    [SerializeField] private float2 scaleMinMax;

    private Texture2D texture;

    private void Start()
    {
        Generate();
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            Generate();
        }
    }

    public void Generate()
    {
        StopAllCoroutines();
        StartCoroutine(GenerateTextureNoiseCoroutine(seed));
    }

    private IEnumerator GenerateTextureNoiseCoroutine(int seed)
    {
        UnityEngine.Random.InitState(seed);

        noiseOffset = new float2(
            UnityEngine.Random.Range(offsetMinMax.x, offsetMinMax.y),
            UnityEngine.Random.Range(offsetMinMax.x, offsetMinMax.y)
        );

        noiseScale = UnityEngine.Random.Range(scaleMinMax.x, scaleMinMax.y);

        texture = new Texture2D(textureSizeX, textureSizeY);
        targetRenderer.material.mainTexture = texture;

        float noiseValue = 0f;

        for (int x = 0; x < textureSizeX; x++)
        {
            for (int y = 0; y < textureSizeY; y++)
            {
                float2 coordsOffseted = new float2(x + noiseOffset.x, y + noiseOffset.y);
                float2 coords = coordsOffseted / noiseScale;

                switch (noiseType)
                {
                    case NOISE_TYPE.CELLULAR:
                        noiseValue = noise.cellular(coords).x;
                        break;

                    case NOISE_TYPE.PERLIN:
                        noiseValue = noise.cnoise(coords);
                        noiseValue = math.remap(-1f, 1f, 0f, 1f, noiseValue);
                        break;

                    case NOISE_TYPE.SIMPLEX:
                        noiseValue = noise.snoise(coords);
                        noiseValue = math.remap(-1f, 1f, 0f, 1f, noiseValue);
                        break;
                }

                texture.SetPixel(x, y, GetColor(noiseValue));
            }

            // Applique la texture partiellement chaque ligne
            texture.Apply();

            // Attend une frame pour effet progressif
            yield return null;
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
