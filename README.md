Howdy, this is a basic Midpoint Terrain Generation program that I wroteas aa part of module based testing for an upcoming game.
The program includes click and drag functionality to look around the terrain to your hearts content.

Some basic configuration settings: 
*Note: All found as constants in the InitialiseWorld function*

initialTerrainPoints is a list of points that is passed into the midpoint displacement algorithm. For basic ground and mountains, just specify points on an equal y plain
iterations is the number of times the algorithm runs. Higher numbers results in higher fidelity terrain but is more computationally expensive.
initialDisplacement is the amount the algorithm initially displaces the points. Higher values can create bigger terrain features.
decayPower is how fast the algorithm reduces the displacement per iteration. Uses the equation 1/2^decayPower. So high decayPowers results in smoother terrain

Suggested Initial Values-

Flatlands:
initialTerrainPoints: Any two points on a flat horizontal plane
iterations: typically around 8-9. Can use lower values and interpolate without jitter later to keep broad details
initialDisplacement: 200-400. Things greater than this can produce canyon-like mountains
decayPower: 1.3-1.4. I've found this typically produces non-jagged terrain, however most terrain is still typically a bit jagged

Mountains:
I've added a seperate repo branch for mountains, just to save some good settings for later experimentation

initialTerrainPoints: As before, just add a singular mid-point with any height to your desire. This initial point will be the main mountain peak
iterations: 8-9 as before
initialDisplacement: 200f. Not too big so that the initial mountain cannot be overriden
decayPower: 1.3. As before, not too much jaggedness.

One additional modification. In the randomPlusMinusOffsetValue function inside the algorithm class, change the random.Next bounds from 0-2 to 0-3.
This biases the random value generation to be 66% positive, resulting in the terrain being more concave as one would expect from the high mountain peaks.
This change is the main reason mountains is it's own repo branch. Due to a change in the algorithm rather than merely its perameters.

On
