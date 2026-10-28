public enum StartMenuState
{
    // Keep these values stable: dialogue assets serialize this enum by number.
    Start = 0,
    TaskHandout = 1,
    // File details for a selected level. This can be opened from multiple areas.
    FileDetails = 2,
    // The level browser drawer and its camera position.
    Drawer = 3,
    Settings = 4,
    Await = 5,
    Wardrobe = 6,
    Hallway = 7
}