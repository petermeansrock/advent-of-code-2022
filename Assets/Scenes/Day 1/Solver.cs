using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Solver : MonoBehaviour {
    [SerializeField]
    private TextAsset input;
    [SerializeField]
    private GameObject[] elfPrefabs;
    [SerializeField]
    private GameObject[] foodPrefabs;
    [SerializeField]
    private TextMeshProUGUI calorieCounter;

    private int maxCalories = 0;

    void Start() {
        // Parse input file and build list of elves
        List<Elf> elves = new();
        var caloriesByElf = Regex.Split(input.text, "\n\n")
            .Select(s => Regex.Split(s, "\n"));
        foreach (string[] caloriesForElf in caloriesByElf) {
            // Instantiate new elf and add to list
            var elf = new Elf();
            elves.Add(elf);

            // Parse elf-specific input and update Elf object
            foreach (string caloriesString in caloriesForElf) {
                var isValid = int.TryParse(caloriesString, out int calories);
                if (isValid) {
                    elf.AddCalories(calories);
                }
            }
        }

        // Start spawning elves
        StartCoroutine(SpawnElves(elves));
    }

    private IEnumerator SpawnElves(List<Elf> elves) {
        foreach (Elf elf in elves) {
            // Pause between spawns
            yield return new WaitForSeconds(3);

            // Spawn elf at random location
            var x = -18.0f;
            var y = 0.0f;
            var z = Random.Range(-2.8f, 3.22f);
            var elfIndex = Random.Range(0, elfPrefabs.Length);
            var prefab = elfPrefabs[elfIndex];
            var newElf = Instantiate(prefab, new(x, y, z), prefab.transform.rotation);

            // Start food coroutine for elf
            StartCoroutine(SpawnFood(elf, newElf));
        }
    }

    private IEnumerator SpawnFood(Elf elf, GameObject gameObject) {
        var totalCalorieCount = 0;

        foreach (int calories in elf.CalorieList) {
            // Pause between spawns
            yield return new WaitForSeconds(1.5f);

            // Spawn food in the air above the elf's barrel
            var elfPosition = gameObject.transform.position;
            var x = elfPosition.x + 7.5f;
            var y = 33.0f;
            var z = elfPosition.z;
            var foodIndex = Random.Range(0, foodPrefabs.Length);
            var prefab = foodPrefabs[foodIndex];
            Instantiate(prefab, new(x, y, z), prefab.transform.rotation);

            // Update calorie count for this elf
            totalCalorieCount += calories;
            if (totalCalorieCount > maxCalories) {
                maxCalories = totalCalorieCount;
                calorieCounter.text = $"Max Calories: {maxCalories}";
            }
        }

        yield return new WaitForSeconds(20.0f);
        Destroy(gameObject);
    }

    private class Elf {
        private readonly List<int> calorieList = new();
        public ReadOnlyCollection<int> CalorieList {
            get { return calorieList.AsReadOnly(); }
        }

        public void AddCalories(int calories) {
            calorieList.Add(calories);
        }
    }
}
