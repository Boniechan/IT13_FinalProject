Imports System.Drawing
Imports System.Windows.Forms

Public Class ProductEditForm
    Inherits Form

    Private ReadOnly txtName As New TextBox()
    Private ReadOnly txtCategory As New TextBox()
    Private ReadOnly txtPrice As New TextBox()
    Private ReadOnly txtStock As New TextBox()
    Private ReadOnly txtDescription As New TextBox()
    Private ReadOnly txtSupplierID As New TextBox()
    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnCancel As New Button()

    Private _product As Product

    Public Sub New()
        Me.New(Nothing)
    End Sub

    Public Sub New(prod As Product)
        _product = prod
        InitializeComponent()
        If prod IsNot Nothing Then
            txtName.Text = prod.ProductName
            txtCategory.Text = prod.Category
            txtPrice.Text = prod.Price.ToString()
            txtStock.Text = prod.Stock.ToString()
            txtDescription.Text = prod.Description
            txtSupplierID.Text = prod.SupplierID.ToString()
        End If
    End Sub

    Private Sub InitializeComponent()
        Me.Text = If(_product Is Nothing, "Add Product", "Edit Product")
        Me.Size = New Size(350, 350)
        Me.StartPosition = FormStartPosition.CenterParent

        Dim lblName As New Label() With {.Text = "Name", .Location = New Point(20, 20)}
        txtName.Location = New Point(120, 20)
        txtName.Size = New Size(180, 25)

        Dim lblCategory As New Label() With {.Text = "Category", .Location = New Point(20, 60)}
        txtCategory.Location = New Point(120, 60)
        txtCategory.Size = New Size(180, 25)

        Dim lblPrice As New Label() With {.Text = "Price", .Location = New Point(20, 100)}
        txtPrice.Location = New Point(120, 100)
        txtPrice.Size = New Size(180, 25)

        Dim lblStock As New Label() With {.Text = "Stock", .Location = New Point(20, 140)}
        txtStock.Location = New Point(120, 140)
        txtStock.Size = New Size(180, 25)

        Dim lblDescription As New Label() With {.Text = "Description", .Location = New Point(20, 180)}
        txtDescription.Location = New Point(120, 180)
        txtDescription.Size = New Size(180, 25)

        Dim lblSupplierID As New Label() With {.Text = "SupplierID", .Location = New Point(20, 220)}
        txtSupplierID.Location = New Point(120, 220)
        txtSupplierID.Size = New Size(180, 25)

        btnSave.Text = "Save"
        btnSave.Location = New Point(120, 260)
        btnSave.Size = New Size(80, 30)
        AddHandler btnSave.Click, AddressOf btnSave_Click

        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(220, 260)
        btnCancel.Size = New Size(80, 30)
        btnCancel.DialogResult = DialogResult.Cancel

        Me.Controls.Add(lblName)
        Me.Controls.Add(txtName)
        Me.Controls.Add(lblCategory)
        Me.Controls.Add(txtCategory)
        Me.Controls.Add(lblPrice)
        Me.Controls.Add(txtPrice)
        Me.Controls.Add(lblStock)
        Me.Controls.Add(txtStock)
        Me.Controls.Add(lblDescription)
        Me.Controls.Add(txtDescription)
        Me.Controls.Add(lblSupplierID)
        Me.Controls.Add(txtSupplierID)
        Me.Controls.Add(btnSave)
        Me.Controls.Add(btnCancel)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim prod As New Product With {
            .ProductName = txtName.Text.Trim(),
            .Category = txtCategory.Text.Trim(),
            .Price = Decimal.Parse(txtPrice.Text),
            .Stock = Integer.Parse(txtStock.Text),
            .Description = txtDescription.Text.Trim(),
            .SupplierID = Integer.Parse(txtSupplierID.Text)
        }
        Dim dal As New ProductDAL()
        Dim success As Boolean
        If _product Is Nothing Then
            success = dal.AddProduct(prod)
        Else
            prod.ProductID = _product.ProductID
            success = dal.UpdateProduct(prod)
        End If
        If success Then
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            MessageBox.Show("Save failed.")
        End If
    End Sub
End Class