using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public List<Enemy> enemyList;
    public List<GameObject> FoundObjects;
    public GameObject target;
    public float shortDis;
    Animator anim;
    public int damage;
    Skills skills;
    float delay;
    Rigidbody2D rigid;
    public Toggle autoToggle;
    public Button[] skillBtns;
    public FixedJoystick joyStick;
    public int hp;
    public int maxHp;
    public Image hpBarImg;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        skills = GetComponent<Skills>();
        rigid = GetComponent<Rigidbody2D>();
        damage = 10;
        maxHp = 100;
        delay = 0;
    }

    private void Start()
    {
        hp = maxHp;
        Searching();
        ButtonInit();
    }

    private void FixedUpdate()
    {
        if(FoundObjects.Count != 0 && autoToggle.isOn == true)
        {
            if (Vector2.Distance(transform.position, target.transform.position) > 1.5f)
            {
                if (target.transform.position.x - transform.position.x > 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                Vector3 dir = target.transform.position - transform.position;
                rigid.MovePosition(transform.position + dir.normalized * Time.deltaTime * 5f);
                anim.SetBool("isRun", true);
            }
            else
            {
                transform.position = transform.position;
                anim.SetBool("isRun", false);
                delay += Time.deltaTime;
                if (delay > 0.5f)
                {
                    for (int i = 0; i < skills.isCoolTime.Length; i++)
                    {
                        if (skills.isCoolTime[i] == false)
                        {
                            skills.isCoolTime[i] = true;
                            StartCoroutine(AttackFuc(i));
                            break;
                        }
                    }
                }
            }
        }

        //수동조작
        if (autoToggle.isOn == false)
        {
            float x = joyStick.Horizontal;
            float y = joyStick.Vertical;
            Vector3 moveVec = new Vector3(x, y, -3) * 5f * Time.deltaTime;

            rigid.MovePosition(transform.position + moveVec);

            if(x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if(x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            if(joyStick.Horizontal != 0 && joyStick.Vertical != 0)
            {
                anim.SetBool("isRun", true);
            }
            else
            {
                anim.SetBool("isRun", false);
            }
        }
    }

    private void Update()
    {
        hpBarImg.fillAmount = (float)hp / maxHp;
    }

    public void Searching()
    {
        FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Monster"));
        if (FoundObjects.Count != 0)
        {
            shortDis = Vector2.Distance(gameObject.transform.position, FoundObjects[0].transform.position); // 첫번째를 기준으로 잡아주기 
            target = FoundObjects[0]; // 첫번째를 먼저 
        
            foreach (GameObject found in FoundObjects)
            {
                float Distance = Vector2.Distance(gameObject.transform.position, found.transform.position);

                if (Distance < shortDis) // 위에서 잡은 기준으로 거리 재기
                {
                    target = found;
                    shortDis = Distance;
                }
            }
        }
    }

    IEnumerator AttackFuc(int num)
    {
        delay = 0;
        anim.ResetTrigger("isAttack");
        anim.ResetTrigger("isIdle");

        switch (num)
        {
            case 0:
                anim.SetTrigger("isAttack");
                anim.speed = 1f;
                yield return new WaitForSeconds(0.55f);
                anim.SetTrigger("isIdle");
                skills.StartCoroutine("normalAttack", 3f);
                break;
            case 1:
                anim.SetTrigger("isAttack");
                anim.speed = 0.5f;
                yield return new WaitForSeconds(1f);
                anim.SetTrigger("isIdle");
                skills.StartCoroutine("powerfulAttack", 5f);
                break;
        }
    }

    void ButtonInit()
    {
        autoToggle.onValueChanged.AddListener((bool bOn) =>
        {
            anim.Rebind();

            //오토 끄면 조이스틱 활성화
            if(bOn == false)
            {
                joyStick.gameObject.SetActive(true);
            }
            else
            {
                joyStick.gameObject.SetActive(false);
            }
        });

        skillBtns[0].onClick.AddListener(() =>
        {
            if(skills.isCoolTime[0] == false && autoToggle.isOn == false)
            {
                skills.isCoolTime[0] = true;
                StartCoroutine(AttackFuc(0));
            }
        });

        skillBtns[1].onClick.AddListener(() =>
        {
            if (skills.isCoolTime[1] == false && autoToggle.isOn == false)
            {
                skills.isCoolTime[1] = true;
                StartCoroutine(AttackFuc(1));
            }
        });

        skillBtns[2].onClick.AddListener(() =>
        {
            if (skills.isCoolTime[2] == false && autoToggle.isOn == false)
            {
                skills.isCoolTime[2] = true;
                StartCoroutine(AttackFuc(2));
            }
        });

        skillBtns[3].onClick.AddListener(() =>
        {
            if (skills.isCoolTime[3] == false && autoToggle.isOn == false)
            {
                skills.isCoolTime[3] = true;
                StartCoroutine(AttackFuc(3));
            }
        });
    }
}
