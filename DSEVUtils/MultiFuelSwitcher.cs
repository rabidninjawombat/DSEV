using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyrighgt 2014, by Michael Billard (Angel-125)
License: CC BY-NC-SA 4.0
License URL: https://creativecommons.org/licenses/by-nc-sa/4.0/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class MultiFuelSwitcher : ExtendedPartModule
    {
        static protected ConfigNode[] _propellantTypes;
        protected ModuleEnginesFX myEngineModule;
        protected VInfoBox _fuelGauge;

        [KSPField(isPersistant = true)]
        protected int propellantTypeIndex;

        protected float aslISP = 0f;
        protected float vacISP = 0f;
        protected float maxThrust = 0f;

        protected float previousThrottle;
        protected bool previousFlameoutState;

        #region Display Fields
        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Fuel Type")]
        public string propellantTypeGUI;
        #endregion

        #region User Events
        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Toggle", active = true)]
        public void TogglePropellantEvent()
        {
            if (_propellantTypes != null)
            {
                int nextUsableIndex = propellantTypeIndex;

                //Currently, we have a list of propellant types that may or may not be supported in the vessel's current configuration.
                //That may change as the vessel docks with another vessel or loses stages.
                //Hence, when the user toggles propellant types, we need to search through the list of possible propellant types that the engine
                //supports and determine the next propellant type that the vessel currently supports.
                nextUsableIndex = nextUsablePropTypeIndex(propellantTypeIndex);
                if (nextUsableIndex == -1 || nextUsableIndex == propellantTypeIndex)
                {
                    ScreenMessages.PostScreenMessage("Cannot find a propellant to switch to!", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }
                //Ok, we're good
                propellantTypeIndex = nextUsableIndex;

                //Change the toggle button's name
                nextUsableIndex = nextUsablePropTypeIndex(nextUsableIndex);
                if (nextUsableIndex != -1 && nextUsableIndex != propellantTypeIndex)
                    Events["TogglePropellantEvent"].guiName = "Use " + _propellantTypes[nextUsableIndex].GetValue("contextName");

                //Setup the engine stats
                setupEngineStats();
            }

            else
            {
                Log("TogglePropellantEvent _propellantTypes == null!");
            }
            
        }
        #endregion

        #region Actions
        [KSPAction("Toggle Propellant")]
        public void TogglePropellantAction(KSPActionParam param)
        {
            TogglePropellantEvent();
        }
        #endregion

        #region Module Overrides

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            Log("OnLoad: " + node + " Scene: " + HighLogic.LoadedScene.ToString());

            //When the part is loaded for the first time as the game starts up, we'll be reading the MODULE config node in the part's config file.
            //At that point we'll have access to the defined propellant types. Later on when the part is loaded, the game doesn't load the MODULE config node.
            //Instead, we seem to load an instance of the part. So later on we'll load propellant types from our parent class' _subNodes.
            if (HighLogic.LoadedScene == GameScenes.LOADING)
                _propellantTypes = node.GetNodes("PROPELLANT_TYPE");
        }

        public override void OnActive()
        {
            base.OnActive();

            //Get my engine module
            myEngineModule = this.part.Modules["ModuleEnginesFX"] as ModuleEnginesFX;
            _fuelGauge = this.part.stackIcon.DisplayInfo();

            setup_module();
        }

        public virtual void OnEditorAttach()
        {
            foreach (AttachNode attachNode in part.attachNodes)
            {
                if (attachNode.attachedPart != null)
                {
                    List<ModuleEnginesFX> sources = attachNode.attachedPart.FindModulesImplementing<ModuleEnginesFX>();
                    if (sources.Count > 0)
                    {
                        myEngineModule = sources.First();
                        if (myEngineModule != null)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            Log("OnStart: " + state);

            if (state == StartState.Editor)
                part.OnEditorAttach += OnEditorAttach;

            //Get my engine module
            myEngineModule = this.part.Modules["ModuleEnginesFX"] as ModuleEnginesFX;
            _fuelGauge = this.part.stackIcon.DisplayInfo();

            setup_module();
        }

        public override void OnUpdate()
        {
            float currentpropellant = 0;
            float maxpropellant = 0;
            ConfigNode propellantTypeNode;
            List<string> emitters;

            if (!HighLogic.LoadedSceneIsFlight)
                return;

            //Get all the propellant tanks and add up their current and max amounts.
            List<PartResource> partresources = new List<PartResource>();
            part.GetConnectedResources(myEngineModule.propellants[0].id, ResourceFlowMode.ALL_VESSEL, partresources);

            foreach (PartResource partresource in partresources)
            {
                currentpropellant += (float)partresource.amount;
                maxpropellant += (float)partresource.maxAmount;
            }

            if (_fuelGauge != null && _fuelGauge.infoBoxRef != null)
            {
                //If the engine is running, show the fuel gage and update it
                if (myEngineModule.isOperational)
                {
                    if (!_fuelGauge.infoBoxRef.expanded)
                    {
                        _fuelGauge.infoBoxRef.Expand();
                    }
                    _fuelGauge.length = 2;
                    if (maxpropellant > 0)
                    {
                        _fuelGauge.SetValue(currentpropellant / maxpropellant);
                    }
                    else
                    {
                        _fuelGauge.SetValue(0);
                    }
                }

                //Hide the fuel gauge
                else
                {
                    if (!_fuelGauge.infoBoxRef.collapsed)
                    {
                        _fuelGauge.infoBoxRef.Collapse();
                    }
                }
            }

            //Show or hide the particle effects based on the throttle settings.
            //If the throttle hasn't changed then there's nothing to do.
            if (myEngineModule.currentThrottle == previousThrottle && myEngineModule.flameout == previousFlameoutState)
            {
                return;
            }
            else
            {
                previousThrottle = myEngineModule.currentThrottle;
                previousFlameoutState = myEngineModule.flameout;
            }

            //Get the runningEffectName and add it to the emitters list.
            propellantTypeNode = getPropellantTypeNode();
            emitters = new List<string>();
            emitters.Add(propellantTypeNode.GetValue("runningEffectName"));

            //If we're throttled up then turn on the emitters.
            if (myEngineModule.currentThrottle > 0f && myEngineModule.flameout == false)
                showOnlyEmittersInList(emitters);

            //Switch them off
            else
                hideEmittersInList(emitters);
        }

        public override string GetInfo()
        {
            StringBuilder sbInfo = new StringBuilder();
            ConfigNode[] propellantNodes;
            Propellant prop;
            int curIndex;

            foreach (ConfigNode propellantType in _propellantTypes)
            {
                sbInfo.Append(propellantType.GetValue("contextName") + "\n");
                sbInfo.Append("ISP (s/l): " + propellantType.GetValue("aslISP") + "\n");
                sbInfo.Append("ISP (vac): " + propellantType.GetValue("vacISP") + "\n");
                sbInfo.Append("Max Thrust: " + propellantType.GetValue("maxThrust") + "\n\n");

                //Get the propellant nodes
                propellantNodes = propellantType.GetNodes("PROPELLANT");

                //Loop through all the nodes and add their consumption rates to the info
                sbInfo.Append("Consumption Rates (units/sec)\n\n");
                for (curIndex = 0; curIndex < propellantNodes.Length; curIndex++)
                {
                    //Create a propellant object and load the node
                    prop = new Propellant();
                    prop.Load(propellantNodes[curIndex]);
                    sbInfo.Append(prop.name + ": " + calculatePropellantUPS(prop, true) + " (vac)\n");
                    sbInfo.Append(prop.name + ": " + calculatePropellantUPS(prop, false) + " (s/l)\n");
                }


            }

            return sbInfo.ToString();
        }

        #endregion

        #region Helpers

        private int nextUsablePropTypeIndex(int startIndex)
        {
            int nextIndex = startIndex;
            int maxTries = _propellantTypes.Count<ConfigNode>();

            //Cycle through the propellant types
            nextIndex = (nextIndex + 1) % _propellantTypes.Count<ConfigNode>();

            //If we're not in flight, don't worry about whether or not the supported propellants are currently in the vessel.
            if (!HighLogic.LoadedSceneIsFlight)
                return nextIndex;

            //The current vessel configuration might not have the propellant type. If so, then cycle to the next propellant type.
            //If we run out of propellant types that don't exist, then we cannot switch the propellant.
            while (ResourceHelper.VesselHasResource(_propellantTypes[nextIndex].GetValue("gaugeName"), this.vessel) == false && maxTries > 0)
            {
                maxTries -= 1;
                nextIndex = (nextIndex + 1) % _propellantTypes.Count<ConfigNode>();
            }

            if (maxTries <= 0)
                return -1;

            return nextIndex;
        }

        protected override void getProtoNodeValues(ConfigNode protoNode)
        {
            base.getProtoNodeValues(protoNode);

            _propellantTypes = protoNode.GetNodes("PROPELLANT_TYPE");
        }

        protected void setup_module()
        {
            try
            {
                Log("setup_module");

                //Setup toggle button
                int nextIndex = nextUsablePropTypeIndex(propellantTypeIndex);
                if (nextIndex != -1)
                    Events["TogglePropellantEvent"].guiName = "Use " + _propellantTypes[nextIndex].GetValue("contextName");

                //Setup the engine stats
                if (_fuelGauge != null)
                    setupEngineStats();
            }

            catch (Exception ex)
            {
                Log("setup_module generated an exception: " + ex);
            }
        }

        protected void set_ispthrust_mode()
        {
            FloatCurve ispCurve = new FloatCurve();

            //Set engine's thrust and velocity curve
            myEngineModule.maxThrust = maxThrust;
            myEngineModule.useEngineResponseTime = false;
            myEngineModule.useVelocityCurve = false;

            //Set engine's ISP curve
            ispCurve.Add(0, vacISP, 0, 0);
            ispCurve.Add(1, aslISP, 0, 0);
            myEngineModule.atmosphereCurve = ispCurve;
        }

        protected ConfigNode getPropellantTypeNode()
        {
            if (propellantTypeIndex >= 0 && propellantTypeIndex < _propellantTypes.Count<ConfigNode>())
                return _propellantTypes[propellantTypeIndex];
            else
                return null;
        }

        protected virtual void setupEngineStats()
        {
            ConfigNode propellantTypeNode = getPropellantTypeNode();
            //Sanity check
            if (propellantTypeNode == null)
                return;
            Log("Setting up engine stats");

            ConfigNode[] propellantNodes = null;
            List<Propellant> propellants = new List<Propellant>();
            Propellant prop;

            //Get fuel type name
            propellantTypeGUI = propellantTypeNode.GetValue("contextName");
            Log("New propellant: " + propellantTypeGUI + " maxThrust: " + maxThrust + " vac ISP: " + vacISP + " asl ISP: " + aslISP);

            //Get the isp/thrust values & set ispthrust mode
            maxThrust = float.Parse(propellantTypeNode.GetValue("maxThrust"));
            aslISP = float.Parse(propellantTypeNode.GetValue("aslISP"));
            vacISP = float.Parse(propellantTypeNode.GetValue("vacISP"));
            set_ispthrust_mode();

            //Get the PROPELLANT configuration nodes.
            propellantNodes = propellantTypeNode.GetNodes("PROPELLANT");

            //Setup the fuel gauge
            _fuelGauge.SetMessage(propellantTypeNode.GetValue("gaugeName"));
            _fuelGauge.SetMsgBgColor(XKCDColors.DarkLime);
            _fuelGauge.SetMsgTextColor(XKCDColors.ElectricLime);
            _fuelGauge.SetProgressBarColor(XKCDColors.Yellow);
            _fuelGauge.SetProgressBarBgColor(XKCDColors.DarkLime);
            _fuelGauge.SetValue(0f);

            //Loop through all the nodes and add them to the propellant list
            for (int curIndex = 0; curIndex < propellantNodes.Length; curIndex++)
            {
                //Create a propellant object and load the node
                prop = new Propellant();
                prop.Load(propellantNodes[curIndex]);

                if (prop.drawStackGauge && HighLogic.LoadedSceneIsFlight)
                {
                    prop.drawStackGauge = false;

                    //Make sure we're throttled up
                    myEngineModule.thrustPercentage = 100f;
                }

                //Add to the list
                propellants.Add(prop);
            }

            //Now, set the engine's propellant list.
            myEngineModule.propellants.Clear();
            myEngineModule.propellants = propellants;
            myEngineModule.SetupPropellant();

            //Finally, setup the engine effects
            setupEngineEffects(propellantTypeNode);
        }

        protected void setupEngineEffects(ConfigNode propellantTypeNode)
        {
            string effectName;
            List<string> emitterNames = new List<string>();
            bool isOperational = myEngineModule.isOperational;

            //Shut down the engine
            //We do this so that engine sounds are switched.
            myEngineModule.Shutdown();

            /*
             * NOTE: It looks like ModuleEnginesFX will only show one emitter in the particle effects file.
             * I haven't figured out how to get it to show multiple emitters. Also, when switching fuel types,
             * ModuleEnginesFX does not seem to hide the previous particle emitter(s). We must hide the previous
             * fuel type's particle emitters ourselves.
             * For convenience, emitters should be named after the effect name specified in the EFFECT node to make it easy to turn them on or off.
             * Example: My runningEffectName for liquid hydrogen is called hydrogenFlame. All the emitters in the FX_LH2Flame
             * are called hydrogenFlame. Similarly, the FX_PlasmaFlame has emitters named plasmaFlame, and the runningEffectName
             * is also named plasmaFlame.
            */
            effectName = propellantTypeNode.GetValue("runningEffectName");
            if (effectName != null)
            {
                emitterNames.Add(effectName);
                myEngineModule.runningEffectName = effectName;
                Log("New runningEffectName: " + myEngineModule.runningEffectName);
            }

            effectName = propellantTypeNode.GetValue("directThrottleEffectName");
            if (effectName != null)
            {
                emitterNames.Add(effectName);
                myEngineModule.directThrottleEffectName = effectName;
                Log("New directThrottleEffectName: " + myEngineModule.directThrottleEffectName);
            }

            effectName = propellantTypeNode.GetValue("disengageEffectName");
            if (effectName != null)
            {
                emitterNames.Add(effectName);
                myEngineModule.disengageEffectName = effectName;
                Log("New disengageEffectName: " + myEngineModule.disengageEffectName);
            }

            effectName = propellantTypeNode.GetValue("engageEffectName");
            if (effectName != null)
            {
                emitterNames.Add(effectName);
                myEngineModule.engageEffectName = effectName;
                Log("New engageEffectName: " + myEngineModule.engageEffectName);
            }

            effectName = propellantTypeNode.GetValue("flameoutEffectName");
            if (effectName != null)
            {
                emitterNames.Add(effectName);
                myEngineModule.flameoutEffectName = effectName;
                Log("New flameoutEffectName: " + myEngineModule.flameoutEffectName);
            }

            effectName = propellantTypeNode.GetValue("powerEffectName");
            if (effectName != null)
            {
                emitterNames.Add(effectName);
                myEngineModule.powerEffectName = effectName;
                Log("New powerEffectName: " + myEngineModule.powerEffectName);
            }

            //If we're throttled up then show the new emitters.
            if (myEngineModule.currentThrottle > 0f)
                showOnlyEmittersInList(emitterNames);

            //Restart the engine if needed
            if (isOperational)
                myEngineModule.Activate();
        }

        protected float calculatePropellantUPS(Propellant prop, bool useVacISP)
        {
            float propellantUnitsPerSec = -1;
            float isp = useVacISP == true ? vacISP : aslISP;
            float exhaustVelosity = 9.82f * isp; //9.82 is correct, used by KSP
            float massFlowRate = (maxThrust * 1000) / exhaustVelosity; //kilograms per second
            float propellantDensity = 1f;
            PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
            PartResourceDefinition pdPropellant;

            //Get propellant density
            if (definitions.Contains(prop.name))
                pdPropellant = definitions[prop.name];
            else
                return propellantUnitsPerSec;

            //Convert density from metric tons to kilograms per cubic meter
            propellantDensity = pdPropellant.density * 1000000f;

            //Calcuate total units per second based upon max thrust and ISP
            propellantUnitsPerSec = (massFlowRate / propellantDensity) * 1000;

            //Account for mixture ratio
            propellantUnitsPerSec /= prop.ratio;

            return propellantUnitsPerSec;
        }

        #endregion

    }
}
