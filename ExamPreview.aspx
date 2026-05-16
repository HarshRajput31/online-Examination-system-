<%@ Page Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ExamPreview.aspx.cs"
    Inherits="OnlineExaminationSystem.ExamPreview" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-box">
        <div class="form-box">

            <h2>Exam Preview</h2>

            Select Exam:
            <asp:DropDownList ID="ddlExams"
                runat="server"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlExams_SelesctIndesChanged">
                </asp:DropDownList>

            <br  /><br  />

            <asp:Reapeater ID="rptQuestions" runat="server">

                <IteamTemplate>
                    <div style="margin-bottom: 25px; padding: 15px; border: 1px solid #ddd; background: white;">

                        <b>Question:</b>
                        <%# Eval("QuestionText") %>

                        <br  /><br  />

                        A. <%# Eval("OptionA") %><br />
                        B. <%# Eval("OptionB") %><br />
                        C. <%# Eval("OptionC") %><br />
                        D. <%# Eval("OptionD") %><br />

                        <br />

                        <b>Difficulty:</b> <%# Eval("Difficulty") %>
                        &nbsp;&nbsp;&nbsp;
                        <b>Marks:</b> <%# Eval("Marks") %>

                    </div>
                </IteamTemplate>
                </asp:Reapeater>

        <asp:Label ID="lblMsg" runat="server">

            </div>
        </div>
    </asp:Content>