using System;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInputSet input;

    [Header("Movement Parameter")]
    private Vector2 moveInput;
    [SerializeField] private float forceMultiplier = 5f;

    [Header("Jump Parameter")]
    [SerializeField] private float jumpForce = 12f;
    private bool jumpRequested = false;
    [SerializeField] private float groundCheckDistance = .6f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new PlayerInputSet();

    }

    private void Start()
    {
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        // 入力はUpdateで受け付け
        if (input.Player.Jump.WasPressedThisFrame())
            jumpRequested = true;

    }

    private void FixedUpdate()
    {
        Move();

        // ジャンプ処理自体はFixedで担当
        if (jumpRequested)
            Jump();

    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        // 操作性改善: 現在の進行方向と入力方向をチェック
        // 内積が - なら進行方向とは逆に入力があると判断できる
        // powerを上げて止まりやすくする
        float currentPower = forceMultiplier;
        float dot = Vector3.Dot(
            rb.linearVelocity.normalized,
            moveDirection.normalized
        );
        if (dot < 0)
            currentPower *= 2f;

        rb.AddForce(moveDirection * currentPower);

    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    private void Jump()
    {
        // ジャンプなどは瞬間的な衝撃を与えて飛ばしたい
        // ForceMode.Impulseを使う。
        if (IsGrounded())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpRequested = false;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // 接地判定Gizmo
        // これだと、world座標の(0,0,-1)に向かって常に線が引かれてしまうので、
        // transform.positionを常に足してやろう
        //Gizmos.DrawLine(transform.position, Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

}
