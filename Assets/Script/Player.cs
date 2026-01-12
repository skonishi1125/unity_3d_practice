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

        // 回転処理
        if (Input.GetMouseButtonDown(0))
            RotateToMouseCursor();

    }

    private void RotateToMouseCursor()
    {
        // マウス位置からRayを作成
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 座標取得 (A: Player B: クリック地点)
            Vector3 posA = transform.position;
            Vector3 posB = hit.point;

            // ベクトルの引き算
            Vector3 direction = posB - posA;
            direction.y = 0; // 一旦縦方向は考慮せず、地面と水平な回転を考慮

            if (direction != Vector3.zero)
            {
                // 3. 現在の角度と、目標の角度を取得
                //float currentYAngle = transform.eulerAngles.y; // クォータニオンの値を89.5°など、Euler角として出す
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                //float targetYAngle = targetRotation.eulerAngles.y;

                // 4. 数式と結果をログに出力
//                Debug.Log($@"--- 3D Rotation Math Log ---
//[Points] Player(A): {posA}, Click(B): {posB}
//[Formula] B - A = Direction: {direction}
//[Rotation] Current Y: {currentYAngle:F1}°, Target Y: {targetYAngle:F1}°
//[Delta] Rotate Amount: {Mathf.DeltaAngle(currentYAngle, targetYAngle):F1}°
//----------------------------");

                // 回転適用
                transform.rotation = targetRotation;

                // 5. シーンビューに計算に使った矢印を1秒間表示
                //Debug.DrawRay(posA, direction, Color.red, 1.0f);
            }

        }

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

    private void OnDrawGizmos()
    {
        // 常に前方を青い線で描画（Z軸方向）
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // 足元に範囲を表示（CharacterControllerの太さの目安）
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
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
