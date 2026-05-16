<%@ Page Title="Start Exam" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StartExam.aspx.cs"
    Inherits="OnlineExaminationSystem.StartExam" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

<style>
    .exam-shell { display:grid; grid-template-columns: 1fr 280px; gap:24px; }
    .question-block { padding:24px; margin-bottom:18px; }
    .question-text  { font-size:17px; line-height:1.6; margin-bottom:14px; color:#f8fafc; }
    .opt-row { display:flex; align-items:center; padding:12px 16px; margin-bottom:10px;
               border:1px solid rgba(255,255,255,0.10); border-radius:12px; cursor:pointer;
               transition:all 0.18s; background:rgba(255,255,255,0.03); }
    .opt-row:hover { border-color:#38bdf8; background:rgba(56,189,248,0.06); }
    .opt-row input { accent-color:#38bdf8; margin-right:14px; transform:scale(1.2); }
    .opt-letter { display:inline-block; width:26px; height:26px; line-height:26px;
                  text-align:center; border-radius:50%; background:rgba(56,189,248,0.15);
                  color:#38bdf8; font-weight:700; margin-right:10px; }

    .timer-pill { padding:14px 16px; border-radius:14px; background:rgba(56,189,248,0.10);
                  border:1px solid rgba(56,189,248,0.40); text-align:center; margin-bottom:14px; }
    .timer-pill .label { font-size:11px; letter-spacing:1.5px; color:#7dd3fc; text-transform:uppercase; }
    .timer-pill .value { font-size:30px; font-weight:800; color:#fff; font-family:'Courier New', monospace; }
    .timer-warning .value { color:#fcd34d; }
    .timer-critical .value { color:#fecaca; animation: pulseRing 1.6s infinite; }

    .palette { display:grid; grid-template-columns:repeat(5,1fr); gap:6px; }
    .palette button { padding:10px 0; border-radius:10px; border:1px solid rgba(255,255,255,0.12);
                      background:rgba(255,255,255,0.03); color:#cbd5e1; font-weight:700; cursor:pointer; }
    .palette button.answered { background:rgba(16,185,129,0.20); color:#6ee7b7; border-color:rgba(16,185,129,0.4); }
    .palette button.current  { background:#3b82f6; color:#fff; border-color:#60a5fa; }

    .desc-textarea { min-height:140px; }

    @media (max-width: 992px) { .exam-shell { grid-template-columns: 1fr; } }
</style>

<div class="dashboard-container">

    <!-- Top bar: title + submit -->
    <div class="ce-page-header" style="margin-bottom:18px;">
        <span class="ce-page-icon">📝</span>
        <div style="flex:1;">
            <h2 class="ce-page-title"><asp:Literal ID="litExamTitle" runat="server" Text="Exam" /></h2>
            <p class="ce-page-subtitle">
                <asp:Literal ID="litSubject" runat="server" /> &middot;
                <asp:Literal ID="litMarks" runat="server" Text="0" /> marks
            </p>
        </div>
        <asp:Button ID="btnSubmitExam" runat="server" Text="✅ Submit Exam"
            CssClass="ce-btn-create" OnClick="btnSubmit_Click"
            OnClientClick="return confirm('Submit and end your exam?');" />
    </div>

    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-warning">
        This exam has no questions yet. Please contact your faculty.
    </asp:Panel>

    <!-- Main two-column layout -->
    <asp:Panel ID="pnlExam" runat="server">
    <div class="exam-shell">

        <!-- LEFT: questions -->
        <div>
            <asp:HiddenField ID="hfViolations" runat="server" />
            <asp:Repeater ID="rptQuestions" runat="server" OnItemDataBound="rptQuestions_ItemDataBound">
                <ItemTemplate>
                    <div class="ce-form-card question-block" id='q_<%# Container.ItemIndex + 1 %>'>
                        <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:10px;">
                            <span style="font-size:12px; color:#38bdf8; font-weight:700; letter-spacing:1px;">
                                Q<%# Container.ItemIndex + 1 %>
                                &middot; <%# Eval("QuestionType").ToString() == "mcq" ? "MCQ" : "Descriptive" %>
                                &middot; <%# Eval("Marks") %> marks
                            </span>
                        </div>
                        <div class="question-text"><%# Eval("QuestionText") %></div>

                        <asp:HiddenField ID="hfQuestionId" runat="server" Value='<%# Eval("QuestionId") %>' />
                        <asp:HiddenField ID="hfQuestionType" runat="server" Value='<%# Eval("QuestionType") %>' />

                        <%-- MCQ options --%>
                        <asp:PlaceHolder ID="phMcq" runat="server" Visible='<%# Eval("QuestionType").ToString() == "mcq" %>'>
                            <label class="opt-row">
                                <input type="radio" name='ans_<%# Eval("QuestionId") %>' value="A" />
                                <span class="opt-letter">A</span> <%# Eval("OptionA") %>
                            </label>
                            <label class="opt-row">
                                <input type="radio" name='ans_<%# Eval("QuestionId") %>' value="B" />
                                <span class="opt-letter">B</span> <%# Eval("OptionB") %>
                            </label>
                            <label class="opt-row">
                                <input type="radio" name='ans_<%# Eval("QuestionId") %>' value="C" />
                                <span class="opt-letter">C</span> <%# Eval("OptionC") %>
                            </label>
                            <label class="opt-row">
                                <input type="radio" name='ans_<%# Eval("QuestionId") %>' value="D" />
                                <span class="opt-letter">D</span> <%# Eval("OptionD") %>
                            </label>
                        </asp:PlaceHolder>

                        <%-- Descriptive textarea --%>
                        <asp:PlaceHolder ID="phDesc" runat="server" Visible='<%# Eval("QuestionType").ToString() != "mcq" %>'>
                            <textarea class="ce-input desc-textarea"
                                      name='ans_<%# Eval("QuestionId") %>'
                                      placeholder="Type your answer here..."></textarea>
                        </asp:PlaceHolder>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- RIGHT: timer + palette -->
        <aside style="position:sticky; top:90px; align-self:start;">
            <div id="examTimer"
                 class="timer-pill"
                 data-duration-seconds='<%= DurationSeconds %>'
                 data-exam-id='<%= ExamId %>'
                 data-submit-button='<%= btnSubmitExam.ClientID %>'>
                <div class="label">Time Remaining</div>
                <div class="value" id="examTimerText">--:--</div>
            </div>

            <div class="ce-form-card" style="padding:18px;">
                <div style="font-size:11px; color:#94a3b8; letter-spacing:1.5px; margin-bottom:10px;">QUESTION PALETTE</div>
                <div class="palette" id="palette"></div>
                <div style="margin-top:14px; font-size:11px; color:#94a3b8;">
                    <div><span style="display:inline-block; width:10px; height:10px; background:rgba(16,185,129,0.6); border-radius:3px;"></span> Answered</div>
                    <div><span style="display:inline-block; width:10px; height:10px; background:#3b82f6; border-radius:3px;"></span> Current</div>
                </div>
            </div>

            <button type="button" onclick="ExamFullscreen.enter()" class="btn btn-secondary" style="width:100%; margin-top:14px;">
                <i class="fa-solid fa-expand"></i> &nbsp; Fullscreen
            </button>
        </aside>

    </div>
    </asp:Panel>

</div>

<script src="<%= ResolveUrl("~/Scripts/examTimer.js") %>"></script>
<script src="<%= ResolveUrl("~/Scripts/fullscreen.js") %>"></script>
<script src="<%= ResolveUrl("~/Scripts/antiCheat.js") %>"></script>
<script>
    document.body.setAttribute('data-anticheat', 'on');
    document.body.setAttribute('data-max-violations', '3');

    // Build palette + answered tracking
    document.addEventListener('DOMContentLoaded', function () {
        var blocks = document.querySelectorAll('.question-block');
        var palette = document.getElementById('palette');
        if (!palette) return;

        blocks.forEach(function (block, i) {
            var btn = document.createElement('button');
            btn.type = 'button';
            btn.textContent = i + 1;
            btn.addEventListener('click', function () {
                block.scrollIntoView({ behavior: 'smooth', block: 'start' });
                document.querySelectorAll('.palette button').forEach(function (b) { b.classList.remove('current'); });
                btn.classList.add('current');
            });
            palette.appendChild(btn);
        });

        // Mark answered when an input changes
        document.querySelectorAll('input[type=radio], textarea').forEach(function (input, idx) {
            input.addEventListener('change', refresh);
            input.addEventListener('input', refresh);
        });

        function refresh() {
            blocks.forEach(function (block, i) {
                var hasAnswer = false;
                block.querySelectorAll('input[type=radio]:checked').forEach(function () { hasAnswer = true; });
                block.querySelectorAll('textarea').forEach(function (t) {
                    if (t.value && t.value.trim().length > 0) hasAnswer = true;
                });
                var btn = palette.children[i];
                if (btn) btn.classList.toggle('answered', hasAnswer);
            });
        }
    });
</script>
</asp:Content>
