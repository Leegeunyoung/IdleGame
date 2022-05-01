using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skills : MonoBehaviour
{
    int damage;
    Player player;
    public GameObject[] coolTimeBgs;
    public Image[] coolTimeImg;
    public Text[] coolTimeText;
    public bool[] isCoolTime;
    public float[] coolTime;
    private float[] currentCoolTime;

    private void Start()
    {
        player = GetComponent<Player>();
        damage = player.damage;
        isCoolTime = new bool[] { false, false, false, false };
        coolTime = new float[] { 0, 0, 0, 0 };
        currentCoolTime = new float[] { 0, 0, 0, 0 };
    }

    public IEnumerator normalAttack(float range)
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(this.transform.position, range);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Monster")
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                enemy.TakeDamage(damage * 1);
                GameObject go = Instantiate(Resources.Load("hitEffect")) as GameObject;
                go.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 0.7f, enemy.transform.position.z - 3);

                enemy.dieAction = () =>
                {
                    player.FoundObjects.Clear();
                    player.Invoke("Searching", 1f);
                };
            }
        }
        CoolTimeFuc(1.5f, 0);
        yield return null;
    }

    public IEnumerator powerfulAttack(float range)
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(this.transform.position, range);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Monster")
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                enemy.TakeDamage(damage * 2);
                GameObject go = Instantiate(Resources.Load("powerfulEffect")) as GameObject;
                go.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 0.7f, enemy.transform.position.z - 3);

                enemy.dieAction = () =>
                {
                    player.FoundObjects.Clear();
                    player.Invoke("Searching", 1f);
                };
            }
        }
        CoolTimeFuc(10f, 1);
        yield return null;
    }

    void CoolTimeFuc(float time, int num)
    {
        coolTime[num] = time;
        coolTimeImg[num].fillAmount = 1;
        coolTimeText[num].gameObject.SetActive(true);
        coolTimeBgs[num].SetActive(true);
        coolTimeText[num].text = currentCoolTime[num].ToString("0.0");
        currentCoolTime[num] = coolTime[num];
        StartCoroutine(CoolTime(num));
        StartCoroutine(CoolTimeCounter(num));
    }

    IEnumerator CoolTime(int num)
    {
        while (coolTimeImg[num].fillAmount > 0)
        {
            coolTimeImg[num].fillAmount -= 1 * Time.deltaTime / coolTime[num];
            yield return null;
        }
        yield break;
    }

    IEnumerator CoolTimeCounter(int num)
    {
        while (currentCoolTime[num] > 0)
        {
            currentCoolTime[num] -= Time.deltaTime;
            coolTimeText[num].text = currentCoolTime[num].ToString("0.0");
            yield return null;
        }
        isCoolTime[num] = false;
        coolTimeBgs[num].SetActive(false);
        coolTimeText[num].gameObject.SetActive(false);
        yield break;
    }
}
