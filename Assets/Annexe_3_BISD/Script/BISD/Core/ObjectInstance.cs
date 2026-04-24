using UnityEngine;

public abstract class ObjectInstance<STATE, DATA>
    where STATE : ObjectState<DATA>
    where DATA : ObjectData
{
    public STATE state;

    public DATA GetData()
    {
        return this.state.data;
    }
}
