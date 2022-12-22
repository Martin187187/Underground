using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public float speed = 0.1f;
    public Transform parent;

	void Update () {
        transform.Translate(Vector3.back * speed);
	}
}
