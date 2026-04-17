using UnityEngine;

public abstract class ObjectBehaviour<INSTANCE, STATE, DATA> : MonoBehaviour
    where INSTANCE : ObjectInstance<STATE, DATA>
    where STATE : ObjectState<DATA>
    where DATA : ObjectData 
{
    protected INSTANCE instance;


    protected DATA Data => instance.state.data;

    protected STATE State => instance.state;

    public void SetInstance(INSTANCE instance)
    {
        this.instance = instance;
    }
}
