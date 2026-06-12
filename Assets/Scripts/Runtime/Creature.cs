using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    [SerializeField] private string creatureName = "Creature";
    [SerializeField, Range(0, 100)] private int energy = 50;
    [SerializeField, Range(0, 100)] private int mood = 50;

    public string CreatureName => creatureName;

    // ENCAPSULATION
    public int Energy
    {
        get => energy;
        private set => energy = Mathf.Clamp(value, 0, 100);
    }

    public int Mood
    {
        get => mood;
        private set => mood = Mathf.Clamp(value, 0, 100);
    }

    public bool IsTired => Energy < 25;

    public void Initialize(string displayName, int startEnergy, int startMood)
    {
        creatureName = string.IsNullOrWhiteSpace(displayName) ? GetType().Name : displayName;
        Energy = startEnergy;
        Mood = startMood;
    }

    public void Feed(int amount)
    {
        if (amount <= 0) return;

        Energy += amount;
        Mood += Mathf.Max(1, amount / 2);
    }

    public void Tick(float difficulty)
    {
        int drain = Mathf.Max(1, Mathf.RoundToInt(difficulty * Time.deltaTime));
        Energy -= drain;
        if (IsTired)
        {
            Mood -= drain;
        }
    }

    // POLYMORPHISM
    public virtual string PerformCareAction()
    {
        Feed(8);
        return $"{CreatureName} rests and recovers energy.";
    }

    public abstract Color DisplayColor { get; }
    public abstract string FavoriteCare { get; }
}
