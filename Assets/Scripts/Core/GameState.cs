public enum UIState
{
    None,
    Dialogue,
    Inventory,
    Menu,
    Inspecting,
    Transitioning
}

public static class GameState
{
    public static UIState Current = UIState.None;

    public static bool IsDialogueOpen => Current == UIState.Dialogue;
    public static bool IsInventoryOpen => Current == UIState.Inventory;
    public static bool IsMenuOpen => Current == UIState.Menu;
    public static bool IsInspecting => Current == UIState.Inspecting;
    public static bool IsTransitioning => Current == UIState.Transitioning;

    public static bool IsBlocked => Current != UIState.None;
}