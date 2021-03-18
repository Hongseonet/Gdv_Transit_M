using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using System.Text;

public class SqliteMgr : SingletonMgr<SqliteMgr>
{
    //Table in : AresStatus, Contents
    string dbPath = "";
    string dbFileName = "/InOut.db"; //@"/innerData.db";


    public void Init()
    {
        dbPath = Application.persistentDataPath +  dbFileName;

        if (!PlayerPrefs.HasKey("dbPath"))
            PlayerPrefs.SetString("dbPath", dbPath);

        Common.GetInstance.PrintLog('w', "SqliteMgr", dbPath);

        //check DB file exist
        //Debug.LogWarning("copy db files");
        if (ConstValue.CLEARDB && File.Exists(dbPath)) //remove db file on dev mode
            File.Delete(dbPath);

        if (!File.Exists(dbPath))
        {
            BetterStreamingAssets.Initialize();
            byte[] readByte = BetterStreamingAssets.ReadAllBytes(dbFileName);
            File.WriteAllBytes(dbPath, readByte);
        }
    }

    
    public void InsertData(string tableName, List<string> columns, List<string> data)
    {
        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                if (columns != null)
                {
                    StringBuilder sqlColumn = new StringBuilder();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        string addComme = "";
                        if (i != (columns.Count - 1))
                            addComme = ", ";
                        sqlColumn.Append("\'" + columns[i] + "\'" + addComme);
                    }
                }

                StringBuilder newData = new StringBuilder();
                for (int i = 0; i < data.Count; i++)
                {
                    string addComme = "";
                    if (i != (data.Count - 1))
                        addComme = ", ";
                    newData.Append("\'" + data[i] + "\'" + addComme);
                }

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Insert into " + tableName + " values (" + newData.ToString() + ")";
                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            conn.Close();
            conn.Dispose();
        }
    }

    public void InsertData(string query)
    {
        Common.GetInstance.PrintLog('w', "InsertData", query);

        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();
            Debug.Log("get query : " + query);

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            conn.Close();
            conn.Dispose();
        }
    }

    public string ReadData(string query)
    {
        if (string.IsNullOrEmpty(query))
            return null;

        Common.GetInstance.PrintLog('d', "ReadData", query);
        string rtnData = "";

        //idb connection
        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                SqliteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    rtnData = reader.GetString(0);

                reader.Close();
                cmd.Dispose();
            }
            conn.Close();
            conn.Dispose();
        }
        return rtnData;
    }

    public List<string> ReadData(string query, int columnCnt)
    {
        if (string.IsNullOrEmpty(query))
            return null;

        Debug.Log("get query : " + query);
        List<string> rtnData = new List<string>();

        //idb connection
        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                SqliteDataReader reader = cmd.ExecuteReader();
                int cnt = 0;

                while (reader.Read())
                {
                    cnt++;
                    StringBuilder tmpStr = new StringBuilder();

                    for (int i = 0; i < columnCnt; i++)
                        if (reader.IsDBNull(i))
                            tmpStr.Append("" + '\\');
                        else
                        {
                            if(i == columnCnt - 1)
                                tmpStr.Append(reader.GetString(i));
                            else
                                tmpStr.Append(reader.GetString(i) + '\\');
                        }
                    //Debug.Log("dbData : " + tmpStr.ToString());
                    rtnData.Add(tmpStr.ToString());
                }

                if (cnt == 0)
                    rtnData.Clear();
                reader.Close();
                cmd.Dispose();
            }
            conn.Close();
            conn.Dispose();
        }
        return rtnData;
    }

    public List<string> ReadRowProperties(string query) //for PRAGMA
    {
        if (string.IsNullOrEmpty(query))
            return null;

        List<string> rtnData = new List<string>();

        //idb connection
        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                SqliteDataReader reader = cmd.ExecuteReader();
                int cnt = 0;

                while (reader.Read())
                {
                    cnt++;
                    StringBuilder tmpStr = new StringBuilder();

                    tmpStr.Append(reader.GetString(1) + '\\');

                    if (ConstValue.ISDEV)
                        Debug.Log("dbData : " + tmpStr.ToString());
                    rtnData.Add(tmpStr.ToString());
                }

                if (cnt == 0)
                    rtnData.Clear();

                reader.Close();
                cmd.Dispose();
            }
            conn.Close();
            conn.Dispose();
        }
        return rtnData;
    }

    public List<byte[]> ReadData(string query, string colName) //for image
    {
        if (string.IsNullOrEmpty(query))
            return null;

        Debug.Log("receive : " + query);
        List<byte[]> rtnData = new List<byte[]>();

        //idb connection
        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                IDataReader reader = cmd.ExecuteReader();
                int cnt = 0;

                while (reader.Read())
                {
                    cnt++;
                    byte[] img = (byte[])reader[colName];
                    rtnData.Add(img);
                }

                if (cnt == 0)
                    rtnData.Clear();
                cmd.Dispose();
                reader.Close();
            }
            conn.Close();
            conn.Dispose();
        }
        return rtnData;
    }

    public void UpdateData(string query)
    {
        Common.GetInstance.PrintLog('w', "UpdateData", query);

        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            conn.Close();
            conn.Dispose();
        }
    }

    public List<string> GetTableColumn(string tableName) {
        //SELECT c.name FROM pragma_table_info('MyWork') c;
        List<string> rtnData = new List<string>();

        //idb connection
        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select c.name From pragma_table_info(\'" + tableName + "\') c";

                SqliteDataReader reader = cmd.ExecuteReader();
                int cnt = 0;

                while (reader.Read())
                {
                    cnt++;
                    rtnData.Add(reader.GetString(0));

                    if (ConstValue.ISDEV)
                        Debug.Log("dbData : " + reader.GetString(0) + " / " + reader.GetString(1));
                }

                if (cnt == 0)
                    rtnData.Clear();
                
                cmd.Dispose();
            }
            conn.Close();
            conn.Dispose();
        }
        return rtnData;
    }

    public void RemoveRows(string query)
    {
        Common.GetInstance.PrintLog('w', "RemoveRows", query);

        using (SqliteConnection conn = new SqliteConnection("URI=file:" + PlayerPrefs.GetString("dbPath")))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            conn.Close();
            conn.Dispose();
        }
        //Debug.LogWarning("all rows removed");
    }



    void DisconnectSqlite(SqliteConnection conn, SqliteCommand cmd)
    {
        cmd.Dispose();
        conn.Close();
        conn.Dispose();
    }
}