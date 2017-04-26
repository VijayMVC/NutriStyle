using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Npgsql;
using NpgsqlTypes;
using System.Collections;

namespace DynamicConnections.NutriStyle.CRM2011.CreateEntities
{
    public partial class Postgres : Public
    {
        NpgsqlConnection pgConnection;

        //private string errorMessage = null;
                   
        // pgServer
        public string pgServer;
        public string PostgreServer
        {
            get { return pgServer; }
            set { pgServer = value; }
        }
        // pgDatabase
        public string pgDatabase;
        public string PostgreDatabase
        {
            get { return pgDatabase; }
            set { pgDatabase = value; }
        }
        // pgUserId
        public string pgUserId;
        public string PostgreUserId
        {
            get { return pgUserId; }
            set { pgUserId = value; }
        }
        // pgPassword
        public string pgPassword;
        public string PostgrePassword
        {
            get { return pgPassword; }
            set { pgPassword = value; }
        }
        // pgPort
        public string pgPort;
        public string PostgrePort
        {
            get { return pgPort; }
            set { pgPort = value; }
        }
        // pgConnectionString
        public string pgConnectionString;
        public string PostgreConnectionString
        {
            get { return pgConnectionString; }
            set { pgConnectionString = value; }
        }
        // default connection string parameters
        public Postgres()
        {
            // Setup Postgres connection
            pgServer = ConfigurationManager.AppSettings["PostgreServer"];
            pgDatabase = ConfigurationManager.AppSettings["PostgreDatabase"];
            pgUserId = ConfigurationManager.AppSettings["PostgreUserId"];
            pgPassword = ConfigurationManager.AppSettings["PostgrePassword"];
            pgPort = ConfigurationManager.AppSettings["PostgrePort"];           
        }
       
        private void postgreConnectionString()
        {
            try
            {
                pgConnectionString = "Server=" + pgServer +
                                                "Database=" + pgDatabase +
                                                "User Id=" + pgUserId +
                                                "Password=" + pgPassword +
                                                "Port=" + pgPort;
            }
            catch (Exception ex)
            {
                // write to the log file...
            }
        } // end - buildConnectionString

        private void postgreOpenConnection()
        {
            try
            {
                pgConnection = new NpgsqlConnection(pgConnectionString);
                pgConnection.Open();
            }
            catch (Exception ex)
            {
                // write to the log file...
            }
        }

        private void postgreCloseConnection()
        {
            try
            {
                if (pgConnection != null)
                {
                    pgConnection.Close();
                    pgConnection.Dispose();
                }
            }
            catch (Exception ex)
            {
                // write to the log file...
            }
        }

        public void createEntityMetada(string[] arrayTable)
        {
            try
            {
                // buils porstgre connection string
                postgreConnectionString();

                // open porstgre connection object
                postgreOpenConnection();

                getTableMetadataEntityMetadata(arrayTable);

                // close porstgre connection object
                postgreCloseConnection();
            }
            catch (Exception ex)
            {
                // write to the log file...
            }
        }
       
        private void getTableMetadataEntityMetadata(string[] arrayTable)
        {            
            var trackList = new List<Track>();            
          
            for (int i = 0; i < arrayTable.Length; i++)
            {
               string tableName = arrayTable[i];
            
                string sqlScript = "select * from " + tableName;
                // set the command object
                NpgsqlCommand pgCommand = new NpgsqlCommand(sqlScript, pgConnection);

                // set the data object reader
                NpgsqlDataReader pgDataReader = pgCommand.ExecuteReader();

                // set the data tabe schema
                DataTable dataTableSchema = pgDataReader.GetSchemaTable();

                //int rowLoop = 0;

                foreach (DataRow row in dataTableSchema.Rows)
                {
                    string columnName = string.Empty;
                    string dataType = string.Empty;
                    string columnSize = string.Empty;

                    //int colLoop = 0;

                    foreach (DataColumn col in dataTableSchema.Columns)
                    {
                        //System.Diagnostics.Debug.WriteLine(col.ColumnName + " = " + row[col]); 
                        
                        if (col.ColumnName == "ColumnName")
                        {
                            columnName = row[col].ToString();
                        }
                        else if (col.ColumnName == "DataType")
                        {
                            dataType = row[col].ToString();
                        }
                        else if (col.ColumnName == "ColumnSize")
                        {
                            columnSize = row[col].ToString();
                        }
                    }// foreach col

                    //add to the list
                    trackList.Add(new Track
                    {
                        tableName = tableName,
                        columnName = columnName,
                        dataType = dataType,
                        columnSize = columnSize
                    });
                                       
                }// foreach row

                if (pgDataReader != null)
                {
                    pgDataReader.Close();
                    pgDataReader.Dispose();
                }

                if (pgCommand != null)
                {
                    pgCommand.Dispose();
                }

            } // for loop arrayTable 

            //for (int i = 0; i < trackList.Count(); i++)
            //{
            //    System.Diagnostics.Debug.WriteLine(trackList[i].tableName);
            //    System.Diagnostics.Debug.WriteLine(trackList[i].columnName);
            //    System.Diagnostics.Debug.WriteLine(trackList[i].dataType);
            //    System.Diagnostics.Debug.WriteLine(trackList[i].columnSize);
            //}          

            // create new entity process
            using (CreateEntity dcCreateEntity = new CreateEntity())
            {
                //dcCreateEntity.createEntity(arrayTable, trackList);
            }            
            
        }      

    }

    public partial class Track
    {
        public string tableName { get; set; }
        public string columnName { get; set; }
        public string dataType { get; set; }
        public string columnSize { get; set; }
    }

}
