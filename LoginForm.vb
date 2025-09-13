Imports System.Drawing
Imports System.Windows.Forms

Public Class LoginForm
    Inherits Form

    Private ReadOnly lblTitle As New Label()
    Private ReadOnly lblUsername As New Label()
    Private ReadOnly lblPassword As New Label()
    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly txtPassword As New TextBox()
    Private ReadOnly chkShowPassword As New CheckBox()
    Private ReadOnly btnLogin As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly lblError As New Label()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Login"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ClientSize = New Size(420, 300)
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)

        lblTitle.Text = "Fisheries POS - Sign In"
        lblTitle.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblTitle.AutoSize = True
        lblTitle.Location = New Point(20, 20)

        lblUsername.Text = "Username"
        lblUsername.AutoSize = True
        lblUsername.Location = New Point(22, 70)

        txtUsername.Location = New Point(25, 90)
        txtUsername.Size = New Size(360, 27)
        txtUsername.TabIndex = 0

        lblPassword.Text = "Password"
        lblPassword.AutoSize = True
        lblPassword.Location = New Point(22, 125)

        txtPassword.Location = New Point(25, 145)
        txtPassword.Size = New Size(360, 27)
        txtPassword.UseSystemPasswordChar = True
        txtPassword.TabIndex = 1

        chkShowPassword.Text = "Show password"
        chkShowPassword.AutoSize = True
        chkShowPassword.Location = New Point(25, 176)
        AddHandler chkShowPassword.CheckedChanged, AddressOf chkShowPassword_CheckedChanged

        btnLogin.Text = "Login"
        btnLogin.Location = New Point(210, 215)
        btnLogin.Size = New Size(85, 32)
        btnLogin.TabIndex = 2
        AddHandler btnLogin.Click, AddressOf btnLogin_Click

        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(300, 215)
        btnCancel.Size = New Size(85, 32)
        btnCancel.TabIndex = 3
        btnCancel.DialogResult = DialogResult.Cancel

        lblError.AutoSize = True
        lblError.ForeColor = Color.Firebrick
        lblError.Location = New Point(25, 255)
        lblError.Visible = False

        Me.Controls.Add(lblTitle)
        Me.Controls.Add(lblUsername)
        Me.Controls.Add(txtUsername)
        Me.Controls.Add(lblPassword)
        Me.Controls.Add(txtPassword)
        Me.Controls.Add(chkShowPassword)
        Me.Controls.Add(btnLogin)
        Me.Controls.Add(btnCancel)
        Me.Controls.Add(lblError)

        Me.AcceptButton = btnLogin
        Me.CancelButton = btnCancel
    End Sub

    Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs)
        txtPassword.UseSystemPasswordChar = Not chkShowPassword.Checked
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs)
        lblError.Visible = False
        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text

        If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(password) Then
            ShowError("Please enter both username and password.")
            Return
        End If

        Try
            Dim dal As New UserDAL()
            Dim user As User = dal.Authenticate(username, password)
            If user Is Nothing Then
                ShowError("Invalid credentials.")
                Return
            End If

            UserSession.UserID = user.UserID
            UserSession.Username = user.Username
            UserSession.UserType = user.UserType
            UserSession.FullName = user.FullName

            Dim nextForm As Form
            If String.Equals(user.UserType, "Client", StringComparison.OrdinalIgnoreCase) Then
                nextForm = New ClientMarketForm()
            Else
                nextForm = New DashboardForm()
            End If

            AddHandler nextForm.FormClosed, Sub(s, args) Me.Close()
            nextForm.Show()
            Me.Hide()
        Catch ex As Exception
            MessageBox.Show("Login error: " & ex.Message)
        End Try
    End Sub

    Private Sub ShowError(message As String)
        lblError.Text = message
        lblError.Visible = True
    End Sub
End Class