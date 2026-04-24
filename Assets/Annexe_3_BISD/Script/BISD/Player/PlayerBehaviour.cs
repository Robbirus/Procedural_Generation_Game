using UnityEngine;

public class PlayerBehaviour : ObjectBehaviour<PlayerInstance, PlayerState, PlayerData>
{
    private void OnTriggerEnter(Collider other)
    {
        CollectableBehaviour collectableBehaviour = other.gameObject.GetComponentInParent<CollectableBehaviour>();

        if (collectableBehaviour != null)
        {
            Debug.Log("Collectable collected");

            this.instance.Collect(collectableBehaviour.GetInstance());

            Debug.Log(this.GetState().coin);

            Destroy(other.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 newPos = transform.localPosition;
            newPos.x += this.GetData().speed * Time.deltaTime;

            transform.localPosition = newPos;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 newPos = transform.localPosition;
            newPos.x -= this.GetData().speed * Time.deltaTime;

            transform.localPosition = newPos;
        }
    }
}
