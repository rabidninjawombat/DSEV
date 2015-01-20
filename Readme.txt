Nuclear Engines

This zip file contains artwork and software for use as a mod for Kerbal Space Program. These designs are inspired from research provided by the Atomic Rockets website (http://www.projectrho.com/public_html/rocket/). Special thanks for Nyrath for sharing all that fantastic information.

---ENGINES---

The mod has several new nuclear engines including:

Nuclear Aerospike - This engine consists of a solid core oxidizer-augmented nuclear thermal rocket based upon the LANTR concept (http://www.projectrho.com/public_html/rocket/enginelist.php#lantr) and equipped with a plug nozzle. With the oxidizer augmentation "afterburner" system on, thrust increases while ISP drops, but regardless of mode, ISP has good ISP throughout the atmospheric envelope. 

You can change what fuel the engine uses in afterburner mode by changing the afterburnerFuelName in your part.cfg file. You can also change what propellant to use in non-afterburner (NTR) mode by changing the ntrFuelName field. For example, to make NTR mode use liqud hydrogen from NearFuture instead of the default liquid fuel, the LANTREngineController module in your part.cfg file should look like the following:

MODULE
{
	name = LANTREngineController
	aslISPAfterburner = 617
	vacISPAfterburner = 650
	maxThrustAfterburner = 184
	aslISPNRT = 875
	vacISPNTR = 921
	maxThrustNTR = 67
        afterburnerFuelName = LFO
        ntrFuelName = LiquidHydrogen
}


---PROPELLANTS---

The NTRPropellants.cfg file specifies several propellants that the various nuclear engines in this mod use. You can add your own if you wish. A sample entry is as follows:

NTR_PROPELLANT
{
//Internal name of the propellant, used by the mod.
//Use this name when specifying what fuel to burn in afterburner mode (afterburnerFuelName), or non-afterburner mode (ntrFuelName)
//This name will show up on the fuel gauge, so keep it short.
    name = LFO

//Name to show in context menu when you right-click. Shows up under "Fuel Type" entry.
    guiName = Liquid Fuel/Oxidizer

//ISP multiplier to use when burning this type of fuel.
//Multiplies the ISP entries by this value to get the actual ISP. Example: aslISPAfterburner * ispMultiplier
    ispMultiplier = 1.0

//Multiplies the vacuum and sea level thrust values by the multiplier
    thrustMultiplier = 1.0

//Standard propellant definitions.
    PROPELLANT
    {
        name = LiquidFuel
        ratio = 0.9
        DrawGauge = True
    }

    PROPELLANT
    {
        name = Oxidizer
        ratio = 1.1
        DrawGauge = False
    }
}

---INSTALLATION---

Copy the WildBlueIndustries directory into your GameData folder.

---REVISION HISTORY---

0.3: Refactored LANTREngineController into MultiFuelSwitcher. Removed NTRPropellants.cfg; parts now define their set of propellants that user chooses from. Added emissives to nuclear aerospike.

0.2: Fixed fuel gauge display bug. Finished aerospike model and textures. Added ISP and thrust modifiers to propellant list. Rebalanced engine stats based upon fuel types.

0.1: Initial revision

---LICENSE---

Copyright (c) 2014, Michael Billard (Angel-125)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.