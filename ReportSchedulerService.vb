Imports System.IO
Imports System.Threading
Imports System.Windows.Forms

Public Class ReportSchedulerService
    Private ReadOnly reportManager As New ExcelReportManager()
    Private dailyTimer As System.Threading.Timer
    Private weeklyTimer As System.Threading.Timer
    Private ReadOnly reportFolder As String

    Public Sub New()
        reportFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Fisheries_Reports")
        Directory.CreateDirectory(reportFolder)
        Directory.CreateDirectory(Path.Combine(reportFolder, "Daily"))
        Directory.CreateDirectory(Path.Combine(reportFolder, "Weekly"))
        Directory.CreateDirectory(Path.Combine(reportFolder, "Monthly"))
    End Sub

    Public Sub StartScheduledReports()
        ' Set up daily timer (runs every 24 hours at 6 AM)
        SetupDailyTimer()

        ' Set up weekly timer (runs every Monday at 7 AM)
        SetupWeeklyTimer()
    End Sub

    Private Sub SetupDailyTimer()
        ' Calculate time until next 6 AM
        Dim now = DateTime.Now
        Dim targetTime = DateTime.Today.AddHours(6) ' 6 AM today
        If now > targetTime Then
            targetTime = targetTime.AddDays(1) ' 6 AM tomorrow
        End If

        Dim initialDelay = CInt((targetTime - now).TotalMilliseconds)

        dailyTimer = New System.Threading.Timer(AddressOf DailyReportCallback, Nothing, initialDelay, 24 * 60 * 60 * 1000) ' 24 hours
    End Sub

    Private Sub SetupWeeklyTimer()
        ' Calculate time until next Monday 7 AM
        Dim now = DateTime.Now
        Dim daysUntilMonday = ((7 - CInt(now.DayOfWeek)) + 1) Mod 7
        If daysUntilMonday = 0 AndAlso now.Hour >= 7 Then
            daysUntilMonday = 7 ' Next Monday if it's already past 7 AM on Monday
        End If

        Dim targetTime = DateTime.Today.AddDays(daysUntilMonday).AddHours(7) ' 7 AM next Monday
        Dim initialDelay = CInt((targetTime - now).TotalMilliseconds)

        weeklyTimer = New System.Threading.Timer(AddressOf WeeklyReportCallback, Nothing, initialDelay, 7 * 24 * 60 * 60 * 1000) ' 7 days
    End Sub

    Private Sub DailyReportCallback(state As Object)
        Try
            Dim yesterday = DateTime.Today.AddDays(-1)
            Dim fileName = Path.Combine(reportFolder, "Daily", $"Daily_Sales_Report_{yesterday:yyyy_MM_dd}.csv")
            reportManager.GenerateDailySalesReport(yesterday, fileName)
        Catch ex As Exception
            ' Log error or handle as needed
            Console.WriteLine($"Error generating daily report: {ex.Message}")
        End Try
    End Sub

    Private Sub WeeklyReportCallback(state As Object)
        Try
            ' Generate report for last week (Monday to Sunday)
            Dim today = DateTime.Today
            Dim daysToSubtract = (CInt(today.DayOfWeek) + 6) Mod 7
            Dim currentWeekMonday = today.AddDays(-daysToSubtract)
            Dim lastWeekMonday = currentWeekMonday.AddDays(-7)

            Dim fileName = Path.Combine(reportFolder, "Weekly", $"Weekly_Sales_Report_{lastWeekMonday:yyyy_MM_dd}.csv")
            reportManager.GenerateWeeklySalesReport(lastWeekMonday, fileName)
        Catch ex As Exception
            ' Log error or handle as needed
            Console.WriteLine($"Error generating weekly report: {ex.Message}")
        End Try
    End Sub

    Public Sub StopScheduledReports()
        dailyTimer?.Dispose()
        weeklyTimer?.Dispose()
    End Sub

    ' Manual triggers for testing
    Public Sub GenerateYesterdayReport()
        DailyReportCallback(Nothing)
    End Sub

    Public Sub GenerateLastWeekReport()
        WeeklyReportCallback(Nothing)
    End Sub
End Class