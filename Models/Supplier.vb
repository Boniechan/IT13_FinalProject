Public Class Supplier
    Public Property SupplierID As Integer
    Public Property SupplierName As String
    Public Property ContactPerson As String
    Public Property Phone As String
    Public Property Email As String
    Public Property Address As String
    Public Property CreatedDate As DateTime

    Public Overrides Function ToString() As String
        Return SupplierName
    End Function
End Class