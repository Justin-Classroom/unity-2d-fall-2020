﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 2;
    [Range(1, 100)]
    public int maxHealth = 5;
    public int currentHealth;
    public float damageCooldown = 3.0f;
    public GameObject projectilePrefab;
    public UIHealthBar uiHealthBar;

    private Animator _animator;
    private Rigidbody2D _rb2d;
    private float _horizontal;
    private float _vertical;
    private float _timer;
    private bool isInvincible;

    private Vector2 lookDirection = new Vector2(1, 0);

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Control();
        Invincible();
        Fire();
        Interact();
    }

    void FixedUpdate()
    {
        Vector2 position = _rb2d.position;
        position.x = position.x + speed * _horizontal * Time.deltaTime;
        position.y = position.y + speed * _vertical * Time.deltaTime;

        _rb2d.MovePosition(position);
    }

    /// <summary>
    /// Handles input from player to control the character
    /// </summary>
    private void Control()
    {
        // get input
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(_horizontal, _vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        _animator.SetFloat("Look X", lookDirection.x);
        _animator.SetFloat("Look Y", lookDirection.y);
        _animator.SetFloat("Speed", move.magnitude);
    }

    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Launch();
        }
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(_rb2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));

            if (hit.collider != null)
            {
                Debug.Log("Raycast has hit the object: " + hit.collider.gameObject);

                NPC character = hit.collider.GetComponent<NPC>();
                character.DisplayDialog();
            }
        }
    }

    private void Invincible()
    {
        if (isInvincible == true)
        {
            _timer = _timer - Time.deltaTime;
            if (_timer < 0)
            {
                isInvincible = false;
            }
        }
    }

    /// <summary>
    /// Changes current health by adding amount entered
    /// </summary>
    /// <param name="amount"> amount to add to current health</param>

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible == true)
            {
                return;
            }
            isInvincible = true;
            _timer = damageCooldown;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        uiHealthBar.SetValue((float)currentHealth / (float)maxHealth);
    }

    private void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, _rb2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
    }
}
