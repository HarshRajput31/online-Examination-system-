<%@ Page Title="Add Question" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddQuestion.aspx.cs" Inherits="OnlineExaminationSystem.AddQuestion" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="aq-page create-exam-page"> <%-- PAGE HEADER --%>
    <div class="ce-page-header">
        <span class="ce-page-icon">➕</span>
        <div>
            <h2 class="ce-page-title">Add Question</h2>
            <p class="ce-page-subtitle">
                Add MCQ or Descriptive questions to Exam ID: <asp:Label ID="lblExamDisplay" runat="server" ForeColor="#38bdf8" FontWeight="Bold" />
            </p>
        </div>
    </div>

    <%-- FORM CARD --%>
    <div class="ce-form-card">
        <div class="ce-card-glow"></div>

        <%-- 1. SELECT EXAM (Hidden if ID is in QueryString, otherwise visible) --%>
        <div class="ce-field">
            <label class="ce-label">📋 Select Exam</label>
            <asp:DropDownList ID="ddlExam" runat="server" CssClass="ce-input" />
        </div>

        <%-- 2. SET SELECTOR --%>
        <div class="ce-field">
            <label class="ce-label">🗂️ Select Set <span class="aq-label-hint">— Which version of the paper?</span></label>
            <div class="ce-set-selector">
                <asp:RadioButtonList ID="rblSet" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="ce-set-rbl">
                    <asp:ListItem Value="Set 1" Selected="True">Set 1</asp:ListItem>
                    <asp:ListItem Value="Set 2">Set 2</asp:ListItem>
                    <asp:ListItem Value="Set 3">Set 3</asp:ListItem>
                    <asp:ListItem Value="Set 4">Set 4</asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>

        <%-- 3. QUESTION TYPE --%>
        <div class="ce-field">
            <label class="ce-label">🔘 Question Type</label>
            <div class="aq-type-row" style="display: flex; gap: 15px; margin-bottom: 20px;">
                <div class="aq-type-card active" id="cardMcq" onclick="selectType('mcq')" style="flex:1; cursor:pointer; padding:15px; border-radius:15px; background:rgba(255,255,255,0.05); border:1px solid rgba(255,255,255,0.1); text-align:center;">
                    <div class="aq-type-icon">🔘</div>
                    <div class="aq-type-name" style="font-weight:700;">MCQ</div>
                </div>
                <div class="aq-type-card" id="cardDesc" onclick="selectType('desc')" style="flex:1; cursor:pointer; padding:15px; border-radius:15px; background:rgba(255,255,255,0.05); border:1px solid rgba(255,255,255,0.1); text-align:center;">
                    <div class="aq-type-icon">📝</div>
                    <div class="aq-type-name" style="font-weight:700;">Descriptive</div>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="hfQuestionType" runat="server" Value="mcq" />
        <asp:HiddenField ID="hfHasSubQ" runat="server" Value="false" />
        <asp:HiddenField ID="hfSubQuestions" runat="server" Value="" />

        <%-- 4. QUESTION TEXT --%>
        <div class="ce-field">
            <label class="ce-label">❓ Question</label>
            <asp:TextBox ID="txtQuestion" runat="server" TextMode="MultiLine" Rows="3" CssClass="ce-input" placeholder="Enter your question here..." />
        </div>

        <%-- ============ MCQ SECTION ============ --%>
        <div id="mcqSection">
            <div class="ce-row-2">
                <div class="ce-field">
                    <label class="ce-label"><span class="aq-opt-badge opt-a" style="background:#ef4444; padding:2px 6px; border-radius:4px; margin-right:5px;">A</span> Option A</label>
                    <asp:TextBox ID="txtOptA" runat="server" CssClass="ce-input" placeholder="Option A" />
                </div>
                <div class="ce-field">
                    <label class="ce-label"><span class="aq-opt-badge opt-b" style="background:#3b82f6; padding:2px 6px; border-radius:4px; margin-right:5px;">B</span> Option B</label>
                    <asp:TextBox ID="txtOptB" runat="server" CssClass="ce-input" placeholder="Option B" />
                </div>
                <div class="ce-field">
                    <label class="ce-label"><span class="aq-opt-badge opt-c" style="background:#f59e0b; padding:2px 6px; border-radius:4px; margin-right:5px;">C</span> Option C</label>
                    <asp:TextBox ID="txtOptC" runat="server" CssClass="ce-input" placeholder="Option C" />
                </div>
                <div class="ce-field">
                    <label class="ce-label"><span class="aq-opt-badge opt-d" style="background:#10b981; padding:2px 6px; border-radius:4px; margin-right:5px;">D</span> Option D</label>
                    <asp:TextBox ID="txtOptD" runat="server" CssClass="ce-input" placeholder="Option D" />
                </div>
            </div>

            <div class="ce-field">
                <label class="ce-label">✅ Correct Answer</label>
                <div class="ce-set-selector">
                    <asp:RadioButtonList ID="rblCorrect" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="ce-set-rbl">
                        <asp:ListItem Value="A">A</asp:ListItem>
                        <asp:ListItem Value="B">B</asp:ListItem>
                        <asp:ListItem Value="C">C</asp:ListItem>
                        <asp:ListItem Value="D">D</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>

            <div class="ce-field">
                <label class="ce-label">🏆 Marks</label>
                <asp:TextBox ID="txtMarks" runat="server" CssClass="ce-input" TextMode="Number" placeholder="e.g. 2" Width="100px" />
            </div>

            <%-- SUB QUESTIONS TOGGLE --%>
            <div class="ce-field">
                <label class="ce-label">🔽 Sub-Questions</label>
                <div class="ce-btn-row" style="margin-bottom:15px;">
                    <button type="button" class="ce-btn-questions" id="btnNoSub" onclick="toggleSubQ(false)" style="background:#475569 !important; font-size:12px;">❌ No Sub-Questions</button>
                    <button type="button" class="ce-btn-questions" id="btnYesSub" onclick="toggleSubQ(true)" style="background:rgba(56, 189, 248, 0.1) !important; color:#38bdf8; border:1px solid #38bdf8; font-size:12px;">✅ Yes, Has Sub-Questions</button>
                </div>
                <div id="subQPanel" style="display:none; background:rgba(255,255,255,0.02); padding:20px; border-radius:20px; border:1px dashed rgba(255,255,255,0.1);">
                    <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:15px;">
                        <span style="font-size:12px; font-weight:bold; color:#94a3b8;">SUB-QUESTIONS LIST</span>
                        <button type="button" class="action-link edit" onclick="addSubQ()" style="background:none; border:none; cursor:pointer;">+ Add New</button>
                    </div>
                    <div id="subQList"></div>
                </div>
            </div>
        </div>

        <%-- ============ DESCRIPTIVE SECTION ============ --%>
        <div id="descSection" style="display:none;">
            <div class="ce-field">
                <label class="ce-label">📖 Model Answer / Marking Guide</label>
                <asp:TextBox ID="txtModelAnswer" runat="server" TextMode="MultiLine" Rows="5" CssClass="ce-input" placeholder="Reference answer for marking..." />
            </div>
            <div class="ce-row-2">
                <div class="ce-field">
                    <label class="ce-label">🔢 Max Words</label>
                    <asp:TextBox ID="txtMaxWords" runat="server" CssClass="ce-input" TextMode="Number" placeholder="e.g. 500" />
                </div>
                <div class="ce-field">
                    <label class="ce-label">🏆 Marks</label>
                    <asp:TextBox ID="txtDescMarks" runat="server" CssClass="ce-input" TextMode="Number" placeholder="e.g. 10" />
                </div>
            </div>
        </div>

        <hr class="ce-divider" />

        <%-- BUTTONS --%>
        <div class="ce-btn-row">
            <asp:Button ID="btnSave" runat="server" Text="💾 Save" CssClass="ce-btn-create" OnClick="btnSave_Click" />
            <asp:Button ID="btnSaveAnother" runat="server" Text="➕ Save & Next" CssClass="ce-btn-questions" OnClick="btnSaveAnother_Click" />
            
            <%-- FINAL PUBLISH TO ADMIN --%>
            <asp:Button ID="btnFinalPublish" runat="server" Text="🚀 Publish to Admin" CssClass="ce-btn-publish" 
                OnClick="btnFinalPublish_Click" OnClientClick="return confirm('Ready to send this entire exam to Admin for approval?');" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" style="display:block; margin-top:20px;" />

        <div class="ce-status-bar" style="margin-top:30px; padding:15px; background:rgba(56, 189, 248, 0.05); border-radius:14px; text-align:center;">
            <span style="color:#94a3b8; font-size:13px;">📊 Total questions in this paper: 
                <b style="color:#38bdf8;"><asp:Label ID="lblCount" runat="server" Text="0" /></b>
            </span>
        </div>
    </div>
</div>

<script>
    function selectType(type) {
        document.getElementById('<%= hfQuestionType.ClientID %>').value = type;
        const mcq = document.getElementById('mcqSection');
        const desc = document.getElementById('descSection');
        const cMcq = document.getElementById('cardMcq');
        const cDesc = document.getElementById('cardDesc');

        if (type === 'mcq') {
            mcq.style.display = 'block';
            desc.style.display = 'none';
            cMcq.style.borderColor = '#38bdf8';
            cMcq.style.background = 'rgba(56,189,248,0.1)';
            cDesc.style.borderColor = 'rgba(255,255,255,0.1)';
            cDesc.style.background = 'rgba(255,255,255,0.05)';
        } else {
            mcq.style.display = 'none';
            desc.style.display = 'block';
            cDesc.style.borderColor = '#38bdf8';
            cDesc.style.background = 'rgba(56,189,248,0.1)';
            cMcq.style.borderColor = 'rgba(255,255,255,0.1)';
            cMcq.style.background = 'rgba(255,255,255,0.05)';
        }
    }

    function toggleSubQ(show) {
        document.getElementById('<%= hfHasSubQ.ClientID %>').value = show;
        document.getElementById('subQPanel').style.display = show ? 'block' : 'none';
    }

    var sqCount = 0;
    function addSubQ() {
        sqCount++;
        const list = document.getElementById('subQList');
        const div = document.createElement('div');
        div.className = 'aq-subq-item';
        div.id = 'sq_item_' + sqCount;
        div.style = "background:rgba(255,255,255,0.03); padding:15px; border-radius:12px; margin-bottom:10px; position:relative; border:1px solid rgba(255,255,255,0.05);";

        div.innerHTML = `
            <div style="font-size:11px; color:#38bdf8; margin-bottom:8px;">SUB-QUESTION #${sqCount}</div>
            <input type="text" class="ce-input" id="sq_txt_${sqCount}" placeholder="Enter sub-question text" style="margin-bottom:10px;">
            <div class="ce-row-2">
                <input type="text" class="ce-input" id="sq_a_${sqCount}" placeholder="Opt A" style="font-size:12px;">
                <input type="text" class="ce-input" id="sq_b_${sqCount}" placeholder="Opt B" style="font-size:12px;">
            </div>
            <div class="ce-row-2" style="margin-top:5px;">
                <input type="text" class="ce-input" id="sq_c_${sqCount}" placeholder="Opt C" style="font-size:12px;">
                <input type="text" class="ce-input" id="sq_d_${sqCount}" placeholder="Opt D" style="font-size:12px;">
            </div>
            <div style="margin-top:10px; display:flex; align-items:center; gap:10px;">
                <span style="font-size:11px; color:#94a3b8;">CORRECT:</span>
                <select class="ce-input" id="sq_ans_${sqCount}" style="width:70px; padding:5px !important;">
                    <option value="A">A</option><option value="B">B</option><option value="C">C</option><option value="D">D</option>
                </select>
                <button type="button" onclick="removeSubQ(${sqCount})" style="background:none; border:none; color:#f87171; cursor:pointer; font-size:11px; margin-left:auto;">Remove</button>
            </div>`;
        list.appendChild(div);
        syncSubQJson();
    }

    function removeSubQ(n) {
        document.getElementById('sq_item_' + n).remove();
        syncSubQJson();
    }

    function syncSubQJson() {
        const items = document.querySelectorAll('.aq-subq-item');
        let arr = [];
        items.forEach(item => {
            const n = item.id.replace('sq_item_', '');
            arr.push({
                text: document.getElementById('sq_txt_' + n).value.trim(),
                optA: document.getElementById('sq_a_' + n).value.trim(),
                optB: document.getElementById('sq_b_' + n).value.trim(),
                optC: document.getElementById('sq_c_' + n).value.trim(),
                optD: document.getElementById('sq_d_' + n).value.trim(),
                answer: document.getElementById('sq_ans_' + n).value
            });
        });
        document.getElementById('<%= hfSubQuestions.ClientID %>').value = JSON.stringify(arr);
    }

    document.addEventListener('DOMContentLoaded', () => {
        selectType('mcq');
        document.querySelectorAll('.ce-btn-create, .ce-btn-questions').forEach(btn => {
            btn.addEventListener('click', syncSubQJson);
        });
    });
</script>

<style>
    /* Quick helper to ensure cards look good before CSS file loads */
    .aq-type-card.active { border-color: #38bdf8 !important; background: rgba(56, 189, 248, 0.1) !important; }
    .aq-label-hint { font-size: 10px; color: #64748b; font-weight: 400; text-transform: none; margin-left: 5px; }
</style>

</asp:Content>