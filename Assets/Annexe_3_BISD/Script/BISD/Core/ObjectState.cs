using UnityEngine;

public abstract class ObjectState<DATA> 
    where DATA: ObjectData 
{
    public DATA data;
}
