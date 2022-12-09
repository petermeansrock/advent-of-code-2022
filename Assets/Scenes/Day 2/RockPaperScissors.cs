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

    void Start() {
        // Parse moves for all rounds
        var rounds = input.text
            .Split('\n')
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => new Round(s))
            .ToArray();
        StartCoroutine(PlayRounds(rounds));
    }

    private IEnumerator PlayRounds(Round[] rounds) {
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
    }

    private class Round {
        public Contestant.DrawChoice PlayerChoice { get; }
        public Contestant.DrawChoice ElfChoice { get; }

        public Round(string description) {
            var choices = description.Split(' ')
                .Select(s => System.Enum.Parse<Contestant.DrawChoice>(s))
                .ToArray();
            ElfChoice = choices[0];
            PlayerChoice = choices[1];
        }
    }
}
