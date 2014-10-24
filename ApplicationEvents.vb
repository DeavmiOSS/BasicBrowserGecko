﻿Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Private Sub StartFromFile(sender As Object, e As ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
            For Each s As String In My.Application.CommandLineArgs
                If BasicBrowser.openWithURI = "" Then
                    BasicBrowser.openWithURI = s
                Else
                    BasicBrowser.openWithURI = BasicBrowser.openWithURI & s
                End If
            Next
            'Should this open a Gecko or IE tab by default?
            BasicBrowser.NewGeckoTab(Nothing, Nothing)
            BasicBrowser.BringToFront()
        End Sub
    End Class
End Namespace