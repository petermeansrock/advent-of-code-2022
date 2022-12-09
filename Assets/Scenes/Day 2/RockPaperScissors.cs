using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class RockPaperScissors : MonoBehaviour {
    [SerializeField]
    private TextAsset input;
    [SerializeField]
    private Contestant player;
    [SerializeField]
    private Contestant elf;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI roundText;
    [SerializeField]
    private TextMeshProUGUI winnerText;
    [SerializeField]
    private InputMode inputMode;

    void Start() {
        // Gather lines of input
        var lines = input.text
            .Split('\n')
            .Where(s => !string.IsNullOrEmpty(s));

        // Speed up game
        Time.timeScale = 100.0f;

        // Parse lines based on input mode
        if (inputMode == InputMode.TwoChoices) {
            var rounds = lines
                .Select(s => new TwoChoiceRound(s))
                .ToArray();
            StartCoroutine(PlayTwoChoiceRounds(rounds));
        } else {
            var rounds = lines
                .Select(s => new OpponentAndOutcomeRound(s))
                .ToArray();
            StartCoroutine(PlayOpponentAndOutcomeRounds(rounds));
        }
    }

    private IEnumerator PlayTwoChoiceRounds(TwoChoiceRound[] rounds) {
        yield return new WaitForSeconds(1.0f);

        int totalScore = 0;
        int roundNumber = 0;
        foreach (var round in rounds) {
            // Display round information
            roundText.text = $"Round #{++roundNumber}";

            // Start animations & wait an estimated amount of time for them to complete
            player.Draw(round.PlayerChoice);
            elf.Draw(round.ElfChoice);
            yield return new WaitForSeconds(4.0f);

            // Display winner
            var result = round.PlayerChoice.Against(round.ElfChoice);
            switch (result) {
                case Contestant.RoundResult.Win:
                    winnerText.text = "You win!";
                    break;
                case Contestant.RoundResult.Lose:
                    winnerText.text = "You lose!";
                    break;
                case Contestant.RoundResult.Draw:
                    winnerText.text = "Draw!";
                    break;
            }
            winnerText.enabled = true;

            // Update scores
            totalScore += round.PlayerChoice.Score() + result.Score();
            scoreText.text = $"Score: {totalScore}";

            // Pause before reset
            yield return new WaitForSeconds(2.0f);

            // Reset both contestants
            player.Reset();
            elf.Reset();

            // Pause briefly before next round
            yield return new WaitForSeconds(2.0f);
            winnerText.enabled = false;
        }

        roundText.text = "Game Over!";
        Time.timeScale = 1.0f;
    }

    private IEnumerator PlayOpponentAndOutcomeRounds(OpponentAndOutcomeRound[] rounds) {
        yield return new WaitForSeconds(1.0f);

        int totalScore = 0;
        int roundNumber = 0;
        foreach (var round in rounds) {
            // Display round information
            roundText.text = $"Round #{++roundNumber}";

            // Start animations & wait an estimated amount of time for them to complete
            var playerChoice = round.Outcome.From(round.ElfChoice);
            player.Draw(playerChoice);
            elf.Draw(round.ElfChoice);
            yield return new WaitForSeconds(4.0f);

            // Display winner
            switch (round.Outcome) {
                case Contestant.RoundResult.Win:
                    winnerText.text = "You win!";
                    break;
                case Contestant.RoundResult.Lose:
                    winnerText.text = "You lose!";
                    break;
                case Contestant.RoundResult.Draw:
                    winnerText.text = "Draw!";
                    break;
            }
            winnerText.enabled = true;

            // Update scores
            totalScore += playerChoice.Score() + round.Outcome.Score();
            scoreText.text = $"Score: {totalScore}";

            // Pause before reset
            yield return new WaitForSeconds(2.0f);

            // Reset both contestants
            player.Reset();
            elf.Reset();

            // Pause briefly before next round
            yield return new WaitForSeconds(2.0f);
            winnerText.enabled = false;
        }

        roundText.text = "Game Over!";
        Time.timeScale = 1.0f;
    }

    private class TwoChoiceRound {
        public Contestant.DrawChoice PlayerChoice { get; }
        public Contestant.DrawChoice ElfChoice { get; }

        public TwoChoiceRound(string description) {
            var choices = description.Split(' ')
                .Select(s => System.Enum.Parse<Contestant.DrawChoice>(s))
                .ToArray();
            ElfChoice = choices[0];
            PlayerChoice = choices[1];
        }
    }

    private class OpponentAndOutcomeRound {
        public Contestant.DrawChoice ElfChoice { get; }
        public Contestant.RoundResult Outcome { get; }

        public OpponentAndOutcomeRound(string description) {
            var choices = description.Split(' ');
            ElfChoice = System.Enum.Parse<Contestant.DrawChoice>(choices[0]);
            Outcome = System.Enum.Parse<Contestant.RoundResult>(choices[1]);
        }
    }

    public enum InputMode {
        TwoChoices,
        OpponentWithOutcome
    }
}
