using UnityEngine;
using Supernova.Unity;
using System.Collections;

public class CoinRotate : MonoBehaviour
{
    public float turnSpeed = 120.0f;
    private void Update()
    {
        transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
    }
}
