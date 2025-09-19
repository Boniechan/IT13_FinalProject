Imports System.Drawing
Imports System.Windows.Forms

Public Class DashboardForm
    Inherits Form

    Private ReadOnly lblWelcome As New Label()
    Private ReadOnly lblRole As New Label()
    Private ReadOnly btnProducts As New Button()
    Private ReadOnly btnSuppliers As New Button()  ' ADD THIS MISSING DECLARATION
    Private ReadOnly btnRecordSale As New Button()
    Private ReadOnly btnSalesHistory As New Button()
    Private ReadOnly btnLogout As New Button()
    Private ReadOnly btnExit As New Button()

    ' Sales History Panel
    Private ReadOnly dgvSalesHistory As New DataGridView()
    Private ReadOnly pnlSalesHistory As New Panel()
    Private ReadOnly btnCloseHistory As New Button()

    ' Products Panel
    Private ReadOnly dgvProducts As New DataGridView()
    Private ReadOnly pnlProducts As New Panel()
    Private ReadOnly btnAddProduct As New Button()
    Private ReadOnly btnEditProduct As New Button()
    Private ReadOnly btnDeleteProduct As New Button()
    Private ReadOnly btnCloseProducts As New Button()

    ' Suppliers Panel
    Private ReadOnly dgvSuppliers As New DataGridView()
    Private ReadOnly pnlSuppliers As New Panel()
    Private ReadOnly btnAddSupplier As New Button()
    Private ReadOnly btnEditSupplier As New Button()
    Private ReadOnly btnDeleteSupplier As New Button()
    Private ReadOnly btnCloseSuppliers As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Admin Dashboard - Fisheries POS"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimumSize = New Size(1200, 700)
        Me.ClientSize = New Size(1200, 700)
        Me.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        Me.BackColor = Color.FromArgb(245, 245, 245)

        ' Header section with padding
        lblWelcome.AutoSize = True
        lblWelcome.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblWelcome.Location = New Point(40, 30)
        lblWelcome.ForeColor = Color.FromArgb(51, 51, 51)

        lblRole.AutoSize = True
        lblRole.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        lblRole.Location = New Point(40, 65)
        lblRole.ForeColor = Color.FromArgb(102, 102, 102)

        ' Button panel with proper spacing
        btnProducts.Text = "Manage Products"
        btnProducts.Location = New Point(40, 120)
        btnProducts.Size = New Size(200, 50)
        btnProducts.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnProducts.BackColor = Color.FromArgb(51, 122, 183)
        btnProducts.ForeColor = Color.White
        btnProducts.FlatStyle = FlatStyle.Flat
        btnProducts.FlatAppearance.BorderSize = 0
        btnProducts.Margin = New Padding(5)
        AddHandler btnProducts.Click, AddressOf btnProducts_Click

        btnSuppliers.Text = "Manage Suppliers"
        btnSuppliers.Location = New Point(40, 185)
        btnSuppliers.Size = New Size(200, 50)
        btnSuppliers.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnSuppliers.BackColor = Color.FromArgb(153, 102, 255)
        btnSuppliers.ForeColor = Color.White
        btnSuppliers.FlatStyle = FlatStyle.Flat
        btnSuppliers.FlatAppearance.BorderSize = 0
        btnSuppliers.Margin = New Padding(5)
        AddHandler btnSuppliers.Click, AddressOf btnSuppliers_Click

        btnRecordSale.Text = "Record Sale"
        btnRecordSale.Location = New Point(40, 250)
        btnRecordSale.Size = New Size(200, 50)
        btnRecordSale.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnRecordSale.BackColor = Color.FromArgb(92, 184, 92)
        btnRecordSale.ForeColor = Color.White
        btnRecordSale.FlatStyle = FlatStyle.Flat
        btnRecordSale.FlatAppearance.BorderSize = 0
        btnRecordSale.Margin = New Padding(5)
        AddHandler btnRecordSale.Click, AddressOf btnRecordSale_Click

        btnSalesHistory.Text = "Sales History"
        btnSalesHistory.Location = New Point(40, 315)
        btnSalesHistory.Size = New Size(200, 50)
        btnSalesHistory.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnSalesHistory.BackColor = Color.FromArgb(240, 173, 78)
        btnSalesHistory.ForeColor = Color.White
        btnSalesHistory.FlatStyle = FlatStyle.Flat
        btnSalesHistory.FlatAppearance.BorderSize = 0
        btnSalesHistory.Margin = New Padding(5)
        AddHandler btnSalesHistory.Click, AddressOf btnSalesHistory_Click

        btnLogout.Text = "Logout"
        btnLogout.Location = New Point(40, 385)
        btnLogout.Size = New Size(95, 40)
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnLogout.BackColor = Color.FromArgb(217, 83, 79)
        btnLogout.ForeColor = Color.White
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        btnExit.Text = "Exit"
        btnExit.Location = New Point(145, 385)
        btnExit.Size = New Size(95, 40)
        btnExit.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnExit.BackColor = Color.FromArgb(119, 119, 119)
        btnExit.ForeColor = Color.White
        btnExit.FlatStyle = FlatStyle.Flat
        btnExit.FlatAppearance.BorderSize = 0
        AddHandler btnExit.Click, AddressOf btnExit_Click

        ' Setup panels
        SetupSalesHistoryPanel()
        SetupProductsPanel()
        SetupSuppliersPanel()

        ' Add all controls to form
        Me.Controls.Add(pnlSalesHistory)
        Me.Controls.Add(pnlProducts)
        Me.Controls.Add(pnlSuppliers)
        Me.Controls.Add(lblWelcome)
        Me.Controls.Add(lblRole)
        Me.Controls.Add(btnProducts)
        Me.Controls.Add(btnSuppliers)
        Me.Controls.Add(btnRecordSale)
        Me.Controls.Add(btnSalesHistory)
        Me.Controls.Add(btnLogout)
        Me.Controls.Add(btnExit)

        AddHandler Me.Load, AddressOf DashboardForm_Load
    End Sub

    Private Sub SetupSalesHistoryPanel()
        pnlSalesHistory.Size = New Size(850, 520)
        pnlSalesHistory.Location = New Point(280, 120)
        pnlSalesHistory.Visible = False
        pnlSalesHistory.BackColor = Color.White
        pnlSalesHistory.BorderStyle = BorderStyle.FixedSingle

        dgvSalesHistory.Size = New Size(810, 440)
        dgvSalesHistory.Location = New Point(20, 20)
        dgvSalesHistory.ReadOnly = True
        dgvSalesHistory.AllowUserToAddRows = False
        dgvSalesHistory.AllowUserToDeleteRows = False
        dgvSalesHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvSalesHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvSalesHistory.RowHeadersVisible = False
        dgvSalesHistory.BorderStyle = BorderStyle.None
        dgvSalesHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvSalesHistory.DefaultCellStyle.Padding = New Padding(5)
        dgvSalesHistory.ColumnHeadersHeight = 35
        dgvSalesHistory.RowTemplate.Height = 30

        btnCloseHistory.Text = "Close"
        btnCloseHistory.Size = New Size(100, 35)
        btnCloseHistory.Location = New Point(730, 470)
        btnCloseHistory.BackColor = Color.FromArgb(119, 119, 119)
        btnCloseHistory.ForeColor = Color.White
        btnCloseHistory.FlatStyle = FlatStyle.Flat
        btnCloseHistory.FlatAppearance.BorderSize = 0
        AddHandler btnCloseHistory.Click, Sub(s, e) HideAllPanels()

        pnlSalesHistory.Controls.Add(dgvSalesHistory)
        pnlSalesHistory.Controls.Add(btnCloseHistory)
    End Sub

    Private Sub SetupProductsPanel()
        pnlProducts.Size = New Size(850, 520)
        pnlProducts.Location = New Point(280, 120)
        pnlProducts.Visible = False
        pnlProducts.BackColor = Color.White
        pnlProducts.BorderStyle = BorderStyle.FixedSingle

        dgvProducts.Size = New Size(810, 380)
        dgvProducts.Location = New Point(20, 20)
        dgvProducts.ReadOnly = True
        dgvProducts.AllowUserToAddRows = False
        dgvProducts.AllowUserToDeleteRows = False
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvProducts.RowHeadersVisible = False
        dgvProducts.BorderStyle = BorderStyle.None
        dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvProducts.DefaultCellStyle.Padding = New Padding(5)
        dgvProducts.ColumnHeadersHeight = 35
        dgvProducts.RowTemplate.Height = 30

        btnAddProduct.Text = "Add Product"
        btnAddProduct.Size = New Size(120, 40)
        btnAddProduct.Location = New Point(20, 420)
        btnAddProduct.BackColor = Color.FromArgb(92, 184, 92)
        btnAddProduct.ForeColor = Color.White
        btnAddProduct.FlatStyle = FlatStyle.Flat
        btnAddProduct.FlatAppearance.BorderSize = 0
        AddHandler btnAddProduct.Click, AddressOf btnAddProduct_Click

        btnEditProduct.Text = "Edit Product"
        btnEditProduct.Size = New Size(120, 40)
        btnEditProduct.Location = New Point(155, 420)
        btnEditProduct.BackColor = Color.FromArgb(51, 122, 183)
        btnEditProduct.ForeColor = Color.White
        btnEditProduct.FlatStyle = FlatStyle.Flat
        btnEditProduct.FlatAppearance.BorderSize = 0
        AddHandler btnEditProduct.Click, AddressOf btnEditProduct_Click

        btnDeleteProduct.Text = "Delete Product"
        btnDeleteProduct.Size = New Size(120, 40)
        btnDeleteProduct.Location = New Point(290, 420)
        btnDeleteProduct.BackColor = Color.FromArgb(217, 83, 79)
        btnDeleteProduct.ForeColor = Color.White
        btnDeleteProduct.FlatStyle = FlatStyle.Flat
        btnDeleteProduct.FlatAppearance.BorderSize = 0
        AddHandler btnDeleteProduct.Click, AddressOf btnDeleteProduct_Click

        btnCloseProducts.Text = "Close"
        btnCloseProducts.Size = New Size(100, 40)
        btnCloseProducts.Location = New Point(730, 420)
        btnCloseProducts.BackColor = Color.FromArgb(119, 119, 119)
        btnCloseProducts.ForeColor = Color.White
        btnCloseProducts.FlatStyle = FlatStyle.Flat
        btnCloseProducts.FlatAppearance.BorderSize = 0
        AddHandler btnCloseProducts.Click, Sub(s, e) HideAllPanels()

        pnlProducts.Controls.Add(dgvProducts)
        pnlProducts.Controls.Add(btnAddProduct)
        pnlProducts.Controls.Add(btnEditProduct)
        pnlProducts.Controls.Add(btnDeleteProduct)
        pnlProducts.Controls.Add(btnCloseProducts)
    End Sub

    Private Sub SetupSuppliersPanel()
        pnlSuppliers.Size = New Size(850, 520)
        pnlSuppliers.Location = New Point(280, 120)
        pnlSuppliers.Visible = False
        pnlSuppliers.BackColor = Color.White
        pnlSuppliers.BorderStyle = BorderStyle.FixedSingle

        dgvSuppliers.Size = New Size(810, 380)
        dgvSuppliers.Location = New Point(20, 20)
        dgvSuppliers.ReadOnly = True
        dgvSuppliers.AllowUserToAddRows = False
        dgvSuppliers.AllowUserToDeleteRows = False
        dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvSuppliers.RowHeadersVisible = False
        dgvSuppliers.BorderStyle = BorderStyle.None
        dgvSuppliers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvSuppliers.DefaultCellStyle.Padding = New Padding(5)
        dgvSuppliers.ColumnHeadersHeight = 35
        dgvSuppliers.RowTemplate.Height = 30

        btnAddSupplier.Text = "Add Supplier"
        btnAddSupplier.Size = New Size(120, 40)
        btnAddSupplier.Location = New Point(20, 420)
        btnAddSupplier.BackColor = Color.FromArgb(92, 184, 92)
        btnAddSupplier.ForeColor = Color.White
        btnAddSupplier.FlatStyle = FlatStyle.Flat
        btnAddSupplier.FlatAppearance.BorderSize = 0
        AddHandler btnAddSupplier.Click, AddressOf btnAddSupplier_Click

        btnEditSupplier.Text = "Edit Supplier"
        btnEditSupplier.Size = New Size(120, 40)
        btnEditSupplier.Location = New Point(155, 420)
        btnEditSupplier.BackColor = Color.FromArgb(51, 122, 183)
        btnEditSupplier.ForeColor = Color.White
        btnEditSupplier.FlatStyle = FlatStyle.Flat
        btnEditSupplier.FlatAppearance.BorderSize = 0
        AddHandler btnEditSupplier.Click, AddressOf btnEditSupplier_Click

        btnDeleteSupplier.Text = "Delete Supplier"
        btnDeleteSupplier.Size = New Size(120, 40)
        btnDeleteSupplier.Location = New Point(290, 420)
        btnDeleteSupplier.BackColor = Color.FromArgb(217, 83, 79)
        btnDeleteSupplier.ForeColor = Color.White
        btnDeleteSupplier.FlatStyle = FlatStyle.Flat
        btnDeleteSupplier.FlatAppearance.BorderSize = 0
        AddHandler btnDeleteSupplier.Click, AddressOf btnDeleteSupplier_Click

        btnCloseSuppliers.Text = "Close"
        btnCloseSuppliers.Size = New Size(100, 40)
        btnCloseSuppliers.Location = New Point(730, 420)
        btnCloseSuppliers.BackColor = Color.FromArgb(119, 119, 119)
        btnCloseSuppliers.ForeColor = Color.White
        btnCloseSuppliers.FlatStyle = FlatStyle.Flat
        btnCloseSuppliers.FlatAppearance.BorderSize = 0
        AddHandler btnCloseSuppliers.Click, Sub(s, e) HideAllPanels()

        pnlSuppliers.Controls.Add(dgvSuppliers)
        pnlSuppliers.Controls.Add(btnAddSupplier)
        pnlSuppliers.Controls.Add(btnEditSupplier)
        pnlSuppliers.Controls.Add(btnDeleteSupplier)
        pnlSuppliers.Controls.Add(btnCloseSuppliers)
    End Sub

    Private Sub HideAllPanels()
        pnlSalesHistory.Visible = False
        pnlProducts.Visible = False
        pnlSuppliers.Visible = False
    End Sub

    Private Sub DashboardForm_Load(sender As Object, e As EventArgs)
        If UserSession.UserID = 0 Then
            MessageBox.Show("No active session. Please login.")
            Dim login As New LoginForm()
            login.Show()
            Me.Close()
            Return
        End If

        lblWelcome.Text = $"Welcome, {UserSession.FullName}"
        lblRole.Text = $"Role: {UserSession.UserType}"
    End Sub

    Private Sub btnProducts_Click(sender As Object, e As EventArgs)
        ShowProductPanel()
    End Sub

    Private Sub btnSuppliers_Click(sender As Object, e As EventArgs)
        ShowSupplierPanel()
    End Sub

    Private Sub ShowProductPanel()
        HideAllPanels()
        Dim dal As New ProductDAL()
        Dim products = dal.GetAllProducts()
        dgvProducts.DataSource = Nothing
        dgvProducts.DataSource = products
        pnlProducts.Visible = True
    End Sub

    Private Sub ShowSupplierPanel()
        HideAllPanels()
        Dim dal As New SupplierDAL()
        Dim suppliers = dal.GetAllSuppliers()
        dgvSuppliers.DataSource = Nothing
        dgvSuppliers.DataSource = suppliers
        pnlSuppliers.Visible = True
    End Sub

    Private Sub btnAddProduct_Click(sender As Object, e As EventArgs)
        Dim frm As New ProductEditForm()
        If frm.ShowDialog() = DialogResult.OK Then
            ShowProductPanel()
        End If
    End Sub

    Private Sub btnEditProduct_Click(sender As Object, e As EventArgs)
        If dgvProducts.CurrentRow Is Nothing Then Return
        Dim prod As Product = TryCast(dgvProducts.CurrentRow.DataBoundItem, Product)
        If prod Is Nothing Then Return
        Dim frm As New ProductEditForm(prod)
        If frm.ShowDialog() = DialogResult.OK Then
            ShowProductPanel()
        End If
    End Sub

    Private Sub btnDeleteProduct_Click(sender As Object, e As EventArgs)
        If dgvProducts.CurrentRow Is Nothing Then Return
        Dim prod As Product = TryCast(dgvProducts.CurrentRow.DataBoundItem, Product)
        If prod Is Nothing Then Return
        If MessageBox.Show($"Delete product '{prod.ProductName}'?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim dal As New ProductDAL()
            If dal.DeleteProduct(prod.ProductID) Then
                MessageBox.Show("Product deleted.")
                ShowProductPanel()
            Else
                MessageBox.Show("Delete failed.")
            End If
        End If
    End Sub

    Private Sub btnAddSupplier_Click(sender As Object, e As EventArgs)
        Dim frm As New SupplierEditForm()
        If frm.ShowDialog() = DialogResult.OK Then
            ShowSupplierPanel()
        End If
    End Sub

    Private Sub btnEditSupplier_Click(sender As Object, e As EventArgs)
        If dgvSuppliers.CurrentRow Is Nothing Then Return
        Dim supplier As Supplier = TryCast(dgvSuppliers.CurrentRow.DataBoundItem, Supplier)
        If supplier Is Nothing Then Return
        Dim frm As New SupplierEditForm(supplier)
        If frm.ShowDialog() = DialogResult.OK Then
            ShowSupplierPanel()
        End If
    End Sub

    Private Sub btnDeleteSupplier_Click(sender As Object, e As EventArgs)
        If dgvSuppliers.CurrentRow Is Nothing Then Return
        Dim supplier As Supplier = TryCast(dgvSuppliers.CurrentRow.DataBoundItem, Supplier)
        If supplier Is Nothing Then Return
        If MessageBox.Show($"Delete supplier '{supplier.SupplierName}'?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Dim dal As New SupplierDAL()
            If dal.DeleteSupplier(supplier.SupplierID) Then
                MessageBox.Show("Supplier deleted.")
                ShowSupplierPanel()
            Else
                MessageBox.Show("Delete failed.")
            End If
        End If
    End Sub

    Private Sub btnRecordSale_Click(sender As Object, e As EventArgs)
        Dim saleForm As New SaleRecordForm()
        If saleForm.ShowDialog() = DialogResult.OK Then
            MessageBox.Show("Sale recorded successfully!")
        End If
    End Sub

    Private Sub btnSalesHistory_Click(sender As Object, e As EventArgs)
        ShowAllOrderHistory()
    End Sub

    Private Sub ShowAllOrderHistory()
        HideAllPanels()
        Dim dal As New SaleDAL()
        Dim allOrders = dal.GetOrderHistory()
        dgvSalesHistory.DataSource = Nothing
        dgvSalesHistory.DataSource = allOrders
        pnlSalesHistory.Visible = True
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        UserSession.ClearSession()
        Dim login As New LoginForm()
        AddHandler login.FormClosed, Sub(s, args) Me.Close()
        login.Show()
        Me.Hide()
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub
End Class