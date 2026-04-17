using UnityEngine;

public abstract class ObjectInjector<BEHAVIOUR, INSTANCE, STATE, DATA> : MonoBehaviour
     where BEHAVIOUR : ObjectBehaviour<INSTANCE, STATE, DATA>
     where INSTANCE : ObjectInstance<STATE, DATA>, new()
     where STATE : ObjectState<DATA>, new()
     where DATA : ObjectData
{
    [SerializeField] private DATA dataToInject;

    private void Awake()
    {
        STATE state = new STATE();
        state.data = dataToInject;

        INSTANCE instance = new INSTANCE();
        instance.state = state;

        GetComponent<BEHAVIOUR>().SetInstance(instance);
    }
}
