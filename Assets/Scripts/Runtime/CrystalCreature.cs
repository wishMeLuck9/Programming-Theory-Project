using UnityEngine;

// INHERITANCE
public class CrystalCreature : Creature
{
    public override Color DisplayColor => new Color(0.35f, 0.85f, 1f);
    public override string FavoriteCare => "clear rhythm";

    public override string PerformCareAction()
    {
        Feed(10);
        return $"{CreatureName} follows a {FavoriteCare} and stabilizes.";
    }
}
