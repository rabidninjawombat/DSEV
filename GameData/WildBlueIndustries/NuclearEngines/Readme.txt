Nuclear Engines

This zip file contains artwork and software for use as a mod for Kerbal Space Program. These designs are inspired from research provided by the Atomic Rockets website (http://www.projectrho.com/public_html/rocket/). Special thanks for Nyrath for sharing all that fantastic information.

---ENGINES---

The mod has several new nuclear engines including:

Nuclear Aerospike - This engine consists of a solid core oxidizer-augmented nuclear thermal rocket based upon the LANTR concept (http://www.projectrho.com/public_html/rocket/enginelist.php#lantr) and equipped with a plug nozzle. With the oxidizer augmentation "afterburner" system on, thrust increases while ISP drops, but regardless of mode, ISP has good ISP throughout the atmospheric envelope.

WB-8 Supernova Fusion Engine - This engine is a hybrid fusion thermal rocket and pulsed plasma engine. It is based upon NASA's Discovery II spacecraft (http://naca.larc.nasa.gov/search.jsp?R=20050160960) as well as Project Daedalus (http://www.bis-space.com/what-we-do/projects/project-daedalus)

---INSTALLATION---

Copy the WildBlueIndustries directory into your GameData folder.

---REVISION HISTORY---

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

---LICENSE---

Source code copyrighgt 2014, by Michael Billard (Angel-125)
License: CC BY-NC-SA 4.0
License URL: https://creativecommons.org/licenses/by-nc-sa/4.0/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.