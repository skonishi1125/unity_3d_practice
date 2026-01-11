using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;

    [Header("Movement Settings")]
    public float speed = 5f;

    public PlayerInputSet input;
    private Vector2 moveInput;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    private Vector3 velocity;


    private void Awake()
    {
        input = new PlayerInputSet();
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    private void Start()
    {
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        ApplyGravity();
        Move();
    }

    private void ApplyGravity()
    {
        // 地面に接地しているときは、速度をリセット（少しだけマイナスにして地面に押し付ける）
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 重力加速度を計算 (v = g * t)
        velocity.y += gravity * Time.deltaTime;
    }

    private void Move()
    {
        // 入力(Vector2)を、3D空間の移動方向(Vector3)に変換する
        // 2DのY（上下）を、3DのZ（前後）に入れ替える
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(moveDirection * speed * Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);
    }



    // オブジェクトが有効なときだけ入力をうけつけるようにする
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

}
