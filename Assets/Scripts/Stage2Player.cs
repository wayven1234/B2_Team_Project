using System;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Stage2Player : MonoBehaviour
{
    public float moveSpeed;

    void Update()
    {
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }

        Vector3 movement = new Vector3(0f, verticalInput, 0f);
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
