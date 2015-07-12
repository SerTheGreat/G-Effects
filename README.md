# G-Effects mod for Kerbal Space Program

Suppoted KSP version: 1.0.4

The work was originally started by russnash37 who gave a permission to take over and extend [his project] (http://forum.kerbalspaceprogram.com/threads/106055)

I've tried to make focus on realism as much as I was able to research the subject so the default settings assume that all kerbals including tourists are trained astronauts wearing G-suits.
Pilots are more trained than others, of course.

**The following G-effects are simulated:**
* Blackouts/redouts
* Tunnel vision as G rises
* Kerbals grunt while they perform AGSM (anti-G straining maneuver) and take a heavy breath after
* Blood beating in kerbal's ears on redout (wear headphones with good bass and you'll feel it)
* G-LOC (G-induced loss of consciousness)
* Kerbal deaths of a sustained over-G

G forces have different severity in four directions: upward, downward, backward, forward, so you may find that a kerbal launched in a rocket stands more G than
a kerbal piloting a plane upside down on a circular trajectory.
Kerbal's specialization also affects how much he can stand.

**Installation:**

Place contents of the GameData folder of the zip into your KSP/GameData folder

**Configuration:**
Configuration of the mod is done via G-Effects.cfg file. Look through it to have an exhaustive description of its parameters.

_The sounds are still a little WIP so you always have an option to disable them via config file if you find'em too annoying._

**Known issues and limitations:**
- Be careful with various tweaks that may lead to some sudden acceleration rise or collision lags, for example ejection or undocking force tweaks, as they may cause crew to lose consciousness or even die of excessive g forces.
- G-forces caused by a ship's rotation don't affect because they are not likely to be severe enough to induce any significant effects.
- Effects are calculated for the active vessel's crew only. You can switch to another vessel and back and have effects applied as if they have just started.

**To be implemented:**

- Simulation of loss of orientation after G-LOC
