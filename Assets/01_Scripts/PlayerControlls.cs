using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlls : MonoBehaviour
{
    private new Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    private void PlayerInput()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit.TryGetComponent(out Tile target) && Input.GetKey(KeyCode.Mouse0))
            {
                target.HighLight();
            }
        }
    }

    void Update()
    {
        PlayerInput();
    }
}
