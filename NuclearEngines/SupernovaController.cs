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
    public class SupernovaController : ExtendedPartModule
    {
        protected MultiModeEngine multiModeEngine;
        protected ModuleEnginesFXWBI primaryEngine;

        protected float pelletConsumptionRate = 0.01f; //default rate
        protected string primaryEngineID;
        protected string reactorFuel;

        #region Overrides
        protected override void getProtoNodeValues(ConfigNode protoNode)
        {
            base.getProtoNodeValues(protoNode);
            string value;

            value = protoNode.GetValue("pelletConsumptionRate");
            if (string.IsNullOrEmpty(value) == false)
                pelletConsumptionRate = float.Parse(value);

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
                {
                    primaryEngine = engine;
                    break;
                }
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            //The logic below doesn't apply unless we're flying
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            List<PartResource> pelletTanks;
            double pelletsConsumed = 0;
            double pelletsToConsume = pelletConsumptionRate * TimeWarp.fixedDeltaTime;

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
                }
            }
        }

        #endregion

        #region Helpers
        #endregion
    }
}
