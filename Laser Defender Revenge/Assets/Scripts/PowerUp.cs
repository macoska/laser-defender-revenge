using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpID
{
    FasterFiring,
    StrongLaser,
    Rocket,
    UltraBeam,
    HelperShip,
    EMP,
    Health
};

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpID powerUp = default;
    [SerializeField] bool moveX = true, moveY = true;
    [SerializeField] [Range(0f,5f)] float moveSpeedX = 0.5f;
    [SerializeField] [Range(0f,5f)] float moveSpeedY = 1f;

    private void Start()
    {
        if(moveX || moveY)
            Move();
    }

    private void Move()
    {
        float speedX = moveX ? moveSpeedX : 0f;
        float speedY = moveY ? moveSpeedY : 0f;
        Vector2 velocity = new Vector2(speedX, -speedY);
        float posX = Camera.main.WorldToViewportPoint(transform.position).x;
        if (posX > 0.5f)
        {
            velocity.x *= -1;
        }
        GetComponent<Rigidbody2D>().velocity = velocity;
    }
    
    public PowerUpID GetPowerUp() => powerUp;

    public void SetMove(bool isMovableX, bool isMovableY)
    {
        SetMoveX(isMovableX);
        SetMoveY(isMovableY);
    }
    public void SetMoveX(bool isMovable) => moveX = isMovable;
    public void SetMoveY(bool isMovable) => moveY = isMovable;

    public void InvertY()
    {
        moveSpeedY *= -1;
        Start();
    }
}