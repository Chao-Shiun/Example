using System;
using System.Web.UI.WebControls;

public partial class UCL_Pagination : System.Web.UI.UserControl
{
    private int CurrentPage;
    private int EndPage;

    public int CPage
    {
        set
        {
            //設定當前頁面
            CurrentPage = value;
        }
    }
    public int EPage
    {
        set
        {
            //全部的分頁共有幾頁
            EndPage = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && EndPage != 1)
        {
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
                liFirst.Text = @"<li><a href=""Default3.aspx?page=1"">第一頁</a></li>";
                liPre.Text = @"<li><a href=""Default3.aspx?page=" + (CurrentPage - 1) + @""">上一頁</a></li>";
            }
            PaginationContent.Controls.Add(liFirst);
            PaginationContent.Controls.Add(liPre);

            Literal liPage;

            if (EndPage <= 10)
            {
                for (int i = 1 ; i <= EndPage ; i++)
                {
                    liPage = new Literal();
                    if (i == CurrentPage)
                        liPage.Text = @"<li class=""current"">" + CurrentPage + "</li>";
                    else
                        liPage.Text = @"<li><a href=""Default3.aspx?page=" + i + @""">" + i + "</a></li>";
                    PaginationContent.Controls.Add(liPage);
                }
            }
            else if (EndPage > 10)
            {
                if (EndPage - CurrentPage < 5)
                {
                    for (int i = EndPage - 9 ; i <= EndPage ; i++)
                    {
                        liPage = new Literal();
                        if (i == CurrentPage)
                            liPage.Text = @"<li class=""current"">" + CurrentPage + "</li>";
                        else
                            liPage.Text = @"<li><a href=""Default3.aspx?page=" + i + @""">" + i + "</a></li>";
                        PaginationContent.Controls.Add(liPage);
                    }
                }
                else
                {
                    int start, end;
                    start = (CurrentPage - 5) >= 1 ? CurrentPage - 5 : 1;
                    end = (CurrentPage <= 6) ? 10 : CurrentPage + 4;
                    for (int i = start ; i <= end ; i++)
                    {
                        liPage = new Literal();
                        if (i == CurrentPage)
                            liPage.Text = @"<li class=""current"">" + CurrentPage + "</li>";
                        else
                            liPage.Text = @"<li><a href=""Default3.aspx?page=" + i + @""">" + i + "</a></li>";
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
                liNext.Text = @"<li><a href=""Default3.aspx?page=" + (CurrentPage + 1) + @""">下一頁</a></li>";
                liEnd.Text = @"<li><a href=""Default3.aspx?page=" + EndPage + @""">最後一頁</a></li>";
            }

            PaginationContent.Controls.Add(liNext);
            PaginationContent.Controls.Add(liEnd);
        }
    }
}
