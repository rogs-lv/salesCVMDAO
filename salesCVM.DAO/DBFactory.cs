﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salesCVM.DAO.Util;

namespace salesCVM.DAO
{
    public class DBFactory
    {
        //private static readonly string DB_FACTORY_PROPERTY_URL = @"C:\Users\Oliver\Documents\Visual Studio 2019\Projects\CVMSales\ApiRest\salesCVM\salesCVM.DAO\DBFactory.properties";
        private static readonly string DB_FACTORY_PROPERTY_URL = "./DBFactory.properties";
        private static readonly string DEFAULT_DB_CLASS_PROP = "defaultDBClass";
        public static IDBAdapter GetDefaultAdapter() {
            try
            {
                string defaultDBClass = ConfigurationManager.AppSettings["dbClass"];
                Type type = Type.GetType(defaultDBClass);
                return (IDBAdapter)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                return null;
            }
            //IDictionary<string, string> prop = PropertiesUtil.LoadProperty(DB_FACTORY_PROPERTY_URL);
            //if (prop != null)
            //{
            //    string defaultDBClass = prop[DEFAULT_DB_CLASS_PROP];
            //    Type type = Type.GetType(defaultDBClass);
            //    return (IDBAdapter)Activator.CreateInstance(type);
            //}
            //else {
            //    return null;
            //}
        }
    }
}
