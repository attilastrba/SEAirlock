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
   /* class StatusReport
    {
        IMyGridTerminalSystem GridTerminalSystem;
        //http://steamcommunity.com/sharedfiles/filedetails/?id=360966557
        void Main()
        {
            List<IMyTerminalBlock> blocks;
 
            blocks = new List <IMyTerminalBlock>();
            //GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(blocks);
            GridTerminalSystem.GetBlocksOfType<IMyDoor>(blocks);
            if (blocks.Count == 0) return;
            
            //IMyRadioAntenna antenna = blocks[0] as IMyRadioAntenna;
            IMyDoor airlock = blocks[0] as IMyDoor;

            if airlock.
 
            antenna.SetCustomName("Hello Galaxy!");
        }
    }*/


    
class StatusReport
{
    IMyGridTerminalSystem GridTerminalSystem;

public class Airlock
{
    string name;
    IMyDoor outsideDoorBlock;
    IMyDoor insideDoorBlock;
    IMyInteriorLight outsideLight;
    IMyInteriorLight insideLight;
    IMyInteriorLight middleLight;
    IMyAirVent airvent;

    Color red = new Color(255, 0, 0);
    Color green = new Color(0, 255, 0);

    public Airlock(string airlockName,
                    IMyDoor _outsideDoorBlock,
                    IMyDoor _insideDoorBlock,
                    IMyInteriorLight _outsideLight,
                    IMyInteriorLight _insideLight,
                    IMyInteriorLight _middleLight,   
                    IMyAirVent _airvent)
    {
        this.name = airlockName;
        this.outsideDoorBlock = _outsideDoorBlock;
        this.insideDoorBlock = _insideDoorBlock;
        this.outsideLight = _outsideLight;
        this.insideLight = _insideLight;
        this.middleLight = _middleLight;
        this.airvent = _airvent;
    }

    public void enterAirlockFromInside()
    {
        ITerminalAction closeDoors = this.outsideDoorBlock.GetActionWithName("Open_Off");
        ITerminalAction openDoors = this.insideDoorBlock.GetActionWithName("Open_On");
        
        closeDoors.Apply(this.outsideDoorBlock);
        openDoors.Apply(this.insideDoorBlock);

        outsideLight.SetValue("Color", red);
        middleLight.SetValue("Color", green);
        insideLight.SetValue("Color", green);

    }

}


void Main()
{
    List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(allBlocks);

    List<IMyTerminalBlock> airVentBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyAirVent>(airVentBlocks);
    

    

    List<string> airventObjects = new List<string>()
    {
	"outsideDoor", 
	"insideDoor",
	"outsideLight",
	"insideLight",
    "middleLight",
    "airvent"
    };

    for (int i = 0; i < airVentBlocks.Count; i++)
    {

        string tmpBlockName = airVentBlocks[i].CustomName;
        if (tmpBlockName.StartsWith("Airlock"))
        {
            string airVentName = tmpBlockName.Substring(0, 8);
            List<IMyTerminalBlock> tmpObjects = new List<IMyTerminalBlock>();
            Echo(airVentName);
            for (int j = 0; j<airventObjects.Count; j++)
            {
                IMyTerminalBlock tmp = GridTerminalSystem.GetBlockWithName(airVentName + airventObjects[j]) as IMyTerminalBlock;
                Echo(airVentName + airventObjects[j]);
                tmpObjects.Add(tmp);
            }

            Airlock testAirlock = new Airlock(airVentName, (IMyDoor)tmpObjects[0], (IMyDoor)tmpObjects[1], (IMyInteriorLight)tmpObjects[2], (IMyInteriorLight)tmpObjects[3], (IMyInteriorLight)tmpObjects[4], (IMyAirVent)tmpObjects[5]);
            testAirlock.enterAirlockFromInside();
        }
    }
    
    
}

}
}



/*
  List<IMyTerminalBlock> doorBlocks = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> interriorLights = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyDoor>(doorBlocks);
    GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(interriorLights);
    IMyInteriorLight light = GridTerminalSystem.GetBlockWithName("Airlock1Light") as IMyInteriorLight;

    Color red = new Color(255, 0, 0);
    Color green = new Color(0, 255, 0);
    int doorCount = doorBlocks.Count;
  
 
    for (int i = 0; i < doorCount; i++)
    {
        string tmpDoorName = doorBlocks[i].CustomName;
        if (tmpDoorName.StartsWith("Airlock"))
        {
            //  throw new Exception("HUHA"); 
            ITerminalAction closeDoors = doorBlocks[i].GetActionWithName("Open");
            closeDoors.Apply(doorBlocks[i]);
            light.SetValue("Color", red);
        }
        //ITerminalAction closeDoors = doorBlocks[i].GetActionWithName("Open_Off"); 
        //closeDoors.Apply(doorBlocks[i]); 
    } */