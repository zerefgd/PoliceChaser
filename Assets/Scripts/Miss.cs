using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miss : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.ENEMY))
        {
            EventManager.TriggerEvent(Constants.EventNames.MISS, null);
        }
    }
}
