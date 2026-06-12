using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CreatureSelector : MonoBehaviour
{
    [SerializeField] private Creature creature;
    [SerializeField] private Vector3 selectedScale = new Vector3(1.25f, 1.25f, 1.25f);

    private Renderer cachedRenderer;
    private Vector3 originalScale;
    private Color baseColor;

    public Creature Creature => creature;

    private void Awake()
    {
        if (creature == null)
        {
            creature = GetComponent<Creature>();
        }

        cachedRenderer = GetComponent<Renderer>();
        originalScale = transform.localScale;

        if (creature != null)
        {
            baseColor = creature.DisplayColor;
            cachedRenderer.material.color = baseColor;
        }
    }

    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? selectedScale : originalScale;
        if (cachedRenderer != null)
        {
            cachedRenderer.material.color = selected ? Color.white : baseColor;
        }
    }
}
