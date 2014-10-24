Imports Skybound.Gecko

Public Class BasicBrowser

    'use `CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser)` to refer to the GeckoWebBrowser on the active tab
    'use `CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser)` to refer to the WebBrowser on the active tab

    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BasicBrowser)) ' Copied from the designer, so i can get resources at RunTime

    Public openWithURI As String
    Dim TabToClose As Integer

    Private Sub BasicBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each s As String In My.Application.CommandLineArgs
            If openWithURI = "" Then
                openWithURI = s
            Else
                openWithURI = openWithURI & s
            End If
        Next

        For i = 1 To My.Settings.Favourites.Count
            ToolStripURL.Items.Add(My.Settings.Favourites.Item(i - 1))
        Next
    End Sub

    ' MenuStrip options

    'File
    Sub NewTab(sender As Object, e As EventArgs) Handles ToolStripNewTab.Click, MenuStripFileNew.Click
        Dim TabPage As New TabPage()
        Dim WebBrowser As New GeckoWebBrowser
        AddHandler WebBrowser.Navigating, New GeckoNavigatingEventHandler(AddressOf Navigate)
        AddHandler WebBrowser.Navigated, New GeckoNavigatedEventHandler(AddressOf Navigate)
        AddHandler WebBrowser.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf DocumentCompleted)
        AddHandler WebBrowser.ProgressChanged, New GeckoProgressEventHandler(AddressOf GeckoProgressChanged)
        AddHandler WebBrowser.StatusTextChanged, AddressOf GeckoStatusTextChanged
        AddHandler WebBrowser.DocumentTitleChanged, AddressOf DocumentTitleChanged
        AddHandler WebBrowser.CanGoBackChanged, AddressOf PerformStuff
        AddHandler WebBrowser.CanGoForwardChanged, AddressOf PerformStuff
        TabPage.Text = "Loading... [G]"
        TabControl.TabPages.Add(TabPage)
        TabControl.SelectTab(TabControl.TabCount - 1)
        WebBrowser.Parent = TabPage
        WebBrowser.Dock = DockStyle.Fill
        WebBrowser.Visible = True
        ToolStripReload.Enabled = True
        ToolStripHome.Enabled = True
        ToolStripCloseTab.Enabled = True
        ToolStripGo.Enabled = True
        ToolStripURL.Enabled = True
        MenuStripFileCloseTab.Enabled = True
        MenuStripFileOpen.Enabled = True
        MenuStripFileSave.Enabled = True
        MenuStripFilePrint.Enabled = True
        MenuStripFilePrintPreview.Enabled = True
        MenuStripViewSource.Enabled = True
        MenuStripToolsSetup.Enabled = True
        MenuStripToolsProperties.Enabled = True
        If openWithURI = "" Then
            WebBrowser.Navigate("https://google.com")
        Else
            WebBrowser.Navigate(openWithURI)
            openWithURI = ""
        End If
    End Sub

    Private Sub CloseTab(sender As Object, e As EventArgs) Handles ToolStripCloseTab.Click, MenuStripFileCloseTab.Click
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Navigate("about:blank") ' To make sure it doesn't take up extra memory
        ElseIf TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Navigate("about:blank")
        End If
        TabToClose = TabControl.SelectedIndex
        If TabControl.TabCount > 1 Then
            '     If you have selected the first tab, select the second tab
            If TabControl.SelectedIndex = 0 Then
                TabControl.SelectTab(1)
                ' If you have selected the last tab, select the second last tab
            ElseIf TabControl.TabCount = TabControl.SelectedIndex + 1 Then
                TabControl.SelectTab(TabControl.SelectedIndex - 1)
            Else 'Else select the next tab
                TabControl.SelectTab(TabControl.SelectedIndex + 1)
            End If
        End If
        TabControl.TabPages.RemoveAt(TabToClose)

        If TabControl.TabCount = 0 Then
            ToolStripBack.Enabled = False
            ToolStripForward.Enabled = False
            ToolStripReload.Enabled = False
            ToolStripStop.Enabled = False
            ToolStripHome.Enabled = False
            ToolStripCloseTab.Enabled = False
            ToolStripGo.Enabled = False
            ToolStripURL.Enabled = False
            MenuStripFileCloseTab.Enabled = False
            MenuStripFileOpen.Enabled = False
            MenuStripFileSave.Enabled = False
            MenuStripFilePrint.Enabled = False
            MenuStripFilePrintPreview.Enabled = False
            MenuStripViewSource.Enabled = False
            MenuStripToolsSetup.Enabled = False
            MenuStripToolsProperties.Enabled = False
        Else
            PerformStuff()
        End If
    End Sub

    Private Sub MenuStripFileNewWindow_Click(sender As Object, e As EventArgs) Handles MenuStripFileNewWindow.Click
        Dim NewWindow As BasicBrowser = New BasicBrowser
        NewWindow.Show()
    End Sub

    Private Sub MenuStripFileCloseWindow_Click(sender As Object, e As EventArgs) Handles MenuStripFileCloseWindow.Click
        For i = 1 To TabControl.TabCount
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                CType(TabControl.TabPages.Item(0).Controls.Item(0), GeckoWebBrowser).Navigate("about:blank")
            ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Navigate("about:blank")
            End If
            TabControl.TabPages.RemoveAt(0)
        Next
        Me.Close()
    End Sub

    Private Sub MenuStripFileOpen_Click(sender As Object, e As EventArgs) Handles MenuStripFileOpen.Click
        Dim OpenFileDialog As New OpenFileDialog()
        OpenFileDialog.FileName = ""
        OpenFileDialog.Filter = "Webpages|*.html|All Files|*.*"
        OpenFileDialog.Title = "Open Webpage"
        If (OpenFileDialog.ShowDialog() = DialogResult.OK) Then
            If TabControl.SelectedTab.Text.EndsWith("[I]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).DocumentText = System.IO.File.ReadAllText(OpenFileDialog.FileName)
            End If
        End If
    End Sub

    Private Sub MenuStripFileSave_Click(sender As Object, e As EventArgs) Handles MenuStripFileSave.Click
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            Dim SaveFileDialog As New SaveFileDialog()
            SaveFileDialog.FileName = ""
            SaveFileDialog.Filter = "Webpages|*.html|All Files|*.*"
            SaveFileDialog.Title = "Open Webpage"
            If (SaveFileDialog.ShowDialog() = DialogResult.OK) Then
                CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).SaveDocument(SaveFileDialog.FileName)
            End If
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).ShowSaveAsDialog()
        End If
    End Sub

    Private Sub MenuStripFilePrint_Click(sender As Object, e As EventArgs) Handles MenuStripFilePrint.Click
        If TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).ShowPrintDialog()
        End If
    End Sub

    Private Sub MenuStripFilePrintPreview_Click(sender As Object, e As EventArgs) Handles MenuStripFilePrintPreview.Click
        If TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).ShowPrintPreviewDialog()
            Me.WindowState = FormWindowState.Minimized
        End If
    End Sub

    Private Sub ExitBasicBrowser(sender As Object, e As EventArgs) Handles MenuStripFileExit.Click
        MenuStripFileCloseWindow_Click(Nothing, Nothing)
        Application.Exit()
    End Sub

    'Edit
    Private Sub MenuStripEditUndo_Click(sender As Object, e As EventArgs) Handles MenuStripEditUndo.Click
        If ToolStripURL.Focused = True Then
            'ToolStripURL.Undo()
        ElseIf TabControl.TabCount <> 0 Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Undo()
        End If
    End Sub

    Private Sub MenuStripEditRedo_Click(sender As Object, e As EventArgs) Handles MenuStripEditRedo.Click
        If ToolStripURL.Focused = True Then
            'ToolStripURL.Redo()
        ElseIf TabControl.TabCount <> 0 Then
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Redo()
            End If
        End If
    End Sub

    Private Sub MenuStripEditCut_Click(sender As Object, e As EventArgs) Handles MenuStripEditCut.Click
        If ToolStripURL.Focused = True Then
            If ToolStripURL.SelectedText = "" Then
                Clipboard.SetText(ToolStripURL.Text)
                ToolStripURL.Text = ""
            Else
                Clipboard.SetText(ToolStripURL.SelectedText)
                ToolStripURL.Text = ToolStripURL.Text.Remove(ToolStripURL.SelectionStart, ToolStripURL.SelectionLength)
            End If
        ElseIf TabControl.TabCount <> 0 Then
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).CutSelection()
            End If
        End If
    End Sub

    Private Sub MenuStripEditCopy_Click(sender As Object, e As EventArgs) Handles MenuStripEditCopy.Click
        If ToolStripURL.Focused = True Then
            If ToolStripURL.SelectedText = "" Then
                Clipboard.SetText(ToolStripURL.Text)
            Else
                Clipboard.SetText(ToolStripURL.SelectedText)
            End If
        ElseIf TabControl.TabCount <> 0 Then
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).CopySelection()
            End If
        End If
    End Sub

    Private Sub MenuStripEditPaste_Click(sender As Object, e As EventArgs) Handles MenuStripEditPaste.Click
        If ToolStripURL.Focused = True Then
            ToolStripURL.Text = ToolStripURL.Text.Remove(ToolStripURL.SelectionStart) & Clipboard.GetText & ToolStripURL.Text.Remove(0, ToolStripURL.SelectionStart)
        ElseIf TabControl.TabCount <> 0 Then
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Paste()
            End If
        End If
    End Sub

    Private Sub MenuStripEditSelectAll_Click(sender As Object, e As EventArgs) Handles MenuStripEditSelectAll.Click
        If ToolStripURL.Focused = True Then
            ToolStripURL.SelectAll()
        ElseIf TabControl.TabCount <> 0 Then
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).SelectAll()
            End If
        End If
    End Sub

    'View
    Private Sub MenuStripViewKeepOnTop_CheckedChanged(sender As Object, e As EventArgs) Handles MenuStripViewKeepOnTop.CheckedChanged
        Me.TopMost = MenuStripViewKeepOnTop.Checked
    End Sub

    Private Sub MenuStripViewOpacityLbl_Click(sender As Object, e As EventArgs) Handles MenuStripViewOpacityLbl.Click
        MenuStripViewOpacityCbx.Focus()
    End Sub

    Private Sub MenuStripViewOpacityCbx_TextChanged(sender As Object, e As EventArgs) Handles MenuStripViewOpacityCbx.TextChanged
        Me.Opacity = MenuStripViewOpacityCbx.Text.Remove(MenuStripViewOpacityCbx.Text.LastIndexOf("%")) / 100
    End Sub

    Private Sub MenuStripViewSource_Click(sender As Object, e As EventArgs) Handles MenuStripViewSource.Click
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).ViewSource()
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            Dim sourceForm As New Form()
            sourceForm.Width = 450
            sourceForm.Height = 350
            sourceForm.StartPosition = FormStartPosition.CenterParent
            sourceForm.WindowState = Me.WindowState
            sourceForm.Icon = CType(resources.GetObject("SourceCodeIcon"), System.Drawing.Icon)
            sourceForm.ShowIcon = True
            sourceForm.ShowInTaskbar = True
            sourceForm.Text = "Source Code for " & CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Url.ToString
            Dim sourceCode As New TextBox()
            sourceCode.Multiline = True
            sourceCode.ScrollBars = ScrollBars.Both
            sourceForm.Controls.Add(sourceCode)
            sourceCode.Dock = DockStyle.Fill
            sourceCode.Text = CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).DocumentText
            sourceForm.Show()
        End If
    End Sub

    'Tools
    Private Sub MenuStripToolsSetup_Click(sender As Object, e As EventArgs) Handles MenuStripToolsSetup.Click
        If TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).ShowPageSetupDialog()
        End If
    End Sub

    Private Sub MenuStripToolsProperties_Click(sender As Object, e As EventArgs) Handles MenuStripToolsProperties.Click
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).ShowPageProperties()
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).ShowPropertiesDialog()
        End If
    End Sub

    'About
    Private Sub MenuStripHelpAbout_Click(sender As Object, e As EventArgs) Handles MenuStripHelpAbout.Click
        Dim AboutForm As New Form()
        AboutForm.Width = 450
        AboutForm.Height = 350
        AboutForm.StartPosition = FormStartPosition.CenterParent
        AboutForm.Icon = CType(resources.GetObject("SourceCodeIcon"), System.Drawing.Icon)
        AboutForm.ShowIcon = True
        AboutForm.ShowInTaskbar = True
        AboutForm.Text = "About HybridBasicBrowser"
        Dim lblAboutText As New Label()
        'lblAboutText.Font = 
        lblAboutText.TextAlign = ContentAlignment.MiddleCenter
        AboutForm.Controls.Add(lblAboutText)
        lblAboutText.Dock = DockStyle.Fill
        lblAboutText.Text = _
            "Made by Walkman100" & vbNewLine & vbNewLine & _
            "Source code available at: https://github.com/Walkman100/BasicBrowser/tree/hybrid" & vbNewLine & vbNewLine & _
            "Go to http://github.com/Walkman100/BasicBrowser/issues/new to report bugs or suggest features" & vbNewLine & vbNewLine & _
            "Hold ALT to reorganise all the buttons/menus at the top" & vbNewLine & vbNewLine & _
            "Current Version: " & My.Application.Info.Version.ToString
        AboutForm.Show()
    End Sub

    ' ToolStrip options

    Private Sub ToolStripBack_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripBack.ButtonClick
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).GoBack()
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).GoBack()
        End If
        ToolStripStop.Enabled = True
        PerformStuff()
    End Sub

    Private Sub ToolStripBack_DropDownOpening(sender As Object, e As EventArgs) Handles ToolStripBack.DropDownOpening
        ToolStripBack.DropDownItems.Clear()
        For i = 1 To CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).History.Count
            'Dim HistoryItem As New ToolStripMenuItem
            'HistoryItem.Text = CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).History.Item(i - 1).ToString
            'ToolStripBack.DropDownItems.Add(HistoryItem)
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                ToolStripBack.DropDownItems.Add(CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).History.Item(i - 1).ToString)
            ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
                'ToolStripBack.DropDownItems.Add(CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).History.Item(i - 1).ToString)
            End If
        Next
    End Sub

    Private Sub ToolStripForward_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripForward.ButtonClick
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).GoForward()
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).GoForward()
        End If
        ToolStripStop.Enabled = True
        PerformStuff()
    End Sub

    Private Sub ToolStripForward_DropDownOpening(sender As Object, e As EventArgs) Handles ToolStripForward.DropDownOpening
        ToolStripForward.DropDownItems.Clear()
        For i = 1 To CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).History.Count
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                ToolStripBack.DropDownItems.Add(CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).History.Item(i - 1).ToString)
            ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
                'ToolStripBack.DropDownItems.Add(CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).History.Item(i - 1).ToString)
            End If
        Next
    End Sub

    Private Sub ToolStripReload_Click(sender As Object, e As EventArgs) Handles ToolStripReload.Click
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Reload()
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Refresh()
        End If
        ToolStripStop.Enabled = True
        PerformStuff()
    End Sub

    Private Sub ToolStripStop_Click(sender As Object, e As EventArgs) Handles ToolStripStop.Click
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Stop()
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Stop()
        End If
        ToolStripStop.Enabled = False
        PerformStuff()
    End Sub

    Private Sub ToolStripHome_Click(sender As Object, e As EventArgs) Handles ToolStripHome.Click
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Navigate("https://google.com")
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).GoHome()
        End If
        ToolStripStop.Enabled = True
        PerformStuff()
    End Sub

    Private Sub ToolStripURL_KeyDown(sender As Object, e As KeyEventArgs) Handles ToolStripURL.KeyDown
        If e.KeyCode = Keys.Enter Then
            ToolStripGo_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub ToolStripURL_Focused(sender As Object, e As EventArgs) Handles ToolStripURL.GotFocus
        ToolStripURL.SelectAll()
    End Sub

    Private Sub ToolStripGo_Click(sender As Object, e As EventArgs) Handles ToolStripGo.Click
        If ToolStripURL.Text <> "" Then
            If TabControl.SelectedTab.Text.EndsWith("[G]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Navigate(ToolStripURL.Text)
                CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Focus()
            ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
                CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Navigate(ToolStripURL.Text)
                CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Focus()
            End If
        Else
            ToolStripURL.Focus()
        End If
    End Sub

    Private Sub TabControl_Click(sender As Object, e As EventArgs) Handles TabControl.Click, TabControl.KeyUp
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            ToolStripStop.Enabled = CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).IsBusy
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            ToolStripStop.Enabled = CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).IsBusy
        End If
        PerformStuff()
    End Sub

    Private Sub BasicBrowser_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged, MyBase.Resize
        ToolStripURL.Size = New Size(Me.Width - 243, 25)
    End Sub

    'Favourites bar (Integrated into URL bar)
    Private Sub ToolStripAdd_Click(sender As Object, e As EventArgs) Handles ToolStripAdd.Click
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            ToolStripURL.Items.Add(CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Url.ToString)
            My.Settings.Favourites.Add(CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Url.ToString)
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            ToolStripURL.Items.Add(CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Url.ToString)
            My.Settings.Favourites.Add(CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Url.ToString)
        End If
        My.Settings.Save()
    End Sub

    Private Sub ToolStripRemove_Click(sender As Object, e As EventArgs) Handles ToolStripRemove.Click
        ToolStripURL.Items.RemoveAt(ToolStripURL.SelectedIndex)
        My.Settings.Favourites.Remove(ToolStripURL.SelectedItem)
        My.Settings.Save()
    End Sub

    Private Sub ToolStripURL_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripURL.SelectedIndexChanged
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Navigate(ToolStripURL.Items.Item(ToolStripURL.SelectedIndex).ToString)
            CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Focus()
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Navigate(ToolStripURL.Items.Item(ToolStripURL.SelectedIndex).ToString)
            CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Focus()
        End If
        ToolStripURL.Invalidate()
    End Sub

    ' browser stuff

    Sub Navigate()
        ToolStripStop.Enabled = True
        PerformStuff()
    End Sub

    Sub DocumentCompleted()
        ToolStripStop.Enabled = False
        PerformStuff()
    End Sub

    Sub GeckoProgressChanged(ByVal sender As Object, ByVal e As Skybound.Gecko.GeckoProgressEventArgs)
        StatusStripProgressBar.Value = (e.CurrentProgress / e.MaximumProgress) * 100
        If ToolStripURL.Focused = False Then
            ToolStripURL.Text = CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Url.ToString
        End If
    End Sub
    Sub IEProgressChanged(ByVal sender As Object, ByVal e As WebBrowserProgressChangedEventArgs)
        StatusStripProgressBar.Value = (e.CurrentProgress / e.MaximumProgress) * 100
        If ToolStripURL.Focused = False Then
            ToolStripURL.Text = CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Url.ToString
        End If
    End Sub

    Sub GeckoStatusTextChanged()
        StatusStripStatusText.Text = CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).StatusText
    End Sub
    Sub IEStatusTextChanged()
        StatusStripStatusText.Text = CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).StatusText
    End Sub

    Sub DocumentTitleChanged()
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            If CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).DocumentTitle <> "" Then
                Me.Text = CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).DocumentTitle & " - BasicBrowser"
            End If
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            If CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).DocumentTitle <> "" Then
                Me.Text = CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).DocumentTitle & " - BasicBrowser"
            End If
        End If
        For i = 1 To TabControl.TabCount
            If TabControl.TabPages.Item(i - 1).Text.EndsWith("[G]") Then
                TabControl.TabPages.Item(i - 1).Text = CType(TabControl.TabPages.Item(i - 1).Controls.Item(0), GeckoWebBrowser).DocumentTitle & " - [G]"
            ElseIf TabControl.TabPages.Item(i - 1).Text.EndsWith("[I]") Then
                TabControl.TabPages.Item(i - 1).Text = CType(TabControl.TabPages.Item(i - 1).Controls.Item(0), WebBrowser).DocumentTitle & " - [I]"
            End If
        Next
    End Sub
    
    Sub PerformStuff()
        If TabControl.SelectedTab.Text.EndsWith("[G]") Then
            ToolStripBack.Enabled = CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).CanGoBack
            ToolStripForward.Enabled = CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).CanGoForward
            If ToolStripURL.Focused = False Then
                ToolStripURL.Text = CType(TabControl.SelectedTab.Controls.Item(0), GeckoWebBrowser).Url.ToString
            End If
        ElseIf TabControl.SelectedTab.Text.EndsWith("[I]") Then
            ToolStripBack.Enabled = CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).CanGoBack
            ToolStripForward.Enabled = CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).CanGoForward
            If ToolStripURL.Focused = False Then
                ToolStripURL.Text = CType(TabControl.SelectedTab.Controls.Item(0), WebBrowser).Url.ToString
            End If
        End If
    End Sub
End Class