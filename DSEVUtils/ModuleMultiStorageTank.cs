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

        protected bool defaultShowSleeves;
        protected bool defaultShowBrackets;
        protected bool defaultShowDecals;
        protected string sleevesObject;
        protected string bracketsObject;
        protected string decalsObject;
        protected Dictionary<string, int> meshIndexes = new Dictionary<string, int>();


        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Toggle Sleeves", active = true)]
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

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Toggle Decals", active = true)]
        public void ToggleDecals()
        {
            ModuleMultiStorageTank storageTank;

            showDecals = !showDecals;

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

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Toggle Brackets", active = true)]
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
            string value;

            value = protoNode.GetValue("defaultShowSleeves");
            if (string.IsNullOrEmpty(value) == false)
                defaultShowSleeves = bool.Parse(value);

            value = protoNode.GetValue("defaultShowBrackets");
            if (string.IsNullOrEmpty(value) == false)
                defaultShowBrackets = bool.Parse(value);

            value = protoNode.GetValue("defaultShowDecals");
            if (string.IsNullOrEmpty(value) == false)
                defaultShowDecals = bool.Parse(value);

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
            string[] meshes;

            Events["NextMesh"].guiActive = false;
            Events["NextMesh"].active = false;
            Events["PrevMesh"].active = false;
            Events["PrevMesh"].guiActive = false;

            if (HighLogic.LoadedSceneIsEditor)
            {
                showSleeves = defaultShowSleeves;
                showBrackets = defaultShowBrackets;
                showDecals = defaultShowDecals;
            }

            //Go through each entry and split up the entry into its template name and mesh index
            meshes = objects.Split(';');
            for (int index = 0; index < meshes.Count<string>(); index++)
                meshIndexes.Add(meshes[index], index);

            setVisibleObjects();
        }

        public override string GetInfo()
        {
            return "Right-click on the part to change resource settings, toggle decals, toggle end cap sleeves and toggle mounting brackets.";
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
                visibleObjects.Add(meshIndexes[decalsObject]);

            setObjects(visibleObjects);
        }

    }
}
