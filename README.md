# Programming Theory Project

Local Unity project for the Junior Programmer `Submission: Programming theory in action`.

## Concept

`Creature Care Lab` is a small 3D prototype where the player selects one of three creatures and cares for it before its energy and mood drop.

Controls:

- `1`, `2`, `3`: select a creature
- Left/Right arrows: cycle selection
- `Space`: care for the selected creature
- `R`: restart the scene

## OOP Checklist

- `// INHERITANCE`: `ForestCreature`, `CrystalCreature`, and `EmberCreature` inherit from `Creature`.
- `// POLYMORPHISM`: each creature overrides `PerformCareAction()`.
- `// ENCAPSULATION`: `Creature` keeps energy and mood private and exposes controlled properties.
- `// ABSTRACTION`: `CreatureCareGame.CareForSelectedCreature()` hides selection and per-creature behavior behind one public action.

## Still Needed For Submission

- Local git commits with clear messages.
- At least two branches and a merge.
- Public GitHub repository link.
- Screenshot or screen recording.

These were not created automatically because commits and pushes require explicit permission.
