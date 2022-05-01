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
        Invoke("Think", 1); // 초기화 함수 안에 넣어서 실행될 때 마다(최초 1회) nextMove변수가 초기화 되도록함 
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
    {//몬스터가 스스로 생각해서 판단 (-1:왼쪽이동 ,1:오른쪽 이동 ,0:멈춤  으로 3가지 행동을 판단)

        //Random.Range : 최소<= 난수 <최대 /범위의 랜덤 수를 생성(최대는 제외이므로 주의해야함)
        nextMoveX = Random.Range(-1, 2);
        nextMoveY = Random.Range(-1, 2);

        //Think(); : 재귀함수 : 딜레이를 쓰지 않으면 CPU과부화 되므로 재귀함수쓸 때는 항상 주의 ->Think()를 직접 호출하는 대신 Invoke()사용
        Invoke("Think", 5); //매개변수로 받은 함수를 5초의 딜레이를 부여하여 재실행 
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
