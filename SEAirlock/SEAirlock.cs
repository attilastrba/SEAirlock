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

    Airlock[] allAirlocks = new Airlock[10];
    static int airlockCount;
    static bool init = false;



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
        const byte Idle = 0;
        const byte RequestAccesFromInisde = 1;
        const byte RequestAccesFromOutside = 2;
        const byte RequestAccesToOutside = 3;
        const byte RequestAccesToInside = 4;

        const byte closeDoor = 0;
        const byte disableDoor = 1;
        const byte pressurize = 2;
        const byte openDoor = 3;
        

        public byte state;
        public byte enterState;

        public string name;
        public AirlockObjects airlock;
        Color red = new Color(255, 0, 0);
        Color green = new Color(0, 255, 0);

        public Airlock()
        {
         
        }

        public void AirlockInitObject(string _airlockName, AirlockObjects _airlock)
        {
            this.name = _airlockName;
            this.airlock = _airlock;
        }

        public void checkAirlockState()
        {

            if (!airlock.insideLight.Enabled && state == Idle)
            {
                state = RequestAccesFromInisde;
                enterState = closeDoor;
            }

            if (!airlock.outsideLight.Enabled && state == Idle)
            {
                state = RequestAccesFromOutside;

            }
        }

        public void provideAirlockActions()
        {
            switch (state)
            {
                case RequestAccesFromInisde: enterAirlockFromInside(); break;
                case RequestAccesFromOutside: enterAirlockFromOutside(); break;
                case RequestAccesToOutside: requestAccessToOutside(); break;
            }

        }

        public void enterAirlockFromOutside()
        { 
        }

        public void requestAccessToOutside()
        {
        }
            
        public void enterAirlockFromInside()
        {
            switch (enterState)
            {
                case closeDoor: 
                    {
                        airlock.outsideDoorBlock.ApplyAction("Open_Off");
                        airlock.middleLight.SetValue("Color", red);
                        if (airlock.outsideDoorBlock.Open)
                        {
                            enterState = disableDoor;
                        }
                        break;
                    }
             t   case disableDoor:
                    {
                        airlock.outsideDoorBlock.ApplyAction("OnOff_Off");                        
                        airlock.airvent.ApplyAction("Depressurize_Off"); 
                        if (airlock.airvent.CanPressurize)
                        {
                            airlock.insideDoorBlock.ApplyAction("OnOff_On");                        
                            enterState = pressurize;
                        }
                        break;
                    }
                case pressurize:
                    {
                        airlock.insideDoorBlock.ApplyAction("Open_On");
                        airlock.outsideLight.SetValue("Color", red);
                        airlock.middleLight.SetValue("Color", green);
                        airlock.insideLight.SetValue("Color", green);
                        airlock.insideLight.ApplyAction("OnOff_On");
                        enterState = openDoor;
                        state = Idle;
                        break;
                    }
                    
            }
                
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

    void initAirlocks()
    {
        List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(allBlocks);

        List<IMyTerminalBlock> airVentBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyAirVent>(airVentBlocks);

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

                allAirlocks[airlockCount++] = airlock;


            }
        }
        
    }
    
    
    void Main()
    {
       
        if (!init)
        {
            Echo("Init was required");
            initAirlocks();
            init = true;
        }

        Echo("AirlockCount" + airlockCount);
          for (int i = 0; i < airlockCount; i++) 
          {
              Echo("State" + allAirlocks[i].state);
              Echo("EnterState" + allAirlocks[i].enterState);

              allAirlocks[i].checkAirlockState();
              allAirlocks[i].provideAirlockActions();

           } 
    
         



       
    } 

}
}


