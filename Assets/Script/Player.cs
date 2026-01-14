using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public CharacterController controller;
    private Animator anim;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 2f;
    private Vector3 moveDirection;
    private Quaternion targetRotation; // クリック先の向き

    public PlayerInputSet input;
    private Vector2 moveInput;

    [Header("Movefloor")]
    [SerializeField] private float movefloorCheckDistance = 1.1f;
    [SerializeField] private LayerMask whatIsMovingfloor;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    private Vector3 velocity;


    private void Awake()
    {
        input = new PlayerInputSet();
        if (controller == null)
            controller = GetComponent<CharacterController>();

        anim = GetComponentInChildren<Animator>();

    }

    private void Start()
    {
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        SetAnimatorParams();
        ApplyGravity();
        Move();

        // 回転処理
        if (Input.GetMouseButtonDown(0))
            RotateToMouseCursor();

        // targetRotationの値に応じて、そちらに振り向く
        RotateSmoothly();

        // 右クリック テスト
        if (Input.GetMouseButtonDown(1))
            DebugMouseRay();

        // 移動床の処理
        MoveWithFloor();

    }

    private void SetAnimatorParams()
    {
        Debug.Log(moveDirection.normalized);
        float xVelocity = Vector3.Dot(moveDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(moveDirection.normalized, transform.forward);

        anim.SetFloat("xVelocity", xVelocity);
        anim.SetFloat("zVelocity", zVelocity);
    }

    private void DebugMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"マウス座標: {Input.mousePosition} / hit.point: {hit.point}");

            Vector3 p = hit.point;
            float s = .2f;

            // 十字マーカー（赤）
            Debug.DrawLine(p - Vector3.right * s, p + Vector3.right * s, Color.red, 10f);
            Debug.DrawLine(p - Vector3.forward * s, p + Vector3.forward * s, Color.red, 10f);
            Debug.DrawLine(p - Vector3.up * s, p + Vector3.up * s, Color.red, 10f);

            // Ray 自体も可視化（任意）
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 10f);

        }

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

            // B - A
            // Playerとターゲットへのベクトルを出す
            // その後正規化することで、ベクトルの大きさは考慮せず、向き成分だけ抽出する
            Vector3 targetDir = (posB - posA).normalized;
            targetDir.y = 0; // 縦方向は考慮せず、地面と水平な回転を考慮

            // Playerの現在向いている、Z軸（正面) ベクトルの取得
            // 正規化された単位ベクトルの形で返る。
            Vector3 forwardDir = transform.forward;
            Debug.Log(forwardDir);

            // クリックした位置が視野角内かどうかの判定をしてみる
            float dot = Vector3.Dot(forwardDir, targetDir); // 内積計算
            float viewingAngle = 0.707f; // とりあえず視野角 0.707 = cos45°とする
            // 内積: dot = |a| |b| cosθ
            // 正規化しているので|a|と|b|の長さ（ノルム）は 1。
            // なので、単純に dotとcosθの値を比較すればよい。
            if (dot > viewingAngle)
                Debug.Log($"<color=green>クリック位置は視野内です。</color> (内積: {dot:F2})");
            else
                Debug.Log($"<color=yellow>クリック位置は視界外です。</color> (内積: {dot:F2})");

            // 外積で判定をしてみる
            Vector3 cross = Vector3.Cross(forwardDir, targetDir);
            string side = cross.y > 0 ? "右" : "左";
            Debug.Log($"<color=cyan>{side}</color>振り向き (外積のy: {cross.y})");

            // 回転の適用
            if (targetDir != Vector3.zero)
            {
                // 即時振り向きの場合
                // transform.rotation = Quaternion.LookRotation(targetDir);
                targetRotation = Quaternion.LookRotation(targetDir);
            }

        }

    }

    private void RotateSmoothly()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
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
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(moveDirection * speed * Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);
    }

    private void MoveWithFloor()
    {
        RaycastHit hit;

        // 足元が移動床だった場合の処理
        if (Physics.Raycast(
            transform.position, Vector3.down, out hit, movefloorCheckDistance, whatIsMovingfloor
        ))
        {
            Rigidbody floorRb = hit.collider.GetComponent<Rigidbody>();
            if (floorRb != null && floorRb.isKinematic)
            {
                // 床の加速量を自身に加える
                Vector3 floorVelocity = floorRb.linearVelocity;
                controller.Move(floorVelocity * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 移動床設置判定
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.1f);

        // Z軸(前方)を青い線で描画
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
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
