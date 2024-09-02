using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextStage : MonoBehaviour
{
    public Image GetImage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHealthSystem healthSystem) && collision.CompareTag("Player"))
        {
            GetImage.enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GetImage.enabled)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                MapManager.Instance.Next();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHealthSystem healthSystem) && collision.CompareTag("Player"))
        {
            GetImage.enabled = false;
        }
    }
}
