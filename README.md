# Match3Prototype
Match 3 or more same colored tiles.

-Architecture Used:
*MVC Pattern
*Singleton Pattern

-Features :
- Tiles gets spawned based on probability from Scriptable Object. 
- Board width and height controlled by scirptable object.
- Blocks are spawned again randomly when there's a deadlock condition.
- Checks for already matched tiles whenever new ones are spawned.
- Uses itween for simple move animation.
- Uses async await as a substitute for coroutines.


--Not tested/Buggy features (Commented at the end of board controller):
- At deadlock , reshuffle.
- After matched tiles removed, place the upper tile in that position. 

Needs refactoring to improve readability
