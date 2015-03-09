Deep Space Exploration Vessels

Inspiration

This mod is inspired by several designs including:
Discovery II: http://naca.larc.nasa.gov/search.jsp?R=20050160960
NAUTILUS-X MMSEV (Multi-Mission Space Exploration Vehicle): http://www.scribd.com/doc/51592987/Nautilus-X-Holderman-1-26-11
Deep Space Habitat: http://spacefellowship.com/news/art29280/constructing-next-generation-space-habitat-demonstrators.html
The Mars One Crew Manual by Kerry mark Joels: http://www.amazon.com/The-Mars-One-Crew-Manual/dp/0345318811

Additionally, the mod was inspired by Project Daedalus (http://www.bis-space.com/what-we-do/projects/project-daedalus).

Special thanks for Nyrath for sharing all that fantastic information on his Atomic Rockets website (http://www.projectrho.com/public_html/rocket/).

---INSTALLATION---

Copy the WildBlueIndustries directory into your GameData folder.

---REVISION HISTORY---
0.3.9: It's Not A Jungle Gym
- Added the Hexagonal Truss. The truss segment is based on the design from NASA's Discovery II paper. 
- Added the Half-sized Hexagonal Truss. It is based on the design from NASA's Discovery II paper.
- Added the HexPort, used for docking hexagonal truss segments together.
NOTE: If Near Future Construction is installed, then the hexagonal truss segments and HexPort will be hidden.
- Added the WB-2 MHD Fusion Reactor.
- Tweaked resource densities for Glykerol and FusionPellets to bring them in line with CRP values. Thanks Nertea for the new values and for including the resources with CRP! :)

0.3.8: You Spin Me Right Round
- Added the Spindle, Spin Ring, and Counter Torque Ring based upon ZodiusInfuser/Sirkut's rotating space station hub.
- Added SpinHub plugin to gently accelerate/decelerate the hubs, provide context menu and Action Group support, and to prevent ship breakage when in timewarp.
NOTE: Infernal Robotics required to make the hubs spin. Get it here: http://forum.kerbalspaceprogram.com/threads/37707-0-90-Magic-Smoke-Industries-Infernal-Robotics-0-19-3
This was so frustrating to do, it deserves a YouTube video:https://www.youtube.com/watch?v=PGNiXGX2nLU

0.3.7: Stefan-Boltzmann
- Updated the radiator code to use the Stefan-Boltzman law of radiation. Heat convection still not supported, for that you might check out SystemHeat by Nertea.

0.3.6: Hot Fire
- Rebranded the mod as Deep Space Exploration Vessels
- Deprecated the Nuclear Aerospike engine
- Added the WBR-120 Graphene Radiator
- Added the FLM-1800 Multipurpose Storage Tank. Right-click the tank to store a variety of different resources. Right-click to change the endcaps when in the editor. This 1.875m tank fits inside the hollow octo-truss from Near Future Construction and can be radially attached.
- The Supernova now requires radiators to function properly. You can disable heat management for the engine by right-clicking the engine and accessing the debug window.
- The Supernova now requires 48,000 EC to start the reactor before the engine can be used. Right-click the part to start the reactor. The EC startup requirement can be disabled in the debug menu.
- The Supernova now has a debug menu. Right-click the part to access the menu. Pressing Debug Reset will remove any overheating, clear out the heat sink, shut off the reactor, and reset a few internal variables so the engine appears as if it just left the VAB/Hangar.

NOTE: Existing spacecraft built prior to 0.3.6 should not be affected by the changes but just in case, use the debug menu to disable heat management and the EC startup requirement.

IMPORTANT NOTE: To install the mod, DELETE the existing GameData/WildBlueIndustries/NuclearEngine folder before installing DSEV. Be sure to copy ALL the files provided into the GameData folder.

FYI: It would be a good idea for new spacecraft built with this version of the mod to stock up on SpareParts...

0.3.5: Recompiled for 0.9.0 Beta. Added Modular Fuel Tanks config (thanks Kolago. :) ). Completely redesigned the mod's code to improve maintainability - I learned a LOT from Multipurpose Colony Modules!
- Supernova now has two modes: hydrogen mode and pulsed plasma mode. Hydrogen mode burns LiquidFuel as its propellant, unless you have Near Future Propulsion (NFP) installed. If you have NFP, then hydrogen mode burns LiquidHydrogen.
- Nuclear Aerospike now only burns LFO, but it offers superior thrust compared to the LV-N.

0.3.2: Improved plasma fusion flame effect. Added engine sounds. Code cleanup. Consolidated textures where possible; main textures are in the Textures folder while individual models have placeholders.

0.3.1: Added SupernovaController module. Modified MultiFuelSwitcher to support particle effects. Added custom flame effects for the Supernova's pulsed plasma mode, liquid fuel NTR mode, and liquid hydrogen NTR mode.

0.3: Added emissives to aerospike engine. Added initial version of Supernova as well as the large fusion pellet tank. Added FuelRods and FusionPellets resources.

0.2.3: Refactored controller module into MultiFuelSwitcher. Got curious about how R.A.P.I.E.R. engine switched fuel modes. Discovered MultiModeEngine. Said Ugh.
Realized that MultiModeEngine only allows two fuel modes, whereas MultiFuelSwitcher allows many. Cheered. :) Discovered new module fx controller and realized that engine effects can be set by fuel type, including sounds.

0.2: Fixed fuel gauge display bug. Finished aerospike model and textures. Added ISP and thrust modifiers to propellant list. Rebalanced engine stats based upon fuel types.

0.1: Initial revision

---ACKNOWLEDGEMENTS

Module Manager by ialdabaot
Community Resource Pack by RoverDude, Nertea, and the KSP community
Portions of this codebase include source by Snjo and Swamp-IG, used under CC BY-NC SA 4.0 license
Modular Fuel Tanks MM configs by Kolago.
Spindle and Spin Ring concept based upon parts by ZodiusInfuser/Sirkut. 

---LICENSE---

Source code copyrighgt 2014, by Michael Billard (Angel-125)
License: CC BY-NC-SA 4.0
License URL: https://creativecommons.org/licenses/by-nc-sa/4.0/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.