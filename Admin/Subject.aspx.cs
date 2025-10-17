using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SchoolManagementSystem.Models.CommonFn;

namespace SchoolManagementSystem.Admin
{
    public partial class Subject : System.Web.UI.Page
    {
        Commonfnx fn = new Commonfnx();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetClass();
                GetSubject();
            }
        }

        private void GetClass()
        {
            DataTable dt = fn.Fetch("SELECT * FROM Class");
            ddlClass.DataSource = dt;
            ddlClass.DataTextField = "ClassName";
            ddlClass.DataValueField = "ClassId";
            ddlClass.DataBind();
            ddlClass.Items.Insert(0, new ListItem("Select Class", ""));
        }
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int subjId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["SubjectId"]);
                fn.Query("DELETE FROM Subject WHERE SubjectId = '" + subjId + "'");

                lblMsg.Text = "Subject deleted successfully!";
                lblMsg.CssClass = "alert alert-success";

                GetSubject();
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('{ex.Message}');</script>");
            }
        }


        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string classId = ddlClass.SelectedValue;
                string classVal = ddlClass.SelectedItem.Text;
                string subjectName = txtSubject.Text.Trim();

                DataTable dt = fn.Fetch($"SELECT * FROM Subject WHERE ClassId = '{classId}' AND SubjectName = '{subjectName}'");

                if (dt.Rows.Count == 0)
                {
                    string query = $"INSERT INTO Subject (ClassId, SubjectName) VALUES ('{classId}', '{subjectName}')";
                    fn.Query(query);

                    lblMsg.Text = "Inserted Successfully!";
                    lblMsg.CssClass = "alert alert-success";

                    ddlClass.SelectedIndex = 0;
                    txtSubject.Text = string.Empty;
                    GetSubject();
                }
                else
                {
                    lblMsg.Text = $"Entered Subject already exists for <b>{classVal}</b>!";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('{ex.Message}');</script>");
            }
        }

        private void GetSubject()
        {
            DataTable dt = fn.Fetch(@"
                SELECT 
                    ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS [Sr.No], 
                    s.SubjectId, 
                    s.ClassId, 
                    c.ClassName, 
                    s.SubjectName 
                FROM Subject s 
                INNER JOIN Class c ON c.ClassId = s.ClassId");

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GetSubject();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            GetSubject();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetSubject();
            BindDropDownInGrid(e.NewEditIndex);
        }

        private void BindDropDownInGrid(int rowIndex)
        {
            DropDownList ddl = (DropDownList)GridView1.Rows[rowIndex].FindControl("DropDownList1");
            DataTable dt = fn.Fetch("SELECT * FROM Class");
            ddl.DataSource = dt;
            ddl.DataTextField = "ClassName";
            ddl.DataValueField = "ClassId";
            ddl.DataBind();

            // Set selected value to current ClassId
            string classId = ((DataRowView)GridView1.DataKeys[rowIndex].Value).Row["ClassId"].ToString();
            ddl.SelectedValue = classId;
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int subjId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values["SubjectId"]);
                string classId = ((DropDownList)row.FindControl("DropDownList1")).SelectedValue;
                string subjName = ((TextBox)row.FindControl("TextBox1")).Text.Trim();

                fn.Query($"UPDATE Subject SET ClassId = '{classId}', SubjectName = '{subjName}' WHERE SubjectId = '{subjId}'");

                lblMsg.Text = "Subject Updated Successfully!";
                lblMsg.CssClass = "alert alert-success";
                GridView1.EditIndex = -1;
                GetSubject();
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('{ex.Message}');</script>");
            }
        }
    }
}
