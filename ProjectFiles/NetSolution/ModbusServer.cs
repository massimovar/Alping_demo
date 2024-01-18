#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.WebUI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.Recipe;
using FTOptix.DataLogger;
using FTOptix.EventLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
using FTOptix.S7TCP;
using FTOptix.Modbus;
using FTOptix.CODESYS;
using FTOptix.Retentivity;
using FTOptix.Alarm;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using EasyModbus;
using System.Threading.Tasks;
#endregion

public class ModbusServer : BaseNetLogic
{
    PeriodicTask myTask;
    EasyModbus.ModbusServer modbusServer;

    public override void Start()
    {
        modbusServer = new EasyModbus.ModbusServer();
        //modbusServer.LocalIPAddress = System.Net.IPAddress.Parse("127.0.0.1");
        //modbusServer.Port = 502;
        modbusServer.Listen();
        modbusServer.HoldingRegistersChanged += new EasyModbus.ModbusServer.HoldingRegistersChangedHandler(HoldingRegistersChanged);
        myTask = new PeriodicTask(MB_Server_Task, 100, LogicObject);
        myTask.Start();
    }

    public void HoldingRegistersChanged(int startingAddress, int quantity)
    {
        switch (startingAddress)
        {
            case 1:
                Project.Current.GetVariable("Model/ExposeWithModbusServer/Variable1").Value = modbusServer.holdingRegisters[1];
                break;
            case 2:
                Project.Current.GetVariable("Model/ExposeWithModbusServer/Variable2").Value = modbusServer.holdingRegisters[2];
                break;
            case 3:
                Project.Current.GetVariable("Model/ExposeWithModbusServer/Variable3").Value = modbusServer.holdingRegisters[3];
                break;
            default:
                break;
        }
    }

    public override void Stop()
    {
        myTask.Dispose();
        modbusServer.HoldingRegistersChanged -= new EasyModbus.ModbusServer.HoldingRegistersChangedHandler(HoldingRegistersChanged);
    }

    private void MB_Server_Task()
    {
        modbusServer.holdingRegisters[1] = Project.Current.GetVariable("Model/ExposeWithModbusServer/Variable1").Value;
        modbusServer.holdingRegisters[2] = Project.Current.GetVariable("Model/ExposeWithModbusServer/Variable2").Value;
        modbusServer.holdingRegisters[3] = Project.Current.GetVariable("Model/ExposeWithModbusServer/Variable3").Value;
    }
}
