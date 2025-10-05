Imports System.Drawing
Imports System.Windows.Forms

Public Class SaleRecordForm
    Inherits Form

    Private ReadOnly lblTitle As New Label()
    Private ReadOnly lblSaleType As New Label()
    Private ReadOnly rbAdminSale As New RadioButton()
    Private ReadOnly rbClientSale As New RadioButton()
    Private ReadOnly pnlSaleType As New Panel()

    Private ReadOnly lblClient As New Label()
    Private ReadOnly cmbClient As New ComboBox()

    Private ReadOnly lblProduct As New Label()
    Private ReadOnly lblSupplier As New Label()
    Private ReadOnly lblQuantity As New Label()
    Private ReadOnly lblUnitPrice As New Label()
    Private ReadOnly lblTotalAmount As New Label()
    Private ReadOnly lblCustomerName As New Label()
    Private ReadOnly lblNotes As New Label()

    Private ReadOnly cmbProduct As New ComboBox()
    Private ReadOnly cmbSupplier As New ComboBox()
    Private ReadOnly txtQuantity As New NumericUpDown()
    Private ReadOnly txtUnitPrice As New NumericUpDown()
    Private ReadOnly txtTotalAmount As New TextBox()
    Private ReadOnly txtCustomerName As New TextBox()
    Private ReadOnly txtNotes As New TextBox()

    Private ReadOnly btnCalculate As New Button()
    Private ReadOnly btnBuyProducts As New Button()
    Private ReadOnly btnCancel As New Button()

    Private ReadOnly lblProductInfo As New Label()
    Private ReadOnly lblSupplierInfo As New Label()
    Private ReadOnly lblClientInfo As New Label()

    Private products As List(Of Product)
    Private suppliers As List(Of Supplier)
    Private clients As List(Of User)

    Public Sub New()
        InitializeComponent()
        LoadData()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Buy Products"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ClientSize = New Size(650, 750)
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
        Me.BackColor = Color.FromArgb(245, 245, 245)

        ' Title
        lblTitle.Text = "Buy Products"
        lblTitle.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblTitle.AutoSize = True
        lblTitle.Location = New Point(20, 20)
        lblTitle.ForeColor = Color.FromArgb(51, 51, 51)

        ' Sale Type Selection
        lblSaleType.Text = "Purchase Type:"
        lblSaleType.AutoSize = True
        lblSaleType.Location = New Point(20, 60)
        lblSaleType.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblSaleType.ForeColor = Color.FromArgb(51, 51, 51)

        pnlSaleType.Location = New Point(20, 85)
        pnlSaleType.Size = New Size(600, 30)
        pnlSaleType.BackColor = Color.White
        pnlSaleType.BorderStyle = BorderStyle.FixedSingle

        rbAdminSale.Text = "Admin Purchase (Walk-in Customer)"
        rbAdminSale.Location = New Point(10, 5)
        rbAdminSale.Size = New Size(240, 20)
        rbAdminSale.Checked = True
        rbAdminSale.ForeColor = Color.FromArgb(51, 51, 51)
        AddHandler rbAdminSale.CheckedChanged, AddressOf SaleType_CheckedChanged

        rbClientSale.Text = "Client Purchase"
        rbClientSale.Location = New Point(270, 5)
        rbClientSale.Size = New Size(150, 20)
        rbClientSale.ForeColor = Color.FromArgb(51, 51, 51)
        AddHandler rbClientSale.CheckedChanged, AddressOf SaleType_CheckedChanged

        pnlSaleType.Controls.Add(rbAdminSale)
        pnlSaleType.Controls.Add(rbClientSale)

        ' Client Selection (initially hidden)
        lblClient.Text = "Select Client:"
        lblClient.AutoSize = True
        lblClient.Location = New Point(20, 130)
        lblClient.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblClient.ForeColor = Color.FromArgb(51, 51, 51)
        lblClient.Visible = False

        cmbClient.Location = New Point(20, 150)
        cmbClient.Size = New Size(350, 27)
        cmbClient.DropDownStyle = ComboBoxStyle.DropDownList
        cmbClient.Visible = False
        AddHandler cmbClient.SelectedIndexChanged, AddressOf cmbClient_SelectedIndexChanged

        lblClientInfo.Location = New Point(20, 180)
        lblClientInfo.Size = New Size(550, 30)
        lblClientInfo.ForeColor = Color.FromArgb(255, 87, 34)
        lblClientInfo.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular, GraphicsUnit.Point)
        lblClientInfo.Visible = False

        ' Product Selection
        lblProduct.Text = "Product:"
        lblProduct.AutoSize = True
        lblProduct.Location = New Point(20, 220)
        lblProduct.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblProduct.ForeColor = Color.FromArgb(51, 51, 51)

        cmbProduct.Location = New Point(20, 240)
        cmbProduct.Size = New Size(350, 27)
        cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList
        AddHandler cmbProduct.SelectedIndexChanged, AddressOf cmbProduct_SelectedIndexChanged

        lblProductInfo.Location = New Point(20, 270)
        lblProductInfo.Size = New Size(550, 40)
        lblProductInfo.ForeColor = Color.FromArgb(51, 122, 183)
        lblProductInfo.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular, GraphicsUnit.Point)

        ' Supplier Selection
        lblSupplier.Text = "Supplier:"
        lblSupplier.AutoSize = True
        lblSupplier.Location = New Point(20, 320)
        lblSupplier.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblSupplier.ForeColor = Color.FromArgb(51, 51, 51)

        cmbSupplier.Location = New Point(20, 340)
        cmbSupplier.Size = New Size(350, 27)
        cmbSupplier.DropDownStyle = ComboBoxStyle.DropDownList
        AddHandler cmbSupplier.SelectedIndexChanged, AddressOf cmbSupplier_SelectedIndexChanged

        lblSupplierInfo.Location = New Point(20, 370)
        lblSupplierInfo.Size = New Size(550, 40)
        lblSupplierInfo.ForeColor = Color.FromArgb(92, 184, 92)
        lblSupplierInfo.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular, GraphicsUnit.Point)

        ' Quantity - Updated to support decimal values
        lblQuantity.Text = "Quantity (kg):"
        lblQuantity.AutoSize = True
        lblQuantity.Location = New Point(20, 420)
        lblQuantity.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblQuantity.ForeColor = Color.FromArgb(51, 51, 51)

        txtQuantity.Location = New Point(20, 440)
        txtQuantity.Size = New Size(150, 27)
        txtQuantity.DecimalPlaces = 2 ' Allow decimal quantities for partial boxes
        txtQuantity.Minimum = 0.01D
        txtQuantity.Maximum = 999999.99D
        txtQuantity.Value = 1
        AddHandler txtQuantity.ValueChanged, AddressOf CalculateTotal

        ' Unit Price
        lblUnitPrice.Text = "Unit Price:"
        lblUnitPrice.AutoSize = True
        lblUnitPrice.Location = New Point(200, 420)
        lblUnitPrice.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblUnitPrice.ForeColor = Color.FromArgb(51, 51, 51)

        txtUnitPrice.Location = New Point(200, 440)
        txtUnitPrice.Size = New Size(150, 27)
        txtUnitPrice.DecimalPlaces = 2
        txtUnitPrice.Minimum = 0.01D
        txtUnitPrice.Maximum = 999999.99D
        AddHandler txtUnitPrice.ValueChanged, AddressOf CalculateTotal

        ' Total Amount
        lblTotalAmount.Text = "Total Amount:"
        lblTotalAmount.AutoSize = True
        lblTotalAmount.Location = New Point(380, 420)
        lblTotalAmount.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblTotalAmount.ForeColor = Color.FromArgb(51, 51, 51)

        txtTotalAmount.Location = New Point(380, 440)
        txtTotalAmount.Size = New Size(150, 27)
        txtTotalAmount.ReadOnly = True
        txtTotalAmount.BackColor = Color.FromArgb(248, 248, 248)
        txtTotalAmount.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)

        ' Customer Name
        lblCustomerName.Text = "Customer Name:"
        lblCustomerName.AutoSize = True
        lblCustomerName.Location = New Point(20, 480)
        lblCustomerName.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblCustomerName.ForeColor = Color.FromArgb(51, 51, 51)

        txtCustomerName.Location = New Point(20, 500)
        txtCustomerName.Size = New Size(350, 27)

        ' Notes
        lblNotes.Text = "Notes (Optional):"
        lblNotes.AutoSize = True
        lblNotes.Location = New Point(20, 540)
        lblNotes.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblNotes.ForeColor = Color.FromArgb(51, 51, 51)

        txtNotes.Location = New Point(20, 560)
        txtNotes.Size = New Size(550, 60)
        txtNotes.Multiline = True
        txtNotes.ScrollBars = ScrollBars.Vertical

        ' Calculate Button
        btnCalculate.Text = "Recalculate"
        btnCalculate.Location = New Point(20, 640)
        btnCalculate.Size = New Size(100, 35)
        btnCalculate.BackColor = Color.FromArgb(240, 173, 78)
        btnCalculate.ForeColor = Color.White
        btnCalculate.FlatStyle = FlatStyle.Flat
        btnCalculate.FlatAppearance.BorderSize = 0
        AddHandler btnCalculate.Click, AddressOf CalculateTotal

        ' Buy Products Button
        btnBuyProducts.Text = "Buy Products"
        btnBuyProducts.Location = New Point(350, 640)
        btnBuyProducts.Size = New Size(120, 35)
        btnBuyProducts.BackColor = Color.FromArgb(92, 184, 92)
        btnBuyProducts.ForeColor = Color.White
        btnBuyProducts.FlatStyle = FlatStyle.Flat
        btnBuyProducts.FlatAppearance.BorderSize = 0
        AddHandler btnBuyProducts.Click, AddressOf btnBuyProducts_Click

        ' Cancel Button
        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(480, 640)
        btnCancel.Size = New Size(100, 35)
        btnCancel.BackColor = Color.FromArgb(217, 83, 79)
        btnCancel.ForeColor = Color.White
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.DialogResult = DialogResult.Cancel
        AddHandler btnCancel.Click, Sub(s, e) Me.Close()

        ' Add all controls to form
        Me.Controls.Add(lblTitle)
        Me.Controls.Add(lblSaleType)
        Me.Controls.Add(pnlSaleType)
        Me.Controls.Add(lblClient)
        Me.Controls.Add(cmbClient)
        Me.Controls.Add(lblClientInfo)
        Me.Controls.Add(lblProduct)
        Me.Controls.Add(cmbProduct)
        Me.Controls.Add(lblProductInfo)
        Me.Controls.Add(lblSupplier)
        Me.Controls.Add(cmbSupplier)
        Me.Controls.Add(lblSupplierInfo)
        Me.Controls.Add(lblQuantity)
        Me.Controls.Add(txtQuantity)
        Me.Controls.Add(lblUnitPrice)
        Me.Controls.Add(txtUnitPrice)
        Me.Controls.Add(lblTotalAmount)
        Me.Controls.Add(txtTotalAmount)
        Me.Controls.Add(lblCustomerName)
        Me.Controls.Add(txtCustomerName)
        Me.Controls.Add(lblNotes)
        Me.Controls.Add(txtNotes)
        Me.Controls.Add(btnCalculate)
        Me.Controls.Add(btnBuyProducts)
        Me.Controls.Add(btnCancel)

        Me.AcceptButton = btnBuyProducts
        Me.CancelButton = btnCancel
    End Sub

    Private Sub LoadData()
        Try
            ' Load Products
            Dim productDAL As New ProductDAL()
            products = productDAL.GetAllProducts()
            cmbProduct.DataSource = products
            cmbProduct.DisplayMember = "ProductName"
            cmbProduct.ValueMember = "ProductID"

            ' Load Suppliers
            Dim supplierDAL As New SupplierDAL()
            suppliers = supplierDAL.GetAllSuppliers()
            cmbSupplier.DataSource = suppliers
            cmbSupplier.DisplayMember = "SupplierName"
            cmbSupplier.ValueMember = "SupplierID"

            ' Load Clients
            Dim userDAL As New UserDAL()
            clients = userDAL.GetAllClients()
            cmbClient.DataSource = clients
            cmbClient.DisplayMember = "FullName"
            cmbClient.ValueMember = "UserID"

        Catch ex As Exception
            MessageBox.Show($"Error loading data: {ex.Message}")
        End Try
    End Sub

    Private Sub SaleType_CheckedChanged(sender As Object, e As EventArgs)
        If rbClientSale.Checked Then
            ' Show client selection controls
            lblClient.Visible = True
            cmbClient.Visible = True
            lblClientInfo.Visible = True

            ' Adjust positions for client selection
            AdjustControlPositions(True)
        Else
            ' Hide client selection controls
            lblClient.Visible = False
            cmbClient.Visible = False
            lblClientInfo.Visible = False

            ' Reset positions
            AdjustControlPositions(False)
        End If
    End Sub

    Private Sub AdjustControlPositions(showClientControls As Boolean)
        Dim yOffset As Integer = If(showClientControls, 60, 0)

        ' Adjust positions of controls below client selection
        lblProduct.Location = New Point(20, 220 - yOffset)
        cmbProduct.Location = New Point(20, 240 - yOffset)
        lblProductInfo.Location = New Point(20, 270 - yOffset)

        lblSupplier.Location = New Point(20, 320 - yOffset)
        cmbSupplier.Location = New Point(20, 340 - yOffset)
        lblSupplierInfo.Location = New Point(20, 370 - yOffset)

        lblQuantity.Location = New Point(20, 420 - yOffset)
        txtQuantity.Location = New Point(20, 440 - yOffset)

        lblUnitPrice.Location = New Point(200, 420 - yOffset)
        txtUnitPrice.Location = New Point(200, 440 - yOffset)

        lblTotalAmount.Location = New Point(380, 420 - yOffset)
        txtTotalAmount.Location = New Point(380, 440 - yOffset)

        lblCustomerName.Location = New Point(20, 480 - yOffset)
        txtCustomerName.Location = New Point(20, 500 - yOffset)

        lblNotes.Location = New Point(20, 540 - yOffset)
        txtNotes.Location = New Point(20, 560 - yOffset)

        btnCalculate.Location = New Point(20, 640 - yOffset)
        btnBuyProducts.Location = New Point(350, 640 - yOffset)
        btnCancel.Location = New Point(480, 640 - yOffset)

        ' Adjust form height
        Me.ClientSize = New Size(650, If(showClientControls, 750, 690))
    End Sub

    Private Sub cmbClient_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cmbClient.SelectedItem IsNot Nothing Then
            Dim selectedClient As User = DirectCast(cmbClient.SelectedItem, User)
            lblClientInfo.Text = $"Client: {selectedClient.FullName} | Username: {selectedClient.Username} | Email: {selectedClient.Email}"
        End If
    End Sub

    Private Sub cmbProduct_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cmbProduct.SelectedItem IsNot Nothing Then
            Dim selectedProduct As Product = DirectCast(cmbProduct.SelectedItem, Product)
            ' Show stock display with box/kilo indicator
            lblProductInfo.Text = selectedProduct.StockDisplay & $" | {selectedProduct.PriceDisplay} | Category: {selectedProduct.Category}"
            txtUnitPrice.Value = selectedProduct.Price
            CalculateTotal(Nothing, Nothing)
        End If
    End Sub

    Private Sub cmbSupplier_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cmbSupplier.SelectedItem IsNot Nothing Then
            Dim selectedSupplier As Supplier = DirectCast(cmbSupplier.SelectedItem, Supplier)
            lblSupplierInfo.Text = $"Contact: {selectedSupplier.ContactPerson} | Phone: {selectedSupplier.Phone} | Email: {selectedSupplier.Email}"
        End If
    End Sub

    Private Sub CalculateTotal(sender As Object, e As EventArgs)
        Dim total As Decimal = txtQuantity.Value * txtUnitPrice.Value
        txtTotalAmount.Text = total.ToString("C")
    End Sub

    Private Sub btnBuyProducts_Click(sender As Object, e As EventArgs)
        If Not ValidateInput() Then Return

        Try
            Dim selectedProduct As Product = DirectCast(cmbProduct.SelectedItem, Product)
            Dim selectedSupplier As Supplier = DirectCast(cmbSupplier.SelectedItem, Supplier)

            ' Enhanced stock validation for boxed products
            If selectedProduct.IsSoldByBox Then
                Dim availableKg As Decimal = If(selectedProduct.BoxQuantity.HasValue, selectedProduct.BoxQuantity.Value, selectedProduct.BoxSize) +
                                           ((selectedProduct.Stock - 1) * selectedProduct.BoxSize)
                If txtQuantity.Value > availableKg Then
                    MessageBox.Show($"Insufficient stock. Available: {availableKg:F2}kg")
                    Return
                End If
            Else
                If txtQuantity.Value > selectedProduct.Stock Then
                    MessageBox.Show($"Insufficient stock. Available: {selectedProduct.Stock}")
                    Return
                End If
            End If

            ' Determine which user ID to use for the sale
            Dim saleUserID As Integer
            Dim customerName As String = txtCustomerName.Text.Trim()

            If rbClientSale.Checked Then
                ' Client purchase - use selected client's UserID
                If cmbClient.SelectedItem Is Nothing Then
                    MessageBox.Show("Please select a client.")
                    Return
                End If
                Dim selectedClient As User = DirectCast(cmbClient.SelectedItem, User)
                saleUserID = selectedClient.UserID
                customerName = selectedClient.FullName ' Use client's name as customer
            Else
                ' Admin sale - use current admin's UserID
                saleUserID = UserSession.UserID
            End If

            ' Show confirmation dialog
            Dim confirmationMessage As String = BuildConfirmationMessage(selectedProduct, selectedSupplier, customerName)

            Dim result As DialogResult = MessageBox.Show(
                confirmationMessage,
                "Confirm Purchase",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            )

            If result <> DialogResult.Yes Then
                Return ' User cancelled
            End If

            Dim sale As New Sale With {
                .UserID = saleUserID,
                .ProductID = selectedProduct.ProductID,
                .SupplierID = selectedSupplier.SupplierID,
                .Quantity = txtQuantity.Value, ' Now supports decimal
                .UnitPrice = txtUnitPrice.Value,
                .TotalAmount = txtQuantity.Value * txtUnitPrice.Value,
                .SaleDate = DateTime.Now,
                .CustomerName = customerName,
                .Notes = txtNotes.Text.Trim()
            }

            Dim saleDAL As New SaleDAL()
            If saleDAL.AddSaleWithSupplier(sale) Then
                Dim purchaseType As String = If(rbClientSale.Checked, "Client purchase", "Admin purchase")
                MessageBox.Show($"{purchaseType} completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                MessageBox.Show("Failed to complete purchase. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error completing purchase: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function BuildConfirmationMessage(product As Product, supplier As Supplier, customerName As String) As String
        Dim purchaseType As String = If(rbClientSale.Checked, "client purchase", "admin purchase")
        Dim clientInfo As String = ""

        If rbClientSale.Checked AndAlso cmbClient.SelectedItem IsNot Nothing Then
            Dim selectedClient As User = DirectCast(cmbClient.SelectedItem, User)
            clientInfo = $"{vbNewLine}Client: {selectedClient.FullName} ({selectedClient.Username})"
        End If

        Dim quantityUnit As String = If(product.IsSoldByBox, "kg", product.PricingUnit)

        Return $"Please confirm this {purchaseType}:{vbNewLine}{vbNewLine}" &
               $"Product: {product.ProductName}{vbNewLine}" &
               $"Supplier: {supplier.SupplierName}{vbNewLine}" &
               $"Quantity: {txtQuantity.Value:F2} {quantityUnit}{vbNewLine}" &
               $"Unit Price: {txtUnitPrice.Value:C}{vbNewLine}" &
               $"Total Amount: {(txtQuantity.Value * txtUnitPrice.Value):C}{vbNewLine}" &
               $"Customer: {customerName}{clientInfo}{vbNewLine}{vbNewLine}" &
               "Do you want to proceed with this purchase?"
    End Function

    Private Function ValidateInput() As Boolean
        If cmbProduct.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a product.")
            cmbProduct.Focus()
            Return False
        End If

        If cmbSupplier.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a supplier.")
            cmbSupplier.Focus()
            Return False
        End If

        If txtQuantity.Value <= 0 Then
            MessageBox.Show("Please enter a valid quantity.")
            txtQuantity.Focus()
            Return False
        End If

        If txtUnitPrice.Value <= 0 Then
            MessageBox.Show("Please enter a valid unit price.")
            txtUnitPrice.Focus()
            Return False
        End If

        If rbClientSale.Checked Then
            If cmbClient.SelectedItem Is Nothing Then
                MessageBox.Show("Please select a client.")
                cmbClient.Focus()
                Return False
            End If
        Else
            If String.IsNullOrWhiteSpace(txtCustomerName.Text) Then
                MessageBox.Show("Please enter customer name.")
                txtCustomerName.Focus()
                Return False
            End If
        End If

        Return True
    End Function
End Class