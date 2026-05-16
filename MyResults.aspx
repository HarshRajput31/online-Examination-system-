<%@ Page Title="My Results"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="MyResults.aspx.cs"
    Inherits="OnlineExaminationSystem.MyResults" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="container-box">
    <div class="form-box">

        <h2>📊 My Results</h2>

        <asp:GridView ID="gvResults"
            runat="server"
            AutoGenerateColumns="False"
            Width="100%"
            CssClass="table"
            HeaderStyle-BackColor="#0f172a"
            HeaderStyle-ForeColor="White"
            RowStyle-BackColor="#1e293b"
            RowStyle-ForeColor="White"
            AlternatingRowStyle-BackColor="#334155">

            <Columns>

                <asp:BoundField DataField="ExamName" HeaderText="Exam" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" />
                <asp:BoundField DataField="Score" HeaderText="Score" />
                <asp:BoundField DataField="Percentage" HeaderText="Percentage" />
                <asp:BoundField DataField="Status" HeaderText="Result" />
                <asp:BoundField DataField="Date" HeaderText="Date" />

                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <a href='Result.aspx?ExamId=<%# Eval("ExamId") %>'
                           style="color:#38bdf8; font-weight:bold;">
                           View
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>

        </asp:GridView>

        <br />

        <asp:Label ID="lblMsg"
            runat="server"
            CssClass="error-msg" />

    </div>
</div>

</asp:Content>