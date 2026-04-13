using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class RandomManager : MonoBehaviour
{
    private ShuffleBag bag;
    private List<int> randomNumber = new();

    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private int spacing;
    [SerializeField] private int max = 100; 
    private int[] counts;
    [SerializeField] private int drawCount = 1000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        bag = new ShuffleBag(max);

        counts = new int[max];

        for (int i = 0; i < drawCount; i++)
        {
            int value = UnityEngine.Random.Range(0, max);
            counts[value]++;
        }

        SpawnCubes();
    }

    private void SpawnCubes()
    {
        for (int i = 0; i < max; i++)
        {
            int count = counts[i];

            Vector3 position = new Vector3(i + spacing, 0f, 0f);

            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);

            float scale = count == 0 ? 0.1f : count;

            cube.transform.localScale = new Vector3(1, scale, 1);
            cube.name = $"Cube_{i}_Count_{count}";
        }
    }
}
