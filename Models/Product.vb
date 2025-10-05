Public Class Product
    Public Property ProductID As Integer
    Public Property ProductName As String
    Public Property Category As String
    Public Property Price As Decimal
    Public Property Stock As Integer
    Public Property Description As String
    Public Property SupplierID As Integer
    Public Property CreatedDate As DateTime
    Public Property PricingUnit As String ' "Kilo", "Box", "Piece"
    Public Property BoxQuantity As Decimal? ' Remaining quantity in current box
    Public Property BoxSize As Decimal ' How many kilos per box (default 2.0)

    ' Helper property to display pricing information with indicator
    Public ReadOnly Property PriceDisplay As String
        Get
            If PricingUnit = "Box" Then
                Return $"{Price:C} per {PricingUnit} ({BoxSize}kg)"
            Else
                Return $"{Price:C} per {PricingUnit}"
            End If
        End Get
    End Property

    ' Helper property to display stock with indicator
    Public ReadOnly Property StockDisplay As String
        Get
            If PricingUnit = "Box" AndAlso BoxQuantity.HasValue Then
                Return $"{Stock} boxes ({BoxQuantity.Value:F1}kg remaining in current box)"
            ElseIf PricingUnit = "Box" Then
                Return $"{Stock} boxes (each {BoxSize}kg)"
            Else
                Return $"{Stock} {PricingUnit}s"
            End If
        End Get
    End Property

    ' Helper method to check if product is sold by box
    Public ReadOnly Property IsSoldByBox As Boolean
        Get
            Return PricingUnit = "Box"
        End Get
    End Property
End Class