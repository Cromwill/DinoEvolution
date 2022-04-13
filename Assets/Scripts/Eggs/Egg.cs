using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Egg : MonoBehaviour
{
    [SerializeField] private MeshRenderer _dirtyEgg;
    [SerializeField] private MeshRenderer _cleanEgg;
    [Header("Character")]
    [SerializeField] private float _health;
    [SerializeField] private float _damage;

    private SphereCollider _sphereCollider;

    public Player Player { get; private set; }
    public PlayerHand PlayerHand { get; private set; }
    public bool HasInStack { get; private set; } = false;
    public bool WasWashed { get; private set; } = false;
    public bool WasAddNest { get; private set; } = false;
    public bool WasLightsHeated { get; private set; } = false;
    public float Health => _health;
    public float Damage => _damage;

    public event Action Triggered;
    public event Action<Nest> NestsReached;
    public event Action<Transform, float, float> Taked;

    public void ModifyDamage(float damage)
    {
        Animate();
        _damage = GetValue(damage, _damage);
    }

    public void ModifyHealth(float health)
    {
        Animate();
        _health = GetValue(health, _health);
    }

    public void TakeDamage(float damage)
    {
        Animate();

        if(_health < damage)
            Die();
        else
            _health -= damage;

        if(_health <= 0)
            Die();
    }

    public void OnTaked(Player player, PlayerHand playerHand, Transform followPosition, float step, float time)
    {
        HasInStack = true;
        Player = player;
        PlayerHand = playerHand;
        Taked?.Invoke(followPosition, step, time);
    }

    public void Animate()
    {
        Triggered?.Invoke();
    }

    public void MoveToNest(Nest nest)
    {
        Animate();
        WasAddNest = true;
        NestsReached?.Invoke(nest);
        _sphereCollider.enabled = false;
    }

    private void OnEnable()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private float GetValue(float value, float target)
    {
        target += value;
        return target;
    }

    private void Heat()
    {
        WasLightsHeated = true;
        Animate();
    }

    private void Clean()
    {
        _cleanEgg.enabled = true;
        _dirtyEgg.enabled = false;
        WasWashed = true;
        Animate();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Washer washer))
            Clean();

        if(other.TryGetComponent(out UVLamp uFLamp))
            Heat();

        if (other.TryGetComponent(out Nest nest))
            MoveToNest(nest);
    }
}
