using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.activeInHierarchy) // bugfix: without this, NotifyEnemiesClear() gets called twice (since same function is in Enemy-class as well when emeies get destroyed). When called twuice, strange things occur in scene transition.
        {
            // Destroy gameobject
            collision.gameObject.SetActive(false);
            Destroy(collision.gameObject);

            // Notify if enemies clear
            if (collision.gameObject.GetComponent<Enemy>())
            {
                if (FindObjectsOfType<Enemy>().Length == 0)
                    FindObjectOfType<EnemySpawner>().NotifyEnemiesClear();
            }
        }
    }
}
