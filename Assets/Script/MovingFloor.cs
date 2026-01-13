using UnityEngine;

public class MovingFloor : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float sinMultiplier = 3f;
    [SerializeField] private float moveSpeed = 2f;
    private Vector3 startPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float offsetZ = Mathf.Sin(Time.time * moveSpeed) * sinMultiplier;
        Vector3 targetPosition = startPosition + new Vector3(0, 0, offsetZ);

        rb.MovePosition(targetPosition);
    }

    // Transform で制御する方法
    // (上に乗ったものは連動しない）
    // この場合は物理処理ではないのでupdateに書いてもいいかも
    private void MoveOld()
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
