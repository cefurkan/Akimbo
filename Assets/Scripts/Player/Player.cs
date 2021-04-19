using System;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    public static Player Instance;
    protected PlayerAimer aimer;

    public static int level;
    public static int currentExp;
    public static int[] nextLevelExp;
    private int maxLevel;

    public delegate void OnLevelUp();
    public static event OnLevelUp onLevelUp;

    [SerializeField] long coins = 0;

    [SerializeField] UnityEvent onPlayerDeath;

    InputDataSO input;

    [SerializeField] Animator anim;
    [SerializeField] float moveMagnitude;


    protected new void Awake()
    {
        base.Awake();


        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (input == null)
            input = Resources.Load<InputDataSO>("Input");

        if (aimer == null)
            aimer = GetComponentInChildren<PlayerAimer>();
    }
    private void Start()
    {
        level = 1;
        maxLevel = 10;
        nextLevelExp = new int[maxLevel + 1];
        nextLevelExp[1] = 1000;

        for (int i = 2; i < maxLevel; i++)
        {
            nextLevelExp[i] = Mathf.RoundToInt(nextLevelExp[i - 1] * 1.1f);
        }
    }
    protected void FixedUpdate()
    {
        if (aimer.Target == null)
        {
            aimer.Aim();
        }
        else if (!aimer.IsVisible())
        {
            aimer.ResetTarget();
        }
        else if (aimer.DistanceToTarget(aimer.Target) > aimer.DistanceToTarget(aimer.ClosestTarget().transform))
        {
            aimer.Target = aimer.ClosestTarget();
        }
    }

    protected void Update()
    {
        CheckMovementState(input.value);

        moveMagnitude = input.value.magnitude;
        anim.SetFloat("MoveSpeed", moveMagnitude);

    }

    public static float GetExperienceNormalized()
    {
        return (float)currentExp / nextLevelExp[level];
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        if (currentExp >= nextLevelExp[level] && level < maxLevel)
        {
            LevelUp();
            onLevelUp?.Invoke();

        }

        if (level >= maxLevel)
        {
            currentExp = 0;
        }
    }

    private void LevelUp()
    {
        currentExp -= nextLevelExp[level];
        level++;

        maxHp = Mathf.RoundToInt(maxHp * 1.2f);
        damage = Mathf.CeilToInt(damage * 1.1f);
    }

    private void CheckMovementState(Vector2 direction)
    {
        if (walkingState == MovingState.Staying && direction != Vector2.zero)
            walkingState = MovingState.Moving;
        else
        {
            if (walkingState == MovingState.Moving && direction == Vector2.zero)
                walkingState = MovingState.Staying;
        }
    }

    protected override void Death(Entity killer)
    {
        Debug.Log("Player Dead");
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }
}
