using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatureCareGame : MonoBehaviour
{
    [SerializeField] private List<CreatureSelector> creatures = new List<CreatureSelector>();
    [SerializeField] private Text titleText;
    [SerializeField] private Text creatureText;
    [SerializeField] private Text statusText;
    [SerializeField] private float energyDrainDifficulty = 2.5f;

    private int selectedIndex;

    private void Start()
    {
        SelectCreature(0);
        UpdateHud("Choose a creature, then care for it.");
    }

    private void Update()
    {
        if (creatures.Count == 0) return;

        foreach (CreatureSelector selector in creatures)
        {
            if (selector != null && selector.Creature != null)
            {
                selector.Creature.Tick(energyDrainDifficulty);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCreature(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCreature(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCreature(2);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SelectCreature(selectedIndex - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) SelectCreature(selectedIndex + 1);
        if (Input.GetKeyDown(KeyCode.Space)) CareForSelectedCreature();
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        UpdateHud();
    }

    public void SelectCreature(int index)
    {
        if (creatures.Count == 0) return;

        selectedIndex = (index % creatures.Count + creatures.Count) % creatures.Count;
        for (int i = 0; i < creatures.Count; i++)
        {
            if (creatures[i] != null)
            {
                creatures[i].SetSelected(i == selectedIndex);
            }
        }

        Creature selected = GetSelectedCreature();
        UpdateHud(selected != null ? $"Selected {selected.CreatureName}." : "No creature selected.");
    }

    // ABSTRACTION
    public void CareForSelectedCreature()
    {
        Creature selected = GetSelectedCreature();
        if (selected == null)
        {
            UpdateHud("No creature is available.");
            return;
        }

        string result = selected.PerformCareAction();
        UpdateHud(result);
    }

    private Creature GetSelectedCreature()
    {
        if (selectedIndex < 0 || selectedIndex >= creatures.Count) return null;
        CreatureSelector selector = creatures[selectedIndex];
        return selector != null ? selector.Creature : null;
    }

    private void UpdateHud(string message = null)
    {
        if (titleText != null)
        {
            titleText.text = "Creature Care Lab - OOP Programming Theory";
        }

        Creature selected = GetSelectedCreature();
        if (creatureText != null)
        {
            creatureText.text = selected == null
                ? "No creature selected"
                : $"{selected.CreatureName}\nEnergy: {selected.Energy}\nMood: {selected.Mood}\nFavorite care: {selected.FavoriteCare}";
        }

        if (statusText != null && !string.IsNullOrEmpty(message))
        {
            statusText.text = message + "\nKeys: 1/2/3 select, arrows cycle, Space care, R restart.";
        }
    }
}
