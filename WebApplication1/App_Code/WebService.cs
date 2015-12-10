using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Web.Script.Services;
using System.Web.Script.Serialization;


/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "AnyNameSpace")]

[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    public WebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    public DataTable GetDataTable()
    {
        DataTable dt = new DataTable();
        using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=|DataDirectory|Site.mdb"))
        {
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Site";

            cmd.CommandType = CommandType.Text;

            if (conn.State != ConnectionState.Open)
                conn.Open();

            OleDbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            dt.Load(dr);
        }
        return dt;
    }


    public String ConvertDataTableTojSonString(DataTable dataTable)
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer =
               new System.Web.Script.Serialization.JavaScriptSerializer();

        List<Dictionary<String, Object>> tableRows = new List<Dictionary<String, Object>>();

        Dictionary<String, Object> row;

        foreach (DataRow dr in dataTable.Rows)
        {
            row = new Dictionary<String, Object>();
            foreach (DataColumn col in dataTable.Columns)
            {
                row.Add(col.ColumnName, dr[col]);
            }
            tableRows.Add(row);
        }
        return serializer.Serialize(tableRows);
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void ListarSites()
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        Context.Response.Clear();
        Context.Response.ContentType = "application/json";
        ListarSitesData data = new ListarSitesData();
        data.Message = ConvertDataTableTojSonString(GetDataTable());
        Context.Response.Write(js.Serialize(data.Message));
    }
    public class ListarSitesData
    {
        public String Message;
    }
}
