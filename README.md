# DB50 Delaying the Inevitable

## Description
A simple tower defence game where the enemies will inevitably overwhelm the player, so it comes down to how long the player can last and how many of the enemy the player will take with them.

## Gameplay
Enemies come at the player in waves. 

Each wave has 10 + (current wave - 1 / 2) enemies (rounded down). Enemies spawn at the extreme right of the play area, and move left towards the player's base.

When a wave is destroyed, player gets 15 seconds before the next wave spawns. A countdown timer warns the player.

Every fifth wave, (current wave / 5) boss enemies are added to the wave. These are tougher, slighter faster, and hit harder.

The player's score is increased for each enemy destroyed (bosses are worth more).

### Defences
To counter the enemy waves, the player puts down defences in a grid (only one defence per grid square). The player "pays" for these with their score, so the player can only place defences as long as they have enough to pay for that defence. The player cannot reduce their score to zero.

Defences have a set number of hit points. Enemies can destroy defences. If a defence is destroyed, its starting hit points are subtracted from the player's score.

Defences available in the game so far:
* turrets: fire at enemies
* barriers: block enemies' progress
* mines: do a large amount of damage to an enemy that comes into contact with the mine.

### Game Over conditions
The game is over if:
* the player's score is reduced to zero; or
* the player's base building is destroyed.

## Still to do
Lots still to add: see my [Trello board|https://trello.com/invite/b/A681FngS/5f6da1b5dd085fcfb7f6cc2fc51db95b/ld50-delaying-the-inevitable]

## Third-party resources
### Fonts
Advanced Pixel 7 by Style-7 [link|https://www.1001freefonts.com/advanced-pixel-7.font]
Advanced Pixel LCD 7 by Style-7 [link|https://www.1001freefonts.com/advanced-pixel-lcd-7.font]
2015 Cruiser by Pixel Sagas - Neale Davidson [link|https://www.1001freefonts.com/2015-cruiser.font]