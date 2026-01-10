using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public PlayerInputSet input;
    private Vector2 moveInput;

    private void Awake()
    {
        input = new PlayerInputSet();
    }

    private void Start()
    {
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        // プレイヤーの移動
        transform.position = new Vector3(moveInput.x, transform.position.y, moveInput.y);
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
