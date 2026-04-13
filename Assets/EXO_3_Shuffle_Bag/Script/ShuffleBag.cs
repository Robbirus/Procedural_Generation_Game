using System;
using System.Collections.Generic;

public class ShuffleBag
{
    private int max;
    private int index = 0;
    private List<int> availableNumbers = new();

    public ShuffleBag(int max)
    {
        this.max = max;
        CreateBag();
        Shuffle();
    }

    private void CreateBag()
    {
        for(int i = 0; i < max; i++)
        {
            availableNumbers.Add(i);
        }
    }

    public int GetRandomNumber()
    {
        if(index >= availableNumbers.Count)
        {
            ResetBag();
        }
        return availableNumbers[index++];
    }

    private void ResetBag()
    {
        index = 0;
        Shuffle();
    }

    private void Shuffle()
    {
        for (int i = availableNumbers.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            int temp = availableNumbers[i];
            availableNumbers[i] = availableNumbers[j];
            availableNumbers[j] = temp;
        }
    }
}