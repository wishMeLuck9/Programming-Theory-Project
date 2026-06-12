using UnityEngine;

// INHERITANCE
public class ForestCreature : Creature
{
    public override Color DisplayColor => new Color(0.2f, 0.75f, 0.32f);
    public override string FavoriteCare => "sun seeds";

    public override string PerformCareAction()
    {
        Feed(14);
        return $"{CreatureName} absorbs {FavoriteCare} and grows calmer.";
    }
}
