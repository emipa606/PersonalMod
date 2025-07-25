# Copilot Instructions for RimWorld Mod: Vanya Energy Shields

## Mod Overview and Purpose

The Vanya Energy Shields mod introduces advanced energy shielding technology into RimWorld, enhancing defensive capabilities for colonists. The mod is designed to provide dynamic and responsive protection mechanics that are seamlessly integrated into the existing game framework. By introducing shield belts with unique properties and reactive visual feedback, the mod aims to deepen the player's tactical options and immerse them further into the RimWorld universe.

## Key Features and Systems

- **Energy Shield Belts**: A new type of apparel item that provides energy-based protection. The shield belts can absorb a certain amount of damage before requiring a recharge or breaking.
  
- **Dynamic Damage Absorption**: Implements methods to handle damage absorption and to trigger effects when the shield is depleted.

- **Status Display**: Utilizes a custom gizmo to display the current status of the energy shield, allowing players to make informed decisions about their usage.

## Coding Patterns and Conventions

- **File Organization**: The project is organized with separate classes for distinct functionalities, such as `Vanya_ShieldBelt` for the main shield mechanics and `Gizmo_Vanya_EnergyShieldStatus` for visual display interfaces.

- **Naming Conventions**: Classes and methods are named using PascalCase. Internal class names begin with `Gizmo` or `Vanya`, reflecting their roles or associated functionality. Methods in `Vanya_ShieldBelt` are named clearly to indicate their specific tasks—such as `AbsorbedDamage` and `KeepDisplaying`.

- **Access Modifiers**: Public access modifiers are used for key methods that need to be accessed outside their immediate class, while private methods handle internal logic.

## XML Integration

- The mod relies on XML files for defining shield belt properties, such as the amount of damage absorbed and the recharge time. XML definitions make it easy to tweak gameplay settings without altering C# code.

- XML files should include definitions for items, gizmos, and any additional game objects that interact with the shield mechanics. Ensure XML tags are consistently formatted and accurately referenced in the C# code.

## Harmony Patching

- **Harmony Lib Usage**: The mod uses Harmony to patch existing RimWorld methods, ensuring compatibility and additional functionality without modifying game source files directly.

- **Patch Guidelines**: Use Harmony prefix and postfix methods when you need to alter or extend the base game logic associated with shields. Ensure patches are targeted and isolated to avoid unintended side effects.

## Suggestions for Copilot

- **Method Optimization**: For complex methods such as `AbsorbedDamage`, Copilot can suggest refactorings or performance optimizations. Encourage Copilot to suggest patterns for handling conditional logic or iterative processes more efficiently.

- **XML Handling**: Utilize Copilot to automate repetitive XML generation or modification tasks. This ensures consistency and reduces errors during XML editing.

- **Error Handling**: Copilot can help identify areas in the code where error handling can be improved, suggesting try-catch blocks or logging mechanisms when exceptions might occur.

- **Testing Support**: Ask Copilot to create template unit tests for mod methods, focusing on mocking game states or simulating combat scenarios to test shield belt functionality.

In conclusion, leverage GitHub Copilot to enhance productivity by generating templates, refactoring code, and simplifying complex implementations, ensuring that the Vanya Energy Shields mod remains robust and user-friendly.
