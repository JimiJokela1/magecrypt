using Magecrypt;
using SadConsole.Configuration;

Settings.WindowTitle = "Magecrypt";

Builder configuration = new Builder()
        .SetScreenSize(240, 72)
        .SetStartingScreen<RootScreen>()
        .IsStartingScreenFocused(true)
    ;
Game.Create(configuration);
Game.Instance.Run();
Game.Instance.Dispose();
