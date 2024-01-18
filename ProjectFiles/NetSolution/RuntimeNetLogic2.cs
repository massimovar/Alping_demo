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

public class RuntimeNetLogic2 : BaseNetLogic
{
    private SQLiteStore sQLiteStore;
    private PeriodicTask periodicTask;

    public override void Start()
    {
        sQLiteStore = (SQLiteStore)Owner;

        periodicTask = new PeriodicTask(AuditBackup, 50000, LogicObject);
        periodicTask.Start();
    }

    private void AuditBackup()
    {
        sQLiteStore.Backup("%PROJECTDIR%/mybackup_" + DateTime.Now.ToString("yyyy_mm_dd_hh_MM_ss.sqlite"));
    }

    public override void Stop()
    {
        periodicTask.Dispose();
    }

    
}
