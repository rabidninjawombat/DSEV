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

0.3.14: Tanks For Experimentals
- Added the FLM-100 radial storage tank. It slipped into the last update, so it's now officially ready for use.
- Deprecated the WB-120 radial fusion pellet tank.
- Removed the SpareParts storage template. Fixing DSEV vessels will require RocketParts (which, if you have Extraplanetary Launchpads, can be used to build rockets).
- Fixed an issue where the ArcJet RCS would still act like it is firing even when disabled.
- Fixed an issue where the HexPort allowed you to toggle the vestibule while in flight.
- Fixed an issue where the double hex truss allowed you to toggle the center vestibule while in flight.

0.3.13: Broken Wings (https://youtu.be/nKhN1t_7PEY)
- Truncated the Large Graphene Radiator to 3 segments instead of 5.
NOTE: With the smaller radiator size, you'll need 14 radiators instead of 10 to cool the Supernova at full thrust. Be sure to build new ships accordingly.

0.3.12: Like A Record Baby

Hex Trusses
- Added a configurable Hex Node. You can toggle the side attachment nodes and switch between open frame (default), bracing, and pressurized crew tubes.

Centrifuge
- Retextured the Spindle. No more primary colors!

Other
- Added MiniAVC support

Bug Fixes
- Fixed an issue preventing the proper number of equipment racks from showing on the hex trusses during flight.
- Fixed an issue allowing the double-hex truss's center vestibule to be toggled during flight.

0.3.11: Moar Jungle Gyms

Hex Trusses
- Added the double-sized hex truss.
- Hex trusses have additional cross bracing to prevent eye strain. No more escher trusses!
- Hex trusses can be toggled in the editor to have three types of end caps: Open, Braced, and Vestibule. Open is the default.
- Hex trusses can be toggled in the editor to have 2, 3, 6, and no equipment racks. Two racks is the default.
  NOTE: If you have 6 equipment racks then you won't be able to access any equipment installed inside the truss.
- Hex trusses can be hidden if you have Near Future Construction installed (which has a better selection of trusses). simply rename MM_Hex.txt to MM_Hex.cfg.

FLM-1800 Multipurpose Storage Tank
- To prevent accidental dumping of resources during flight, the storage tank now requires a confirmation click to change the stored resource type.

Docking Ports
- The glow-rings on the HexPort and 3.75m Docking Ring can now be toggled in flight or in the editor.

Supernova
- If the Supernova is staged before starting the reactor, then the reactor will be automatically started and the capacitor will begin charging.
- After the capacitor is fully charged, the engine will automatically activate.
- If the Supernova can't get enough electric charge while charging the reactor, then it will switch off the reactor to prevent the ship from becoming E.C. starved.
- The Start Reactor button has been renamed Start Engine.
- The Stop Reactor button has been renamed Shutdown Engine.
- Added engine temperature readout in Celsius.
- Removed the Activate button from the context menu.
- Removed extra Shutdown Engine button from the context menu.
- Removed the automatic mode switching button from the context menu.

Other
- Removed the Nuclear Aerospike. Be sure to retire any spacecraft that use the aerospike before installing this update.

Bug Fixes
- Fixed an issue with the Supernova where staging to activate the engine before starting the reactor would cause the engine to overheat. Thanks for the debug logs, Kamuchi! :)
- Fixed an issue where the Supernova could be activated without starting the reactor, thus bypassing heat management. Thanks for the playtest, BuzZBladE! :)
- Synchronized ship-wide heat generation and heat radiation via a centralized heat manager. This might actually improve performance as well. 
  NOTE: radiators will still have different temperatures due to the way the game heats up nearby parts and dissipates heat.
- Fixed an issue preventing the ArcJet RCS Thruster from firing when a ship is equipped with fuel pipes.
- The WB-120 GrapheneRadiator now shows temperature in Celsius instead of Kelvin. It actually already was showing Celsius, but the label said Kelvin. No more temperatures below absolute zero!
- The 3.75m docking ring now shows the correct names for the variant button (Open, Vestibule), and no longer shows the Previous Variant button.
- Fixed an issue where the 3.75m docking ring would show the variant button while in flight.

Special thanks to DasValdez, Kamuchi, BuzZBladE and the viewers of KSPTV for providing vital feedback and testing. It really helped to see the mod in use and to see what worked and what needed improvements. :)

0.3.10: Bug Fixes
- Added safety feature to prevent heat radiators from exploding when they take on too much heat from reactors and engines.

0.3.9: It's Not A Jungle Gym
- Added the Hexagonal Truss. The truss segment is based on the design from NASA's Discovery II paper and is sized the same as a Near Future Construction's Octo-truss by Nertea to accommodate octo-standard 1.875m by 4m truss payloads.
- Added the Half-sized Hexagonal Truss. It is based on the design from NASA's Discovery II paper and can accommodate 1.875 by 2m truss payloads.
- Added the HexPort hexagonal docking port. It is compatible with the Clamp-O-Tron Sr. Thank you for the tips, sumghai and nil2work!
NOTE: MM configs hide the hex truss, half-hex truss, and HexPort if Near Future Construction is installed since NFC has a much better selection of parts. Delete the MM_Hex.cfg file in the DSEV folder if you still want to use them.
- Added the 3.75m docking ring.
- Added the WB-2 MHD Fusion Reactor.
- Added the RT-4 ArcJet Reaction Control Thruster. It's like a Place-Anywhere 7, but it uses LiquidFuel (LiquidHydrogen if Near Future Propulsion is installed)- and a lot of power.
- Tweaked resource densities for Glykerol and FusionPellets. Thanks Nertea for the new values!
- Parts now support Community Tech Tree if installed.

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
Special thanks to DasValdez, Kamuchi, BuzZBladE and the viewers of KSPTV for providing vital feedback and testing. It really helped to see the mod in use and to see what worked and what needed improvements. :)

---LICENSE---

Source code copyrighgt 2014, by Michael Billard (Angel-125)
License: CC BY-NC-SA 4.0
License URL: https://creativecommons.org/licenses/by-nc-sa/4.0/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.