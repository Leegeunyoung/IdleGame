using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public int maxHp;
    public int hp;
    public int damage;
    public UnityAction dieAction;
    private Animator anim;
    private GameObject target;
    public GameObject hpBar;

    Rigidbody2D rigid;
    public float nextMoveX;
    public float nextMoveY;
    bool isTarget;
    bool isDie;
    float delay;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        Invoke("Think", 1); // �ʱ�ȭ �Լ� �ȿ� �־ ����� �� ����(���� 1ȸ) nextMove������ �ʱ�ȭ �ǵ����� 
    }

    private void Start()
    {
        delay = 0;
        damage = 2;
        maxHp = 50;
        hp = maxHp;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        delay += Time.deltaTime;
        if(delay > 5f)
        {
            StartCoroutine(Attack());
        }
    }

    void Think()
    {//���Ͱ� ������ �����ؼ� �Ǵ� (-1:�����̵� ,1:������ �̵� ,0:����  ���� 3���� �ൿ�� �Ǵ�)

        //Random.Range : �ּ�<= ���� <�ִ� /������ ���� ���� ����(�ִ�� �����̹Ƿ� �����ؾ���)
        nextMoveX = Random.Range(-1, 2);
        nextMoveY = Random.Range(-1, 2);

        //Think(); : ����Լ� : �����̸� ���� ������ CPU����ȭ �ǹǷ� ����Լ��� ���� �׻� ���� ->Think()�� ���� ȣ���ϴ� ��� Invoke()���
        Invoke("Think", 5); //�Ű������� ���� �Լ��� 5���� �����̸� �ο��Ͽ� ����� 
    }

    public void TakeDamage(int damage)
    {
        float x = 0.17f / ((float)maxHp / damage);
        hp = hp - damage;
        anim.SetTrigger("Take Damage");

        if (hp <= 0)
        {
            StartCoroutine(Die());
            hpBar.transform.localScale = new Vector3(0,0,0);
        }
        else
        {
            hpBar.transform.localScale = new Vector3(hpBar.transform.localScale.x - x, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
        }
    }

    IEnumerator Die()
    {
        isDie = true;
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
        dieAction();
    }

    public void Move()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(this.transform.position, 3f);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Player")
            {
                isTarget = true;
                target = collider.gameObject;
                if (Vector2.Distance(transform.position, target.transform.position) > 1.3f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, 0.015f);
                    transform.position = new Vector3(transform.position.x, transform.position.y, -3);
                }
                else
                {
                    rigid.velocity = Vector2.zero;
                }
            }
            else
            {
                isTarget = false;
            }
        }

        if(isTarget == false)
        {
            rigid.velocity = new Vector2(nextMoveX / 2, nextMoveY / 2);
        }
    }

    IEnumerator Attack()
    {
        delay = 0;
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(this.transform.position, 0.5f);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Player")
            {
                anim.SetTrigger("Attack Soundwave");
                collider.GetComponent<Player>().hp -= damage;
                //Debug.Log(collider.GetComponent<Player>().hp);
            }
        }
        yield return null;
    }
}
