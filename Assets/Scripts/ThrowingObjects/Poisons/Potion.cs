using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] private GameObject effect;


    private AudioSource drinkAudio;
    private WaitForSeconds waitSound;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Item item;

    protected ThrowingObjectController throwObjectScript;

    protected bool isDrunk;

    private void Awake()
    {
        item = GetComponent<Item>();
        throwObjectScript = GetComponent<ThrowingObjectController>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        drinkAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        waitSound = new WaitForSeconds(1);
    }

    public Collider2D[] FindAllobjects()
    {
        return Physics2D.OverlapCircleAll(transform.position, 2);
    }

    public virtual void DoEffectWithBody(Collider2D body)
    {
        if (effect == null)
        {
            return;
        }
        Instantiate(effect, transform.position, Quaternion.identity);
        effect = null;
    }

    [PunRPC]
    public virtual void DoWhenUseMotion()
    {
        item.Used();
        rigidBody.isKinematic = true;
        gameObject.SetActive(true);
        StartCoroutine(DrunkEffect());
    }

    public virtual void DoWhenUseMotion(PlayerContoller player)
    {
    }

    public bool IsDrunk()
    {
        if (isDrunk)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator DrunkEffect()
    {
        drinkAudio.Play();
        yield return waitSound;
        gameObject.SetActive(false);
        rigidBody.isKinematic = false;
    }
}