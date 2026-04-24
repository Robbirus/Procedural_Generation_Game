using UnityEngine;

public abstract class ObjectBehaviour<INSTANCE, STATE, DATA> : MonoBehaviour
    where INSTANCE : ObjectInstance<STATE, DATA>
    where STATE : ObjectState<DATA>
    where DATA : ObjectData 
{
    protected INSTANCE instance;

    public INSTANCE GetInstance()
    {
        return instance;
    }

    public DATA GetData()
    {
        return instance.state.data;
    }

    public STATE GetState()
    {
        return instance.state;
    }

    public void SetInstance(INSTANCE instance)
    {
        this.instance = instance;
    }
}
