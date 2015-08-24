using System;
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
        string name;
        public AirlockObjects airlock;
        ITerminalAction closeDoor;
        ITerminalAction openDoor;
        ITerminalAction disableDoor;
        ITerminalAction enableDoor;
        Color red = new Color(255, 0, 0);
        Color green = new Color(0, 255, 0);

        public Airlock(string _airlockName, AirlockObjects _airlock)
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

    List<Airlock> allAirlocks = new List<Airlock>();
    void Main()
    {
        List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(allBlocks);

        List<IMyTerminalBlock> airVentBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyAirVent>(airVentBlocks);



        for (int i = 0; i < airVentBlocks.Count; i++)
        {

            string tmpBlockName = airVentBlocks[i].CustomName;
            if (tmpBlockName.StartsWith("Airlock"))
            {
                AirlockObjects airlockObj = new AirlockObjects();
                collectObjectsForAirvent(tmpBlockName, airlockObj);
                Airlock airlock = new Airlock(tmpBlockName, airlockObj);

                if (!airlock.airlock.insideLight.Enabled)
                {
                    if (airlock.airlock.outsideLight.Enabled &&
                        airlock.airlock.middleLight.Enabled)
                        Echo("HUHA");
                    airlock.enterAirlockFromInside();
                }

                //            allAirlocks.Add(airlock); 
            }
        }

        /*    for (int i = 0; i < allAirlocks.Count; i++) 
            { 
            } 
    
          */

    } 

}
}


