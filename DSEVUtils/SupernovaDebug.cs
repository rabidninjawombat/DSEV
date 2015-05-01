using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace WildBlueIndustries
{
    public class SupernovaDebug : Window<SupernovaDebug>
    {
        public SupernovaController supernovaController;

        public SupernovaDebug() :
        base("Supernova Debug Menu", 300, 300)
        {
            Resizable = false;
        }

        protected override void DrawWindowContents(int windowId)
        {
            GUILayout.BeginVertical();

            supernovaController.requiresECToStart = GUILayout.Toggle(supernovaController.requiresECToStart, "EC required to start");

            if (GUILayout.Button("Debug Reset"))
                supernovaController.DebugReset();

            if (GUILayout.Button("Charge Capacitor"))
                supernovaController.currentElectricCharge = supernovaController.ecNeededToStart;
            GUILayout.EndVertical();
        }
    }
}
