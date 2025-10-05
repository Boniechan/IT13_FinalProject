Imports System.Drawing
Imports System.Windows.Forms
Imports System.Linq

Public Class DashboardForm
    Inherits Form

    Private ReadOnly lblWelcome As New Label()
    Private ReadOnly lblRole As New Label()
    Private ReadOnly btnProducts As New Button()
    Private ReadOnly btnSuppliers As New Button()
    Private ReadOnly btnRecordSale As New Button()
    Private ReadOnly btnSalesHistory As New Button()
    Private ReadOnly btnClients As New Button()
    Private ReadOnly btnLogout As New Button()
    Private ReadOnly btnExit As New Button()
    Private ReadOnly btnGenerateReports As New Button()

    ' Sales History Panel with two DataGridViews
    Private ReadOnly pnlSalesHistory As New Panel()
    Private ReadOnly lblClientSales As New Label()
    Private ReadOnly lblAdminOrders As New Label()
    Private ReadOnly dgvClientSales As New DataGridView()
    Private ReadOnly dgvAdminOrders As New DataGridView()
    Private ReadOnly btnCloseHistory As New Button()

    Private ReadOnly dgvProducts As New DataGridView()
    Private ReadOnly pnlProducts As New Panel()
    Private ReadOnly btnAddProduct As New Button()
    Private ReadOnly btnEditProduct As New Button()
    Private ReadOnly btnDeleteProduct As New Button()
    Private ReadOnly btnCloseProducts As New Button()

    Private ReadOnly dgvSuppliers As New DataGridView()
    Private ReadOnly pnlSuppliers As New Panel()
    Private ReadOnly btnAddSupplier As New Button()
    Private ReadOnly btnEditSupplier As New Button()
    Private ReadOnly btnDeleteSupplier As New Button()
    Private ReadOnly btnCloseSuppliers As New Button()

    Private ReadOnly dgvClients As New DataGridView()
    Private ReadOnly pnlClients As New Panel()
    Private ReadOnly btnCreateClient As New Button()
    Private ReadOnly btnCloseClients As New Button()

    Private ReadOnly pnlReports As New Panel()
    Private ReadOnly btnDailyReport As New Button()
    Private ReadOnly btnWeeklyReport As New Button()
    Private ReadOnly btnMonthlyReport As New Button()
    Private ReadOnly btnInventoryReport As New Button()
    Private ReadOnly btnCloseReports As New Button()
    Private ReadOnly lblReportsTitle As New Label()
    Private ReadOnly excelReportManager As New ExcelReportManager()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Admin Dashboard - Fisheries POS"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimumSize = New Size(1300, 750)
        Me.ClientSize = New Size(1300, 750)
        Me.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        Me.BackColor = Color.FromArgb(245, 245, 245)

        lblWelcome.AutoSize = True
        lblWelcome.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblWelcome.Location = New Point(40, 30)
        lblWelcome.ForeColor = Color.FromArgb(51, 51, 51)

        lblRole.AutoSize = True
        lblRole.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        lblRole.Location = New Point(40, 65)
        lblRole.ForeColor = Color.FromArgb(102, 102, 102)

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

        btnRecordSale.Text = "Buy Products"
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

        btnClients.Text = "Manage Clients"
        btnClients.Location = New Point(40, 380)
        btnClients.Size = New Size(200, 50)
        btnClients.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnClients.BackColor = Color.FromArgb(255, 87, 34)
        btnClients.ForeColor = Color.White
        btnClients.FlatStyle = FlatStyle.Flat
        btnClients.FlatAppearance.BorderSize = 0
        btnClients.Margin = New Padding(5)
        AddHandler btnClients.Click, AddressOf btnClients_Click

        btnLogout.Text = "Logout"
        btnLogout.Location = New Point(40, 510)
        btnLogout.Size = New Size(95, 40)
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnLogout.BackColor = Color.FromArgb(217, 83, 79)
        btnLogout.ForeColor = Color.White
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        btnExit.Text = "Exit"
        btnExit.Location = New Point(145, 510)
        btnExit.Size = New Size(95, 40)
        btnExit.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnExit.BackColor = Color.FromArgb(119, 119, 119)
        btnExit.ForeColor = Color.White
        btnExit.FlatStyle = FlatStyle.Flat
        btnExit.FlatAppearance.BorderSize = 0
        AddHandler btnExit.Click, AddressOf btnExit_Click

        btnGenerateReports.Text = "Generate Reports"
        btnGenerateReports.Location = New Point(40, 445)
        btnGenerateReports.Size = New Size(200, 50)
        btnGenerateReports.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        btnGenerateReports.BackColor = Color.FromArgb(156, 39, 176)
        btnGenerateReports.ForeColor = Color.White
        btnGenerateReports.FlatStyle = FlatStyle.Flat
        btnGenerateReports.FlatAppearance.BorderSize = 0
        btnGenerateReports.Margin = New Padding(5)
        AddHandler btnGenerateReports.Click, AddressOf btnGenerateReports_Click

        SetupSalesHistoryPanel()
        SetupProductsPanel()
        SetupSuppliersPanel()
        SetupClientsPanel()
        SetupReportsPanel()

        ' Add panels first (they go to the back)
        Me.Controls.Add(pnlSalesHistory)
        Me.Controls.Add(pnlProducts)
        Me.Controls.Add(pnlSuppliers)
        Me.Controls.Add(pnlClients)
        Me.Controls.Add(pnlReports)

        ' Add other controls (they stay in front)
        Me.Controls.Add(lblWelcome)
        Me.Controls.Add(lblRole)
        Me.Controls.Add(btnProducts)
        Me.Controls.Add(btnSuppliers)
        Me.Controls.Add(btnRecordSale)
        Me.Controls.Add(btnSalesHistory)
        Me.Controls.Add(btnClients)
        Me.Controls.Add(btnLogout)
        Me.Controls.Add(btnExit)
        Me.Controls.Add(btnGenerateReports)

        ' Ensure proper z-order - panels should be at the back
        pnlSalesHistory.SendToBack()
        pnlProducts.SendToBack()
        pnlSuppliers.SendToBack()
        pnlClients.SendToBack()
        pnlReports.SendToBack()

        AddHandler Me.Load, AddressOf DashboardForm_Load
    End Sub

    Private Sub SetupSalesHistoryPanel()
        ' Main panel for sales history
        pnlSalesHistory.Size = New Size(980, 570)
        pnlSalesHistory.Location = New Point(280, 100)
        pnlSalesHistory.Visible = False
        pnlSalesHistory.BackColor = Color.White
        pnlSalesHistory.BorderStyle = BorderStyle.FixedSingle

        ' Client Sales Label
        lblClientSales.Text = "Client Sales History"
        lblClientSales.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblClientSales.Location = New Point(20, 15)
        lblClientSales.AutoSize = True
        lblClientSales.ForeColor = Color.FromArgb(51, 122, 183)

        ' Client Sales DataGridView
        dgvClientSales.Size = New Size(940, 200)
        dgvClientSales.Location = New Point(20, 40)
        dgvClientSales.ReadOnly = True
        dgvClientSales.AllowUserToAddRows = False
        dgvClientSales.AllowUserToDeleteRows = False
        dgvClientSales.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvClientSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvClientSales.RowHeadersVisible = False
        dgvClientSales.BorderStyle = BorderStyle.None
        dgvClientSales.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvClientSales.DefaultCellStyle.Padding = New Padding(5)
        dgvClientSales.ColumnHeadersHeight = 35
        dgvClientSales.RowTemplate.Height = 30

        ' Admin Orders Label
        lblAdminOrders.Text = "Admin Order History"
        lblAdminOrders.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblAdminOrders.Location = New Point(20, 260)
        lblAdminOrders.AutoSize = True
        lblAdminOrders.ForeColor = Color.FromArgb(92, 184, 92)

        ' Admin Orders DataGridView
        dgvAdminOrders.Size = New Size(940, 200)
        dgvAdminOrders.Location = New Point(20, 285)
        dgvAdminOrders.ReadOnly = True
        dgvAdminOrders.AllowUserToAddRows = False
        dgvAdminOrders.AllowUserToDeleteRows = False
        dgvAdminOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvAdminOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvAdminOrders.RowHeadersVisible = False
        dgvAdminOrders.BorderStyle = BorderStyle.None
        dgvAdminOrders.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvAdminOrders.DefaultCellStyle.Padding = New Padding(5)
        dgvAdminOrders.ColumnHeadersHeight = 35
        dgvAdminOrders.RowTemplate.Height = 30

        ' Close button
        btnCloseHistory.Text = "Close"
        btnCloseHistory.Size = New Size(100, 35)
        btnCloseHistory.Location = New Point(860, 520)
        btnCloseHistory.BackColor = Color.FromArgb(119, 119, 119)
        btnCloseHistory.ForeColor = Color.White
        btnCloseHistory.FlatStyle = FlatStyle.Flat
        btnCloseHistory.FlatAppearance.BorderSize = 0
        AddHandler btnCloseHistory.Click, Sub(s, e) HideAllPanels()

        ' Add controls to panel
        pnlSalesHistory.Controls.Add(lblClientSales)
        pnlSalesHistory.Controls.Add(dgvClientSales)
        pnlSalesHistory.Controls.Add(lblAdminOrders)
        pnlSalesHistory.Controls.Add(dgvAdminOrders)
        pnlSalesHistory.Controls.Add(btnCloseHistory)
    End Sub

    Private Sub SetupProductsPanel()
        pnlProducts.Size = New Size(880, 570)
        pnlProducts.Location = New Point(280, 100)
        pnlProducts.Visible = False
        pnlProducts.BackColor = Color.White
        pnlProducts.BorderStyle = BorderStyle.FixedSingle

        dgvProducts.Size = New Size(840, 420)
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
        btnAddProduct.Location = New Point(20, 460)
        btnAddProduct.BackColor = Color.FromArgb(92, 184, 92)
        btnAddProduct.ForeColor = Color.White
        btnAddProduct.FlatStyle = FlatStyle.Flat
        btnAddProduct.FlatAppearance.BorderSize = 0
        AddHandler btnAddProduct.Click, AddressOf btnAddProduct_Click

        btnEditProduct.Text = "Edit Product"
        btnEditProduct.Size = New Size(120, 40)
        btnEditProduct.Location = New Point(155, 460)
        btnEditProduct.BackColor = Color.FromArgb(51, 122, 183)
        btnEditProduct.ForeColor = Color.White
        btnEditProduct.FlatStyle = FlatStyle.Flat
        btnEditProduct.FlatAppearance.BorderSize = 0
        AddHandler btnEditProduct.Click, AddressOf btnEditProduct_Click

        btnDeleteProduct.Text = "Delete Product"
        btnDeleteProduct.Size = New Size(120, 40)
        btnDeleteProduct.Location = New Point(290, 460)
        btnDeleteProduct.BackColor = Color.FromArgb(217, 83, 79)
        btnDeleteProduct.ForeColor = Color.White
        btnDeleteProduct.FlatStyle = FlatStyle.Flat
        btnDeleteProduct.FlatAppearance.BorderSize = 0
        AddHandler btnDeleteProduct.Click, AddressOf btnDeleteProduct_Click

        btnCloseProducts.Text = "Close"
        btnCloseProducts.Size = New Size(100, 40)
        btnCloseProducts.Location = New Point(760, 460)
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
        pnlSuppliers.Size = New Size(880, 570)
        pnlSuppliers.Location = New Point(280, 100)
        pnlSuppliers.Visible = False
        pnlSuppliers.BackColor = Color.White
        pnlSuppliers.BorderStyle = BorderStyle.FixedSingle

        dgvSuppliers.Size = New Size(840, 420)
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
        btnAddSupplier.Location = New Point(20, 460)
        btnAddSupplier.BackColor = Color.FromArgb(92, 184, 92)
        btnAddSupplier.ForeColor = Color.White
        btnAddSupplier.FlatStyle = FlatStyle.Flat
        btnAddSupplier.FlatAppearance.BorderSize = 0
        AddHandler btnAddSupplier.Click, AddressOf btnAddSupplier_Click

        btnEditSupplier.Text = "Edit Supplier"
        btnEditSupplier.Size = New Size(120, 40)
        btnEditSupplier.Location = New Point(155, 460)
        btnEditSupplier.BackColor = Color.FromArgb(51, 122, 183)
        btnEditSupplier.ForeColor = Color.White
        btnEditSupplier.FlatStyle = FlatStyle.Flat
        btnEditSupplier.FlatAppearance.BorderSize = 0
        AddHandler btnEditSupplier.Click, AddressOf btnEditSupplier_Click

        btnDeleteSupplier.Text = "Delete Supplier"
        btnDeleteSupplier.Size = New Size(120, 40)
        btnDeleteSupplier.Location = New Point(290, 460)
        btnDeleteSupplier.BackColor = Color.FromArgb(217, 83, 79)
        btnDeleteSupplier.ForeColor = Color.White
        btnDeleteSupplier.FlatStyle = FlatStyle.Flat
        btnDeleteSupplier.FlatAppearance.BorderSize = 0
        AddHandler btnDeleteSupplier.Click, AddressOf btnDeleteSupplier_Click

        btnCloseSuppliers.Text = "Close"
        btnCloseSuppliers.Size = New Size(100, 40)
        btnCloseSuppliers.Location = New Point(760, 460)
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

    Private Sub SetupClientsPanel()
        pnlClients.Size = New Size(880, 570)
        pnlClients.Location = New Point(280, 100)
        pnlClients.Visible = False
        pnlClients.BackColor = Color.White
        pnlClients.BorderStyle = BorderStyle.FixedSingle

        dgvClients.Size = New Size(840, 420)
        dgvClients.Location = New Point(20, 20)
        dgvClients.ReadOnly = True
        dgvClients.AllowUserToAddRows = False
        dgvClients.AllowUserToDeleteRows = False
        dgvClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvClients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvClients.RowHeadersVisible = False
        dgvClients.BorderStyle = BorderStyle.None
        dgvClients.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvClients.DefaultCellStyle.Padding = New Padding(5)
        dgvClients.ColumnHeadersHeight = 35
        dgvClients.RowTemplate.Height = 30

        btnCreateClient.Text = "Create Client Account"
        btnCreateClient.Size = New Size(150, 40)
        btnCreateClient.Location = New Point(20, 460)
        btnCreateClient.BackColor = Color.FromArgb(255, 87, 34)
        btnCreateClient.ForeColor = Color.White
        btnCreateClient.FlatStyle = FlatStyle.Flat
        btnCreateClient.FlatAppearance.BorderSize = 0
        AddHandler btnCreateClient.Click, AddressOf btnCreateClient_Click

        btnCloseClients.Text = "Close"
        btnCloseClients.Size = New Size(100, 40)
        btnCloseClients.Location = New Point(760, 460)
        btnCloseClients.BackColor = Color.FromArgb(119, 119, 119)
        btnCloseClients.ForeColor = Color.White
        btnCloseClients.FlatStyle = FlatStyle.Flat
        btnCloseClients.FlatAppearance.BorderSize = 0
        AddHandler btnCloseClients.Click, Sub(s, e) HideAllPanels()

        pnlClients.Controls.Add(dgvClients)
        pnlClients.Controls.Add(btnCreateClient)
        pnlClients.Controls.Add(btnCloseClients)
    End Sub

    Private Sub SetupReportsPanel()
        pnlReports.Size = New Size(980, 570)
        pnlReports.Location = New Point(280, 100)
        pnlReports.Visible = False
        pnlReports.BackColor = Color.White
        pnlReports.BorderStyle = BorderStyle.FixedSingle

        lblReportsTitle.Text = "Generate Business Reports"
        lblReportsTitle.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblReportsTitle.Location = New Point(20, 20)
        lblReportsTitle.AutoSize = True
        lblReportsTitle.ForeColor = Color.FromArgb(156, 39, 176)

        ' Daily Report Button
        btnDailyReport.Text = "Generate Daily Sales Report"
        btnDailyReport.Size = New Size(250, 60)
        btnDailyReport.Location = New Point(50, 80)
        btnDailyReport.BackColor = Color.FromArgb(40, 167, 69)
        btnDailyReport.ForeColor = Color.White
        btnDailyReport.FlatStyle = FlatStyle.Flat
        btnDailyReport.FlatAppearance.BorderSize = 0
        btnDailyReport.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        AddHandler btnDailyReport.Click, AddressOf btnDailyReport_Click

        ' Weekly Report Button
        btnWeeklyReport.Text = "Generate Weekly Sales Report"
        btnWeeklyReport.Size = New Size(250, 60)
        btnWeeklyReport.Location = New Point(320, 80)
        btnWeeklyReport.BackColor = Color.FromArgb(51, 122, 183)
        btnWeeklyReport.ForeColor = Color.White
        btnWeeklyReport.FlatStyle = FlatStyle.Flat
        btnWeeklyReport.FlatAppearance.BorderSize = 0
        btnWeeklyReport.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        AddHandler btnWeeklyReport.Click, AddressOf btnWeeklyReport_Click

        ' Monthly Report Button
        btnMonthlyReport.Text = "Generate Monthly Sales Report"
        btnMonthlyReport.Size = New Size(250, 60)
        btnMonthlyReport.Location = New Point(590, 80)
        btnMonthlyReport.BackColor = Color.FromArgb(255, 193, 7)
        btnMonthlyReport.ForeColor = Color.White
        btnMonthlyReport.FlatStyle = FlatStyle.Flat
        btnMonthlyReport.FlatAppearance.BorderSize = 0
        btnMonthlyReport.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        AddHandler btnMonthlyReport.Click, AddressOf btnMonthlyReport_Click

        ' Inventory Report Button
        btnInventoryReport.Text = "Generate Inventory Report"
        btnInventoryReport.Size = New Size(250, 60)
        btnInventoryReport.Location = New Point(185, 160)
        btnInventoryReport.BackColor = Color.FromArgb(138, 43, 226)
        btnInventoryReport.ForeColor = Color.White
        btnInventoryReport.FlatStyle = FlatStyle.Flat
        btnInventoryReport.FlatAppearance.BorderSize = 0
        btnInventoryReport.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        AddHandler btnInventoryReport.Click, AddressOf btnInventoryReport_Click

        ' Instructions
        Dim lblInstructions As New Label()
        lblInstructions.Text = "Select a report type to generate. Reports will be saved to your Desktop and can be opened in Excel."
        lblInstructions.Location = New Point(50, 250)
        lblInstructions.Size = New Size(800, 60)
        lblInstructions.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        lblInstructions.ForeColor = Color.FromArgb(102, 102, 102)
        lblInstructions.AutoSize = False
        lblInstructions.TextAlign = ContentAlignment.MiddleCenter

        ' Close button
        btnCloseReports.Text = "Close"
        btnCloseReports.Size = New Size(100, 40)
        btnCloseReports.Location = New Point(840, 520)
        btnCloseReports.BackColor = Color.FromArgb(119, 119, 119)
        btnCloseReports.ForeColor = Color.White
        btnCloseReports.FlatStyle = FlatStyle.Flat
        btnCloseReports.FlatAppearance.BorderSize = 0
        AddHandler btnCloseReports.Click, Sub(s, e) HideAllPanels()

        pnlReports.Controls.Add(lblReportsTitle)
        pnlReports.Controls.Add(btnDailyReport)
        pnlReports.Controls.Add(btnWeeklyReport)
        pnlReports.Controls.Add(btnMonthlyReport)
        pnlReports.Controls.Add(btnInventoryReport)
        pnlReports.Controls.Add(lblInstructions)
        pnlReports.Controls.Add(btnCloseReports)
    End Sub

    Private Sub HideAllPanels()
        pnlSalesHistory.Visible = False
        pnlProducts.Visible = False
        pnlSuppliers.Visible = False
        pnlClients.Visible = False
        pnlReports.Visible = False  ' Add this line
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

    Private Sub SetupDataGridView(dgv As DataGridView)
        dgv.ReadOnly = True
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.RowHeadersVisible = False
        dgv.BorderStyle = BorderStyle.None
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgv.DefaultCellStyle.Padding = New Padding(5)
        dgv.ColumnHeadersHeight = 35
        dgv.RowTemplate.Height = 30
    End Sub

    Private Sub HideRedundantColumns(dgv As DataGridView)
        ' Hide the specified columns to reduce redundancy
        If dgv.Columns.Contains("SaleID") Then
            dgv.Columns("SaleID").Visible = False
        End If

        If dgv.Columns.Contains("UserID") Then
            dgv.Columns("UserID").Visible = False
        End If

        If dgv.Columns.Contains("Username") Then
            dgv.Columns("Username").Visible = False
        End If

        If dgv.Columns.Contains("ProductID") Then
            dgv.Columns("ProductID").Visible = False
        End If
    End Sub

    Private Sub btnProducts_Click(sender As Object, e As EventArgs)
        ShowProductPanel()
    End Sub

    Private Sub btnSuppliers_Click(sender As Object, e As EventArgs)
        ShowSupplierPanel()
    End Sub

    Private Sub btnClients_Click(sender As Object, e As EventArgs)
        ShowClientPanel()
    End Sub

    Private Sub ShowProductPanel()
        HideAllPanels()
        Dim dal As New ProductDAL()
        Dim products = dal.GetAllProducts()
        dgvProducts.DataSource = Nothing
        dgvProducts.DataSource = products
        pnlProducts.Visible = True
        pnlProducts.BringToFront()
    End Sub

    Private Sub ShowSupplierPanel()
        HideAllPanels()
        Dim dal As New SupplierDAL()
        Dim suppliers = dal.GetAllSuppliers()
        dgvSuppliers.DataSource = Nothing
        dgvSuppliers.DataSource = suppliers
        pnlSuppliers.Visible = True
        pnlSuppliers.BringToFront()
    End Sub

    Private Sub ShowClientPanel()
        HideAllPanels()
        Dim userDAL As New UserDAL()
        Dim clients = userDAL.GetAllClients()
        dgvClients.DataSource = Nothing
        dgvClients.DataSource = clients
        pnlClients.Visible = True
        pnlClients.BringToFront()
    End Sub

    Private Sub ShowAllOrderHistory()
        HideAllPanels()

        Try
            Dim dal As New SaleDAL()
            Dim userDAL As New UserDAL()
            Dim allHistory = dal.GetOrderHistory()

            ' Get all clients to filter by UserType
            Dim allClients = userDAL.GetAllClients()
            Dim clientUserIDs As New HashSet(Of Integer)(allClients.Select(Function(c) c.UserID))

            ' Separate client sales and admin orders
            Dim clientSales = allHistory.Where(Function(h) clientUserIDs.Contains(h.UserID)).ToList()
            Dim adminOrders = allHistory.Where(Function(h) Not clientUserIDs.Contains(h.UserID)).ToList()

            ' Bind data to respective DataGridViews
            dgvClientSales.DataSource = Nothing
            dgvClientSales.DataSource = clientSales

            dgvAdminOrders.DataSource = Nothing
            dgvAdminOrders.DataSource = adminOrders

            ' Hide specified columns in both DataGridViews to reduce redundancy
            HideRedundantColumns(dgvClientSales)
            HideRedundantColumns(dgvAdminOrders)

            ' Show the panel
            pnlSalesHistory.Visible = True
            pnlSalesHistory.BringToFront()

        Catch ex As Exception
            MessageBox.Show("Error loading sales history: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCreateClient_Click(sender As Object, e As EventArgs)
        Dim frm As New ClientAccountForm()
        If frm.ShowDialog() = DialogResult.OK Then
            ShowClientPanel() ' Refresh the client list
        End If
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
            MessageBox.Show("Purchase completed successfully!")
        End If
    End Sub

    Private Sub btnSalesHistory_Click(sender As Object, e As EventArgs)
        ShowAllOrderHistory()
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

    Private Sub btnGenerateReports_Click(sender As Object, e As EventArgs)
        ShowReportsPanel()
    End Sub

    Private Sub ShowReportsPanel()
        HideAllPanels()
        pnlReports.Visible = True
        pnlReports.BringToFront()
    End Sub

    Private Sub btnDailyReport_Click(sender As Object, e As EventArgs)
        Try
            Using dateForm As New ReportDateForm("Select Date for Daily Sales Report")
                If dateForm.ShowDialog() = DialogResult.OK Then
                    Me.Cursor = Cursors.WaitCursor
                    excelReportManager.GenerateDailySalesReport(dateForm.SelectedDate)
                    Me.Cursor = Cursors.Default
                End If
            End Using
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show($"Error generating daily report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnWeeklyReport_Click(sender As Object, e As EventArgs)
        Try
            Using dateForm As New ReportDateForm("Select Week Start Date (Monday)")
                If dateForm.ShowDialog() = DialogResult.OK Then
                    Dim selectedDate = dateForm.SelectedDate
                    Dim daysToSubtract = (CInt(selectedDate.DayOfWeek) + 6) Mod 7
                    Dim weekStart = selectedDate.AddDays(-daysToSubtract)

                    Me.Cursor = Cursors.WaitCursor
                    excelReportManager.GenerateWeeklySalesReport(weekStart)
                    Me.Cursor = Cursors.Default
                End If
            End Using
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show($"Error generating weekly report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnMonthlyReport_Click(sender As Object, e As EventArgs)
        Try
            Using monthForm As New MonthYearSelectionForm()
                If monthForm.ShowDialog() = DialogResult.OK Then
                    Me.Cursor = Cursors.WaitCursor
                    excelReportManager.GenerateMonthlySalesReport(monthForm.SelectedYear, monthForm.SelectedMonth)
                    Me.Cursor = Cursors.Default
                End If
            End Using
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show($"Error generating monthly report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnInventoryReport_Click(sender As Object, e As EventArgs)
        Try
            Me.Cursor = Cursors.WaitCursor
            excelReportManager.GenerateInventoryReport()
            Me.Cursor = Cursors.Default
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show($"Error generating inventory report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class