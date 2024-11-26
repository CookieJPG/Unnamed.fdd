using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    [SerializeField] private BoxCollider2D atkHitbox;

    private PlayerMovement pm;
    private int direction = 1;

    [Header("Attacks")]
    internal Dictionary<string, Attack> Attacks = new Dictionary<string, Attack>
    {
        { "ground_jab", new Attack(0, 2, 3) },   // 1
        { "ground_fw", new Attack(1, 7, 5, 2) }, // 2
        { "ground_bk", new Attack (1, 5, 7) },   // 3
        { "ground_up", new Attack(10, 1, 6) },   // 4
        { "ground_down", new Attack(12, 0, 7) }, // 5
        { "air_jab", new Attack(0, 2, 3, 3) },   // 6
        { "air_fw", new Attack(2, 6, 4) },       // 7
        { "air_bk", new Attack(1, 8, 10) },      // 8
        { "air_up", new Attack(1, 7, 5) },       // 9
        { "air_down", new Attack(12, 2, 7) }     // 10
    };

    internal string AttackType;
    internal Vector2 inputDir;
    internal Color cur;

    private bool atkStarted = false;
    private bool atkCooldown = false;
    private int jabCounter = 0;
    private float jabTime = 0.2f;


    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        cur = pm.sp.color;
        atkHitbox.enabled = false;
    }

    void Update()
    {
        inputDir = pm.moveInput;
        inputDir.Normalize();

        if (pm.dead)
        {
            return;
        }

        if (pm.UserInput.attackInput && !pm.dummy && !atkCooldown && !atkStarted)
        {
            atkStarted = true;
            direction = pm.isFacingRight ? 1 : -1;
            Attack(HandleInput().ToUpper());
            StartCoroutine(ResetHitbox());
        }
        else if (pm.dummy && Input.GetKeyDown("o"))
        {
            direction = pm.isFacingRight ? 1 : -1;
            Attack(HandleInput().ToLower());
            StartCoroutine(ResetHitbox());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
        {
            StartCoroutine(GetAttacked(collision));
        }
    }

    private IEnumerator GetAttacked(Collider2D collision)
    {
        if (!pm.canDash)
        {
            yield break;
        }

        StartCoroutine(ChangeColorAttacked());

        PlayerMovement incAtkr = collision.GetComponentInParent<PlayerMovement>();
        AttackBehaviour incAtk = collision.GetComponentInParent<AttackBehaviour>();

        if (incAtkr.isFacingRight == pm.isFacingRight)
        {
            Vector3 localScale = pm.transform.localScale;
            pm.isFacingRight = !pm.isFacingRight;
            localScale.x *= -1f;
            pm.transform.localScale = localScale;
        }

        pm.attacked = true;
        yield return new WaitForSeconds(0.01f);
        pm.rb.velocity = Vector3.zero;

        int direction = incAtkr.isFacingRight ? 1 : -1;
        int vertical = incAtkr.moveInput.normalized.y > 0f ? 1 : -1;

        incAtk.Attacks.TryGetValue(incAtk.AttackType, out Attack curAtk);

        Debug.Log($"{incAtk.AttackType}");

        if (curAtk.multi != 0)
        {
            for (int i = 1; i < curAtk.multi; i++)
            {
                pm.percent += Attacks[incAtk.AttackType].damage;
                pm.knockback += Attacks[incAtk.AttackType].damage / 2;
                pm.rb.velocity = Vector2.zero;
                yield return new WaitForSeconds(0.5f);
            }
        }

        pm.rb.velocity =
            new Vector2
            (
                curAtk.horizKb != 0 ? (curAtk.horizKb + pm.knockback) * direction : 0,
                (curAtk.vertKb + pm.knockback) * vertical
            );

        pm.percent += Attacks[incAtk.AttackType].damage;
        pm.knockback += Attacks[incAtk.AttackType].damage / 2;

        yield return new WaitForSeconds(pm.knockback / 10);
    }

    private IEnumerator ChangeColorAttacked()
    {
        pm.sp.color = Color.white;
        yield return new WaitForSeconds(0.3f);
        pm.sp.color = cur;
    }

    private void Attack(string direction, float ofset = 0.5f)
    {
        atkCooldown = true;
        atkHitbox.enabled = true;
        if (pm.IsGrounded())
        {
            switch (direction)
            {
                case "UP":
                    AttackType = "ground_up";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x,
                            atkHitbox.transform.position.y + ofset
                        );
                    atkHitbox.transform.localScale = new Vector2(1, 1.5f);
                    Invoke(nameof(ResetAtkCooldown), 0.7f);
                    break;
                case "DOWN":
                    AttackType = "ground_down";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x,
                            atkHitbox.transform.position.y - ofset
                        );
                    atkHitbox.transform.localScale = new Vector2(1, 1.5f);
                    Invoke(nameof(ResetAtkCooldown), 0.7f);
                    break;
                case "BACKWARD":
                    AttackType = "ground_bk";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x - (ofset * this.direction),
                            atkHitbox.transform.position.y
                        );
                    atkHitbox.transform.localScale = new Vector2(1.5f, 1);
                    Invoke(nameof(ResetAtkCooldown), 0.4f);
                    break;
                case "FORWARD":
                    AttackType = "ground_fw";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x + (ofset * this.direction),
                            atkHitbox.transform.position.y
                        );
                    atkHitbox.transform.localScale = new Vector2(1.5f, 1);
                    Invoke(nameof(ResetAtkCooldown), 0.4f);
                    break;
                default:
                    AttackType = "ground_jab";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x + (ofset * this.direction),
                            atkHitbox.transform.position.y
                        );
                    atkHitbox.transform.localScale = new Vector2(1.5f, 1);
                    Invoke(nameof(ResetAtkCooldown), jabTime);
                    break;
            }
        }
        else
        {
            switch (direction)
            {
                case "UP":
                    AttackType = "air_up";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x,
                            atkHitbox.transform.position.y + ofset
                        );
                    atkHitbox.transform.localScale = new Vector2(1, 1.5f);
                    Invoke(nameof(ResetAtkCooldown), 0.7f);
                    break;
                case "DOWN":
                    AttackType = "air_down";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x,
                            atkHitbox.transform.position.y - ofset
                        );
                    atkHitbox.transform.localScale = new Vector2(1, 1.5f);
                    Invoke(nameof(ResetAtkCooldown), 0.7f);
                    break;
                case "BACKWARD":
                    AttackType = "air_bk";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x - (ofset * this.direction),
                            atkHitbox.transform.position.y
                        );
                    atkHitbox.transform.localScale = new Vector2(1.5f, 1);
                    Invoke(nameof(ResetAtkCooldown), 0.4f);
                    break;
                case "FORWARD":
                    AttackType = "air_fw";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x + (ofset * this.direction),
                            atkHitbox.transform.position.y
                        );
                    atkHitbox.transform.localScale = new Vector2(1.5f, 1);
                    Invoke(nameof(ResetAtkCooldown), 0.4f);
                    break;
                default:
                    AttackType = "air_jab";
                    atkHitbox.transform.position =
                        new Vector2
                        (
                            atkHitbox.transform.position.x + (ofset * this.direction),
                            atkHitbox.transform.position.y
                        );
                    atkHitbox.transform.localScale = new Vector2(1.5f, 1);
                    Invoke(nameof(ResetAtkCooldown), 0.9f);
                    break;
            }
        }

        atkStarted = false;
        Debug.Log($"Current Attack: {AttackType}");
    }

    private void ResetAtkCooldown()
    {
        if (AttackType == "ground_jab")
        {
            jabCounter++;
            if (jabCounter > 2)
            {
                jabCounter = 0;
                jabTime = 1.2f;
            }
            else
            {
                jabTime = 0.2f;
            }
        }
        else
        {
            jabCounter = 0;
        }
        atkCooldown = false;
    }

    private string HandleInput()
    {
        int facing = pm.isFacingRight ? 1 : -1;
        Debug.Log(inputDir);

        if (inputDir.y > 0)
        {
            return "UP";
        }
        else if (inputDir.y < 0)
        {
            return "DOWN";
        }
        if (inputDir.x != 0)
        {
            if (inputDir.x == facing)
            {
                return "FORWARD";
            }
            else if (inputDir.x != facing)
            {
                return "BACKWARD";
            }
            else
            {
                return "NONE";
            }
        }
        else
        {
            return "NONE";
        }
    }

    private IEnumerator ResetHitbox()
    {
        yield return new WaitForSeconds(0.1f);
        atkHitbox.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        atkHitbox.transform.position = pm.transform.position;
        atkHitbox.transform.localScale = Vector3.one;
        atkHitbox.enabled = false;
    }
}
