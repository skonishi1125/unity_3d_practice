using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadAimController : MonoBehaviour
{
    private Player player;
    [SerializeField] private Rig rig;
    [SerializeField] private MultiAimConstraint headAim;
    [SerializeField] private GameObject target;

    private void Awake()
    {
        player = GetComponentInParent<Player>();

        if (rig == null)
            rig = GetComponentInChildren<Rig>();

        if (headAim == null)
            headAim = GetComponentInChildren<MultiAimConstraint>();

        if (target == null)
            Debug.LogWarning("Target is not assigned.");
    }

    private void Start()
    {
        headAim.weight = 0f;
    }

    private void Update()
    {

    }


}
