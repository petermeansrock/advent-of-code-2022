using System;
using System.Collections.Generic;
using UnityEngine;
using static Contestant;

public class Contestant : MonoBehaviour {
    [SerializeField]
    private GameObject rock;
    [SerializeField]
    private GameObject paper;
    [SerializeField]
    private GameObject scissors;

    private Animator animator;

    private Dictionary<DrawChoice, GameObject> itemsByChoice = new();
    private DrawChoice nextDraw;

    void Awake() {
        animator = GetComponent<Animator>();
        itemsByChoice.Add(DrawChoice.Rock, rock);
        itemsByChoice.Add(DrawChoice.Paper, paper);
        itemsByChoice.Add(DrawChoice.Scissors, scissors);
    }

    public void Draw(DrawChoice choice) {
        nextDraw = choice;
        animator.SetTrigger("Draw");
    }

    public void Reset() {
        animator.SetTrigger("Reset");
        itemsByChoice[nextDraw].SetActive(false);
    }

    public void OnDrawItem() {
        itemsByChoice[nextDraw].SetActive(true);
    }

    public enum DrawChoice {
        Rock,
        Paper,
        Scissors,
        A = Rock,
        B = Paper,
        C = Scissors,
        X = Rock,
        Y = Paper,
        Z = Scissors,
    }

    public enum RoundResult {
        Win,
        Lose,
        Draw,
        X = Lose,
        Y = Draw,
        Z = Win,
    }
}

public static class DrawChoiceExtensions {
    public static int Score(this DrawChoice choice) {
        return choice switch {
            DrawChoice.Rock => 1,
            DrawChoice.Paper => 2,
            DrawChoice.Scissors => 3,
            _ => throw new InvalidOperationException($"{choice} has no associated score"),
        };
    }

    public static RoundResult Against(this DrawChoice playerChoice, DrawChoice otherChoice) {
        if (playerChoice == otherChoice) {
            return RoundResult.Draw;
        } else if ((playerChoice == DrawChoice.Rock && otherChoice == DrawChoice.Scissors) ||
            (playerChoice == DrawChoice.Paper && otherChoice == DrawChoice.Rock) ||
            (playerChoice == DrawChoice.Scissors && otherChoice == DrawChoice.Paper)) {
            return RoundResult.Win;
        } else {
            return RoundResult.Lose;
        }
    }
}

public static class RoundResultExtensions {
    public static int Score(this RoundResult result) {
        return result switch {
            RoundResult.Win => 6,
            RoundResult.Lose => 0,
            RoundResult.Draw => 3,
            _ => throw new InvalidOperationException($"{result} has no associated score"),
        };
    }

    public static DrawChoice From(this RoundResult result, DrawChoice opponent) {
        if (result == RoundResult.Draw) {
            return opponent;
        }

        switch (opponent) {
            case DrawChoice.Rock:
                if (result == RoundResult.Win) {
                    return DrawChoice.Paper;
                } else {
                    return DrawChoice.Scissors;
                }
            case DrawChoice.Paper:
                if (result == RoundResult.Win) {
                    return DrawChoice.Scissors;
                } else {
                    return DrawChoice.Rock;
                }
            case DrawChoice.Scissors:
                if (result == RoundResult.Win) {
                    return DrawChoice.Rock;
                } else {
                    return DrawChoice.Paper;
                }
            default:
                throw new InvalidOperationException($"No valid choice to play against opponent");
        }
    }
}
