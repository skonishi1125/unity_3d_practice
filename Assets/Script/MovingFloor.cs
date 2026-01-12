using UnityEngine;

public class MovingFloor : MonoBehaviour
{
    [SerializeField] private float sinValue;
    [Range(0, 1)]
    [SerializeField] private float sinMultiplier = .5f;
    [SerializeField] private float moveSpeed = 2f;

    private void Update()
    {
        Vector3 moveVector = transform.position;

        float sinValue = Mathf.Sin(Time.time) * sinMultiplier;
        Debug.Log(sinValue);

        if (sinValue > 0)
            moveVector.z = moveVector.z - moveSpeed * Time.deltaTime;
        else
            moveVector.z = moveVector.z + moveSpeed * Time.deltaTime;

        transform.position = moveVector;

    }

}
