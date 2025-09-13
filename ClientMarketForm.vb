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
    Private ReadOnly btnClose As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Product Market"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = True
        Me.ClientSize = New Size(980, 620)
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)

        lblHeader.AutoSize = True
        lblHeader.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblHeader.Location = New Point(20, 15)
        lblHeader.Text = "Browse Products and Place Order"

        lblSearch.AutoSize = True
        lblSearch.Text = "Search"
        lblSearch.Location = New Point(22, 58)

        txtSearch.Location = New Point(80, 55)
        txtSearch.Size = New Size(280, 27)
        AddHandler txtSearch.TextChanged, AddressOf txtSearch_TextChanged

        btnRefresh.Text = "Refresh"
        btnRefresh.Location = New Point(370, 54)
        btnRefresh.Size = New Size(90, 28)
        AddHandler btnRefresh.Click, AddressOf btnRefresh_Click

        dgvProducts.Location = New Point(25, 90)
        dgvProducts.Size = New Size(600, 420)
        dgvProducts.ReadOnly = True
        dgvProducts.AllowUserToAddRows = False
        dgvProducts.AllowUserToDeleteRows = False
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvProducts.MultiSelect = False
        dgvProducts.AutoGenerateColumns = False
        SetupProductsGrid()

        lblQty.AutoSize = True
        lblQty.Text = "Qty"
        lblQty.Location = New Point(25, 520)

        nudQty.Location = New Point(60, 517)
        nudQty.Minimum = 1
        nudQty.Maximum = 1000
        nudQty.Value = 1
        nudQty.Size = New Size(80, 27)

        btnAddToCart.Text = "Add To Cart"
        btnAddToCart.Location = New Point(150, 516)
        btnAddToCart.Size = New Size(120, 30)
        AddHandler btnAddToCart.Click, AddressOf btnAddToCart_Click

        dgvCart.Location = New Point(640, 90)
        dgvCart.Size = New Size(320, 330)
        dgvCart.ReadOnly = True
        dgvCart.AllowUserToAddRows = False
        dgvCart.AllowUserToDeleteRows = False
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvCart.MultiSelect = False
        dgvCart.AutoGenerateColumns = False
        SetupCartGrid()
        dgvCart.DataSource = cart

        btnRemove.Text = "Remove"
        btnRemove.Location = New Point(640, 430)
        btnRemove.Size = New Size(90, 30)
        AddHandler btnRemove.Click, AddressOf btnRemove_Click

        btnClear.Text = "Clear Cart"
        btnClear.Location = New Point(735, 430)
        btnClear.Size = New Size(90, 30)
        AddHandler btnClear.Click, AddressOf btnClear_Click

        lblCustomer.AutoSize = True
        lblCustomer.Text = "Customer Name"
        lblCustomer.Location = New Point(640, 475)

        txtCustomerName.Location = New Point(640, 495)
        txtCustomerName.Size = New Size(320, 27)

        lblTotal.AutoSize = True
        lblTotal.Font = New Font("Segoe UI Semibold", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblTotal.Location = New Point(640, 530)
        lblTotal.Text = "Total: 0.00"

        btnPlaceOrder.Text = "Place Order"
        btnPlaceOrder.Location = New Point(640, 560)
        btnPlaceOrder.Size = New Size(120, 35)
        AddHandler btnPlaceOrder.Click, AddressOf btnPlaceOrder_Click

        btnClose.Text = "Close"
        btnClose.Location = New Point(840, 560)
        btnClose.Size = New Size(120, 35)
        AddHandler btnClose.Click, Sub(s, e) Me.Close()

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
        Me.Controls.Add(btnClose)

        AddHandler Me.Load, AddressOf ClientMarketForm_Load
    End Sub

    Private Sub SetupProductsGrid()
        dgvProducts.Columns.Clear()
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "ID", .DataPropertyName = "ProductID", .Width = 50})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Product", .DataPropertyName = "ProductName", .Width = 160})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Category", .DataPropertyName = "Category", .Width = 110})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Price", .DataPropertyName = "Price", .Width = 80, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N2"}})
        dgvProducts.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Stock", .DataPropertyName = "Stock", .Width = 70})
    End Sub

    Private Sub SetupCartGrid()
        dgvCart.Columns.Clear()
        dgvCart.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Product", .DataPropertyName = "ProductName", .Width = 140})
        dgvCart.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Unit", .DataPropertyName = "UnitPrice", .Width = 70, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N2"}})
        dgvCart.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Qty", .DataPropertyName = "Quantity", .Width = 50})
        dgvCart.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Total", .DataPropertyName = "LineTotal", .Width = 70, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N2"}})
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
        Dim term = txtSearch.Text.Trim().ToLowerInvariant()
        If String.IsNullOrWhiteSpace(term) Then
            BindProducts(products)
        Else
            Dim filtered = products.Where(Function(p) (p.ProductName IsNot Nothing AndAlso p.ProductName.ToLower().Contains(term)) _
                                              OrElse (p.Category IsNot Nothing AndAlso p.Category.ToLower().Contains(term)))
            BindProducts(filtered)
        End If
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs)
        LoadProducts()
        txtSearch.Clear()
    End Sub

    Private Sub btnAddToCart_Click(sender As Object, e As EventArgs)
        If dgvProducts.CurrentRow Is Nothing Then
            MessageBox.Show("Select a product.")
            Return
        End If

        Dim selected As Product = TryCast(dgvProducts.CurrentRow.DataBoundItem, Product)
        If selected Is Nothing Then
            MessageBox.Show("Invalid selection.")
            Return
        End If

        Dim qty As Integer = Convert.ToInt32(nudQty.Value)
        If qty <= 0 Then
            MessageBox.Show("Quantity must be at least 1.")
            Return
        End If

        If selected.Stock < qty Then
            MessageBox.Show("Insufficient stock.")
            Return
        End If

        Dim existing = cart.FirstOrDefault(Function(c) c.ProductID = selected.ProductID)
        If existing IsNot Nothing Then
            If selected.Stock < (existing.Quantity + qty) Then
                MessageBox.Show("Insufficient stock for combined quantity.")
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
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs)
        If dgvCart.CurrentRow Is Nothing Then Return
        Dim item As CartItem = TryCast(dgvCart.CurrentRow.DataBoundItem, CartItem)
        If item IsNot Nothing Then
            cart.Remove(item)
            UpdateTotal()
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs)
        cart.Clear()
        UpdateTotal()
    End Sub

    Private Sub UpdateTotal()
        Dim total As Decimal = cart.Sum(Function(i) i.LineTotal)
        lblTotal.Text = $"Total: {total:N2}"
    End Sub

    Private Sub btnPlaceOrder_Click(sender As Object, e As EventArgs)
        If cart.Count = 0 Then
            MessageBox.Show("Cart is empty.")
            Return
        End If

        Dim customer As String = txtCustomerName.Text.Trim()
        If String.IsNullOrWhiteSpace(customer) Then
            MessageBox.Show("Enter customer name.")
            Return
        End If

        ' Process each cart line: record sale then update stock
        Dim anyFailed As Boolean = False
        For Each line In cart.ToList()
            Dim sale As New Sale With {
                .UserID = UserSession.UserID,
                .ProductID = line.ProductID,
                .Quantity = line.Quantity,
                .UnitPrice = line.UnitPrice,
                .TotalAmount = line.LineTotal,
                .SaleDate = DateTime.Now,
                .CustomerName = customer
            }

            Dim recorded = saleDal.AddSale(sale)
            If Not recorded Then
                anyFailed = True
                Exit For
            End If

            Dim stockUpdated = saleDal.UpdateProductStock(line.ProductID, line.Quantity)
            If Not stockUpdated Then
                anyFailed = True
                Exit For
            End If
        Next

        If anyFailed Then
            MessageBox.Show("Order failed. Please retry.")
            Return
        End If

        MessageBox.Show("Order placed successfully.")
        cart.Clear()
        UpdateTotal()
        LoadProducts()
    End Sub

    Private Class CartItem
        Public Property ProductID As Integer
        Public Property ProductName As String
        Public Property UnitPrice As Decimal
        Public Property Quantity As Integer
        Public Property LineTotal As Decimal
    End Class
End Class