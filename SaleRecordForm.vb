Imports System.Drawing
Imports System.Windows.Forms

Public Class SaleRecordForm
    Inherits Form

    Private ReadOnly lblTitle As New Label()
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
    Private ReadOnly btnRecordSale As New Button()
    Private ReadOnly btnCancel As New Button()

    Private ReadOnly lblProductInfo As New Label()
    Private ReadOnly lblSupplierInfo As New Label()

    Private products As List(Of Product)
    Private suppliers As List(Of Supplier)

    Public Sub New()
        InitializeComponent()
        LoadData()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Record Sale"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ClientSize = New Size(600, 650)
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)

        ' Title
        lblTitle.Text = "Record New Sale"
        lblTitle.Font = New Font("Segoe UI Semibold", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
        lblTitle.AutoSize = True
        lblTitle.Location = New Point(20, 20)

        ' Product Selection
        lblProduct.Text = "Product:"
        lblProduct.AutoSize = True
        lblProduct.Location = New Point(20, 70)

        cmbProduct.Location = New Point(20, 90)
        cmbProduct.Size = New Size(350, 27)
        cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList
        AddHandler cmbProduct.SelectedIndexChanged, AddressOf cmbProduct_SelectedIndexChanged

        lblProductInfo.Location = New Point(20, 120)
        lblProductInfo.Size = New Size(550, 40)
        lblProductInfo.ForeColor = Color.DarkBlue
        lblProductInfo.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular, GraphicsUnit.Point)

        ' Supplier Selection
        lblSupplier.Text = "Supplier:"
        lblSupplier.AutoSize = True
        lblSupplier.Location = New Point(20, 170)

        cmbSupplier.Location = New Point(20, 190)
        cmbSupplier.Size = New Size(350, 27)
        cmbSupplier.DropDownStyle = ComboBoxStyle.DropDownList
        AddHandler cmbSupplier.SelectedIndexChanged, AddressOf cmbSupplier_SelectedIndexChanged

        lblSupplierInfo.Location = New Point(20, 220)
        lblSupplierInfo.Size = New Size(550, 40)
        lblSupplierInfo.ForeColor = Color.DarkGreen
        lblSupplierInfo.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular, GraphicsUnit.Point)

        ' Quantity
        lblQuantity.Text = "Quantity:"
        lblQuantity.AutoSize = True
        lblQuantity.Location = New Point(20, 270)

        txtQuantity.Location = New Point(20, 290)
        txtQuantity.Size = New Size(150, 27)
        txtQuantity.Minimum = 1
        txtQuantity.Maximum = 999999
        txtQuantity.Value = 1
        AddHandler txtQuantity.ValueChanged, AddressOf CalculateTotal

        ' Unit Price
        lblUnitPrice.Text = "Unit Price:"
        lblUnitPrice.AutoSize = True
        lblUnitPrice.Location = New Point(200, 270)

        txtUnitPrice.Location = New Point(200, 290)
        txtUnitPrice.Size = New Size(150, 27)
        txtUnitPrice.DecimalPlaces = 2
        txtUnitPrice.Minimum = 0.01D
        txtUnitPrice.Maximum = 999999.99D
        AddHandler txtUnitPrice.ValueChanged, AddressOf CalculateTotal

        ' Total Amount
        lblTotalAmount.Text = "Total Amount:"
        lblTotalAmount.AutoSize = True
        lblTotalAmount.Location = New Point(380, 270)

        txtTotalAmount.Location = New Point(380, 290)
        txtTotalAmount.Size = New Size(150, 27)
        txtTotalAmount.ReadOnly = True
        txtTotalAmount.BackColor = Color.LightGray

        ' Customer Name
        lblCustomerName.Text = "Customer Name:"
        lblCustomerName.AutoSize = True
        lblCustomerName.Location = New Point(20, 330)

        txtCustomerName.Location = New Point(20, 350)
        txtCustomerName.Size = New Size(350, 27)

        ' Notes
        lblNotes.Text = "Notes (Optional):"
        lblNotes.AutoSize = True
        lblNotes.Location = New Point(20, 390)

        txtNotes.Location = New Point(20, 410)
        txtNotes.Size = New Size(550, 60)
        txtNotes.Multiline = True
        txtNotes.ScrollBars = ScrollBars.Vertical

        ' Calculate Button
        btnCalculate.Text = "Recalculate"
        btnCalculate.Location = New Point(20, 490)
        btnCalculate.Size = New Size(100, 35)
        btnCalculate.BackColor = Color.FromArgb(240, 173, 78)
        btnCalculate.ForeColor = Color.White
        btnCalculate.FlatStyle = FlatStyle.Flat
        AddHandler btnCalculate.Click, AddressOf CalculateTotal

        ' Record Sale Button
        btnRecordSale.Text = "Record Sale"
        btnRecordSale.Location = New Point(350, 490)
        btnRecordSale.Size = New Size(120, 35)
        btnRecordSale.BackColor = Color.FromArgb(92, 184, 92)
        btnRecordSale.ForeColor = Color.White
        btnRecordSale.FlatStyle = FlatStyle.Flat
        AddHandler btnRecordSale.Click, AddressOf btnRecordSale_Click

        ' Cancel Button
        btnCancel.Text = "Cancel"
        btnCancel.Location = New Point(480, 490)
        btnCancel.Size = New Size(100, 35)
        btnCancel.BackColor = Color.FromArgb(217, 83, 79)
        btnCancel.ForeColor = Color.White
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.DialogResult = DialogResult.Cancel
        AddHandler btnCancel.Click, Sub(s, e) Me.Close()

        ' Add all controls
        Me.Controls.AddRange({
            lblTitle, lblProduct, cmbProduct, lblProductInfo,
            lblSupplier, cmbSupplier, lblSupplierInfo,
            lblQuantity, txtQuantity, lblUnitPrice, txtUnitPrice,
            lblTotalAmount, txtTotalAmount, lblCustomerName, txtCustomerName,
            lblNotes, txtNotes, btnCalculate, btnRecordSale, btnCancel
        })

        Me.AcceptButton = btnRecordSale
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

        Catch ex As Exception
            MessageBox.Show($"Error loading data: {ex.Message}")
        End Try
    End Sub

    Private Sub cmbProduct_SelectedIndexChanged(sender As Object, e As EventArgs)
        If cmbProduct.SelectedItem IsNot Nothing Then
            Dim selectedProduct As Product = DirectCast(cmbProduct.SelectedItem, Product)
            lblProductInfo.Text = $"Stock: {selectedProduct.Stock} | Price: {selectedProduct.Price:C} | Category: {selectedProduct.Category}"
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

    Private Sub btnRecordSale_Click(sender As Object, e As EventArgs)
        If Not ValidateInput() Then Return

        Try
            Dim selectedProduct As Product = DirectCast(cmbProduct.SelectedItem, Product)
            Dim selectedSupplier As Supplier = DirectCast(cmbSupplier.SelectedItem, Supplier)

            ' Check stock availability
            If txtQuantity.Value > selectedProduct.Stock Then
                MessageBox.Show($"Insufficient stock. Available: {selectedProduct.Stock}")
                Return
            End If

            Dim sale As New Sale With {
                .UserID = UserSession.UserID,
                .ProductID = selectedProduct.ProductID,
                .SupplierID = selectedSupplier.SupplierID,
                .Quantity = CInt(txtQuantity.Value),
                .UnitPrice = txtUnitPrice.Value,
                .TotalAmount = txtQuantity.Value * txtUnitPrice.Value,
                .SaleDate = DateTime.Now,
                .CustomerName = txtCustomerName.Text.Trim(),
                .Notes = txtNotes.Text.Trim()
            }

            Dim saleDAL As New SaleDAL()
            If saleDAL.AddSaleWithSupplier(sale) Then
                MessageBox.Show("Sale recorded successfully!")
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                MessageBox.Show("Failed to record sale. Please try again.")
            End If

        Catch ex As Exception
            MessageBox.Show($"Error recording sale: {ex.Message}")
        End Try
    End Sub

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

        If String.IsNullOrWhiteSpace(txtCustomerName.Text) Then
            MessageBox.Show("Please enter customer name.")
            txtCustomerName.Focus()
            Return False
        End If

        Return True
    End Function
End Class