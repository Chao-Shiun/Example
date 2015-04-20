using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ePro.DBUtility;
using eProAgent.DAL;

public partial class Test_Default3 : System.Web.UI.Page
{
    int pagenumber;
    EnvironmentDB env = SQLDB.getEnviorment("0");

    protected void Page_Load(object sender, EventArgs e)
    {
        Dictionary<string, string> QueryString = new Dictionary<string, string>();
        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(Request["Number"]))
            {
                Number.Text = Request["Number"].ToString();
                QueryString.Add("Number", Request["Number"].ToString());
            }
            if (!string.IsNullOrEmpty(Request["Branch"]))
            {
                Branch.Text = Request["Branch"].ToString();
                QueryString.Add("Branch", Request["Branch"].ToString());
            }
            if (string.IsNullOrEmpty(Request["page"]) || !int.TryParse(Request["page"].ToString(), out pagenumber) || pagenumber < 1)
                pagenumber = 1;
            ucPagination.CPage = pagenumber;

            Query(pagenumber, QueryString);
        }
        else
        {
            ucPagination.CPage = 1;
            if (!Number.Text.Trim().Equals(string.Empty))
                QueryString.Add("Number", Number.Text.Trim());
            if (!Branch.Text.Trim().Equals(string.Empty))
                QueryString.Add("Branch", Branch.Text.Trim());
            Query(1, QueryString);
        }
    }

    /// <summary>
    /// 設定該查詢有多少分頁
    /// </summary>
    /// <param name="sqlstr">查詢字串</param>
    /// <param name="para">條件參數</param>
    /// <param name="pagenumber">目前頁數</param>
    private void SetEndPage(string sqlstr, SqlParameter[] para, int pagenumber)
    {
        using (SqlDataReader dr = (para == null ? SQLDB.ExecuteReader(env, "ePro", sqlstr) : SQLDB.ExecuteReader(env, "ePro", sqlstr, para)))
        {
            dr.Read();
            int count = dr.GetInt32(0) % 10 == 0 ? dr.GetInt32(0) / 10 : (dr.GetInt32(0) / 10) + 1;
            ucPagination.EPage = count;
            if (count <= pagenumber)
                ucPagination.CPage = count;
            dr.Close();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pagenumber">第幾頁</param>
    /// <param name="QueryString">網址的參數</param>
    private void Query(int pagenumber, Dictionary<string, string> QueryString)
    {
        string sqlstr = null;
        List<SqlParameter> para = null;
        List<SqlParameter> CountPara = null;
        string connstr = SQLDB.getConnectionString(DBPublic.getEnvironmentDB(), "ePro");

        if (QueryString.ContainsKey("Number") || QueryString.ContainsKey("Branch"))
        {
            int i = 0;
            para = new List<SqlParameter>();
            CountPara = new List<SqlParameter>();

            sqlstr = string.Format(@"SELECT * FROM(
	                                            SELECT TOP 10 * FROM(
		                                            SELECT top {0} B02002,B02003 FROM B02 
		                                            where B02006='北區' and B02002 is not null 
                                                    and ", 10 * pagenumber);

            //這是用來計算頁數的sql
            string CountSqlStr = string.Format(@"SELECT count(*) FROM B02 
		                                         where B02006='北區' and B02002 is not null and ");

            if (QueryString.ContainsKey("Number"))
            {
                sqlstr += " B02002 = @B02002 " + (QueryString.ContainsKey("Branch") ? " and " : string.Empty);
                para.Add(new SqlParameter("@B02002", SqlDbType.VarChar, 10));
                para[i].Value = QueryString["Number"];

                CountSqlStr += " B02002 = @B02002 " + (QueryString.ContainsKey("Branch") ? " and " : string.Empty);
                CountPara.Add(new SqlParameter("@B02002", SqlDbType.VarChar, 10));
                CountPara[i++].Value = QueryString["Number"];
            }
            if (QueryString.ContainsKey("Branch"))
            {
                sqlstr += " B02003 like @B02003 ";
                para.Add(new SqlParameter("@B02003", SqlDbType.VarChar, 60));
                para[i].Value = QueryString["Branch"] + "%";

                CountSqlStr += " B02003 like @B02003 ";
                CountPara.Add(new SqlParameter("@B02003", SqlDbType.VarChar, 60));
                CountPara[i].Value = QueryString["Branch"] + "%";
            }
            sqlstr += @"ORDER BY B02002 ASC
	                    )inside
	                    ORDER BY inside.B02002 DESC
                    )outside ORDER BY outside.B02002 ASC";

            ucPagination.CPage = pagenumber;
            ucPagination.CList = QueryString;

            SetEndPage(CountSqlStr, CountPara.ToArray() as SqlParameter[], pagenumber);
        }
        else
        {
            sqlstr = string.Format(@"SELECT * FROM(
	                                            SELECT TOP 10 * FROM(
		                                            SELECT top {0} B02002,B02003 FROM B02 
		                                            where B02006='北區' and B02002 is not null ORDER BY B02002 ASC
	                                            )inside
	                                            ORDER BY inside.B02002 DESC
                                            )outside
                                        ORDER BY outside.B02002 ASC ", 10 * pagenumber);

            SetEndPage("SELECT count(B02002) FROM B02 where B02006='北區' and B02002 is not null", null, pagenumber);
        }

        using (SqlConnection conn = new SqlConnection(connstr))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
            {
                if (QueryString.ContainsKey("Number") || QueryString.ContainsKey("Branch"))
                    cmd.Parameters.AddRange(para.ToArray() as SqlParameter[]);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    Repeater1.DataSource = dr;
                    Repeater1.DataBind();
                    if (!dr.HasRows)
                        ClientScript.RegisterStartupScript(GetType(), "Not Found!", "<script>alert('查無資料');</script>");
                    dr.Close();
                }
            }
            conn.Close();
        }
    }
}
