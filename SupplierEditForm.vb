Imports System.Drawing
Imports System.Windows.Forms

Public Class SupplierEditForm
    Inherits Form

    Private ReadOnly lblTitle As New Label()
    Private ReadOnly lblSupplierName As New Label()
    Private ReadOnly lblContactPerson As New Label()
    Private ReadOnly lblPhone As New Label()
    Private ReadOnly lblEmail As New Label()
    Private ReadOnly lblAddress As New Label()

    Private ReadOnly txtSupplierName As New TextBox()
    Private ReadOnly txtContactPerson As New TextBox()
    Private ReadOnly txtPhone As New TextBox()
    Private ReadOnly txtEmail As New TextBox()
    Private ReadOnly txtAddress As New TextBox()

    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnCancel As New Button()

    Private supplier As Supplier
    Private isEditMode As Boolean

    Public Sub New()
        InitializeComponent()
        isEditMode = False
        lblTitle.Text = "Add New Supplier"
    End Sub

    Public Sub New(existingSupplier As Supplier)
        InitializeComponent()
        supplier = existingSupplier
        isEditMode = True
        lblTitle.Text = "Edit Supplier"
        LoadSupplierData()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Supplier Management"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ClientSize = New Size(500, 450)
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)

        ' Title
        lblTitle.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblTitle.AutoSize = True
        lblTitle.Location = New Point(20, 20)

        ' Supplier Name
        lblSupplierName.Text = "Supplier Name:"
        lblSupplierName.AutoSize = True
        lblSupplierName.Location = New Point(20, 70)

        txtSupplierName.Location = New Point(20, 90)
        txtSupplierName.Size = New Size(450, 27)

        ' Contact Person
        lblContactPerson.Text = "Contact Person:"
        lblContactPerson.AutoSize = True
        lblContactPerson.Location = New Point(20, 130)

        txtContactPerson.Location = New Point(20, 150)
        txtContactPerson.Size = New Size(450, 27)

        ' Phone
        lblPhone.Text = "Phone:"
        lblPhone.AutoSize = True
        lblPhone.Location = New Point(20, 190)

        txtPhone.Location = New Point(20, 210)
        txtPhone.Size = New Size(220, 27)

        ' Email
        lblEmail.Text = "Email:"
        lblEmail.AutoSize = True
        lblEmail.Location = New Point(250, 190)

        txtEmail.Location = New Point(250, 210)
        txtEmail.Size = New Size(220, 27)

        ' Address
        lblAddress.Text = "Address:"
        lblAddress.AutoSize = True
        lblAddress.Location = New Point(20, 250)

        txtAddress.Location = New Point(20, 270)
        txtAddress.Size = New Size(450, 80)
        txtAddress.Multiline = True
        txtAddress.ScrollBars = ScrollBars.Vertical

        ' Save Button
        btnSave.Text = "Save"
        btnSave.Location = New Point(270, 370)
        btnSave.Size = New Size(100, 35)
        btnSave.BackColor = Color.FromArgb(92, 184, 92)
        btnSave.ForeColor = Color.White
        btnSave.FlatStyle = FlatStyle.Flat
        AddHandler btnSave.Click, AddressOf btnSave_Click

        ' Cancel Button
        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(380, 370)
        btnCancel.Size = New Size(100, 35)
        btnCancel.BackColor = Color.FromArgb(217, 83, 79)
        btnCancel.ForeColor = Color.White
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.DialogResult = DialogResult.Cancel
        AddHandler btnCancel.Click, Sub(s, e) Me.Close()

        ' Add all controls
        Me.Controls.AddRange({
            lblTitle, lblSupplierName, txtSupplierName,
            lblContactPerson, txtContactPerson, lblPhone, txtPhone,
            lblEmail, txtEmail, lblAddress, txtAddress,
            btnSave, btnCancel
        })

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel
    End Sub

    Private Sub LoadSupplierData()
        If supplier IsNot Nothing Then
            txtSupplierName.Text = supplier.SupplierName
            txtContactPerson.Text = supplier.ContactPerson
            txtPhone.Text = supplier.Phone
            txtEmail.Text = supplier.Email
            txtAddress.Text = supplier.Address
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        If Not ValidateInput() Then Return

        Try
            If supplier Is Nothing Then
                supplier = New Supplier()
            End If

            supplier.SupplierName = txtSupplierName.Text.Trim()
            supplier.ContactPerson = txtContactPerson.Text.Trim()
            supplier.Phone = txtPhone.Text.Trim()
            supplier.Email = txtEmail.Text.Trim()
            supplier.Address = txtAddress.Text.Trim()

            Dim dal As New SupplierDAL()
            Dim success As Boolean

            If isEditMode Then
                success = dal.UpdateSupplier(supplier)
            Else
                success = dal.AddSupplier(supplier)
            End If

            If success Then
                MessageBox.Show($"Supplier {If(isEditMode, "updated", "added")} successfully!")
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                MessageBox.Show($"Failed to {If(isEditMode, "update", "add")} supplier.")
            End If

        Catch ex As Exception
            MessageBox.Show($"Error saving supplier: {ex.Message}")
        End Try
    End Sub

    Private Function ValidateInput() As Boolean
        If String.IsNullOrWhiteSpace(txtSupplierName.Text) Then
            MessageBox.Show("Supplier name is required.")
            txtSupplierName.Focus()
            Return False
        End If

        If txtSupplierName.Text.Length > 100 Then
            MessageBox.Show("Supplier name must be less than 100 characters.")
            txtSupplierName.Focus()
            Return False
        End If

        Return True
    End Function
End Class