# DB50 Delaying the Inevitable

## Description
A simple tower defence game where the enemies will inevitably overwhelm the player, so it comes down to how long the player can last and how many of the enemy the player will take with them.

## Gameplay
Enemies come at the player in waves. 

Each wave has 10 + (current wave - 1 / 2) enemies (rounded down). Enemies spawn at the extreme right of the play area, and move left towards the player's base.

When a wave is destroyed, player gets 15 seconds before the next wave spawns. A countdown timer warns the player.

Every fifth wave, (current wave / 5) boss enemies are added to the wave. These are tougher, slighter faster, and hit harder.

The player's score is increased for each enemy destroyed (bosses are worth more).

### Enemies
Enemies in the game so far:
* soldiers: regular, red-coloured tanks; trundle across the map and melee defences
* bosses: green-coloured tanks; behave the same way as soldiers but have more hit points and do more damage
* buggies: very fast, lightly armed and armoured vehicles that can get in quick and do damage before defences realise they are there
* shooty tanks: blue-coloured tanks; behave much the same way as soldiers and bosses, but are able to do damage at range

### Defences
To counter the enemy waves, the player puts down defences in a grid (only one defence per grid square). The player "pays" for these with their score, so the player can only place defences as long as they have enough to pay for that defence. The player cannot reduce their score to zero.

Defences have a set number of hit points. Enemies can destroy defences. If a defence is destroyed, its starting hit points are subtracted from the player's score.

Defences available in the game so far:
* turrets: fire at enemies
* barriers: block enemies' progress
* mines: do a large amount of damage to an enemy that comes into contact with the mine.
* sniper towers: slow rate-of-fire but enormous range and damage

### Game Over conditions
The game is over if:
* the player's score is reduced to zero; or
* the player's base building is destroyed.

## Still to do
Lots still to add: see my [Trello board](https://trello.com/invite/b/A681FngS/5f6da1b5dd085fcfb7f6cc2fc51db95b/ld50-delaying-the-inevitable).

## That gnarly bug...
On the first attempt to produce a standalone *Windows* x86_64 build, an unusual bug appeared: the player couldn't open the build menu; three UI elements -- the time elapsed, the enemies destroyed, and the score -- were not being updated; and the Game-Over condition was never getting triggered.

My first thought was it was a problem with the new Unity Input System, but however much I tinkered with it, I couldn't get it to change its mind, the controls to scroll the view left and right still worked, and it didn't explain why some UI elements weren't working.

I next suspected that maybe it was something to do with `UnityEvent`s, but only a couple of things were using `UnityEvent`s.

I then compared the bits that worked to the bits that didn't, and everything that wasn't working could be tied back to the `PlayerManager` class: it relied on an event invoked by the `PlayerManager`; it needed `PlayerManager` to listen for an event; or `PlayerManager` needed to do something during its `Update()` method. All the things that worked didn't touch the `PlayerManager` class.

I generated a Development Build (in hindsight, I probably should've done this first) which threw up a `NullReferenceException` in the development log. Checking the log file, and the exception was occuring in ... `PlayerManager.Awake()`. Specifically, the line: `this.GetComponent<DefenceBuilding>().OnDefenceDestroyed.AddListener(GameOver);` the `OnDefenceDestroyed` event was set up in the `DefenceBuilding.Awake()` method. I moved the offending line to `PlayerManager.Start()` *et voilá*, the problem went away.

I double checked the Unity manuals, and on the page describing [`MonoBehavior.Awake`](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html), this little gem:

>The order that Unity calls each GameObject's Awake is not deterministic. Because of this, you should not rely on one GameObject's Awake being called before or after another (for example, you should not assume that a reference set up by one GameObject's Awake will be usable in another GameObject's Awake). Instead, you should use Awake to set up references between scripts, and use Start, which is called after all Awake calls are finished, to pass any information back and forth.

## Third-party resources
### Fonts
* [Advanced Pixel 7 by Style-7](https://www.1001freefonts.com/advanced-pixel-7.font)
* [Advanced Pixel LCD 7 by Style-7](https://www.1001freefonts.com/advanced-pixel-lcd-7.font)
* [2015 Cruiser by Pixel Sagas - Neale Davidson](https://www.1001freefonts.com/2015-cruiser.font)
* [Feast of Flesh by Blambot Fonts](https://www.1001freefonts.com/feast-of-flesh.font)

### Unity references
* [Brackeys (YouTube channel)](https://www.youtube.com/c/Brackeys)
* [Code Monkey (YouTube channel)](https://www.youtube.com/c/CodeMonkeyUnity)
