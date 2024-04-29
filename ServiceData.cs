using Genesyst.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    public class ServiceData
    {
        public SqlConnection conn;

        public ServiceData()
        {

        }

        public ServiceData(string connectionString)
        {
            this.conn = new SqlConnection(connectionString);
        }

        public DataTable EntityToDataTable(IQueryable result, ObjectContext ctx)
        {
            try
            {
                EntityConnection conn = ctx.Connection as EntityConnection;
                using (SqlConnection SQLCon = new SqlConnection(conn.StoreConnection.ConnectionString))
                {
                    ObjectQuery query = result as ObjectQuery;
                    using (SqlCommand Cmd = new SqlCommand(query.ToTraceString(), SQLCon))
                    {
                        foreach (var param in query.Parameters)
                        {
                            Cmd.Parameters.AddWithValue(param.Name, param.Value);
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(Cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                return dt;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public int SQLExecuteQuery(string sql_text)
        {
            int Res = 0;
            try
            {
                this.conn.Open();
                SqlTransaction transaction;
                transaction = this.conn.BeginTransaction(IsolationLevel.ReadCommitted);
                SqlCommand cmd = new SqlCommand(sql_text, this.conn,transaction);
                try
                {
                    Res = cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (SqlException ex)
                    {
                        if (transaction.Connection != null)
                        {
                            Console.WriteLine("An exception of type " + ex.GetType() +
                                " was encountered while attempting to roll back the transaction.");
                        }
                    }

                    Console.WriteLine("An exception of type " + e.GetType() +
                        " was encountered while inserting the data.");
                    Console.WriteLine("Neither record was written to database.");
                }
                finally
                {
                    this.conn.Close();
                }
            }
            catch(Exception ex)
            {
                
            }

            return Res;
        }

        public int SQLExecuteScalar(string sql_text)
        {
            int Res = -1;
            try
            {
                this.conn.Open();

                SqlCommand cmd = new SqlCommand(sql_text,this.conn);
                Res = (Int32)cmd.ExecuteScalar();
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                this.conn.Close();
                this.conn.Dispose();
            }

            return Res;
        }

        public int SQLExecuteScalar(string sql_text,SqlConnection conn)
        {
            int Res = -1;
            try
            {
                if(conn.State!=ConnectionState.Open)
                    conn.Open();

                SqlCommand cmd = new SqlCommand(sql_text, conn);
                Res = (Int32)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return Res;
        }

        public static string SqlTextParams(string sqltext_pattern,List<mdlGStringValue> Params)
        {
            string Res = sqltext_pattern;
            try
            {
                foreach(var param in Params)
                {
                    Res = Res.Replace(param.Key.Trim(), param.Value.Trim());
                }
            }
            catch(Exception ex)
            {
                throw;
            }
            return Res;
        }

        public string SQLSelectionFromFile(string filePath, Dictionary<string, string> AddWhere)
        {
            string Res = "";
            try
            {
                Res = File.ReadAllText(filePath);
                foreach (KeyValuePair<string, string> whereField in AddWhere)
                {
                    Res = Res.Replace(whereField.Key.Trim(), whereField.Value.Trim());
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return Res;
        }

        public string SQLSelectionFromFile(string filePath, Dictionary<string,string> AddWhere,int? first,int? perload)
        {
            string Res = "";
            try
            {
                Res = File.ReadAllText(filePath);
                foreach (KeyValuePair<string, string> whereField in AddWhere)
                {
                    Res = Res.Replace(whereField.Key.Trim(), whereField.Value.Trim());
                }

                if(first != null && perload !=null)
                {
                    int last = first.Value * perload.Value;
                    int row_first = last - (perload.Value - 1);
                    Res = Res.Replace("@first", row_first.ToString());
                    Res = Res.Replace("@last", last.ToString());
                }
            }
            catch(Exception ex)
            {
                throw;
            }

            return Res;
        }

    }
}
