# TitaniumEggBot

Uses the `Discord.NET` library to provide enhanced Discord functionality to users.

Enables the access to the League of Legends API, Realmeye user data via webscraper using the Anglesharp library, as well as a small minigame that allows users to fight monsters, forage, and hunt.

## Components

### League of Legends Module

![Image of League Rank](/images/lolrank_egg.png)

Uses the [Camille](https://github.com/MingweiSamuel/Camille) C# library to access League of Legends API.

### Realm of the Mad God Module

![Image of ROTMG player stats](/images/rotmg_player.png)

Uses Anglesharp to get html elements through css selectors corresponding to each individual character's stats and inventory.

### Hattington Minigame

Character:

![Image of hat character](/images/hatchar.png)

Fighting:

![Image of hat fight](/images/hatfight.png)

Inventory:

![Image of hat inventory](/images/hatinv.png)

Hunting:

![Image of hat hunting](/images/hathunt.png)

Eating:

![Image of hat meal](/images/hateat.png)

Hattington minigame uses Entity Framework Core to access a SQLite database that stores the character data, enemies, inventory, and food types. Game logic is all found in HattingtonGame project, with specific types of events sent to each type of handler (ex. HattingtonFightHandler, HattingtonFoodHandler)
