using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
Source code copyrighgt 2014, by Michael Billard (Angel-125)
License: CC BY-NC-SA 4.0
License URL: https://creativecommons.org/licenses/by-nc-sa/4.0/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public enum EReactorStates
    {
        None,
        Off,
        Charging,
        Ready,
        Idling,
        Running,
        Overheated
    }

    public class SupernovaController : WBIHeater
    {
        protected MultiModeEngine multiModeEngine;
        protected ModuleEnginesFXWBI primaryEngine;
        protected ModuleEnginesFXWBI secondaryEngine;

        [KSPField(guiActive = true, guiName = "Reactor", isPersistant = true)]
        public string reactorStatus;

        [KSPField(isPersistant = true)]
        public EReactorStates reactorState = EReactorStates.None;

        [KSPField(isPersistant = true)]
        public double currentElectricCharge = 0f;

        public bool requiresECToStart = true;
        public float fuelConsumption;
        public string primaryEngineID;
        public string reactorFuel;
        public float ecNeededToStart = 0f;
        public float ecChargePerSec = 0f;
        public SupernovaDebug debugMenu = new SupernovaDebug();
        public bool showDebugButton = true;

        #region Events And Actions
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Show Debug", active = true)]
        public void ShowDebug()
        {
            debugMenu.supernovaController = this;
            debugMenu.ToggleVisible();
        }

        [KSPAction("Start Reactor")]
        public void StartReactor()
        {
            StopReactor();
            ToggleHeater();
        }

        [KSPAction("Stop Reactor")]
        public void StopReactor()
        {
            primaryEngine.Events["Shutdown"].Invoke();
            primaryEngine.currentThrottle = 0;
            primaryEngine.requestedThrottle = 0;

            secondaryEngine.Events["Shutdown"].Invoke();
            secondaryEngine.currentThrottle = 0;
            secondaryEngine.requestedThrottle = 0;

            Events["ToggleHeater"].guiName = "Start Reactor";

            if (reactorState != EReactorStates.Charging)
                currentElectricCharge = 0f;
            reactorState = EReactorStates.Off;
            if (currentElectricCharge == 0f)
                reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart);
            else
                reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart - currentElectricCharge);
            heaterIsOn = false;
        }

        public void DebugReset()
        {
            PartResource heatSink = this.part.Resources["SystemHeat"];
            heatSink.amount = 0f;

            StopReactor();
            isOverheated = false;
            vesselPartCount = -1;
            currentElectricCharge = 0f;
            reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart);
            throttleFactor = 1f;
        }

        #endregion

        #region Overrides
        protected override void getProtoNodeValues(ConfigNode protoNode)
        {
            base.getProtoNodeValues(protoNode);
            string value;

            value = protoNode.GetValue("fuelConsumption");
            if (string.IsNullOrEmpty(value) == false)
                fuelConsumption = float.Parse(value);

            value = protoNode.GetValue("ecNeededToStart");
            if (string.IsNullOrEmpty(value) == false)
                ecNeededToStart = float.Parse(value);

            value = protoNode.GetValue("ecChargePerSec");
            if (string.IsNullOrEmpty(value) == false)
                ecChargePerSec = float.Parse(value);

            value = protoNode.GetValue("showDebugButton");
            if (string.IsNullOrEmpty(value) == false)
                showDebugButton = bool.Parse(value);

            primaryEngineID = protoNode.GetValue("primaryEngineID");

            reactorFuel = protoNode.GetValue("reactorFuel");
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            List<ModuleEnginesFXWBI> engineList = this.part.FindModulesImplementing<ModuleEnginesFXWBI>();

            multiModeEngine = this.part.FindModuleImplementing<MultiModeEngine>();

            foreach (ModuleEnginesFXWBI engine in engineList)
            {
                if (engine.engineID == primaryEngineID)
                    primaryEngine = engine;
                else
                    secondaryEngine = engine;
            }

            Events["ToggleHeater"].guiName = "Start Reactor";
            Actions["ToggleHeaterAction"].guiName = "Toggle Reactor";
            Events["ShowDebug"].guiActive = showDebugButton;

            if (reactorState == EReactorStates.None)
            {
                currentElectricCharge = 0f;
                reactorState = EReactorStates.Off;
                reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart);
            }
        }

        public override void ToggleHeater()
        {
            base.ToggleHeater();

            //If we got this far then it means we have the potential to start.
            //First check to see if we have enough electric charge.
            if (reactorState == EReactorStates.Off)
            {
                if ((currentElectricCharge < ecNeededToStart) && requiresECToStart)
                {
                    reactorState = EReactorStates.Charging;
                    heaterIsOn = false;
                    Events["ToggleHeater"].guiName = "Stop Charging";
                }

                else
                {
                    reactorState = EReactorStates.Idling;
                    heaterIsOn = true;
                    Events["ToggleHeater"].guiName = "Stop Reactor";
                }
                return;
            }

            else if (reactorState == EReactorStates.Charging)
            {
                reactorState = EReactorStates.Off;
                reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart - currentElectricCharge);
                heaterIsOn = false;
            }

            if (heaterIsOn)
            {
                Events["ToggleHeater"].guiName = "Stop Reactor";
            }

            else
            {
                Events["ToggleHeater"].guiName = "Start Reactor";
            }
        }

        public override void ModTotalHeatToShed()
        {
            base.ModTotalHeatToShed();

            if (reactorState != EReactorStates.Idling && reactorState != EReactorStates.Running)
            {
                totalHeatToShed = 0f;
                throttleFactor = 0f;
                return;
            }

            if (reactorState == EReactorStates.Idling)
            {
                totalHeatToShed *= 0.1f;
                throttleFactor *= 0.1f;
            }

            else if (primaryEngine.isOperational)
            {
                totalHeatToShed = totalHeatToShed * primaryEngine.currentThrottle;
                throttleFactor = primaryEngine.currentThrottle;
            }

            else if (secondaryEngine.isOperational)
            {
                totalHeatToShed = totalHeatToShed * secondaryEngine.currentThrottle;
                throttleFactor = secondaryEngine.currentThrottle;
            }

            else
            {
                reactorState = EReactorStates.Idling;
                reactorStatus = reactorState.ToString();
                totalHeatToShed *= 0.1f;
                throttleFactor *= 0.1f;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (manageHeat == false)
                return;

            if (heaterIsOn == false && (primaryEngine.staged || secondaryEngine.staged))
            {
                ScreenMessages.PostScreenMessage("Start the reactor before using the engine.", 5.0f, ScreenMessageStyle.UPPER_CENTER);

                primaryEngine.Events["Shutdown"].Invoke();
                primaryEngine.currentThrottle = 0;
                primaryEngine.requestedThrottle = 0;

                secondaryEngine.Events["Shutdown"].Invoke();
                secondaryEngine.currentThrottle = 0;
                secondaryEngine.requestedThrottle = 0;

                if (reactorState != EReactorStates.Charging)
                    Events["ToggleHeater"].guiName = "Start Reactor";
            }
        }

        public override void HeaterHasCooled()
        {
            base.HeaterHasCooled();
            reactorState = EReactorStates.Off;
            reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart);
        }

        public override void OverheatWarning()
        {
            ScreenMessages.PostScreenMessage("Engine shutdown! Heat is beyond capacity!", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            primaryEngine.Events["Shutdown"].Invoke();
            primaryEngine.currentThrottle = 0;
            primaryEngine.requestedThrottle = 0;

            secondaryEngine.Events["Shutdown"].Invoke();
            secondaryEngine.currentThrottle = 0;
            secondaryEngine.requestedThrottle = 0;

            Events["ToggleHeater"].guiName = "Start Reactor";

            reactorState = EReactorStates.Overheated;
            currentElectricCharge = 0f;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            List<PartResource> pelletTanks;
            double pelletsConsumed = 0;
            double pelletsToConsume = fuelConsumption * TimeWarp.fixedDeltaTime;
            bool isFlameout = false;

            //The logic below doesn't apply unless we're flying
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            //If reactor is off then exit
            if (reactorState == EReactorStates.Off)
                return;

            //If we've flamed out then idle the reactor
            else if (reactorState == EReactorStates.Running)
            {
                if (primaryEngine.CalculateThrust() <= 0f && primaryEngine.currentThrottle > 0f)
                    isFlameout = true;
                if (secondaryEngine.CalculateThrust() <= 0f && secondaryEngine.currentThrottle > 0f)
                    isFlameout = true;

                if (isFlameout)
                {
                    secondaryEngine.currentThrottle = 0;
                    secondaryEngine.requestedThrottle = 0;
                    primaryEngine.currentThrottle = 0;
                    primaryEngine.requestedThrottle = 0;
                    reactorState = EReactorStates.Idling;
                    return;
                }
            }

            //If we are charging up, request electric charge and then exit.
            if (reactorState == EReactorStates.Charging)
            {
                currentElectricCharge += this.part.RequestResource("ElectricCharge", ecChargePerSec * TimeWarp.fixedDeltaTime);

                if (currentElectricCharge < ecNeededToStart)
                {
                    reactorStatus = EReactorStates.Charging.ToString() + String.Format(" {0:F2}%", (currentElectricCharge / ecNeededToStart) * 100);
                    return;
                }

                //If we have enough charge then we can start the engine.
                heaterIsOn = true;
                currentElectricCharge = 0f;
                Events["ToggleHeater"].guiName = "Stop Reactor";
            }

            //Set idle or running status
            if (primaryEngine.currentThrottle > 0f || secondaryEngine.currentThrottle > 0f)
                reactorState = EReactorStates.Running;
            else
                reactorState = EReactorStates.Idling;
            reactorStatus = reactorState.ToString();

            //Consume a small amount of fusion pellets to represent the fusion reactor's operation in NTR mode.
            if (multiModeEngine.runningPrimary == true)
            {
                //Get the pellet tanks
                pelletTanks = ResourceHelper.GetConnectedResources(reactorFuel, this.part);

                //Consume fusion pellets
                pelletsConsumed = ResourceHelper.ConsumeResource(pelletTanks, pelletsToConsume);

                //If we haven't consumed enough pellets then the reactor cannot be sustained
                //and the engine flames out.
                if (pelletsConsumed < pelletsToConsume)
                {
                    ScreenMessages.PostScreenMessage("Engines throttled down, out of reactor fuel!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    primaryEngine.Events["Shutdown"].Invoke();
                    primaryEngine.currentThrottle = 0;
                    primaryEngine.requestedThrottle = 0;
                    primaryEngine.flameout = true;
                    heaterIsOn = false;
                    Events["ToggleHeater"].guiName = "Start Reactor";
                    reactorState = EReactorStates.Off;
                    reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart);
                }
            }
        }

        #endregion

        #region Helpers
        #endregion
    }
}
