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

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    class ModuleMultiStorageTank : WBIMeshHelper
    {
        [KSPField(isPersistant = true)]
        public bool showSleeves;

        [KSPField(isPersistant = true)]
        public bool showBrackets;

        [KSPField(isPersistant = true)]
        public bool showDecals;

        [KSPField(isPersistant = true)]
        public bool showLargeDiameter;

        [KSPField(isPersistant = true)]
        public bool toggleDiameters = false;

        [KSPField(isPersistant = true)]
        public float largeDiameterCapacity = 0f;

        [KSPField(isPersistant = true)]
        public float mediumDiameterCapacity = 0f;

        protected string sleevesObject;
        protected string bracketsObject;
        protected string decalsObject;

        [KSPEvent(guiActiveEditor = true, guiActive = false, guiName = "Toggle Sleeves", active = true)]
        public void ToggleSleeves()
        {
            ModuleMultiStorageTank storageTank;

            showSleeves = !showSleeves;

            setVisibleObjects();

            //Handle symmetrical parts
            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (Part symmetryPart in this.part.symmetryCounterparts)
                {
                    storageTank = symmetryPart.GetComponent<ModuleMultiStorageTank>();
                    storageTank.ShowSleeves(showSleeves);
                }
            }
        }

        [KSPEvent(guiActiveEditor = true, guiActive = false, guiName = "Toggle Decals", active = true)]
        public void ToggleDecals()
        {
            ModuleMultiStorageTank storageTank;

            showDecals = !showDecals;
            Log("FRED showDecals: " + showDecals);

            setVisibleObjects();

            //Handle symmetrical parts
            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (Part symmetryPart in this.part.symmetryCounterparts)
                {
                    storageTank = symmetryPart.GetComponent<ModuleMultiStorageTank>();
                    storageTank.ShowDecals(showDecals);
                }
            }
        }

        [KSPEvent(guiActiveEditor = true, guiActive = false, guiName = "Toggle Brackets", active = true)]
        public void ToggleBrackets()
        {
            ModuleMultiStorageTank storageTank;

            showBrackets = !showBrackets;

            setVisibleObjects();

            //Handle symmetrical parts
            if (HighLogic.LoadedSceneIsEditor)
            {
                foreach (Part symmetryPart in this.part.symmetryCounterparts)
                {
                    storageTank = symmetryPart.GetComponent<ModuleMultiStorageTank>();
                    storageTank.ShowBrackets(showBrackets);
                }
            }
        }

        [KSPEvent(guiActiveEditor = true, guiActive = false, guiName = "Toggle Diameter", active = true)]
        public void ToggleDiameter()
        {
            ModuleMultiStorageTank storageTank;
            if (HighLogic.LoadedSceneIsEditor)
            {
                //If there are parts attached then don't allow the switch.
                foreach (Part childPart in this.part.children)
                    if (childPart.attachMode == AttachModes.SRF_ATTACH)
                    {
                        ScreenMessages.PostScreenMessage("Cannot change tank diameter until attached parts are removed.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        return;
                    }

                showLargeDiameter = !showLargeDiameter;

                ShowLargeDiameter(showLargeDiameter);

                //Handle symmetrical parts
                if (HighLogic.LoadedSceneIsEditor)
                {
                    foreach (Part symmetryPart in this.part.symmetryCounterparts)
                    {
                        storageTank = symmetryPart.GetComponent<ModuleMultiStorageTank>();
                        storageTank.ShowLargeDiameter(showLargeDiameter);
                    }
                }
            }
        }

        public void ShowLargeDiameter(bool isLargeDiameter)
        {
            showLargeDiameter = isLargeDiameter;

            setVisibleObjects();

            //Get the resource switcher
            WBIResourceSwitcher resourceSwitcher = this.part.FindModuleImplementing<WBIResourceSwitcher>();
            if (resourceSwitcher == null)
                return;

            //Reset capacity
            if (showLargeDiameter)
                resourceSwitcher.capacityFactor = largeDiameterCapacity;
            else
                resourceSwitcher.capacityFactor = mediumDiameterCapacity;

            //Now reload resource template
            resourceSwitcher.ReloadTemplate();
        }

        public void ShowBrackets(bool bracketsAreVisible)
        {
            showBrackets = bracketsAreVisible;

            setVisibleObjects();
        }

        public void ShowSleeves(bool sleevesAreVisible)
        {
            showSleeves = sleevesAreVisible;

            setVisibleObjects();
        }

        public void ShowDecals(bool decalsAreVisible)
        {
            showDecals = decalsAreVisible;

            setVisibleObjects();
        }

        protected override void getProtoNodeValues(ConfigNode protoNode)
        {
            base.getProtoNodeValues(protoNode);

            sleevesObject = protoNode.GetValue("sleevesObject");

            bracketsObject = protoNode.GetValue("bracketsObject");

            decalsObject = protoNode.GetValue("decalsObject");
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            setObject(-1);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["NextMesh"].guiActive = false;
            Events["NextMesh"].active = false;
            Events["PrevMesh"].active = false;
            Events["PrevMesh"].guiActive = false;

            if (string.IsNullOrEmpty(sleevesObject))
            {
                Events["ToggleSleeves"].guiActive = false;
                Events["ToggleSleeves"].active = false;
            }
            if (string.IsNullOrEmpty(bracketsObject))
            {
                Events["ToggleBrackets"].guiActive = false;
                Events["ToggleBrackets"].active = false;
            }
            if (string.IsNullOrEmpty(decalsObject))
            {
                Events["ToggleDecals"].guiActive = false;
                Events["ToggleDecals"].active = false;
            }
            if (toggleDiameters == false)
            {
                Events["ToggleDiameter"].guiActive = false;
                Events["ToggleDiameter"].active = false;
            }

            setVisibleObjects();
        }

        public override string GetInfo()
        {
            string info = "Right-click on the part to change resource settings, toggle decals, toggle end cap sleeves and toggle mounting brackets.";

            if (toggleDiameters)
                info += "/n You can also toggle the diameter.";

            return info;
        }

        public override void OnEditorAttach()
        {
            base.OnEditorAttach();
            setVisibleObjects();
        }

        protected virtual void setVisibleObjects()
        {
            List<int> visibleObjects = new List<int>();

            if (showSleeves)
                visibleObjects.Add(meshIndexes[sleevesObject]);

            if (showBrackets)
                visibleObjects.Add(meshIndexes[bracketsObject]);

            if (showDecals)
            {
                string[] decals = decalsObject.Split(new char[] {','});
                foreach (string decal in decals)
                    visibleObjects.Add(meshIndexes[decal]);
            }

            if (toggleDiameters)
            {
                if (showLargeDiameter)
                    visibleObjects.Add(0);
                else
                    visibleObjects.Add(1);
            }

            setObjects(visibleObjects);
        }

    }
}
