using UnityEngine;

public class PlayerBehaviour : ObjectBehaviour<PlayerInstance, PlayerState, PlayerData>
{
    private void OnTriggerEnter(Collider other)
    {
        CollectableBehaviour collectableBehaviour = other.gameObject.GetComponentInParent<CollectableBehaviour>();

        if (collectableBehaviour != null)
        {
            Debug.Log("Collectable collected");
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 newPos = transform.localPosition;
            newPos.x += Data.speed * Time.deltaTime;

            transform.localPosition = newPos;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 newPos = transform.localPosition;
            newPos.x -= Data.speed * Time.deltaTime;

            transform.localPosition = newPos;
        }
    }
}
