using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
namespace snaps.wms.api.report.Data
{
    public class Connector
    {
        public Connector() { }
        public Connector(string _connectionString)
        {
            ConnectonString = _connectionString;
        }
        public string ConnectonString { get; set; }

        /// <summary>
        /// Get Single Data Value
        /// </summary>
        /// <param name="commandText">select top 1 Id from table where id=@Id</param>
        /// <param name="parameters">new KeyValuePair<string, object>("@Id", 5)</param>
        /// <returns>integer</returns>
        public int GetInt32(string commandText, params KeyValuePair<string, object>[] parameters)
        {
            int result = -1;
            using (SqlConnection connection = new SqlConnection(ConnectonString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            command.Parameters.AddWithValue(parameter.Key, parameter.Value);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                            if (dataReader.HasRows)
                                while (dataReader.Read())
                                    result = dataReader.GetInt32(0);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Get decimal Data Value
        /// </summary>
        /// <param name="commandText">select top 1 price from table where id=@Id</param>
        /// <param name="parameters">new KeyValuePair<string, object>("@Id", 5)</param>
        /// <returns>decimal</returns>
        public decimal GetDecimal(string commandText, params KeyValuePair<string, object>[] parameters)
        {
            decimal result = -1;

            using (SqlConnection connection = new SqlConnection(ConnectonString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            command.Parameters.AddWithValue(parameter.Key, parameter.Value);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                            if (dataReader.HasRows)
                                while (dataReader.Read())
                                    result = dataReader.GetDecimal(0);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
            return result;
        }


        /// <summary>
        /// Get string Data Value
        /// </summary>
        /// <param name="commandText">select top 1 name from table where id=@Id</param>
        /// <param name="parameters">new KeyValuePair<string, object>("@Id", 5)</param>
        /// <returns>string</returns>
        public string GetString(string commandText, params KeyValuePair<string, object>[] parameters)
        {
            string result = string.Empty;

            using (SqlConnection connection = new SqlConnection(ConnectonString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            command.Parameters.AddWithValue(parameter.Key, parameter.Value);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                            if (dataReader.HasRows)
                                while (dataReader.Read())
                                    result = dataReader.GetString(0);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
            return result;
        }
        /// <summary>
        /// Get DataTable Object
        /// </summary>
        /// <param name="commandText">select * from table where category=@cat</param>
        /// <param name="parameters">new KeyValuePair<string, object>("@cat", 5)</param>
        /// <returns>System.Data.DataTable</returns>
        public DataTable GetDataTable(string commandText, params KeyValuePair<string, object>[] parameters)
        {
            DataTable result = new DataTable();
            using (SqlConnection connection = new SqlConnection(ConnectonString))
            {
                try
                {
                    connection.Open();
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(commandText, connection))
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            dataAdapter.SelectCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);

                        dataAdapter.Fill(result);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Execute Sql Command ExecuteNonQuery
        /// </summary>
        /// <param name="commandText">insert into table value(1,@name)</param>
        /// <param name="parameters">new KeyValuePair<string, string>("@name", "sample")</param>
        /// <returns>true , false</returns>
        public bool Execute(string commandText, params KeyValuePair<string, object>[] parameters)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(ConnectonString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            command.Parameters.AddWithValue(parameter.Key, parameter.Value);

                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
            return result;
        }

    }
}
