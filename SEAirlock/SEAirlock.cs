﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VRageMath;
using VRage;

using Sandbox.Common;
using Sandbox.Common.Components;
//using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game;



namespace Scripts
{

    
class StatusReport
{
    IMyGridTerminalSystem GridTerminalSystem;

    List<string> airventObjects = new List<string>() 
{ 
    "outsideDoor",  
    "insideDoor", 
    "outsideLight", 
    "insideLight", 
    "middleLight", 
    "airvent" 
};


    public class AirlockObjects
    {
        public IMyDoor outsideDoorBlock;
        public IMyDoor insideDoorBlock;
        public IMyInteriorLight outsideLight;
        public IMyInteriorLight insideLight;
        public IMyInteriorLight middleLight;
        public IMyAirVent airvent;

        public AirlockObjects() { }
    }

    public class Airlock
    {
        public string name;
        public AirlockObjects airlock;
        ITerminalAction closeDoor;
        ITerminalAction openDoor;
        ITerminalAction disableDoor;
        ITerminalAction enableDoor;
        Color red = new Color(255, 0, 0);
        Color green = new Color(0, 255, 0);

        public Airlock()
        {
         
        }

        public void AirlockInitObject(string _airlockName, AirlockObjects _airlock)
        {
            this.name = _airlockName;
            this.airlock = _airlock;

            closeDoor = this.airlock.outsideDoorBlock.GetActionWithName("Open_Off");
            openDoor = this.airlock.outsideDoorBlock.GetActionWithName("Open_On");
            disableDoor = this.airlock.outsideDoorBlock.GetActionWithName("OnOff_Off");
            enableDoor = this.airlock.outsideDoorBlock.GetActionWithName("OnOff_On");
        }

        public void enterAirlockFromInside()
        {

            closeDoor.Apply(this.airlock.outsideDoorBlock);
            disableDoor.Apply(this.airlock.outsideDoorBlock);
            openDoor.Apply(this.airlock.insideDoorBlock);
            airlock.outsideLight.SetValue("Color", red);
            airlock.middleLight.SetValue("Color", red);
            //   disableDoor.Apply(this.airlock.middleLight); 
            airlock.insideLight.SetValue("Color", green);

        }

    }


    void collectObjectsForAirvent(String blockName, AirlockObjects airlock)
    {
        string airVentName = blockName.Substring(0, 8);
        List<IMyTerminalBlock> tmpObjects = new List<IMyTerminalBlock>();
        Echo(airVentName);
        for (int j = 0; j < airventObjects.Count; j++)
        {

            IMyTerminalBlock tmp = GridTerminalSystem.GetBlockWithName(airVentName + airventObjects[j]) as IMyTerminalBlock;
            Echo(airVentName + airventObjects[j]);
            tmpObjects.Add(tmp);
        }

        airlock.outsideDoorBlock = (IMyDoor)tmpObjects[0];
        airlock.insideDoorBlock = (IMyDoor)tmpObjects[1];
        airlock.outsideLight = (IMyInteriorLight)tmpObjects[2];
        airlock.insideLight = (IMyInteriorLight)tmpObjects[3];
        airlock.middleLight = (IMyInteriorLight)tmpObjects[4];
        airlock.airvent = (IMyAirVent)tmpObjects[5];

    }

    
    Airlock[] allAirlocks = new Airlock[10];

    int airlockCount;
    static bool init = false;

    void Main()
    {
        List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(allBlocks);

        List<IMyTerminalBlock> airVentBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyAirVent>(airVentBlocks);


        if (!init)
        {
            Echo("Init was required");

            airlockCount = 0;
            for (int i = 0; i < airVentBlocks.Count; i++)
            {

                string tmpBlockName = airVentBlocks[i].CustomName;
                if (tmpBlockName.StartsWith("Airlock"))
                {
                    AirlockObjects airlockObj = new AirlockObjects();

                    collectObjectsForAirvent(tmpBlockName, airlockObj);

                    Airlock airlock = new Airlock();
                    airlock.AirlockInitObject(tmpBlockName, airlockObj);
                    airlock.name = tmpBlockName;

                    allAirlocks[airlockCount] = airlock;
                    
    
                }
            }
            init = true;
        }
        else
        {
            Echo("Init was NOT required");
        }

        Echo("AirlockCount" + airlockCount);
          for (int i = 0; i < airlockCount; i++) 
          { 
                if (!allAirlocks[i].airlock.insideLight.Enabled)
                {
                    if (allAirlocks[i].airlock.outsideLight.Enabled &&
                        allAirlocks[i].airlock.middleLight.Enabled)
                        allAirlocks[i].enterAirlockFromInside();
                }

           } 
    
         



       
    } 

}
}


