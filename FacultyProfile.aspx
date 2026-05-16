<%@ Page Title="Faculty Profile" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="FacultyProfile.aspx.cs"
    Inherits="OnlineExaminationSystem.FacultyProfile" %>
<asp:Content ID="FacultyProfileContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="profile-page">
        <!-- TITLE -->
        <h2 class="profile-heading">Faculty Profile</h2>
        <div class="profile-wrapper">
            <!-- LEFT: PHOTO CARD -->
            <div class="photo-card">
                <!-- PROFILE IMAGE -->
                <div class="photo-circle-wrapper">
                    <asp:Image ID="imgProfile" runat="server"
                        ImageUrl="~/Images/default-user.png"
                        CssClass="profile-photo" />
                </div>
                <!-- FACULTY NAME BELOW PHOTO -->
                <h3 class="faculty-name-display">
                    <asp:Label ID="lblNameDisplay" runat="server" Text="Faculty Name" />
                </h3>
                <p class="faculty-role-display">Faculty Member</p>
                <!-- UPLOAD OPTIONS -->
                <div class="upload-section">
                    <p class="upload-label">Update Profile Photo</p>
                    <!-- FILE UPLOAD -->
                    <label class="upload-btn-custom">
                        Choose from Folder
                        <asp:FileUpload ID="fuPhoto" runat="server"
                            CssClass="hidden-file-input"
                            accept="image/*"
                            onchange="previewSelectedPhoto(this)" />
                    </label>
                    <!-- CAMERA CAPTURE -->
                    <button type="button" class="upload-btn-camera" onclick="openCamera()">
                        Take a Photo
                    </button>
                    <asp:HiddenField ID="hfCapturedPhoto" runat="server" />
                    <div id="cameraPanel" class="camera-panel" style="display:none;">
                        <video id="cameraVideo" class="camera-video" autoplay playsinline muted></video>
                        <canvas id="cameraCanvas" style="display:none;"></canvas>
                        <div class="camera-actions">
                            <button type="button" class="camera-action-btn capture" onclick="captureCameraPhoto()">Capture</button>
                            <button type="button" class="camera-action-btn close" onclick="closeCamera()">Close</button>
                        </div>
                        <span id="cameraMessage" class="camera-message"></span>
                    </div>
                    <!-- UPLOAD BUTTON -->
                    <asp:Button ID="btnUpload" runat="server" Text="Upload Photo"
                        CssClass="btn-upload-submit" OnClick="btnUpload_Click" />
                    <!-- MESSAGE -->
                    <asp:Label ID="lblPhotoMsg" runat="server" CssClass="photo-msg" />
                </div>
            </div>
            <!-- RIGHT: INFO CARD -->
            <div class="info-card">
                <h4 class="info-card-title">Profile Information</h4>
                <!-- FACULTY NAME -->
                <div class="info-row">
                    <div class="info-content">
                        <span class="info-label">Full Name</span>
                        <asp:TextBox ID="txtName" runat="server"
                            CssClass="info-input" Placeholder="Full name is managed by admin"
                            ReadOnly="true" />
                    </div>
                </div>
                <!-- FACULTY ID -->
                <div class="info-row">
                    <div class="info-content">
                        <span class="info-label">Faculty ID</span>
                        <asp:TextBox ID="txtFacultyId" runat="server"
                            CssClass="info-input" ReadOnly="true" />
                    </div>
                </div>
                <!-- EMAIL -->
                <div class="info-row">
                    <div class="info-content">
                        <span class="info-label">Email Address</span>
                        <asp:TextBox ID="txtEmail" runat="server"
                            CssClass="info-input" Placeholder="Email is managed by admin"
                            TextMode="Email" ReadOnly="true" />
                    </div>
                </div>
                <!-- MOBILE -->
                <div class="info-row">
                    <div class="info-content">
                        <span class="info-label">Mobile Number</span>
                        <asp:TextBox ID="txtMobile" runat="server"
                            CssClass="info-input" Placeholder="Mobile number is managed by admin"
                            MaxLength="10" ReadOnly="true" />
                    </div>
                </div>
                <!-- COURSE -->
                <div class="info-row">
                    <div class="info-content">
                        <span class="info-label">Course Teaching</span>
                        <asp:TextBox ID="txtCourse" runat="server"
                            CssClass="info-input" Placeholder="Course is managed by admin"
                            ReadOnly="true" />
                    </div>
                </div>
                <!-- DEPARTMENT -->
                <div class="info-row">
                    <div class="info-content">
                        <span class="info-label">Department</span>
                        <asp:DropDownList ID="ddlDepartment" runat="server"
                            CssClass="info-input" Enabled="false">
                            <asp:ListItem Value="">-- Select Department --</asp:ListItem>
                            <asp:ListItem Value="Computer Science">Computer Science</asp:ListItem>
                            <asp:ListItem Value="Information Technology">Information Technology</asp:ListItem>
                            <asp:ListItem Value="Electronics">Electronics</asp:ListItem>
                            <asp:ListItem Value="Mechanical">Mechanical</asp:ListItem>
                            <asp:ListItem Value="Civil">Civil</asp:ListItem>
                            <asp:ListItem Value="MBA">MBA</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <asp:Label ID="lblMsg" runat="server" CssClass="save-msg"
                    Text="Profile details are managed by Admin." />
            </div>
        </div>
    </div>
    <script>
        var cameraStream = null;
        function previewSelectedPhoto(input) {
            if (!input.files || !input.files[0]) {
                return;
            }
            document.getElementById('<%= hfCapturedPhoto.ClientID %>').value = '';
            var reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById('<%= imgProfile.ClientID %>').src = e.target.result;
            };
            reader.readAsDataURL(input.files[0]);
        }
        async function openCamera() {
            var panel = document.getElementById('cameraPanel');
            var video = document.getElementById('cameraVideo');
            var message = document.getElementById('cameraMessage');
            message.textContent = '';
            panel.style.display = 'grid';
            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                message.textContent = 'Camera is not supported in this browser.';
                return;
            }
            try {
                cameraStream = await navigator.mediaDevices.getUserMedia({
                    video: {
                        facingMode: 'user'
                    },
                    audio: false
                });
                video.srcObject = cameraStream;
                await video.play();
            } catch (error) {
                message.textContent = 'Camera permission denied or camera not available.';
                console.error(error);
            }
        }
        function captureCameraPhoto() {
            var video = document.getElementById('cameraVideo');
            var canvas = document.getElementById('cameraCanvas');
            var message = document.getElementById('cameraMessage');
            if (!cameraStream || !video.videoWidth || !video.videoHeight) {
                message.textContent = 'Open the camera first, then capture.';
                return;
            }
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            var context = canvas.getContext('2d');
            context.drawImage(video, 0, 0, canvas.width, canvas.height);
            var dataUrl = canvas.toDataURL('image/jpeg', 0.9);
            document.getElementById('<%= hfCapturedPhoto.ClientID %>').value = dataUrl;
            document.getElementById('<%= imgProfile.ClientID %>').src = dataUrl;
            message.textContent = 'Photo captured. Click Upload Photo to save it.';
            closeCamera(false);
        }
        function closeCamera(clearMessage) {
            var panel = document.getElementById('cameraPanel');
            var video = document.getElementById('cameraVideo');
            var message = document.getElementById('cameraMessage');
            if (cameraStream) {
                cameraStream.getTracks().forEach(function (track) {
                    track.stop();
                });
                cameraStream = null;
            }
            video.srcObject = null;
            panel.style.display = 'none';
            if (clearMessage !== false) {
                message.textContent = '';
            }
        }
    </script>
</asp:Content>