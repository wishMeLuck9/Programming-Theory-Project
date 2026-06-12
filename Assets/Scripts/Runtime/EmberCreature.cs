using UnityEngine;

// INHERITANCE
public class EmberCreature : Creature
{
    public override Color DisplayColor => new Color(1f, 0.42f, 0.18f);
    public override string FavoriteCare => "warm spark";

    public override string PerformCareAction()
    {
        Feed(12);
        return $"{CreatureName} catches a {FavoriteCare} and brightens.";
    }
}
