using System.Collections;
using UnityEngine;

public class Poison : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject throwEffect;

    protected Collider2D[] bodiesForEffect;
    private Collider2D currentCollider;
    private Rigidbody2D rigitBody;

    protected bool isBreak;
    private float forceSpeed;

    private void Awake()
    {
        currentCollider = GetComponent<Collider2D>();
        rigitBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        forceSpeed = 15;
    }

    public void Throw(Vector3 direction)
    {
        rigitBody.velocity = direction * forceSpeed;
    }

    public void Initialization(bool isBreak)
    {
        this.isBreak = isBreak;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBreak)
        {
            isBreak = true;
            bodiesForEffect = Physics2D.OverlapCircleAll(transform.position, 2);
            Instantiate(throwEffect, transform.position, Quaternion.Euler(0, 0, 0));
            OnBreak(bodiesForEffect);
        }
        if (collision.gameObject.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.TakeDamage(damage);
        }
        StartCoroutine(GetMaxDrag());
        currentCollider.isTrigger = true;
    }

    private IEnumerator GetMaxDrag()
    {
        for (int i = 0; i < 1000; i++)
        {
            i *= 2;
            rigitBody.drag = i;
            yield return 0.1f;
        }
    }

    protected void OnBreak(Collider2D[] bodiesForEffect)
    {
        foreach (var body in bodiesForEffect)
        {
            DoEffectWithBody(body);
        }
    }

    protected virtual void DoEffectWithBody(Collider2D body) { }
}