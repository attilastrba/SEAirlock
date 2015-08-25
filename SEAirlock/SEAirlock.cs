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
    const byte MAX_PRESSURIZATION = 3;


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
        public byte pressurizationCount;
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

        public void checkSimpleAirlock()
        {
            if (airlock.outsideDoorBlock.Open)
            {
                airlock.insideDoorBlock.ApplyAction("OnOff_Off");
                if (airlock.insideLight != null)
                {
                    airlock.insideLight.SetValue("Color", red);
                }
            }
            else
            {
                airlock.insideDoorBlock.ApplyAction("OnOff_On");
                if (airlock.insideLight != null)
                {
                    airlock.insideLight.SetValue("Color", green);
                }

            }

            if (airlock.insideDoorBlock.Open)
            {
                airlock.outsideDoorBlock.ApplyAction("OnOff_Off");
                if (airlock.outsideLight != null)
                {
                    airlock.outsideLight.SetValue("Color", red);
                }

            }
            else
            {
                airlock.outsideDoorBlock.ApplyAction("OnOff_On");
                if (airlock.outsideLight != null)
                {
                    airlock.outsideLight.SetValue("Color", green);
                }

            }


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

            if (airlock.middleLight.BlinkIntervalSeconds != 0f && state == Idle)
            {
                state = RequestAccesToOutside;
                enterState = closeDoor;

            }
            
            if (!airlock.middleLight.Enabled && state == Idle)
            {
                state = RequestAccesToInside;
                enterState = closeDoor;

            }

            if (!airlock.outsideLight.Enabled && state == Idle)
            {
                state = RequestAccesFromOutside;
                enterState = closeDoor;

            }

        }

        public void provideAirlockActions()
        {
            
            switch (state)
            {
                case RequestAccesFromInisde: enterAirlockFromInside(); break;
                case RequestAccesFromOutside: requestAccessToOutside(); break;
                case RequestAccesToOutside: requestAccessToOutside(); break;
                case RequestAccesToInside: enterAirlockFromInside(); break;
            }

        }

    

        
        public void requestAccessToOutside()
        {
            switch (enterState)
            {
                case closeDoor:
                    {
                        enterState = disableDoor;
                        airlock.insideLight.SetValue("Color", red);
                        airlock.outsideLight.SetValue("Color", red);
                        airlock.middleLight.SetValue("Color", red);
                        airlock.insideDoorBlock.ApplyAction("Open_Off");
                        break;
                    }

                case disableDoor:
                    {
                        if (!airlock.insideDoorBlock.Open)
                        {
                            enterState = disableDoor;
                            airlock.insideDoorBlock.ApplyAction("OnOff_Off");
                            airlock.airvent.ApplyAction("Depressurize_On");
                            enterState = pressurize;
                            pressurizationCount = 0;
                        }
                        break;
                    }

                case pressurize:
                    {
                        if (airlock.airvent.GetOxygenLevel() < 0.2f || pressurizationCount > MAX_PRESSURIZATION)
                        {
                            airlock.outsideDoorBlock.ApplyAction("OnOff_On");
                            airlock.middleLight.ApplyAction("DecreaseBlink Interval");
                            airlock.middleLight.SetValue("Color", red);
                            airlock.outsideLight.SetValue("Color", green);
                            airlock.outsideLight.ApplyAction("OnOff_On");
                            airlock.outsideDoorBlock.ApplyAction("Open_On");
                            enterState = closeDoor;
                            state = Idle;                            
                        }
                        pressurizationCount++;
                        break;
                    }
            }
        
        }
            
        public void enterAirlockFromInside()
        {
            switch (enterState)
            {
                case closeDoor: 
                    {
                        airlock.outsideLight.SetValue("Color", red);
                        airlock.outsideDoorBlock.ApplyAction("Open_Off");                        
                        airlock.middleLight.SetValue("Color", red);
                        if (!airlock.outsideDoorBlock.Open)
                        {
                            enterState = disableDoor;
                            airlock.insideDoorBlock.ApplyAction("Open_Off");
                            pressurizationCount = 0;
                        }
                        break;
                    }
                case disableDoor:
                    {
                        airlock.outsideDoorBlock.ApplyAction("OnOff_Off");                        
                        airlock.airvent.ApplyAction("Depressurize_Off");
                        if (airlock.airvent.GetOxygenLevel() > 0.8f || pressurizationCount > MAX_PRESSURIZATION)
                        {
                            airlock.insideDoorBlock.ApplyAction("OnOff_On");                        
                            enterState = pressurize;
                        }
                        pressurizationCount++;
                        break;
                    }
                case pressurize:
                    {
                        airlock.insideDoorBlock.ApplyAction("Open_On");
                        airlock.outsideLight.SetValue("Color", red);
                        airlock.middleLight.SetValue("Color", green);
                        airlock.insideLight.SetValue("Color", green);
                        airlock.insideLight.ApplyAction("OnOff_On");
                        airlock.middleLight.ApplyAction("OnOff_On");
                        enterState = closeDoor;
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
            if (tmp != null)
            {
                Echo(airVentName + airventObjects[j]);
            }
            tmpObjects.Add(tmp);
        }

        airlock.outsideDoorBlock = (IMyDoor)tmpObjects[0];
        airlock.insideDoorBlock = (IMyDoor)tmpObjects[1];
        airlock.outsideLight = (IMyInteriorLight)tmpObjects[2];
        airlock.insideLight = (IMyInteriorLight)tmpObjects[3];
        airlock.middleLight = (IMyInteriorLight)tmpObjects[4];
        airlock.airvent = (IMyAirVent)tmpObjects[5];
        
        

    }

    /* collect all inside doorblocks that start with airvent, these should be unique*/
    void initAirlocks()
    {
        
        List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(allBlocks);

        List<IMyTerminalBlock> doorBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyDoor>(doorBlocks);


        airlockCount = 0;
        for (int i = 0; i < doorBlocks.Count; i++)
        {

            string tmpBlockName = doorBlocks[i].CustomName;
            if (tmpBlockName.StartsWith("Airlock") && tmpBlockName.Substring(8) == "insideDoor")
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
              Echo("Airlock: " + i);
              Echo("--------------");
              Echo("State" + allAirlocks[i].state);
              Echo("EnterState" + allAirlocks[i].enterState);

              if (allAirlocks[i].airlock.outsideLight != null && allAirlocks[i].airlock.insideLight != null
                && allAirlocks[i].airlock.middleLight != null && allAirlocks[i].airlock.airvent != null)
              {
                  Echo("Oxygen:" + allAirlocks[i].airlock.airvent.GetOxygenLevel());

                  allAirlocks[i].checkAirlockState();
                  allAirlocks[i].provideAirlockActions();
              }
              else
              {
                  allAirlocks[i].checkSimpleAirlock();
              }
                Echo("");

            
           } 
    
       
    } 

}
}


