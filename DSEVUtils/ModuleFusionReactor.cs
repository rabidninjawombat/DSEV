using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyright 2015, by Michael Billard (Angel-125)
License: CC BY-NC-SA 4.0
License URL: https://creativecommons.org/licenses/by-nc-sa/4.0/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

Portions of this software use code from the Firespitter plugin by Snjo, used with permission. Thanks Snjo for sharing how to switch meshes. :)

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class ModuleFusionReactor : WBIHeater
    {
        [KSPField(isPersistant = true)]
        public float ecNeededToStart;

        [KSPField(isPersistant = true)]
        public string reactorFuel;

        [KSPField(isPersistant = true)]
        public float fuelConsumption;

        [KSPField(isPersistant = true)]
        public float ecProduced;

        [KSPField(isPersistant = true)]
        public float fuelRequest;

        [KSPField(guiActive = true)]
        public string status;

        protected Light[] lights;

        public override string GetInfo()
        {
            return string.Format("Requires {0:F2}ec to start.\nProduces {1:F2} SystemHeat per second.\nConsumes {2:F4} FusionPellets per second.", ecNeededToStart, heatGenerated, fuelConsumption);
        }

        public override void ToggleHeater()
        {
            double ecObtained = 0f;

            if (heaterIsOn == false)
            {
                ecObtained = this.part.RequestResource("ElectricCharge", ecNeededToStart);
                if (ecObtained < ecNeededToStart)
                {
                    ScreenMessages.PostScreenMessage("Fully charge the reactor before starting.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }

                //We're good to go, try to start.
                this.Activate();

                if (heaterIsOn)
                {
                    Events["ToggleHeater"].guiName = "Reactor Off";
                }
            }

            //Shut off the reactor
            else
            {
                this.Shutdown();
                Events["ToggleHeater"].guiName = "Reactor On";
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (isOverheated)
                status = "Overheated";
            else if (heaterIsOn == false)
                status = "Off";
            else
                status = "Output: " + ecProduced + "EC/sec";
        }

        public override void OnFixedUpdate()
        {
            PartResource ecReserve = this.part.Resources["ElectricCharge"];

            if (HighLogic.LoadedSceneIsEditor)
                return;

            base.OnFixedUpdate();
            float fuelPerTimeTick = fuelConsumption * TimeWarp.fixedDeltaTime;

            if (heaterIsOn)
            {
                //If the EC reserve is full then run the reactor in idle mode to consume fewer resources.
                if (ecReserve.amount == ecReserve.maxAmount)
                    fuelPerTimeTick = fuelPerTimeTick / 10.0f;

                //Consume reactor fuel. There seems to be a minimum amount to request.
                fuelRequest += fuelPerTimeTick;
                if (fuelRequest >= 0.01f)
                {
                    float fuelObtained = this.part.RequestResource(reactorFuel, fuelRequest);

                    if (fuelObtained < fuelRequest)
                    {
                        ScreenMessages.PostScreenMessage("Shutting down, reactor is out of " + reactorFuel, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        this.Shutdown();
                        fuelRequest = 0f;
                        return;
                    }

                    //Reset the fuel to request
                    fuelRequest = 0f;
                }

                //We are still running, generate electricity
                this.part.RequestResource("ElectricCharge", -ecProduced * TimeWarp.fixedDeltaTime);
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["ToggleHeater"].guiName = "Reactor On";
            Actions["ToggleHeaterAction"].guiName = "Toggle Reactor";
        }

        public override void OverheatWarning()
        {
            ScreenMessages.PostScreenMessage("Reactor shutting down due to overheating!", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            Events["ToggleReactor"].guiName = "Reactor On";

            if (onOverheatDelegate != null)
                onOverheatDelegate();
        }

        public override void Activate()
        {
            base.Activate();
            List<string> fusionEmitters = new List<string>();

            fusionEmitters.Add("Fusion");

            showOnlyEmittersInList(fusionEmitters);
        }

        public override void Shutdown()
        {
            base.Shutdown();

            hideAllEmitters();
        }
    }
}
