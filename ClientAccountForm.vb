Imports System.Drawing
Imports System.Windows.Forms

Public Class ClientAccountForm
    Inherits Form

    Private ReadOnly lblTitle As New Label()
    Private ReadOnly lblUsername As New Label()
    Private ReadOnly lblPassword As New Label()
    Private ReadOnly lblConfirmPassword As New Label()
    Private ReadOnly lblFullName As New Label()
    Private ReadOnly lblEmail As New Label()
    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly txtPassword As New TextBox()
    Private ReadOnly txtConfirmPassword As New TextBox()
    Private ReadOnly txtFullName As New TextBox()
    Private ReadOnly txtEmail As New TextBox()
    Private ReadOnly btnCreate As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly lblError As New Label()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Create Client Account"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ClientSize = New Size(450, 420)
        Me.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        Me.BackColor = Color.White

        ' Title
        lblTitle.Text = "Create New Client Account"
        lblTitle.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblTitle.AutoSize = True
        lblTitle.Location = New Point(30, 20)
        lblTitle.ForeColor = Color.FromArgb(51, 51, 51)

        ' Username
        lblUsername.Text = "Username:"
        lblUsername.AutoSize = True
        lblUsername.Location = New Point(30, 70)
        lblUsername.ForeColor = Color.FromArgb(51, 51, 51)

        txtUsername.Location = New Point(30, 95)
        txtUsername.Size = New Size(380, 25)
        txtUsername.TabIndex = 0

        ' Full Name
        lblFullName.Text = "Full Name:"
        lblFullName.AutoSize = True
        lblFullName.Location = New Point(30, 130)
        lblFullName.ForeColor = Color.FromArgb(51, 51, 51)

        txtFullName.Location = New Point(30, 155)
        txtFullName.Size = New Size(380, 25)
        txtFullName.TabIndex = 1

        ' Email
        lblEmail.Text = "Email:"
        lblEmail.AutoSize = True
        lblEmail.Location = New Point(30, 190)
        lblEmail.ForeColor = Color.FromArgb(51, 51, 51)

        txtEmail.Location = New Point(30, 215)
        txtEmail.Size = New Size(380, 25)
        txtEmail.TabIndex = 2

        ' Password
        lblPassword.Text = "Password:"
        lblPassword.AutoSize = True
        lblPassword.Location = New Point(30, 250)
        lblPassword.ForeColor = Color.FromArgb(51, 51, 51)

        txtPassword.Location = New Point(30, 275)
        txtPassword.Size = New Size(380, 25)
        txtPassword.UseSystemPasswordChar = True
        txtPassword.TabIndex = 3

        ' Confirm Password
        lblConfirmPassword.Text = "Confirm Password:"
        lblConfirmPassword.AutoSize = True
        lblConfirmPassword.Location = New Point(30, 310)
        lblConfirmPassword.ForeColor = Color.FromArgb(51, 51, 51)

        txtConfirmPassword.Location = New Point(30, 335)
        txtConfirmPassword.Size = New Size(380, 25)
        txtConfirmPassword.UseSystemPasswordChar = True
        txtConfirmPassword.TabIndex = 4

        ' Error Label
        lblError.AutoSize = True
        lblError.ForeColor = Color.Firebrick
        lblError.Location = New Point(30, 370)
        lblError.Visible = False
        lblError.MaximumSize = New Size(380, 0)

        ' Buttons
        btnCreate.Text = "Create Account"
        btnCreate.Location = New Point(220, 375)
        btnCreate.Size = New Size(120, 35)
        btnCreate.BackColor = Color.FromArgb(92, 184, 92)
        btnCreate.ForeColor = Color.White
        btnCreate.FlatStyle = FlatStyle.Flat
        btnCreate.FlatAppearance.BorderSize = 0
        btnCreate.TabIndex = 5
        AddHandler btnCreate.Click, AddressOf btnCreate_Click

        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(350, 375)
        btnCancel.Size = New Size(80, 35)
        btnCancel.BackColor = Color.FromArgb(119, 119, 119)
        btnCancel.ForeColor = Color.White
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.TabIndex = 6
        btnCancel.DialogResult = DialogResult.Cancel

        ' Add controls
        Me.Controls.Add(lblTitle)
        Me.Controls.Add(lblUsername)
        Me.Controls.Add(txtUsername)
        Me.Controls.Add(lblFullName)
        Me.Controls.Add(txtFullName)
        Me.Controls.Add(lblEmail)
        Me.Controls.Add(txtEmail)
        Me.Controls.Add(lblPassword)
        Me.Controls.Add(txtPassword)
        Me.Controls.Add(lblConfirmPassword)
        Me.Controls.Add(txtConfirmPassword)
        Me.Controls.Add(lblError)
        Me.Controls.Add(btnCreate)
        Me.Controls.Add(btnCancel)

        Me.AcceptButton = btnCreate
        Me.CancelButton = btnCancel
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As EventArgs)
        lblError.Visible = False

        ' Validate inputs
        If String.IsNullOrWhiteSpace(txtUsername.Text) Then
            ShowError("Username is required.")
            txtUsername.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(txtFullName.Text) Then
            ShowError("Full Name is required.")
            txtFullName.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(txtEmail.Text) Then
            ShowError("Email is required.")
            txtEmail.Focus()
            Return
        End If

        If Not IsValidEmail(txtEmail.Text) Then
            ShowError("Please enter a valid email address.")
            txtEmail.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(txtPassword.Text) Then
            ShowError("Password is required.")
            txtPassword.Focus()
            Return
        End If

        If txtPassword.Text.Length < 6 Then
            ShowError("Password must be at least 6 characters long.")
            txtPassword.Focus()
            Return
        End If

        If txtPassword.Text <> txtConfirmPassword.Text Then
            ShowError("Passwords do not match.")
            txtConfirmPassword.Focus()
            Return
        End If

        ' Check username availability
        Dim userDAL As New UserDAL()
        If Not userDAL.IsUsernameAvailable(txtUsername.Text.Trim()) Then
            ShowError("Username is already taken. Please choose a different username.")
            txtUsername.Focus()
            Return
        End If

        ' Create user object
        Dim newUser As New User With {
            .Username = txtUsername.Text.Trim(),
            .Password = txtPassword.Text,
            .FullName = txtFullName.Text.Trim(),
            .Email = txtEmail.Text.Trim(),
            .UserType = "Client"
        }

        ' Create account
        Try
            If userDAL.CreateClientAccount(newUser) Then
                MessageBox.Show("Client account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                ShowError("Failed to create account. Please try again.")
            End If
        Catch ex As Exception
            ShowError("Error creating account: " & ex.Message)
        End Try
    End Sub

    Private Sub ShowError(message As String)
        lblError.Text = message
        lblError.Visible = True
    End Sub

    Private Function IsValidEmail(email As String) As Boolean
        Try
            Dim addr As New System.Net.Mail.MailAddress(email)
            Return addr.Address = email
        Catch
            Return False
        End Try
    End Function
End Class