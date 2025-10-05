Imports System.Windows.Forms
Imports System.Drawing

Public Class MonthYearSelectionForm
    Inherits Form

    Private ReadOnly cmbMonth As New ComboBox()
    Private ReadOnly cmbYear As New ComboBox()
    Private ReadOnly btnOK As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly lblMonth As New Label()
    Private ReadOnly lblYear As New Label()

    Public ReadOnly Property SelectedMonth As Integer
        Get
            Return cmbMonth.SelectedIndex + 1
        End Get
    End Property

    Public ReadOnly Property SelectedYear As Integer
        Get
            Return CInt(cmbYear.SelectedItem)
        End Get
    End Property

    Public Sub New()
        InitializeComponent()
        SetupData()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Select Month and Year"
        Me.Size = New Size(350, 200)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)

        lblMonth.Text = "Month:"
        lblMonth.Location = New Point(20, 30)
        lblMonth.Size = New Size(60, 25)
        lblMonth.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold, GraphicsUnit.Point)

        cmbMonth.Location = New Point(90, 28)
        cmbMonth.Size = New Size(200, 30)
        cmbMonth.DropDownStyle = ComboBoxStyle.DropDownList

        lblYear.Text = "Year:"
        lblYear.Location = New Point(20, 70)
        lblYear.Size = New Size(60, 25)
        lblYear.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold, GraphicsUnit.Point)

        cmbYear.Location = New Point(90, 68)
        cmbYear.Size = New Size(200, 30)
        cmbYear.DropDownStyle = ComboBoxStyle.DropDownList

        btnOK.Text = "Generate Report"
        btnOK.Location = New Point(140, 120)
        btnOK.Size = New Size(120, 35)
        btnOK.BackColor = Color.FromArgb(40, 167, 69)
        btnOK.ForeColor = Color.White
        btnOK.FlatStyle = FlatStyle.Flat
        btnOK.FlatAppearance.BorderSize = 0
        btnOK.DialogResult = DialogResult.OK
        AddHandler btnOK.Click, Sub(s, e) Me.DialogResult = DialogResult.OK

        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(270, 120)
        btnCancel.Size = New Size(80, 35)
        btnCancel.BackColor = Color.FromArgb(119, 119, 119)
        btnCancel.ForeColor = Color.White
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.DialogResult = DialogResult.Cancel
        AddHandler btnCancel.Click, Sub(s, e) Me.DialogResult = DialogResult.Cancel

        Me.Controls.Add(lblMonth)
        Me.Controls.Add(cmbMonth)
        Me.Controls.Add(lblYear)
        Me.Controls.Add(cmbYear)
        Me.Controls.Add(btnOK)
        Me.Controls.Add(btnCancel)

        Me.AcceptButton = btnOK
        Me.CancelButton = btnCancel
    End Sub

    Private Sub SetupData()
        ' Add months
        Dim months() As String = {"January", "February", "March", "April", "May", "June",
                                 "July", "August", "September", "October", "November", "December"}
        cmbMonth.Items.AddRange(months)
        cmbMonth.SelectedIndex = DateTime.Now.Month - 1

        ' Add years (current year and 5 years back)
        Dim currentYear As Integer = DateTime.Now.Year
        For year As Integer = currentYear To currentYear - 5 Step -1
            cmbYear.Items.Add(year)
        Next
        cmbYear.SelectedIndex = 0
    End Sub
End Class