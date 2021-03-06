﻿using UnityEngine;
using System.Collections;

public class ScoreManager : Singleton<ScoreManager>
{
    protected ScoreManager() { }

    public int Score = 0;
    public int Lives = 3;
    public int NextLife = 3500;
    public int WalkSpeed = 2;
    public int MaxWalkSpeed = 4;
    public int HitPoints = 3;
    public int MaxHitPoints = 5;
    public int Level = 1;
    public int Bombs = 0;

    public void Reset()
    {
        Continue();
        Level = 1;
    }

    public void Continue()
    {
        Score = 0;
        Lives = 3;
        NextLife = 3500;
        HitPoints = 3;
        MaxHitPoints = 5;
        WalkSpeed = 2;
        MaxWalkSpeed = 4;
        Bombs = 0;
    }
}
