Option Strict On
Option Explicit On


Public Class MessageLog


    Public Sub addMessage(ByVal fromId As Integer, ByVal toId As Integer, ByVal mesg As String)
        Dim item As New ListViewItem
        Dim timestampStr As String
        Dim tostr As String

        '
        '   Remove extra messages (based on the max message count)
        '
        While ((maxQuestions.Value > 0) And (logView.Items.Count >= maxQuestions.Value))
            logView.Items.RemoveAt(0)
        End While

        tostr = toId.ToString

        If (toId = -1) Then
            tostr = "<all>"
        End If

        mesg = mesg.Trim(ControlChars.Quote)             ' Remove the outer quotes

        timestampStr = Now.ToString()
        item.Text = timestampStr
        ' item.SubItems.Add(timestampStr)
        item.SubItems.Add(fromId.ToString)
        item.SubItems.Add(tostr)
        item.SubItems.Add(mesg)

        logView.Items.Add(item)

    End Sub
    Private Sub clearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles clearButton.Click
        logView.Items.Clear()
    End Sub

    Private Sub MessageLog_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If (e.CloseReason = CloseReason.UserClosing) Then
            Me.Hide()
            e.Cancel = True
            controlForm.MessageLogToolStripMenuItem.Checked = False
        End If

    End Sub



    Private Sub MessageLog_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
        controlForm.MessageLogToolStripMenuItem.Checked = Me.Visible
    End Sub



    Private Sub saveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles saveButton.Click
        Dim dialog As New SaveFileDialog
        Dim stream As IO.Stream
        Dim wstream As IO.StreamWriter
        Dim i As Integer
        Dim line As String

        dialog.DefaultExt = ".txt"     ' Save to a text file
        dialog.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*"
        dialog.ShowDialog()
        stream = dialog.OpenFile()

        wstream = New IO.StreamWriter(stream)

        For i = 0 To logView.Items.Count - 1
            line = logView.Items(i).Text
            line &= vbTab

            line &= logView.Items(i).SubItems(1).Text
            line &= vbTab

            line &= logView.Items(i).SubItems(2).Text
            line &= vbTab

            line &= logView.Items(i).SubItems(3).Text
            line &= vbCrLf

            wstream.Write(line)
        Next i

        wstream.Close()

    End Sub


End Class