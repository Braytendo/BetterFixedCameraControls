# Better "Fixed Camera" Player Controls for your PS1 style game
This Unity script solves a movement issue that has plagued fixed camera games like Resident Evil (1996) or Final Fantasy VII (1997) for decades. 
<br>
All it takes is a generated plane, some raycasts, and a whole lot of math.
<br>
## The Issue
If you've ever played a fixed camera game, you know that controlling the player always feels a little unpredictable. Especially when the camera is tilted at strange or "cinematic" angles. This is because player movement is usually based on the camera's "forward" direction, which isn't completely accurate to the player's input.
<br>
<br>
When you press down, you expect the player to move in a downward or backward direction toward the bottom of the screen, but that isn't always what happens. To put it simply:
<br>
<br>
<img width="600" alt="problem" src="https://github.com/Braytendo/BetterFixedCameraControls/assets/10760359/7de3528a-b473-4e17-85da-b4b2623c14b5">
<br>
<br>
The player presses down, but if the character is on the left or right side of the screen, they won't move down, they'll move diagonally because it matches the camera's forward. It's correct, but it doesn't feel good. And this gets much worse if the camera is rotated. Suddenly all movement is diagonal and never matches the direction you're pressing.
<br>
<br>
So how do we fix it?
<br>
## The Solution
<img width="600" alt="solution" src="https://github.com/Braytendo/BetterFixedCameraControls/assets/10760359/184ff7b7-3cf7-4379-b48f-f2ffe42c0299">
<br>
<br>
We want the player to move in absolutes. Down is down. Up is up and so on. To solve this, we can use the player's screen to world position to create raycast points around the player on a generated plane at the player's feet. We can use those raycast points to adjust the math of which direction the character should move based on the player's input.
<br>
<br>
<img width="600" alt="example" src="https://github.com/Braytendo/BetterFixedCameraControls/assets/10760359/a2e96617-bee1-4406-8c82-d9ff14b89a62">
<br>
<br>
The above screenshot is a visual representation of the script at work (I've removed these visuals from the released code so you won't see them). The little red blocks represent the new directions for forward, left, right, and backward that are used when moving the player. 
<br>
<br>
<img width="600" alt="example2" src="https://github.com/Braytendo/BetterFixedCameraControls/assets/10760359/bca3cca4-21ca-47f1-a5f2-55d634477914">
<br>
<br>
Even when tilting the camera, the directions used for movement have been fixed! No more weird diagonal movement! 
<br>
Left is always left, right is always right, up is up and down is down.
<br>
And I can assure you, it feels much better playing this way. If you don't believe me, try testing the script yourself! :D
<br>
<br>
<b>NOTE: This script was written for my own game project using the Rewired Unity Plugin for input. I cleaned a few things up, but you'll need to tweak some code to get it to work (or consider getting Rewired from the asset store, it's pretty great for controller setup). For example, switching out all the references to "player" with your own input methods. There might be some other parts you need to modify, but the majority of the heavy lifting has been accomplished here.</b>
