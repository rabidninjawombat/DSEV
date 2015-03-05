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
    public class ModuleSpinHub : ExtendedPartModule
    {
        const float kDefaultRPM = 6.0f;

        [KSPField(isPersistant = true)]
        public float rotationsPerMinute;

        [KSPField(guiActive = true, guiActiveEditor = true)]
        public bool isSpinning;

        public List<Part> attachedParts = new List<Part>();

        protected string spinSection;
        protected Transform spinSectionTransform;

        protected string nonSpinSection;
        protected Transform nonSpinTransform;
//        Part nonSpinPart;

        public void FindAttachedParts()
        {
            Log("FindAttachedParts called");

            //Get the transforms
            nonSpinTransform = this.part.FindModelTransform(nonSpinSection);
            spinSectionTransform = this.part.FindModelTransform(spinSection);
            jointSpin = setupJoint(this.part);

            jointNonSpin = setupJoint(this.part.parent);

            //Clear the existing list.
            attachedParts.Clear();

            //Find the parts attached to the hub.
            foreach (Part part in this.part.children)
            {
                if (part.attachMode == AttachModes.STACK)
                {
//                    jointNonSpin = setupJoint(part);
//                    nonSpinPart = part;
                }

                if (part.attachMode == AttachModes.SRF_ATTACH)
                {
                    attachedParts.Add(part);
                    Log("Added " + part.name);
                }
            }
            Log("Attached parts count: " + attachedParts.Count);
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Toggle Centrifuge", active = true)]
        public void ToggleCentrifuge()
        {
            isSpinning = !isSpinning;

            if (isSpinning)
                FindAttachedParts();
        }

        #region Overrides
        private ConfigurableJoint jointSpin;
        private ConfigurableJoint jointNonSpin;

        public ConfigurableJoint setupJoint(Part partToMod)
        {
            ConfigurableJoint joint;

            joint = partToMod.attachJoint.Joint.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = partToMod.attachJoint.Joint.connectedBody;

            joint.breakForce = 1e15f;
            joint.breakTorque = 1e15f;

            // And to default joint
            part.attachJoint.Joint.breakForce = 1e15f;
            part.attachJoint.Joint.breakTorque = 1e15f;
            part.attachJoint.SetBreakingForces(1e15f, 1e15f);

            // lock all movement by default
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.projectionDistance = 0f;
            joint.projectionAngle = 0f;
            joint.projectionMode = JointProjectionMode.PositionAndRotation;

            // Copy drives
            joint.linearLimit = part.attachJoint.Joint.linearLimit;
            joint.lowAngularXLimit = partToMod.attachJoint.Joint.lowAngularXLimit;
            joint.highAngularXLimit = partToMod.attachJoint.Joint.highAngularXLimit;
            joint.angularXDrive = partToMod.attachJoint.Joint.angularXDrive;
            joint.angularYZDrive = partToMod.attachJoint.Joint.angularYZDrive;
            joint.xDrive = partToMod.attachJoint.Joint.xDrive;
            joint.yDrive = partToMod.attachJoint.Joint.yDrive;
            joint.zDrive = partToMod.attachJoint.Joint.zDrive;

            // Set anchor position
            joint.anchor = joint.rigidbody.transform.InverseTransformPoint(joint.connectedBody.transform.position);
            joint.connectedAnchor = Vector3.zero;

            // Set correct axis
            joint.axis = joint.rigidbody.transform.InverseTransformDirection(joint.connectedBody.transform.right);
            joint.secondaryAxis = joint.rigidbody.transform.InverseTransformDirection(joint.connectedBody.transform.up);

            joint.rotationDriveMode = RotationDriveMode.XYAndZ;
            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;

            // Reset default joint drives
            JointDrive resetDrv = new JointDrive();
            resetDrv.mode = JointDriveMode.PositionAndVelocity;
            resetDrv.positionSpring = 0;
            resetDrv.positionDamper = 0;
            resetDrv.maximumForce = 0;

            partToMod.attachJoint.Joint.angularXDrive = resetDrv;
            partToMod.attachJoint.Joint.angularYZDrive = resetDrv;
            partToMod.attachJoint.Joint.xDrive = resetDrv;
            partToMod.attachJoint.Joint.yDrive = resetDrv;
            partToMod.attachJoint.Joint.zDrive = resetDrv;
            return joint;
        }

        [KSPField(guiActive = true, guiActiveEditor = true)]
        public float rotate;

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (isSpinning == false)
                return;

            //Calcuate degrees per time slice
            float degreesPerTick = rotationsPerMinute;
            if (degreesPerTick == 0f)
                degreesPerTick = kDefaultRPM;
            degreesPerTick = ((degreesPerTick / 60.0f) * TimeWarp.fixedDeltaTime) * 360.0f;

            //Rotate the joint
//            setupJoint(this.part);
            rotate += degreesPerTick;
            if (rotate >= 360f)
                rotate = 0f;
            jointSpin.targetRotation = Quaternion.AngleAxis(rotate, Vector3.up);
//            if (jointNonSpin != null)
//                jointNonSpin.targetRotation = Quaternion.AngleAxis(rotate, Vector3.up);

            //Rotate the hub
            //spinSectionTransform.transform.Rotate(Vector3.forward, -degreesPerTick, Space.Self);
            nonSpinTransform.Rotate(Vector3.forward, degreesPerTick, Space.Self);
        }

        protected override void getProtoNodeValues(ConfigNode protoNode)
        {
            base.getProtoNodeValues(protoNode);

            spinSection = protoNode.GetValue("spinSection");
            nonSpinSection = protoNode.GetValue("nonSpinSection");
        }

        #endregion
    }
}
