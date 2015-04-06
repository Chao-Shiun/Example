using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

public partial class UC_Pagination : System.Web.UI.UserControl
{
    private int CurrentPage;
    private int EndPage;
    private string TargetPage;
    private Dictionary<string, string> ConditionList;
    
    /// <summary>
    /// 設定當前頁面
    /// </summary>
    public int CPage
    {
        set
        {
            CurrentPage = value;
        }
    }
    
    /// <summary>
    /// 全部的分頁共有幾頁
    /// </summary>
    public int EPage
    {
        set
        {
            EndPage = value;
        }
    }

    /// <summary>
    /// 設定網頁路徑
    /// </summary>
    public string TPage
    {
        set
        {
            TargetPage = value;
        }
    }

    /// <summary>
    /// 設定其他Request參數
    /// </summary>
    public Dictionary<string, string> CList
    {
        set
        {
            ConditionList = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack && EndPage != 1)
        {
            string ConditionStr = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (ConditionList != null && ConditionList.Count != 0)
            {
                foreach (KeyValuePair<string, string> para in ConditionList)
                {
                    sb.Append("&" + para.Key + "=" + para.Value);
                }
                ConditionStr = sb.ToString();
            }

            if (CurrentPage > EndPage)
                CurrentPage = 1;
            Literal liFirst = new Literal();
            Literal liPre = new Literal();
            if (CurrentPage == 1)
            {
                liFirst.Text = @"<li class=""disabled"">第一頁</li>";
                liPre.Text = @"<li class=""disabled"">上一頁</li>";
            }
            else
            {
                liFirst.Text = @"<li><a href=""" + TargetPage + @"?page=1" + ConditionStr + @""">第一頁</a></li>";
                liPre.Text = @"<li><a href=""" + TargetPage + "?page=" + (CurrentPage - 1) + ConditionStr + @""">上一頁</a></li>";
            }
            PaginationContent.Controls.Add(liFirst);
            PaginationContent.Controls.Add(liPre);
            Literal liPage;
            if (EndPage <= 10)
            {
                for (int i = 1; i <= EndPage; i++)
                {
                    liPage = new Literal();
                    if (i == CurrentPage)
                        liPage.Text = @"<li class=""current"">" + CurrentPage + "</li>";
                    else
                        liPage.Text = @"<li><a href=""" + TargetPage + "?page=" + i + ConditionStr + @""">" + i + "</a></li>";
                    PaginationContent.Controls.Add(liPage);
                }
            }
            else if (EndPage > 10)
            {
                if (EndPage - CurrentPage < 5)
                {
                    for (int i = EndPage - 9; i <= EndPage; i++)
                    {
                        liPage = new Literal();
                        if (i == CurrentPage)
                            liPage.Text = @"<li class=""current"">" + CurrentPage + "</li>";
                        else
                            liPage.Text = @"<li><a href=""" + TargetPage + "?page=" + i + ConditionStr + @""">" + i + "</a></li>";

                        PaginationContent.Controls.Add(liPage);
                    }
                }
                else
                {
                    int start, end;
                    start = (CurrentPage - 5) >= 1 ? CurrentPage - 5 : 1;
                    end = (CurrentPage <= 6) ? 10 : CurrentPage + 4;
                    for (int i = start; i <= end; i++)
                    {
                        liPage = new Literal();
                        if (i == CurrentPage)
                            liPage.Text = @"<li class=""current"">" + CurrentPage + "</li>";
                        else
                            liPage.Text = @"<li><a href=""" + TargetPage + "?page=" + i + ConditionStr + @""">" + i + "</a></li>";

                        PaginationContent.Controls.Add(liPage);
                    }
                }
            }
            Literal liNext = new Literal();
            Literal liEnd = new Literal();
            if (CurrentPage == EndPage)
            {
                liNext.Text = @"<li class=""disabled"">下一頁</li>";
                liEnd.Text = @"<li class=""disabled"">最後一頁</li>";
            }
            else
            {
                liNext.Text = @"<li><a href=""" + TargetPage + "?page=" + (CurrentPage + 1) + ConditionStr + @""">下一頁</a></li>";
                liEnd.Text = @"<li><a href=""" + TargetPage + "?page=" + EndPage + ConditionStr + @""">最後一頁</a></li>";
            }
            PaginationContent.Controls.Add(liNext);
            PaginationContent.Controls.Add(liEnd);
        }
    }
}
