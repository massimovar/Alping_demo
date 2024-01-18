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
using System.Linq;
#endregion

public class DesignTimeNetLogic1 : BaseNetLogic
{
    [ExportMethod]
    public void TestMe()
    {
        Log.Info("it works!");
    }

    [ExportMethod]
    public void GenerateAlarmsFromComDriver()
    {
        try
        {
            var alpingAlarms = "AlpingAlarmsFromComDriver";
            var alarmDir = Project.Current.Get<Folder>("Alarms");
            if (alarmDir == null) return;

            var alpingAlarmsFolder = alarmDir.Get<Folder>(alpingAlarms);
            if (alpingAlarmsFolder == null)
            {
                alpingAlarmsFolder = InformationModel.Make<Folder>(alpingAlarms);
                alarmDir.Add(alpingAlarmsFolder);
            }

            alpingAlarmsFolder.Children.Clear();
            for (int i = 1; i <= 3; i++) {
                var dAl = InformationModel.Make<DigitalAlarm>("alpingDAl_" + i);
                var modbusTag = Project.Current.Get<FTOptix.Modbus.Tag>("CommDrivers/ModbusDriver1/ModbusStation1/Tags/On" + i);
                dAl.InputValueVariable.SetDynamicLink(modbusTag, DynamicLinkMode.ReadWrite);

                alpingAlarmsFolder.Add(dAl);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }

    [ExportMethod]
    public void GenerateAlarmsFromModel()
    {
        var modelFolder = Project.Current.Get<Folder>("Model");
        var alarmsBit = InformationModel.Make<Folder>("AlarmsBits");
        modelFolder.Add(alarmsBit);

        for (int i = 1; i <= 100; i++)
        {
            var bV = InformationModel.MakeVariable("bVar_"+i, OpcUa.DataTypes.Boolean);
            alarmsBit.Add(bV);
        }

        var alarmDir = Project.Current.Get<Folder>("Alarms");
        var alpingAlarmsFolder = InformationModel.Make<Folder>("AlpingAlarmsFromModel");
        alarmDir.Add(alpingAlarmsFolder);

        foreach (IUAVariable b in alarmsBit.Children)
        {
            var dAl = InformationModel.Make<DigitalAlarm>("alpingDAl_" + b.BrowseName);
            dAl.InputValueVariable.SetDynamicLink(b, DynamicLinkMode.ReadWrite);
            alpingAlarmsFolder.Add(dAl);
        }

    }
}
