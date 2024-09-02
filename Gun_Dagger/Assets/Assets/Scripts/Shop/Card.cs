using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject CardShop;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKey(KeyCode.Z))
            CardShop.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHealthSystem healthSystem) && collision.CompareTag("Player"))
            CardShop.SetActive(false);
    }
}
