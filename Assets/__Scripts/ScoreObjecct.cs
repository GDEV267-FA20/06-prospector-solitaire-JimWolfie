using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreObjecct : ScriptableObject
{
    private int _score;

    public int score
    {
        get {
            return (_score);
        }
        set {
            _score = value+1;
            
            
        }
    }
}
