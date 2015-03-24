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
    public class ModuleRCSArcJet : ModuleRCS
    {
        private const float maxSoundDistance = 10.0f;

        [KSPField(isPersistant = true)]
        public string rcsEffectName;

        [KSPField(isPersistant = true)]
        public string rcsID;

        [KSPField(isPersistant = true)]
        public string soundFilePath;

        public bool isRCSOn = false;

        protected FixedUpdateHelper fixedUpdateHelper;
        protected KSPParticleEmitter[] emitters;

        public FXGroup soundClip = null;
        protected bool soundIsPlaying = false;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //Create fixed update helper
            fixedUpdateHelper = this.part.gameObject.AddComponent<FixedUpdateHelper>();
            fixedUpdateHelper.onFixedUpdateDelegate = OnUpdateFixed;
            fixedUpdateHelper.enabled = true;

            //Get the emitters
            emitters = this.part.GetComponentsInChildren<KSPParticleEmitter>();

            //Load the sound clip
            if (string.IsNullOrEmpty(soundFilePath) == false)
                LoadSoundFX();
        }

        public void LoadSoundFX()
        {
            if (!GameDatabase.Instance.ExistsAudioClip(soundFilePath))
                return;

            soundClip.audio = part.gameObject.AddComponent<AudioSource>();
            soundClip.audio.volume = GameSettings.SHIP_VOLUME;
            soundClip.audio.maxDistance = maxSoundDistance;

            soundClip.audio.clip = GameDatabase.Instance.GetAudioClip(soundFilePath);
            soundClip.audio.loop = true;

            soundClip.audio.rolloffMode = AudioRolloffMode.Logarithmic;
            soundClip.audio.panLevel = 1f;
            soundClip.audio.dopplerLevel = 0f;

            soundClip.audio.playOnAwake = false;
        }

        public void OnUpdateFixed()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            //If RCS isn't activated, then just return.
            if (this.part.vessel.ActionGroups[KSPActionGroup.RCS] == false)
                return;

            //Hide the default RCS thruster effects
            isRCSOn = false;
            foreach (FXGroup thrusterFX in this.thrusterFX)
            {
                //If at least one RCS FX power is > 0, then it means the thruster is firing.
                //Power will be 0 if the thruster isn't firing.
                if (thrusterFX.Power > 0f)
                    isRCSOn = true;

                //Set power to zero so that the built-in thruster FX won't show.
                thrusterFX.Power = 0f;
            }

            //Light up the emitters
            foreach (KSPParticleEmitter emitter in emitters)
            {
                if (emitter.name.Contains(rcsEffectName))
                {
                    emitter.emit = isRCSOn && rcsEnabled;
                    emitter.enabled = isRCSOn && rcsEnabled;
                }
            }

            //Play sound
            if (soundClip != null)
            {
                if (isRCSOn && rcsEnabled && soundIsPlaying == false)
                {
                    soundClip.audio.Play();
                    soundIsPlaying = true;
                }
                else if (isRCSOn == false)
                {
                    soundClip.audio.Stop();
                    soundIsPlaying = false;
                }
            }

        }
    }
}
