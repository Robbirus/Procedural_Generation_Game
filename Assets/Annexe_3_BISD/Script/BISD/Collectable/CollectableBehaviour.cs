using UnityEngine;

public class CollectableBehaviour : ObjectBehaviour<CollectableInstance, CollectableState, CollectableData>
{
    private void Update()
    {
        this.gameObject.transform.Rotate(Vector3.up, Data.speedRotation * Time.deltaTime);
    }
}
