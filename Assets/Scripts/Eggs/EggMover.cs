using UnityEngine;

[RequireComponent(typeof(Egg))]
[RequireComponent(typeof(EggAnimator))]
[RequireComponent(typeof(SphereCollider))]
public class EggMover : MonoBehaviour
{
    private SphereCollider _collider;
    private EggAnimator _eggAnimator;
    private EggMover _previousEgg;
    private EggMover _nextEgg;

    private float _power;
    private float _step;
    private bool _hasStack = false;

    private const float SPEED = 20f;
    private const float DELAY = 0.05f;

    public Egg Egg { get; private set; }
    public PlayerHand PlayerHand { get; private set; }

    public void Disable(Transform parent)
    {
        if (_nextEgg != null && _previousEgg != null)
        {
            _previousEgg.SetNextEgg(_nextEgg);
            _nextEgg.SetPreviousEgg(_previousEgg);
        }

        if(_nextEgg != null && _previousEgg == null)
        {
            _nextEgg.SetPreviousEgg(null);
            _nextEgg.transform.parent = PlayerHand.EggStackPosition;
            _nextEgg.transform.position = PlayerHand.EggStackPosition.position;
        }

        if(_nextEgg == null && _previousEgg != null)
            _previousEgg.SetNextEgg(null);

        _collider.enabled = false;
        transform.parent = parent;

        _nextEgg = null;
        _previousEgg = null;

        enabled = false;
    }

    public void SetNextEgg(EggMover egg)
    {
        _nextEgg = egg;
    }

    public void SetPreviousEgg(EggMover previousEgg)
    {
        _previousEgg = previousEgg;
    }

    public void OnTakedHand(PlayerHand playerHand)
    {
        PlayerHand = playerHand;
    }

    public void OnTaked(PlayerHand playerHand, EggMover followEgg, float step, float power)
    {
        transform.parent = null;
        PlayerHand = playerHand;
        _step = step;
        _power = power;
        _hasStack = Egg.HasInStack;
        SetPreviousEgg(followEgg);
        Animate();
    }

    public void Animate()
    {
        _eggAnimator.ScaleAnimation();
        Invoke(nameof(AnimateLeaderEgg), DELAY);
    }

    public void AnimateLeaderEgg()
    {
        if(_previousEgg != null)
            _previousEgg.Animate();
    }

    private void OnEnable()
    {
        Egg = GetComponent<Egg>();
        _collider = GetComponent<SphereCollider>();
        _eggAnimator = GetComponent<EggAnimator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Egg egg))
        {
            if (egg.HasInStack == false && this == PlayerHand.LastInStack)
            {
                SetNextEgg(egg.EggMover);
                egg.OnNextTaked(this, PlayerHand);
                PlayerHand.SetLastEgg(egg.EggMover);
            }
            else if (egg.HasInStack == false && this != PlayerHand.LastInStack)
            {
                PlayerHand.LastInStack.SetNextEgg(egg.EggMover);
                egg.OnNextTaked(PlayerHand.LastInStack, PlayerHand);
                PlayerHand.SetLastEgg(egg.EggMover);
            }
        }
    }

    private void LateUpdate()
    {
        if (_hasStack == false || _previousEgg == null)
            return;

        Vector3 position;
        Vector3 targetPosition = new Vector3(_previousEgg.transform.position.x, _previousEgg.transform.position.y, _previousEgg.transform.position.z + _step);

        position = Vector3.Lerp(transform.position, targetPosition, _power);
        transform.position = position;
    }
}
