Option Strict On
Option Explicit On

'
'           v0.1    - Initial version
'           v0.2    - Conform to Option Explicit
'           V0.3    - Make it work with newer version of SimpleTCPNetwork
'
'

'
'   TODO:  Remove the reference to playingField.tcpNet and replace it with a calls
'
Public Class controlForm
    Private startupDone As Boolean = False

    Private NewLabelText As String = ""


    Private Sub controlForm_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        If (playingField IsNot Nothing) Then
            playingField.Close()
        End If

    End Sub


    Private Sub controlForm_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        playingField.Show()

        ' startupTimer.Start()
    End Sub

    Private Sub controlForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Randomize()
    End Sub

    '
    '   Control whether the game is in progress or not
    '
    Private Sub startStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles startStop.Click
        If ((startStop.Text = "Start") Or (startStop.Text = "Resume")) Then
            playingField.start_action()
            stepButton.Enabled = False
            startStop.Text = "Pause"
            playingField.reminderBox.Visible = False
        Else
            playingField.stop_action()
            startStop.Text = "Resume"
            stepButton.Enabled = True
        End If
    End Sub


    Private Sub createPlayerButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles createPlayerButton.Click
        Dim XSpeed As Double
        Dim YSpeed As Double
        Dim I As Integer
        Dim red As Integer
        Dim blue As Integer
        Dim team As playingField.teamSide = playingField.teamSide.RED_TEAM

        playingField.Get_Player_Count(red, blue)
        If (red > blue) Then
            team = playingField.teamSide.BLUE_TEAM
        End If

        I = playingField.create_player(-1, -1, -1, team)

        XSpeed = Rnd() * 10
        YSpeed = Rnd() * 10

        If (Rnd() > 0.5) Then XSpeed = -XSpeed
        If (Rnd() > 0.5) Then YSpeed = -YSpeed

        playingField.Move_Piece(I, XSpeed, YSpeed)

        playerNum.Maximum = playingField.theoreticalPlayers
    End Sub

    Private Sub upButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles upButton.Click
        playingField.Change_Speed(CInt(playerNum.Value), 0, -speedNum.Value)
    End Sub

    Private Sub downButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles downButton.Click
        playingField.Change_Speed(CInt(playerNum.Value), 0, speedNum.Value)
    End Sub

    Private Sub rightButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rightButton.Click
        playingField.Change_Speed(CInt(playerNum.Value), speedNum.Value, 0)
    End Sub

    Private Sub leftButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles leftButton.Click
        playingField.Change_Speed(CInt(playerNum.Value), -speedNum.Value, 0)
    End Sub

    Private Sub northWestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles northWestButton.Click
        playingField.Change_Speed(CInt(playerNum.Value), -speedNum.Value / 2, -speedNum.Value / 2)
    End Sub

    Private Sub northEastButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles northEastButton.Click
        playingField.Change_Speed(CInt(playerNum.Value), speedNum.Value / 2, -speedNum.Value / 2)
    End Sub

    Private Sub southWestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles southWestButton.Click
        playingField.Change_Speed(CInt(playerNum.Value), -speedNum.Value / 2, speedNum.Value / 2)
    End Sub

    Private Sub southEastButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles southEastButton.Click
        playingField.Change_Speed(CInt(playerNum.Value), speedNum.Value / 2, speedNum.Value / 2)
    End Sub

    Private Sub stopButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles stopButton.Click
        playingField.Multiply_Speed(CInt(playerNum.Value), 0.0, 0.0)
    End Sub

    Private Sub stepButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles stepButton.Click
        playingField.stop_action()
        playingField.step_action()
    End Sub

    Private Sub deleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles deleteButton.Click
        playingField.Delete_Player(CInt(playerNum.Value))
        playerNum.Maximum = playingField.numPlayers
    End Sub

    Private Sub resetGameButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles resetGameButton.Click
        playingField.Reset_Game(True)
    End Sub

    Private Sub netStartStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles netStartStop.Click
        If (playingField.tcpNet.serverRunning()) Then
            playingField.tcpNet.StopServer()
        Else
            playingField.tcpNet.StartServer(CInt(portNum.Value))
            playingField.mode() = playingField.playingMode.server
        End If

        If (playingField.tcpNet.serverRunning()) Then
            netStartStop.Text = "Stop Server"
            fieldControls.Visible = True
            startViewerButton.Enabled = False
            playingField.reminderBox.Visible = True
        Else
            netStartStop.Text = "Start Server"
            fieldControls.Visible = False
            startViewerButton.Enabled = True
            playingField.reminderBox.Visible = False
        End If

        UpdateStatus()
    End Sub

    Public Sub UpdateStatus()
        Dim connections As Integer
        Dim newText As String

        If (playingField.tcpNet.serverRunning()) Then
            connections = playingField.tcpNet.numConnections()

            If (connections = 0) Then
                newText = "Server Ready..."
            Else
                newText = connections.ToString & " connections"
            End If
        Else
            newText = "Server Stopped..."
        End If

        NewLabelText = newText

    End Sub


    Private Sub startViewerButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles startViewerButton.Click
        Dim ok As Boolean

        playingField.mode = playingField.playingMode.viewer

        If (playingField.tcpNet.numConnections() = 0) Then
            netStartStop.Enabled = False
            ParametersToolStripMenuItem.Enabled = False
            startViewerButton.Enabled = False
            Me.Refresh()

            ok = playingField.tcpNet.StartClient(serverName.Text, CInt(portNum.Value))

            If (ok) Then
                startViewerButton.Text = "Stop Viewer"

                StatusLabel.Text = "Connected to server..."
                Me.WindowState = FormWindowState.Minimized
            Else
                netStartStop.Enabled = True
                ParametersToolStripMenuItem.Enabled = True

                MsgBox("Connection to server failed")
                StatusLabel.Text = "Failed to connect to server..."
            End If

            startViewerButton.Enabled = True
        Else
            netStartStop.Enabled = True
            ParametersToolStripMenuItem.Enabled = True
            startViewerButton.Text = "Start Viewer"
            StatusLabel.Text = "Closed connection to server..."
            playingField.tcpNet.CloseConnection()
        End If
    End Sub


    Private Sub ResetBall_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetBall.Click
        playingField.Reset_Game(False)
    End Sub

    Private Sub createBall_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles createBall.Click
        Dim XSpeed As Double
        Dim YSpeed As Double
        Dim I As Integer

        I = playingField.create_player(-1, _
                                       CInt((playingField.Width / 2) - 5 + (10 * Rnd())), _
                                       CInt((playingField.Height / 2) - 5 + (10 * Rnd())), _
                                       playingField.teamSide.NO_TEAM, "", playerOrObject.playerType.ball)

        XSpeed = 0
        YSpeed = 0

        playingField.Move_Piece(I, XSpeed, YSpeed)

        playerNum.Maximum = playingField.theoreticalPlayers
    End Sub

    Private Sub UpdateStatusTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpdateStatusTimer.Tick
        If (NewLabelText <> "") Then
            StatusLabel.Text = NewLabelText
            NewLabelText = ""
            playerNum.Maximum = playingField.theoreticalPlayers()
        End If

        DynamicLabel.Text = playingField.numPlayersTotal.ToString & " robots, " & _
            playingField.numTicksPerSecond.ToString & " moves/sec"
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        MainAboutBox.Show()
    End Sub

    Private Sub PlayfieldToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlayfieldToolStripMenuItem.Click
        If (playingField.Visible) Then
            playingField.Hide()
        Else
            playingField.Show()

            If (playingField.WindowState = FormWindowState.Minimized) Then
                playingField.WindowState = FormWindowState.Normal
            End If

            playingField.BringToFront()
        End If

    End Sub

    Private Sub ParametersToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ParametersToolStripMenuItem.Click
        If (Parameters.Visible) Then
            Parameters.Hide()
        Else
            Parameters.Show()
            If (Parameters.WindowState = FormWindowState.Minimized) Then
                Parameters.WindowState = FormWindowState.Normal
            End If

            Parameters.BringToFront()
        End If
    End Sub

    Private Sub moveBallRandomButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles moveBallRandomButton.Click
        playingField.randomly_place_balls()
    End Sub

    Private Sub MessageLogToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MessageLogToolStripMenuItem.Click
        MessageLog.Show()
    End Sub



    Private Sub playerNum_ValueChanged(sender As System.Object, e As System.EventArgs) Handles playerNum.ValueChanged
        idLabel.Text = playingField.describe_object(CInt(playerNum.Value))
    End Sub

    Private Sub playerNum_VisibleChanged(sender As Object, e As System.EventArgs) Handles playerNum.VisibleChanged
        If (playerNum.Visible) Then
            idLabel.Text = playingField.describe_object(CInt(playerNum.Value))
        End If
    End Sub

    '
    '  Do the delayed start stuff
    '
    Private Sub startupTimer_Tick(sender As System.Object, e As System.EventArgs) Handles startupTimer.Tick
        startupTimer.Stop()

        If (Not startupDone) Then
            startupDone = True

            Me.Left = playingField.Right
            Me.Top = playingField.Top - ((Me.Height - playingField.Height) \ 2)

            ' Don't allow the control to be off of the top of the screen
            If (Me.Top < 10) Then
                Me.Top = 10
            End If


            hostNameLabel.Text = SimpleTCPNetwork.mydnsname
            ipAddrLabel.Text = SimpleTCPNetwork.myipaddr
            serverName.Text = hostNameLabel.Text
            fieldControls.Visible = False

            UpdateStatus()
        End If

    End Sub

    Private Sub SplotchFieldToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles SplotchFieldToolStripMenuItem.Click
        playingField.splotchField()
    End Sub

End Class