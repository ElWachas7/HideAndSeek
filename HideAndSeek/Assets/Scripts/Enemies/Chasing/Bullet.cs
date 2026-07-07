using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 direction;
    float speed;
    float lifetime;
    public void Initialize(Vector3 dir, float speed, float life) 
    {
        this.direction = dir;
        this.speed = speed;
        this.lifetime = life;
    }
    public void Start()
    {
        Destroy(gameObject, lifetime);
    }
    void Update()
    {
        transform.LookAt(direction);
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
