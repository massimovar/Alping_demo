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
#endregion

public class TasksTest : BaseNetLogic
{
    private PeriodicTask periodicTask;
    private LongRunningTask longRunningTask;
    private DelayedTask delayedTask;
    private FTOptix.Modbus.Tag on4;
    private RemoteVariableSynchronizer remoteVariableSynchronizer;

    public override void Start()
    {
        on4 = Project.Current.Get<FTOptix.Modbus.Tag>("CommDrivers/ModbusDriver1/ModbusStation1/Tags/On4");

        remoteVariableSynchronizer = new RemoteVariableSynchronizer(new TimeSpan(0, 0, 4));
        remoteVariableSynchronizer.Add(on4);
        remoteVariableSynchronizer.Remove(on4);

        var screen = (Screen)Owner;
        Log.Info("Screen" + Owner.BrowseName + " started");

        periodicTask = new PeriodicTask(ReadTag, 10000, LogicObject);
        periodicTask.Start();

        longRunningTask = new LongRunningTask(CalculateSomething, LogicObject);
        longRunningTask.Start();

        delayedTask = new DelayedTask(WriteTag, 3000, LogicObject);
        delayedTask.Start();
    }

    private void WriteTag()
    {
        on4.RemoteWrite(true);
    }

    private void CalculateSomething()
    {
        Log.Info("Calculating something");
    }

    public override void Stop()
    {
        Log.Info("RuntimeNetLogic2", "Screen" + Owner.BrowseName + " stopped");
        periodicTask.Dispose();
        longRunningTask.Dispose();
        delayedTask.Dispose();
        remoteVariableSynchronizer.Dispose();
    }

    private void ReadTag()
    {
        Log.Info(on4.BrowseName + " value from filed: " + on4.RemoteRead());
    }
}
