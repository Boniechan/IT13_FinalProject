Imports System.Windows.Forms
Imports System.Drawing

Public Class ReportDateForm
    Inherits Form

    Private ReadOnly dtpReportDate As New DateTimePicker()
    Private ReadOnly btnOK As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly lblTitle As New Label()

    Public Property SelectedDate As DateTime
        Get
            Return dtpReportDate.Value.Date
        End Get
        Set(value As DateTime)
            dtpReportDate.Value = value
        End Set
    End Property

    Public Sub New(title As String)
        InitializeComponent(title)
        SelectedDate = DateTime.Today
    End Sub

    Private Sub InitializeComponent(title As String)
        Me.Text = "Select Report Date"
        Me.Size = New Size(400, 180)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)

        lblTitle.Text = title
        lblTitle.Location = New Point(20, 20)
        lblTitle.Size = New Size(350, 25)
        lblTitle.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold, GraphicsUnit.Point)

        dtpReportDate.Location = New Point(20, 55)
        dtpReportDate.Size = New Size(350, 30)
        dtpReportDate.Format = DateTimePickerFormat.Long
        dtpReportDate.Value = DateTime.Today

        btnOK.Text = "Generate Report"
        btnOK.Location = New Point(200, 100)
        btnOK.Size = New Size(120, 35)
        btnOK.BackColor = Color.FromArgb(40, 167, 69)
        btnOK.ForeColor = Color.White
        btnOK.FlatStyle = FlatStyle.Flat
        btnOK.FlatAppearance.BorderSize = 0
        btnOK.DialogResult = DialogResult.OK
        AddHandler btnOK.Click, Sub(s, e) Me.DialogResult = DialogResult.OK

        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(330, 100)
        btnCancel.Size = New Size(80, 35)
        btnCancel.BackColor = Color.FromArgb(119, 119, 119)
        btnCancel.ForeColor = Color.White
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.DialogResult = DialogResult.Cancel
        AddHandler btnCancel.Click, Sub(s, e) Me.DialogResult = DialogResult.Cancel

        Me.Controls.Add(lblTitle)
        Me.Controls.Add(dtpReportDate)
        Me.Controls.Add(btnOK)
        Me.Controls.Add(btnCancel)

        Me.AcceptButton = btnOK
        Me.CancelButton = btnCancel
    End Sub
End Class