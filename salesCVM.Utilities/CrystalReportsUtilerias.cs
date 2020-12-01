﻿using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salesCVM.Utilities
{
    public class CrystalReportsUtilerias
    {
        public static void RecurseAndRemap(ReportDocument CR, string servidor, string baseDeDatos, string usuario, string contrasenia)
        {
            for (int i = 0; i < CR.DataSourceConnections.Count; i++)
            {
                CR.DataSourceConnections[i].SetLogon(usuario, contrasenia);
                CR.DataSourceConnections[i].SetConnection(servidor, baseDeDatos, false);
            }

            CR.SetDatabaseLogon(usuario, contrasenia);

            for (int i = 0; i < CR.Database.Tables.Count; i++)
            {
                CR.Database.Tables[i].LogOnInfo.ConnectionInfo.UserID = usuario;
                CR.Database.Tables[i].LogOnInfo.ConnectionInfo.Password = contrasenia;

            }

            if (CR.IsSubreport)
            {
                for (int i = 0; i < CR.Subreports.Count; i++)
                {
                    RecurseAndRemap(CR.Subreports[i], servidor, baseDeDatos, usuario, contrasenia);
                }
            }
        }
    }
}
