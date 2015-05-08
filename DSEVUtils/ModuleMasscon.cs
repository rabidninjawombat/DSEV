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

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class ModuleMasscon : ExtendedPartModule
    {
        private const float smallAmountDelta = 0.1f;
        private const float bigAmountDelta = 1.0f;

        [KSPField(guiActiveEditor = true, guiActive = false, guiName = "Change Amount")]
        public float amountDelta = 0.1f;

        [KSPEvent(guiActiveEditor = true, guiActive = false, guiName = "+ Masscon", active = true)]
        public void IncrementMasscon()
        {
            PartResource massconResource = this.part.Resources["Masscon"];

            //Sanity checks
            if (massconResource == null)
                return;
            if (massconResource.amount == massconResource.maxAmount)
                return;

            //Increment the masscon amount
            massconResource.amount += amountDelta;
            if (massconResource.amount > massconResource.maxAmount)
                massconResource.amount = massconResource.maxAmount;

            //Update symmetry parts
            foreach (Part symmetryPart in this.part.symmetryCounterparts)
                symmetryPart.Resources["Masscon"].amount = massconResource.amount;
        }

        [KSPEvent(guiActiveEditor = true, guiActive = false, guiName = "- Masscon", active = true)]
        public void DecrementMasscon()
        {
            PartResource massconResource = this.part.Resources["Masscon"];

            //Sanity checks
            if (massconResource == null)
                return;
            if (massconResource.amount == 0f)
                return;

            //Decrement the masscon amount
            massconResource.amount -= amountDelta;
            if (massconResource.amount < 0.0001)
                massconResource.amount = 0f;

            //Update symmetry parts
            foreach (Part symmetryPart in this.part.symmetryCounterparts)
                symmetryPart.Resources["Masscon"].amount = massconResource.amount;
        }

        [KSPEvent(guiActiveEditor = true, guiActive = false, guiName = "Toggle Delta", active = true)]
        public void ToggleAmountChange()
        {
            if (amountDelta == smallAmountDelta)
                amountDelta = bigAmountDelta;
            else
                amountDelta = smallAmountDelta;
        }

        [KSPEvent(guiActiveEditor = false, guiActive = true, guiName = "Balance Masscon", active = true)]
        public void BalanceMasscon()
        {
            double totalMasscon = this.part.Resources["Masscon"].amount;
            double massconPerPart = 0f;

            if (this.part.symmetryCounterparts.Count == 0)
                return;

            //Get the total amount of masscon
            foreach (Part symmetryPart in this.part.symmetryCounterparts)
                totalMasscon += symmetryPart.Resources["Masscon"].amount;

            //Now divide it up between the symmetry parts.
            massconPerPart = totalMasscon / this.part.symmetryCounterparts.Count;
            foreach (Part symmetryPart in this.part.symmetryCounterparts)
                symmetryPart.Resources["Masscon"].amount = massconPerPart;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
        }

        public override string GetInfo()
        {
            return "Use the action menu to add or remove Masscon from the module.";
        }

        public void OnGUI()
        {
        }
    }
}
