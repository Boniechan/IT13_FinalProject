Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel
Imports System.Linq

Public Class ClientMarketForm
    Inherits Form

    Private ReadOnly productDal As New ProductDAL()
    Private ReadOnly saleDal As New SaleDAL()

    Private products As List(Of Product) = New List(Of Product)()
    Private ReadOnly cart As New BindingList(Of CartItem)()

    Private ReadOnly lblHeader As New Label()
    Private ReadOnly lblSearch As New Label()
    Private ReadOnly txtSearch As New TextBox()
    Private ReadOnly btnRefresh As New Button()

    Private ReadOnly dgvProducts As New DataGridView()
    Private ReadOnly lblQty As New Label()
    Private ReadOnly nudQty As New NumericUpDown()
    Private ReadOnly btnAddToCart As New Button()

    Private ReadOnly dgvCart As New DataGridView()
    Private ReadOnly btnRemove As New Button()
    Private ReadOnly btnClear As New Button()

    Private ReadOnly lblCustomer As New Label()
    Private ReadOnly txtCustomerName As New TextBox()
    Private ReadOnly lblTotal As New Label()
    Private ReadOnly btnPlaceOrder As New Button()
    Private ReadOnly btnOrderHistory As New Button()
    Private ReadOnly btnLogout As New Button()
    Private ReadOnly btnClose As New Button()

    ' Order History Panel
    Private ReadOnly pnlOrderHistory As New Panel()
    Private ReadOnly dgvOrderHistory As New DataGridView()
    Private ReadOnly btnCloseHistory As New Button()
    Private ReadOnly lblHistoryTitle As New Label()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Product Market - Fisheries POS"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimumSize = New Size(1400, 800)
        Me.ClientSize = New Size(1400, 800)
        Me.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        Me.BackColor = Color.FromArgb(245, 245, 245)

        lblHeader.AutoSize = True
        lblHeader.Font = New Font("Segoe UI", 18.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblHeader.Location = New Point(40, 30)
        lblHeader.Text = "Browse Products and Place Order"
        lblHeader.ForeColor = Color.FromArgb(51, 51, 51)

        lblSearch.AutoSize = True
        lblSearch.Text = "Search Products:"
        lblSearch.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        lblSearch.Location = New Point(40, 85)
        lblSearch.ForeColor = Color.FromArgb(102, 102, 102)

        txtSearch.Location = New Point(170, 82)
        txtSearch.Size = New Size(350, 30)
        txtSearch.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        txtSearch.BorderStyle = BorderStyle.FixedSingle
        txtSearch.Text = "Search by product name or category..."
        txtSearch.ForeColor = Color.Gray
        AddHandler txtSearch.TextChanged, AddressOf txtSearch_TextChanged
        AddHandler txtSearch.Enter, AddressOf txtSearch_Enter
        AddHandler txtSearch.Leave, AddressOf txtSearch_Leave

        btnRefresh.Text = "Refresh"
        btnRefresh.Location = New Point(540, 81)
        btnRefresh.Size = New Size(100, 32)
        btnRefresh.BackColor = Color.FromArgb(51, 122, 183)
        btnRefresh.ForeColor = Color.White
        btnRefresh.FlatStyle = FlatStyle.Flat
        btnRefresh.FlatAppearance.BorderSize = 0
        AddHandler btnRefresh.Click, AddressOf btnRefresh_Click

        dgvProducts.Location = New Point(40, 130)
        dgvProducts.Size = New Size(750, 450)
        dgvProducts.ReadOnly = True
        dgvProducts.AllowUserToAddRows = False
        dgvProducts.AllowUserToDeleteRows = False
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvProducts.MultiSelect = False
        dgvProducts.AutoGenerateColumns = False
        dgvProducts.RowHeadersVisible = False
        dgvProducts.BorderStyle = BorderStyle.FixedSingle
        dgvProducts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvProducts.DefaultCellStyle.Padding = New Padding(8)
        dgvProducts.ColumnHeadersHeight = 40
        dgvProducts.RowTemplate.Height = 35
        dgvProducts.BackgroundColor = Color.White
        dgvProducts.GridColor = Color.LightGray
        SetupProductsGrid()

        lblQty.AutoSize = True
        lblQty.Text = "Quantity:"
        lblQty.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        lblQty.Location = New Point(40, 600)

        nudQty.Location = New Point(120, 598)
        nudQty.DecimalPlaces = 2 ' Allow decimal input
        nudQty.Minimum = 0.01D
        nudQty.Maximum = 1000D
        nudQty.Value = 1D
        nudQty.Size = New Size(100, 30)
        nudQty.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        nudQty.Increment = 0.25D ' Increment by 0.25kg

        btnAddToCart.Text = "Add To Cart"
        btnAddToCart.Location = New Point(240, 595)
        btnAddToCart.Size = New Size(130, 35)
        btnAddToCart.BackColor = Color.FromArgb(92, 184, 92)
        btnAddToCart.ForeColor = Color.White
        btnAddToCart.FlatStyle = FlatStyle.Flat
        btnAddToCart.FlatAppearance.BorderSize = 0
        AddHandler btnAddToCart.Click, AddressOf btnAddToCart_Click

        ' Cart section with better spacing
        dgvCart.Location = New Point(820, 130)
        dgvCart.Size = New Size(480, 350)
        dgvCart.ReadOnly = True
        dgvCart.AllowUserToAddRows = False
        dgvCart.AllowUserToDeleteRows = False
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvCart.MultiSelect = False
        dgvCart.AutoGenerateColumns = False
        dgvCart.RowHeadersVisible = False
        dgvCart.BorderStyle = BorderStyle.FixedSingle
        dgvCart.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvCart.DefaultCellStyle.Padding = New Padding(6)
        dgvCart.ColumnHeadersHeight = 35
        dgvCart.RowTemplate.Height = 30
        dgvCart.BackgroundColor = Color.White
        dgvCart.GridColor = Color.LightGray
        SetupCartGrid()
        dgvCart.DataSource = cart

        btnRemove.Text = "Remove Item"
        btnRemove.Location = New Point(820, 495)
        btnRemove.Size = New Size(120, 35)
        btnRemove.BackColor = Color.FromArgb(217, 83, 79)
        btnRemove.ForeColor = Color.White
        btnRemove.FlatStyle = FlatStyle.Flat
        btnRemove.FlatAppearance.BorderSize = 0
        AddHandler btnRemove.Click, AddressOf btnRemove_Click

        btnClear.Text = "Clear Cart"
        btnClear.Location = New Point(955, 495)
        btnClear.Size = New Size(120, 35)
        btnClear.BackColor = Color.FromArgb(240, 173, 78)
        btnClear.ForeColor = Color.White
        btnClear.FlatStyle = FlatStyle.Flat
        btnClear.FlatAppearance.BorderSize = 0
        AddHandler btnClear.Click, AddressOf btnClear_Click

        lblCustomer.AutoSize = True
        lblCustomer.Text = "Customer Name:"
        lblCustomer.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        lblCustomer.Location = New Point(820, 550)

        txtCustomerName.Location = New Point(820, 575)
        txtCustomerName.Size = New Size(480, 30)
        txtCustomerName.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        txtCustomerName.BorderStyle = BorderStyle.FixedSingle

        lblTotal.AutoSize = True
        lblTotal.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblTotal.Location = New Point(820, 620)
        lblTotal.Text = "Total: $0.00"
        lblTotal.ForeColor = Color.FromArgb(51, 51, 51)

        btnPlaceOrder.Text = "Place Order"
        btnPlaceOrder.Location = New Point(820, 655)
        btnPlaceOrder.Size = New Size(120, 45)
        btnPlaceOrder.BackColor = Color.FromArgb(40, 167, 69)
        btnPlaceOrder.ForeColor = Color.White
        btnPlaceOrder.FlatStyle = FlatStyle.Flat
        btnPlaceOrder.FlatAppearance.BorderSize = 0
        btnPlaceOrder.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        AddHandler btnPlaceOrder.Click, AddressOf btnPlaceOrder_Click

        btnOrderHistory.Text = "Order History"
        btnOrderHistory.Location = New Point(950, 655)
        btnOrderHistory.Size = New Size(120, 45)
        btnOrderHistory.BackColor = Color.FromArgb(51, 122, 183)
        btnOrderHistory.ForeColor = Color.White
        btnOrderHistory.FlatStyle = FlatStyle.Flat
        btnOrderHistory.FlatAppearance.BorderSize = 0
        btnOrderHistory.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        AddHandler btnOrderHistory.Click, AddressOf btnOrderHistory_Click

        btnLogout.Text = "Logout"
        btnLogout.Location = New Point(1080, 655)
        btnLogout.Size = New Size(100, 45)
        btnLogout.BackColor = Color.FromArgb(217, 83, 79)
        btnLogout.ForeColor = Color.White
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        btnClose.Text = "Close"
        btnClose.Location = New Point(1190, 655)
        btnClose.Size = New Size(80, 45)
        btnClose.BackColor = Color.FromArgb(119, 119, 119)
        btnClose.ForeColor = Color.White
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.FlatAppearance.BorderSize = 0
        btnClose.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular, GraphicsUnit.Point)
        AddHandler btnClose.Click, Sub(s, e) Me.Close()

        ' Order History Panel Setup
        pnlOrderHistory.Size = New Size(1320, 600)
        pnlOrderHistory.Location = New Point(40, 80)
        pnlOrderHistory.Visible = False
        pnlOrderHistory.BackColor = Color.White
        pnlOrderHistory.BorderStyle = BorderStyle.FixedSingle
        pnlOrderHistory.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom

        lblHistoryTitle.AutoSize = True
        lblHistoryTitle.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblHistoryTitle.Location = New Point(20, 20)
        lblHistoryTitle.Text = "My Order History"
        lblHistoryTitle.ForeColor = Color.FromArgb(51, 51, 51)

        dgvOrderHistory.Size = New Size(1280, 480)
        dgvOrderHistory.Location = New Point(20, 60)
        dgvOrderHistory.ReadOnly = True
        dgvOrderHistory.AllowUserToAddRows = False
        dgvOrderHistory.AllowUserToDeleteRows = False
        dgvOrderHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvOrderHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvOrderHistory.RowHeadersVisible = False
        dgvOrderHistory.BorderStyle = BorderStyle.FixedSingle
        dgvOrderHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248)
        dgvOrderHistory.DefaultCellStyle.Padding = New Padding(5)
        dgvOrderHistory.ColumnHeadersHeight = 35
        dgvOrderHistory.RowTemplate.Height = 30
        dgvOrderHistory.BackgroundColor = Color.White
        dgvOrderHistory.GridColor = Color.LightGray
        SetupOrderHistoryGrid()

        btnCloseHistory.Text = "Close History"
        btnCloseHistory.Size = New Size(140, 45)
        btnCloseHistory.Location = New Point(1160, 550)
        btnCloseHistory.BackColor = Color.FromArgb(119, 119, 119)
        btnCloseHistory.ForeColor = Color.White
        btnCloseHistory.FlatStyle = FlatStyle.Flat
        btnCloseHistory.FlatAppearance.BorderSize = 0
        btnCloseHistory.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold, GraphicsUnit.Point)
        AddHandler btnCloseHistory.Click, AddressOf btnCloseHistory_Click

        pnlOrderHistory.Controls.Add(lblHistoryTitle)
        pnlOrderHistory.Controls.Add(dgvOrderHistory)
        pnlOrderHistory.Controls.Add(btnCloseHistory)

        ' Add all controls to form
        Me.Controls.Add(lblHeader)
        Me.Controls.Add(lblSearch)
        Me.Controls.Add(txtSearch)
        Me.Controls.Add(btnRefresh)
        Me.Controls.Add(dgvProducts)
        Me.Controls.Add(lblQty)
        Me.Controls.Add(nudQty)
        Me.Controls.Add(btnAddToCart)
        Me.Controls.Add(dgvCart)
        Me.Controls.Add(btnRemove)
        Me.Controls.Add(btnClear)
        Me.Controls.Add(lblCustomer)
        Me.Controls.Add(txtCustomerName)
        Me.Controls.Add(lblTotal)
        Me.Controls.Add(btnPlaceOrder)
        Me.Controls.Add(btnOrderHistory)
        Me.Controls.Add(btnLogout)
        Me.Controls.Add(btnClose)
        Me.Controls.Add(pnlOrderHistory)

        AddHandler Me.Load, AddressOf ClientMarketForm_Load
    End Sub

    Private Sub SetupProductsGrid()
        dgvProducts.Columns.Clear()
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "ID", .DataPropertyName = "ProductID", .Width = 60})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Product Name", .DataPropertyName = "ProductName", .Width = 200})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Category", .DataPropertyName = "Category", .Width = 130})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Price", .DataPropertyName = "PriceDisplay", .Width = 120})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Stock", .DataPropertyName = "StockDisplay", .Width = 200})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Description", .DataPropertyName = "Description", .Width = 180})
    End Sub

    Private Sub SetupCartGrid()
        dgvCart.Columns.Clear()
        dgvCart.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Product", .DataPropertyName = "ProductName", .Width = 180})
        dgvCart.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Unit Price", .DataPropertyName = "UnitPrice", .Width = 90, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}})
        dgvCart.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Qty", .DataPropertyName = "Quantity", .Width = 60, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "F2"}})
        dgvCart.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Total", .DataPropertyName = "LineTotal", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}})
    End Sub

    Private Sub SetupOrderHistoryGrid()
        dgvOrderHistory.Columns.Clear()
        dgvOrderHistory.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Product", .DataPropertyName = "ProductName", .Width = 250})
        dgvOrderHistory.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Customer", .DataPropertyName = "CustomerName", .Width = 200})
        dgvOrderHistory.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Quantity", .DataPropertyName = "Quantity", .Width = 100, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "F2"}})
        dgvOrderHistory.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Unit Price", .DataPropertyName = "UnitPrice", .Width = 120, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}})
        dgvOrderHistory.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Total Amount", .DataPropertyName = "TotalAmount", .Width = 130, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2"}})
        dgvOrderHistory.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Order Date", .DataPropertyName = "SaleDate", .Width = 180, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "MM/dd/yyyy hh:mm tt"}})
    End Sub


    Private Sub txtSearch_Enter(sender As Object, e As EventArgs)
        If txtSearch.Text = "Search by product name or category..." Then
            txtSearch.Text = ""
            txtSearch.ForeColor = Color.Black
        End If
    End Sub

    Private Sub txtSearch_Leave(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtSearch.Text) Then
            txtSearch.Text = "Search by product name or category..."
            txtSearch.ForeColor = Color.Gray
        End If
    End Sub

    Private Sub ClientMarketForm_Load(sender As Object, e As EventArgs)
        If UserSession.UserID = 0 Then
            MessageBox.Show("No active session. Please login.")
            Me.Close()
            Return
        End If
        LoadProducts()
    End Sub

    Private Sub LoadProducts()
        products = productDal.GetAllProducts()
        BindProducts(products)
    End Sub

    Private Sub BindProducts(source As IEnumerable(Of Product))
        dgvProducts.DataSource = Nothing
        dgvProducts.DataSource = source.ToList()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs)

        If txtSearch.Text = "Search by product name or category..." Then
            Return
        End If

        Dim term = txtSearch.Text.Trim()
        If String.IsNullOrWhiteSpace(term) Then
            BindProducts(products)
        Else
            Dim filtered = products.Where(Function(p)
                                              Return (p.ProductName IsNot Nothing AndAlso p.ProductName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) OrElse
                                                     (p.Category IsNot Nothing AndAlso p.Category.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) OrElse
                                                     (p.Description IsNot Nothing AndAlso p.Description.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                                          End Function)
            BindProducts(filtered)
        End If
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs)
        LoadProducts()
        txtSearch.Text = "Search by product name or category..."
        txtSearch.ForeColor = Color.Gray
    End Sub

    Private Sub btnAddToCart_Click(sender As Object, e As EventArgs)
        If dgvProducts.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a product to add to cart.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selected As Product = TryCast(dgvProducts.CurrentRow.DataBoundItem, Product)
        If selected Is Nothing Then
            MessageBox.Show("Invalid product selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim qty As Decimal = Convert.ToDecimal(nudQty.Value)
        If qty <= 0 Then
            MessageBox.Show("Quantity must be at least 0.1.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Enhanced stock validation for boxed products
        Dim availableStock As Decimal
        If selected.IsSoldByBox Then
            ' For boxed products, calculate total available kg
            Dim currentBoxQuantity As Decimal = If(selected.BoxQuantity.HasValue, selected.BoxQuantity.Value, selected.BoxSize)
            availableStock = currentBoxQuantity + ((selected.Stock - 1) * selected.BoxSize)

            If qty > availableStock Then
                MessageBox.Show($"Insufficient stock. Only {availableStock:F2}kg available.", "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
        Else
            ' For regular products (kilo/piece)
            availableStock = selected.Stock
            If qty > availableStock Then
                MessageBox.Show($"Insufficient stock. Only {availableStock} items available.", "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
        End If

        Dim existing = cart.FirstOrDefault(Function(c) c.ProductID = selected.ProductID)
        If existing IsNot Nothing Then
            If (existing.Quantity + qty) > availableStock Then
                MessageBox.Show($"Cannot add {qty:F2} more items. Only {availableStock - existing.Quantity:F2} more available.", "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            existing.Quantity += qty
            existing.LineTotal = existing.UnitPrice * existing.Quantity
            dgvCart.Refresh()
        Else
            cart.Add(New CartItem With {
                .ProductID = selected.ProductID,
                .ProductName = selected.ProductName,
                .UnitPrice = selected.Price,
                .Quantity = qty,
                .LineTotal = selected.Price * qty
            })
        End If

        UpdateTotal()
        nudQty.Value = 1 ' Reset quantity to 1
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs)
        If dgvCart.CurrentRow Is Nothing Then
            MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim item As CartItem = TryCast(dgvCart.CurrentRow.DataBoundItem, CartItem)
        If item IsNot Nothing Then
            cart.Remove(item)
            UpdateTotal()
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs)
        If cart.Count = 0 Then
            MessageBox.Show("Cart is already empty.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If MessageBox.Show("Are you sure you want to clear the cart?", "Clear Cart", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            cart.Clear()
            UpdateTotal()
        End If
    End Sub

    Private Sub UpdateTotal()
        Dim total As Decimal = cart.Sum(Function(i) i.LineTotal)
        lblTotal.Text = $"Total: {total:C2}"
    End Sub

    Private Sub btnPlaceOrder_Click(sender As Object, e As EventArgs)
        If cart.Count = 0 Then
            MessageBox.Show("Cart is empty. Please add items before placing an order.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim customer As String = txtCustomerName.Text.Trim()
        If String.IsNullOrWhiteSpace(customer) Then
            MessageBox.Show("Please enter customer name.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCustomerName.Focus()
            Return
        End If

        ' Show order confirmation dialog
        Dim confirmationMessage As String = BuildOrderConfirmation(customer)

        Dim result As DialogResult = MessageBox.Show(
            confirmationMessage,
            "Confirm Your Order",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2
        )

        If result <> DialogResult.Yes Then
            Return ' User cancelled the order
        End If

        ' Process all orders in a single transaction-like approach
        Try
            Dim anyFailed As Boolean = False
            For Each line In cart.ToList()
                Dim sale As New Sale With {
                    .UserID = UserSession.UserID,
                    .ProductID = line.ProductID,
                    .SupplierID = 1, ' Default supplier ID for client orders
                    .Quantity = line.Quantity,
                    .UnitPrice = line.UnitPrice,
                    .TotalAmount = line.LineTotal,
                    .SaleDate = DateTime.Now,
                    .CustomerName = customer,
                    .Notes = "Client order from market"
                }

                ' Use AddSaleWithSupplier which handles both sale recording and stock updates
                Dim success = saleDal.AddSaleWithSupplier(sale)
                If Not success Then
                    anyFailed = True
                    Exit For
                End If
            Next

            If anyFailed Then
                MessageBox.Show("Order failed. Please try again.", "Order Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            MessageBox.Show("Order placed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            cart.Clear()
            UpdateTotal()
            txtCustomerName.Clear()
            LoadProducts() ' Refresh products to show updated stock

        Catch ex As Exception
            MessageBox.Show($"Error processing order: {ex.Message}", "Order Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function BuildOrderConfirmation(customerName As String) As String
        Dim orderDetails As New System.Text.StringBuilder()

        orderDetails.AppendLine("Please confirm your order:")
        orderDetails.AppendLine()
        orderDetails.AppendLine($"Customer: {customerName}")
        orderDetails.AppendLine($"Order Date: {DateTime.Now:MM/dd/yyyy hh:mm tt}")
        orderDetails.AppendLine()
        orderDetails.AppendLine("Items:")
        orderDetails.AppendLine(New String("-"c, 50))

        For Each item In cart
            ' Find the product to determine the unit
            Dim product = products.FirstOrDefault(Function(p) p.ProductID = item.ProductID)
            Dim unit As String = If(product IsNot Nothing AndAlso product.IsSoldByBox, "kg", "pcs")

            orderDetails.AppendLine($"{item.ProductName}")
            orderDetails.AppendLine($"  Quantity: {item.Quantity:F2} {unit} x {item.UnitPrice:C} = {item.LineTotal:C}")
            orderDetails.AppendLine()
        Next

        orderDetails.AppendLine(New String("-"c, 50))
        orderDetails.AppendLine($"Total Amount: {cart.Sum(Function(i) i.LineTotal):C}")
        orderDetails.AppendLine()
        orderDetails.AppendLine("Do you want to place this order?")

        Return orderDetails.ToString()
    End Function

    Private Sub btnOrderHistory_Click(sender As Object, e As EventArgs)
        HideMainContent() ' Hide main content first
        pnlOrderHistory.Visible = True
        pnlOrderHistory.BringToFront() ' Bring to front
        LoadOrderHistory()
    End Sub

    Private Sub LoadOrderHistory()
        Try
            ' Use GetOrderHistory instead of GetSalesByCustomer
            Dim allOrders = saleDal.GetOrderHistory()

            ' Filter to show only current user's orders
            Dim myOrders = allOrders.Where(Function(o) o.UserID = UserSession.UserID).ToList()

            dgvOrderHistory.DataSource = Nothing
            dgvOrderHistory.DataSource = myOrders

            ' Hide unwanted columns after data binding
            HideUnwantedColumns()

            ' Show a message if no orders found
            If myOrders.Count = 0 Then
                MessageBox.Show("No order history found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error loading order history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Add this new method to hide unwanted columns
    Private Sub HideUnwantedColumns()
        ' List of columns to hide from clients
        Dim columnsToHide() As String = {
            "SaleID", "UserID", "FullName", "Username",
            "ProductID", "SupplierName", "Notes", "SupplierID"
        }

        ' Hide each unwanted column if it exists
        For Each columnName As String In columnsToHide
            If dgvOrderHistory.Columns.Contains(columnName) Then
                dgvOrderHistory.Columns(columnName).Visible = False
            End If
        Next
    End Sub

    Private Sub btnCloseHistory_Click(sender As Object, e As EventArgs)
        pnlOrderHistory.Visible = False
        ShowMainContent() ' Show main content when closing history
    End Sub

    Private Sub HideMainContent()
        lblHeader.Visible = False
        lblSearch.Visible = False
        txtSearch.Visible = False
        btnRefresh.Visible = False
        dgvProducts.Visible = False
        dgvCart.Visible = False
        lblQty.Visible = False
        nudQty.Visible = False
        btnAddToCart.Visible = False
        btnRemove.Visible = False
        btnClear.Visible = False
        lblCustomer.Visible = False
        txtCustomerName.Visible = False
        lblTotal.Visible = False
        btnPlaceOrder.Visible = False
    End Sub

    Private Sub ShowMainContent()
        lblHeader.Visible = True
        lblSearch.Visible = True
        txtSearch.Visible = True
        btnRefresh.Visible = True
        dgvProducts.Visible = True
        dgvCart.Visible = True
        lblQty.Visible = True
        nudQty.Visible = True
        btnAddToCart.Visible = True
        btnRemove.Visible = True
        btnClear.Visible = True
        lblCustomer.Visible = True
        txtCustomerName.Visible = True
        lblTotal.Visible = True
        btnPlaceOrder.Visible = True
    End Sub

    Private Class CartItem
        Public Property ProductID As Integer
        Public Property ProductName As String
        Public Property UnitPrice As Decimal
        Public Property Quantity As Decimal ' Changed from Integer to Decimal
        Public Property LineTotal As Decimal
    End Class

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        If MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ' Clear user session
            UserSession.ClearSession()

            ' Close this form and show login form
            Dim loginForm As New LoginForm()
            AddHandler loginForm.FormClosed, Sub(s, args) Me.Close()
            loginForm.Show()
            Me.Hide()
        End If
    End Sub
End Class