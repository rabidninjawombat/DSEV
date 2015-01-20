using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WildBlueIndustries
{
    public class ModuleEnginesFXWBI : ModuleEnginesFX
    {
        private float _lastCurrentThrottle = -1;

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            KSPParticleEmitter[] emitters = part.GetComponentsInChildren<KSPParticleEmitter>();

            if (emitters == null)
                return;

            if (_lastCurrentThrottle == currentThrottle)
                return;

            foreach (KSPParticleEmitter emitter in emitters)
            {
                //If the emitter is on the list then show it
                if (emitter.name == this.engineID)
                {
                    if (currentThrottle > 0)
                    {
                        emitter.emit = true;
                        emitter.enabled = true;
                    }
                    else
                    {
                        emitter.emit = false;
                        emitter.enabled = false;
                    }
                }

                //Emitter is not on the list, hide it.
                else
                {
                    emitter.emit = false;
                    emitter.enabled = false;
                }
            }
        }
    }
}
