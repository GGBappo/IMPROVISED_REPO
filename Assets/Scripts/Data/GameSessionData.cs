public static class GameSessionData
{
    // this class acts as a scoreboard!
    // this is mostly for the states to know things, like if the player lost/won
    // but i think displaying these for the player would be kinda cool too!

    #region Player Statistics
    // i wont implement everything here yet
    // however i do think it'd be nice to keep track of each levels best run!

    /// <summary>
    /// The total number of strikes the player has accumulated EVER. This number should go up at the instance of a strike not calculated at the end of a level.
    /// <br>This should also not be effected by level resets.</br> 
    /// </summary>
    public static int totalStrikes;
    #endregion

    #region Level Outcomes
    public static bool lostOnTime;
    public static bool lostOnStrikes;
    public static bool lost;
    public static bool won;  
    #endregion
}