using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
Source code copyright 2015, by Michael Billard (Angel-125)
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
        const float kMinimumECToCharge = 1.0f;
        const string kStartEngine = "Start Engine";
        const string kToggleEngine = "Toggle Engine";
        const string kShutdownEngine = "Shutdown Engine";
        const string kStopCharging = "Stop Charging";
        const string kOverheatWarning = "Engine shutdown! Heat is beyond capacity!";
        const string kOutOfFuel = "Engines throttled down, out of reactor fuel!";
        const string kOutOfEC = "Stopping the reactor, can't get enough electricity to charge.";
        const string kChargingCapacitor = "Charging capacitor before engine start...";

        protected MultiModeEngine multiModeEngine;
        protected ModuleEnginesFXWBI primaryEngine;
        protected ModuleEnginesFXWBI secondaryEngine;

        [KSPField(guiActive = true, guiName = "Reactor", isPersistant = true)]
        public string reactorStatus;

        [KSPField(isPersistant = true)]
        public EReactorStates reactorState = EReactorStates.None;

        [KSPField(isPersistant = true)]
        public double currentElectricCharge = 0f;

        [KSPField(isPersistant = true)]
        public float fuelRequest;

        [KSPField(guiActive = true, guiName = "Engine Temperature")]
        string engineTemperature;

        public bool requiresECToStart = true;
        public float fuelConsumption;
        public string primaryEngineID;
        public string reactorFuel;
        public float ecNeededToStart = 0f;
        public float ecChargePerSec = 0f;
        public SupernovaDebug debugMenu = new SupernovaDebug();
        public bool showDebugButton = true;
        public float primaryEngineHeat = 0f;
        public float secondaryEngineHeat = 0;

        #region Events And Actions
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Show Debug", active = true)]
        public void ShowDebug()
        {
            debugMenu.supernovaController = this;
            debugMenu.ToggleVisible();
        }

        [KSPAction(kStartEngine)]
        public void StartReactor(KSPActionParam param)
        {
            StopReactor(param);
            ToggleHeater();
        }

        [KSPAction(kShutdownEngine)]
        public void StopReactor(KSPActionParam param)
        {
            primaryEngine.Events["Shutdown"].Invoke();
            primaryEngine.currentThrottle = 0;
            primaryEngine.requestedThrottle = 0;

            secondaryEngine.Events["Shutdown"].Invoke();
            secondaryEngine.currentThrottle = 0;
            secondaryEngine.requestedThrottle = 0;

            Events["ToggleHeater"].guiName = kStartEngine;

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

            StopReactor(null);
            isOverheated = false;
            currentElectricCharge = 0f;
            reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart);
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

            value = protoNode.GetValue("primaryEngineHeat");
            if (string.IsNullOrEmpty(value) == false)
                primaryEngineHeat = float.Parse(value);

            value = protoNode.GetValue("secondaryEngineHeat");
            if (string.IsNullOrEmpty(value) == false)
                secondaryEngineHeat = float.Parse(value);

            primaryEngineID = protoNode.GetValue("primaryEngineID");

            reactorFuel = protoNode.GetValue("reactorFuel");
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            List<ModuleEnginesFXWBI> engineList = this.part.FindModulesImplementing<ModuleEnginesFXWBI>();

            multiModeEngine = this.part.FindModuleImplementing<MultiModeEngine>();
            multiModeEngine.autoSwitch = false;
            multiModeEngine.Events["DisableAutoSwitch"].guiActive = false;
            multiModeEngine.Events["DisableAutoSwitch"].guiActiveEditor = false;
            multiModeEngine.Events["EnableAutoSwitch"].guiActive = false;
            multiModeEngine.Events["EnableAutoSwitch"].guiActiveEditor = false;

            foreach (ModuleEnginesFXWBI engine in engineList)
            {
                if (engine.engineID == primaryEngineID)
                    primaryEngine = engine;
                else
                    secondaryEngine = engine;

                engine.onActiveDelegate = OnEngineActive;
                engine.Events["Activate"].guiActive = false;
                engine.Events["Shutdown"].guiActive = false;
            }

            Events["ToggleHeater"].guiName = kStartEngine;
            Actions["ToggleHeaterAction"].guiName = kToggleEngine;
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
                //If we need ec to start, then set our state to charging
                if ((currentElectricCharge < ecNeededToStart) && requiresECToStart)
                {
                    reactorState = EReactorStates.Charging;
                    heaterIsOn = false;
                    Events["ToggleHeater"].guiName = kStopCharging;
                }

                //Capacitor is fully charged, activate the engine and star idling the reactor
                else
                {
                    reactorState = EReactorStates.Idling;
                    heaterIsOn = true;
                    Events["ToggleHeater"].guiName = kShutdownEngine;

                    //Start your engine
                    if (multiModeEngine.runningPrimary)
                        primaryEngine.Activate();
                    else
                        secondaryEngine.Activate();
                }

                //Ok, we're done
                return;
            }

            else if (reactorState == EReactorStates.Charging || reactorState == EReactorStates.Idling)
            {
                reactorState = EReactorStates.Off;
                reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart - currentElectricCharge);
                heaterIsOn = false;
            }

            else if (reactorState == EReactorStates.Running)
            {
                StopReactor(null);
            }

            //Just set the GUI
            if (heaterIsOn)
            {
                Events["ToggleHeater"].guiName = kShutdownEngine;
            }

            else
            {
                Events["ToggleHeater"].guiName = kStartEngine;
            }
        }

        public override void ModTotalHeatToShed()
        {
            base.ModTotalHeatToShed();

            if (reactorState != EReactorStates.Idling && reactorState != EReactorStates.Running)
            {
                totalHeatToShed = 0f;
                return;
            }

            if (reactorState == EReactorStates.Idling)
            {
                totalHeatToShed *= 0.1f;
            }

            else if (primaryEngine.isOperational)
            {
                totalHeatToShed = primaryEngineHeat * primaryEngine.currentThrottle;
            }

            else if (secondaryEngine.isOperational)
            {
                totalHeatToShed = secondaryEngineHeat * secondaryEngine.currentThrottle;
            }

            else
            {
                reactorState = EReactorStates.Idling;
                reactorStatus = reactorState.ToString();
                totalHeatToShed *= 0.1f;
            }
        }

        public override void Activate()
        {
            if (manageHeat == false)
            {
                base.Activate();
                return;
            }

            if (heaterIsOn == false && (primaryEngine.staged || secondaryEngine.staged))
            {
                primaryEngine.Events["Shutdown"].Invoke();
                primaryEngine.currentThrottle = 0;
                primaryEngine.requestedThrottle = 0;

                secondaryEngine.Events["Shutdown"].Invoke();
                secondaryEngine.currentThrottle = 0;
                secondaryEngine.requestedThrottle = 0;

                if (reactorState != EReactorStates.Charging)
                    Events["ToggleHeater"].guiName = kStartEngine;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (manageHeat == false)
                return;

            engineTemperature = String.Format("{0:#.##}c", this.part.temperature);
        }

        public override void HeaterHasCooled()
        {
            base.HeaterHasCooled();
            reactorState = EReactorStates.Off;
            reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart);
        }

        public override void OverheatWarning()
        {
            ScreenMessages.PostScreenMessage(kOverheatWarning, 5.0f, ScreenMessageStyle.UPPER_CENTER);

            primaryEngine.Events["Shutdown"].Invoke();
            primaryEngine.currentThrottle = 0;
            primaryEngine.requestedThrottle = 0;

            secondaryEngine.Events["Shutdown"].Invoke();
            secondaryEngine.currentThrottle = 0;
            secondaryEngine.requestedThrottle = 0;

            Events["ToggleHeater"].guiName = kStartEngine;

            reactorState = EReactorStates.Overheated;
            currentElectricCharge = 0f;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            float fuelPerTimeTick = fuelConsumption * TimeWarp.fixedDeltaTime;

            //Adjust fuel consuption for idling
            if (primaryEngine.thrustPercentage == 0f && secondaryEngine.thrustPercentage == 0f)
                fuelPerTimeTick = fuelPerTimeTick / 10.0f;

            //The logic below doesn't apply unless we're flying
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            if (manageHeat)
            {
                //If reactor is not running then exit
                if (reactorState != EReactorStates.Running)
                    return;
            }

            //Consume a small amount of fusion pellets to represent the fusion reactor's operation in NTR mode.
            if (multiModeEngine.runningPrimary == true)
            {
                //Make sure we reach the minimum threshold
                fuelRequest += fuelPerTimeTick;
                if (fuelRequest >= 0.01f)
                {
                    //Consume fusion pellets
                    float fuelObtained = this.part.vessel.rootPart.RequestResource(reactorFuel, fuelRequest);

                    //If we haven't consumed enough pellets then the reactor cannot be sustained
                    //and the engine flames out.
                    if (fuelObtained < fuelRequest)
                    {
                        ScreenMessages.PostScreenMessage(kOutOfFuel, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        primaryEngine.Events["Shutdown"].Invoke();
                        primaryEngine.currentThrottle = 0;
                        primaryEngine.requestedThrottle = 0;
                        primaryEngine.flameout = true;
                        heaterIsOn = false;
                        Events["ToggleHeater"].guiName = kStartEngine;
                        reactorState = EReactorStates.Off;
                        reactorStatus = EReactorStates.Off + string.Format(" Needs {0:F2} EC", ecNeededToStart);
                    }

                    //Reset the fuel request
                    fuelRequest = 0f;
                }
            }
        }

        public override void  ManageHeat(List<WBIRadiator> radiators)
        {
            base.ManageHeat(radiators);
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

                //If we can't get the minimum EC required to charge the reactor, then shut off the reactor.
                //This way, the ship won't be starved for power.
                if (currentElectricCharge < kMinimumECToCharge)
                {
                    ScreenMessages.PostScreenMessage(kOutOfEC, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    StopReactor(null);
                    return;
                }

                if (currentElectricCharge < ecNeededToStart)
                {
                    reactorStatus = EReactorStates.Charging.ToString() + String.Format(" {0:F2}%", (currentElectricCharge / ecNeededToStart) * 100);
                    return;
                }

                //If we have enough charge then we can start the engine.
                heaterIsOn = true;
                currentElectricCharge = 0f;
                Events["ToggleHeater"].guiName = kShutdownEngine;
                if (multiModeEngine.runningPrimary)
                    primaryEngine.Activate();
                else
                    secondaryEngine.Activate();
            }

            //Set status
            if (primaryEngine.currentThrottle > 0f || secondaryEngine.currentThrottle > 0f)
                reactorState = EReactorStates.Running;
            else if (isOverheated == false)
                reactorState = EReactorStates.Idling;
            reactorStatus = reactorState.ToString();
        }

        #endregion

        #region Helpers
        public void OnEngineActive(string engineID, bool isStaged)
        {
            if (heaterIsOn == false && isStaged)
            {
                primaryEngine.Events["Shutdown"].Invoke();
                primaryEngine.currentThrottle = 0;
                primaryEngine.requestedThrottle = 0;

                secondaryEngine.Events["Shutdown"].Invoke();
                secondaryEngine.currentThrottle = 0;
                secondaryEngine.requestedThrottle = 0;

                //If the reactor is off, then then start charging the capacitor.
                if (reactorState != EReactorStates.Charging)
                {
                    ScreenMessages.PostScreenMessage(kChargingCapacitor, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    Events["ToggleHeater"].guiName = kStartEngine;
                    ToggleHeater();
                }
            }
        }
        #endregion
    }
}
