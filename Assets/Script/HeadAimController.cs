using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadAimController : MonoBehaviour
{
    private Transform currentTarget;

    [SerializeField] private MultiAimConstraint headAim;
    [SerializeField] private float baseAngle = 60f; // ex: 60fの場合、視野角は120°
    [SerializeField] private LayerMask npcLayer; // NPC用のレイヤーを指定
    [SerializeField] private float searchRadius = 3f;

    private void Awake()
    {
        if (headAim == null)
            headAim = GetComponentInChildren<MultiAimConstraint>();

    }

    private void Start()
    {
        headAim.weight = 0f;
    }

    private void Update()
    {
        FindBestTarget();

        if (currentTarget != null)
        {
            Debug.Log("感知");
            LookAtTarget(currentTarget.position);
        }
        else
        {
            Debug.Log("感知外");
            headAim.weight = Mathf.Lerp(headAim.weight, 0f, Time.deltaTime * 5f);
        }

    }

    private void FindBestTarget()
    {
        // 指定範囲内、NPCレイヤーを持つコライダーを取得
        Collider[] closeTargets = Physics.OverlapSphere(transform.position, searchRadius, npcLayer);

        float minAngle = baseAngle; // 設定した視野角より狭い範囲で探す
        Transform detectedTarget = null;

        foreach (var col in closeTargets)
        {
            Vector3 toTarget = (col.transform.position - transform.position).normalized; // B - A で方向ベクトルだけ取得
            float dot = Vector3.Dot(transform.forward, toTarget); // 内積。単位ベクトル同士なのでcosθが得られる
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg; // cosθを°に変換

            if (angle < minAngle) // 視野角内に収まっているか
            {
                // TODO: 遮蔽物があるならチェックする
                detectedTarget = col.transform;
                minAngle = angle; // より正面に近い人を優先
            }
        }
        currentTarget = detectedTarget;
    }


    private void LookAtTarget(Vector3 targetPosition)
    {
        headAim.weight = Mathf.Lerp(headAim.weight, 1f, Time.deltaTime * 5f);
        // ターゲットの位置を更新
        var source = headAim.data.sourceObjects.GetTransform(0);
        source.position = targetPosition;
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;

        // 索敵範囲
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, searchRadius);

        // 視野範囲
        Gizmos.color = Color.red;
        Vector3 forward = transform.forward * searchRadius;
        Gizmos.DrawRay(origin, forward); // 正面

        // 視野の左端と右端の線を計算して描画
        // baseAngle分、左右に回転させたベクトルを作る
        Quaternion leftRayRotation = Quaternion.AngleAxis(-baseAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(baseAngle, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(origin, leftRayDirection * searchRadius);
        Gizmos.DrawRay(origin, rightRayDirection * searchRadius);
    }


}
