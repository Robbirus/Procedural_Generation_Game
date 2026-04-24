using UnityEngine;

public class PlayerInstance : ObjectInstance<PlayerState, PlayerData>
{
    public event System.Action<PlayerInstance> HasCollected;

    public void Collect(CollectableInstance collectable)
    {
        state.coin += collectable.GetData().value;

        HasCollected?.Invoke(this);
    }
}
