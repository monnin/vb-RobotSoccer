Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class playingField
    Enum playingMode
        server = 1
        viewer = 2
    End Enum

    Const PROTO_VERSION As String = "1.0"

    Const NO_TEAM As Integer = 0
    Const RED_TEAM As Integer = 1
    Const BLUE_TEAM As Integer = 2

    Private Const maxArray As Integer = 100
    Private WithEvents tcpNet As New SimpleTCPNetwork
    Private mymode As playingMode = playingMode.server

    '
    '   Structure to be used to hold a single player
    '   and the relevant graphics parts
    '
    Private Structure onePlayer
        Public obj As playerOrObject        ' The location and physics of the thing
        Public pic As PictureBox            ' The image of your robot (a square, actually)
        Public lbl As Label                 ' A label with your name on it
        Public team As Integer              ' Whose side are you on
        Public remoteId As Integer          ' Who created the player
        Public deletePending As Boolean           ' Invalid player that needs to be deleted?
    End Structure

    Private allPlayers(maxArray) As onePlayer
    Private maxPlayers As Integer = 0

    Private gameRunning As Boolean = False      ' Is the game running?
    Private redgoal As Integer = -1             ' Which thing is the red goal?
    Private bluegoal As Integer = -1            ' Which thing is the blue goal?
    Private ballid As Integer = -1              ' Which thing is the ball

    Private redPlayers As Integer = 0           ' Number of Red Players
    Private bluePlayers As Integer = 0          ' Number of Blue Players

    Private redScore As Integer = 0             ' Current score of red team
    Private blueScore As Integer = 0            ' Current score of blue team
    Private tickNum As Integer = 1
    Private secondsRemain As Integer = -1

    Private actionEnabled As Boolean = False

    Private scoreCounter As Integer = -1     ' Are we in the display score mode?
    Private invalidatorNeedsToRun As Boolean = False  ' Do we need to delete invalid players?

    '
    '   Select between the different modes we can opertate in
    '
    Property mode() As playingMode
        Get
            Return mymode
        End Get

        Set(ByVal value As playingMode)
            Select Case value
                Case playingMode.server
                    If (mymode <> playingMode.server) Then
                        mymode = value
                        Initialize_Server()
                    End If
                Case playingMode.viewer
                    mymode = value
                    Delete_All_Players()
                    actionTimer.Enabled = True  ' To handle the messages
            End Select
        End Set
    End Property



    Public Sub start_action()
        actionEnabled = True
    End Sub

    Public Sub stop_action()
        actionEnabled = False
    End Sub

    Public ReadOnly Property numPlayers() As Integer
        Get
            Dim i As Integer
            Dim c As Integer = 0
            For i = 0 To maxPlayers
                If (is_active(i)) Then
                    c = c + 1
                End If
            Next
            Return c
        End Get
    End Property

    Public ReadOnly Property theoreticalPlayers() As Integer
        Get
            Return maxPlayers
        End Get
    End Property

    Public Function is_active(ByVal piece As Integer) As Boolean
        Dim result As Boolean = False

        '
        '  Don't allow imaginary pieces
        '
        If ((piece >= 0) And (piece < maxArray)) Then
            If (Not (allPlayers(piece).obj Is Nothing)) Then
                result = True
            End If
        End If

        Return result
    End Function

    Private Sub Count_Players()
        Dim i As Integer

        redPlayers = 0
        bluePlayers = 0

        For i = 0 To maxPlayers
            '
            '  Find only real players
            '
            If (is_active(i) And allPlayers(i).obj.type = playerOrObject.playerType.player) Then
                Select Case allPlayers(i).team
                    Case RED_TEAM
                        redPlayers = redPlayers + 1

                    Case BLUE_TEAM
                        bluePlayers = bluePlayers + 1
                End Select
            End If
        Next i

    End Sub

    Public Sub move_one_piece(ByVal piece As Integer)
        Dim oldX As Integer
        Dim oldY As Integer
        '
        '  Don't move imaginary pieces
        '
        If is_active(piece) Then
            If (mymode = playingMode.server) Then
                oldX = allPlayers(piece).obj.X
                oldY = allPlayers(piece).obj.Y

                ' Move the piece
                allPlayers(piece).obj.selfMove()
            End If

            ' Update the screen
            allPlayers(piece).pic.Left = allPlayers(piece).obj.ScreenX
            allPlayers(piece).pic.Top = allPlayers(piece).obj.ScreenY

            If (Not (allPlayers(piece).lbl Is Nothing)) Then
                allPlayers(piece).lbl.Left = allPlayers(piece).obj.X
                allPlayers(piece).lbl.Top = allPlayers(piece).obj.ScreenY - _
                                              (allPlayers(piece).lbl.Height)
            End If

            If (mymode = playingMode.server) Then
                '
                '  Did the piece move?
                '
                If ((oldX <> allPlayers(piece).obj.X) Or (oldY <> allPlayers(piece).obj.Y)) Then
                    Send_Loc(piece, True)
                End If
            End If

        End If
    End Sub


    '
    '-------------------------------------------------------------------------------
    '

    Private Sub fix_field()
        playerOrObject.Set_Boundries(Me.DisplayRectangle.Width, Me.DisplayRectangle.Height)

        '
        '  Don't mess with the goals unless acting as the server
        '
        If (mymode = playingMode.server) Then

            If (redgoal >= 0) Then
                allPlayers(redgoal).obj.X = 0
                allPlayers(redgoal).obj.Y = Me.DisplayRectangle.Height / 2
                move_one_piece(redgoal)
            End If

            If (bluegoal >= 0) Then
                allPlayers(bluegoal).obj.X = Me.DisplayRectangle.Width
                allPlayers(bluegoal).obj.Y = Me.DisplayRectangle.Height / 2
                move_one_piece(bluegoal)
            End If


            SimpleTCPNetwork.SendMessageToAll("FIELD" & vbTab & _
                                              Me.DisplayRectangle.Width.ToString & _
                                              vbTab & Me.DisplayRectangle.Height.ToString)
        End If

    End Sub


    Private Sub playingField_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Initialize_Server()
    End Sub

    Private Sub Initialize_Server()
        Delete_All_Players()

        redgoal = create_player(-1, 0, 0, RED_TEAM, "", playerOrObject.playerType.goal)
        bluegoal = create_player(-1, 0, 0, BLUE_TEAM, "", playerOrObject.playerType.goal)

        fix_field()

        ballid = create_player(-1, 0, 0, NO_TEAM, "", playerOrObject.playerType.ball)

        Reset_Game()
    End Sub


    Private Sub Delete_All_Players()
        Dim i As Integer

        For i = 0 To maxPlayers
            If (is_active(i)) Then
                Delete_Player(i)
            End If
        Next i

        maxPlayers = 0

        '
        '  Reset a couple of things related to displays too
        '
        scoreCounter = -1
        scoreLogo.Visible = False
    End Sub


    Private Sub playingField_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        fix_field()
    End Sub

    '
    '-------------------------------------------------------------------------------
    '

    Public Function create_player(ByVal creator As Integer, _
                                  ByVal X As Integer, ByVal Y As Integer, ByVal myteam As Integer, _
                                  Optional ByVal Name As String = "", Optional ByVal ptype As playerOrObject.playerType = playerOrObject.playerType.player, _
                                  Optional ByVal desiredSize As Integer = -1) As Integer
        Dim i As Integer = 0
        Dim g As Graphics
        Dim sf As SizeF
        Dim j As Integer
        Dim bmap As Bitmap
        Dim mypen As Pen
        Dim jerseycolor As Color

        Select Case myteam
            Case NO_TEAM
                jerseycolor = Color.DarkOrange
            Case RED_TEAM
                jerseycolor = Color.Red
            Case BLUE_TEAM
                jerseycolor = Color.Blue
            Case Else
                jerseycolor = Color.Yellow
        End Select

        '
        '  Find the first empty slot
        '
        While ((i < maxArray) And (Not (allPlayers(i).obj Is Nothing)))
            i = i + 1
        End While

        '
        '   Add only if we can
        '
        If (i < maxArray) Then
            allPlayers(i).deletePending = False
            allPlayers(i).team = myteam
            allPlayers(i).obj = New playerOrObject(X, Y, ptype)

            '
            '  Allow for overriding of size
            '
            If (desiredSize > 0) Then
                allPlayers(i).obj.size() = desiredSize
            End If

            allPlayers(i).remoteId = creator


            allPlayers(i).pic = New PictureBox()
            allPlayers(i).pic.Size = New Size(allPlayers(i).obj.size(), allPlayers(i).obj.size())

            Select Case ptype
                Case playerOrObject.playerType.player

                    allPlayers(i).pic.BackColor = jerseycolor
                    allPlayers(i).pic.BorderStyle = BorderStyle.FixedSingle

                Case playerOrObject.playerType.goal

                    mypen = New Pen(jerseycolor, 3)

                    allPlayers(i).pic.BackColor = Color.Transparent
                    bmap = New Bitmap(allPlayers(i).obj.size(), allPlayers(i).obj.size())
                    g = Graphics.FromImage(bmap)

                    g.FillEllipse(Brushes.Black, 1, 1, allPlayers(i).obj.size() - 1, allPlayers(i).obj.size() - 1)
                    g.DrawEllipse(mypen, 1, 1, allPlayers(i).obj.size() - 1, allPlayers(i).obj.size() - 1)

                    allPlayers(i).pic.Image = bmap
                    ' allPlayers(i).pic.deletePendingate()

                Case playerOrObject.playerType.ball

                    mypen = New Pen(jerseycolor, 2)

                    allPlayers(i).pic.BackColor = Color.Transparent
                    bmap = New Bitmap(allPlayers(i).obj.size(), allPlayers(i).obj.size())
                    g = Graphics.FromImage(bmap)

                    g.FillEllipse(Brushes.White, 0, 0, allPlayers(i).obj.size() - 1, allPlayers(i).obj.size() - 1)
                    g.DrawEllipse(mypen, 0, 0, allPlayers(i).obj.size() - 1, allPlayers(i).obj.size() - 1)

                    allPlayers(i).pic.Image = bmap
                    ' allPlayers(i).pic.deletePendingate()

            End Select

            allPlayers(i).pic.Parent = Me

            If ((Name = "") And (ptype = playerOrObject.playerType.player)) Then
                Name = "Robot #%d"
            End If

            If (Name <> "") Then
                '
                '   Allow for player numbers to be imbedded into name
                '
                Name = Name.Replace("%d", i.ToString)
                allPlayers(i).lbl = New Label()

                allPlayers(i).lbl.Text = Name
                g = allPlayers(i).lbl.CreateGraphics()
                sf = g.MeasureString(Name, allPlayers(i).lbl.Font)

                allPlayers(i).lbl.Width = sf.Width + 12

                allPlayers(i).lbl.BackColor = Color.Transparent
                allPlayers(i).lbl.ForeColor = jerseycolor

                allPlayers(i).lbl.Parent = Me
            Else
                allPlayers(i).lbl = Nothing
            End If

            '
            ' Choose a team
            '
            Select Case jerseycolor
                Case Color.Red : allPlayers(i).team = 1
                Case Color.Blue : allPlayers(i).team = 2
                Case Else : allPlayers(i).team = 3
            End Select


            If (mymode = playingMode.server) Then
                Send_Add(i, True)
            End If
            '
            '   Put the piece on the correct location on the board
            '
            move_one_piece(i)

        Else
            MsgBox("Max players has been reached")
            i = -1
        End If

        If (i > maxPlayers) Then maxPlayers = i

        '
        '   Send all labels to the back of the field
        '
        For j = 0 To maxPlayers
            If (Not (allPlayers(j).lbl Is Nothing)) Then
                allPlayers(j).lbl.SendToBack()
            End If
        Next

        If (mymode = playingMode.server) Then
            Count_Players()

            If (Not gameRunning) Then
                Reset_Game()            ' Move everyone around until the game begins
            End If
        End If


        redScoreBox.SendToBack()
        redScoreBox.SendToBack()
        redLabel.SendToBack()
        blueLabel.SendToBack()



        Return i
    End Function

    Public Sub Move_Piece(ByVal piece As Integer, ByVal xspeed As Double, ByVal yspeed As Double)
        If (is_active(piece)) Then
            allPlayers(piece).obj.Set_Speed(xspeed, yspeed)
        End If
    End Sub


    Public Sub Change_Speed(ByVal piece As Integer, ByVal deltaxspeed As Double, ByVal deltayspeed As Double)
        If (is_active(piece)) Then
            allPlayers(piece).obj.Change_Speed(deltaxspeed, deltayspeed)
        End If
    End Sub

    Public Sub Multiply_Speed(ByVal piece As Integer, ByVal xfactor As Double, ByVal yfactor As Double)
        If (is_active(piece)) Then
            allPlayers(piece).obj.Multiply_Speed(xfactor, yfactor)
        End If
    End Sub

    Private Sub Check_All_Collisions()
        Dim i As Integer
        Dim j As Integer
        Dim collision As Boolean
        '
        '   Looking at one thing at a time
        '
        For i = 0 To maxArray
            '
            '  Ignore imaginary players
            '
            If (Not (allPlayers(i).obj Is Nothing)) Then
                For j = i + 1 To maxArray
                    '
                    '  Ignore imaginary players also
                    '
                    If (Not (allPlayers(j).obj Is Nothing)) Then
                        '
                        '  Two real players (and also not yet checked)
                        '   -- check them
                        '
                        collision = allPlayers(i).obj.Check_Collision(allPlayers(j).obj)

                        '
                        '   If there was a collision, see if it was with a ball and a goal
                        '
                        If (collision) Then
                            '
                            '   Was one a ball?
                            '
                            If ((allPlayers(i).obj.type = playerOrObject.playerType.ball) Or _
                                (allPlayers(j).obj.type = playerOrObject.playerType.ball)) Then

                                '
                                '  Was one a red goal (which is by definition not a ball)
                                '
                                If ((i = redgoal) Or (j = redgoal)) Then
                                    blueScore = blueScore + 1
                                    Score()
                                End If

                                '
                                '  Was one a blue goal (which is by definition not a ball)
                                '
                                If ((i = bluegoal) Or (j = bluegoal)) Then
                                    redScore = redScore + 1
                                    Score()
                                End If

                            End If
                        End If
                    End If
                Next
            End If

        Next
    End Sub

    Public Sub Delete_Player(ByVal player As Integer)
        Dim i As Integer

        allPlayers(i).deletePending = False   ' Invalid = pending delete.  Deleted is valid (oddly enough)

        If (Not (allPlayers(player).lbl Is Nothing)) Then allPlayers(player).lbl.Parent = Nothing
        allPlayers(player).lbl = Nothing

        If (Not (allPlayers(player).pic Is Nothing)) Then allPlayers(player).pic.Parent = Nothing
        allPlayers(player).pic = Nothing

        '
        '   See if we have to update the maxPlayers
        '
        If (Not (allPlayers(player).obj Is Nothing)) Then
            allPlayers(player).obj = Nothing

            '
            '   We we the last (or should have been the last)
            '
            If (maxPlayers <= player) Then
                '
                '  Find a new last player
                '
                maxPlayers = 0

                For i = 0 To player
                    If (Not (allPlayers(i).obj Is Nothing)) Then maxPlayers = i
                Next

            End If
        End If

        If (mymode = playingMode.server) Then
            SimpleTCPNetwork.SendMessageToAll("DEL" & vbTab & player.ToString)
            Count_Players()
        End If

    End Sub
    Private Sub Score()
        scoreCounter = 100

        redScoreBox.Text = redScore.ToString
        blueScoreBox.Text = blueScore.ToString

        send_scores(True)

        scoreLogo.Visible = True

        SimpleTCPNetwork.SendMessageToAll("DISPLAY" & vbTab & scoreCounter.ToString & vbTab & _
                                          """" & scoreLogo.Text & """")

    End Sub

    Private Sub Handle_Tick()
        Dim i As Integer

        SimpleTCPNetwork.SendMessageToAll("TICK" & vbTab & tickNum & vbTab & secondsRemain)
        tickNum = tickNum + 1
        gameRunning = True

        For i = 0 To maxArray
            move_one_piece(i)
        Next

        Check_All_Collisions()

    End Sub
    Private Sub actionTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles actionTimer.Tick
        Dim offset As Integer
        Dim x As Integer
        Dim i As Integer

        If (scoreCounter = 0) Then
            If (mymode = playingMode.server) Then
                Reset_Game()
            End If

            scoreCounter = -1
            scoreLogo.Visible = False
        ElseIf (scoreCounter > 0) Then
            offset = scoreCounter / 5
            offset = offset Mod 2
            x = Me.DisplayRectangle.Width / 2 - scoreLogo.Width / 2
            x = x - 20 + (40 * offset)
            scoreLogo.Left = x
            scoreCounter = scoreCounter - 1
        ElseIf (actionEnabled) Then

            If (mymode = playingMode.server) Then
                Handle_Tick()
            End If

        End If

        If (invalidatorNeedsToRun) Then
            invalidatorNeedsToRun = False
            For i = 0 To maxPlayers
                If (is_active(i)) Then
                    If (allPlayers(i).obj IsNot Nothing) Then
                        If (allPlayers(i).deletePending) Then
                            Delete_Player(i)
                        End If
                    End If
                End If
            Next
        End If
    End Sub

    Public Sub step_action()
        If (mymode = playingMode.server) Then
            Handle_Tick()
        End If
    End Sub
    '
    '-------------------------------------------------------------------------------
    '
    Public Sub Reset_Game(Optional ByVal resetScore As Boolean = False)
        Dim i As Integer
        Dim x As Integer
        Dim y As Integer
        Dim redLine As Integer
        Dim blueLine As Integer
        Dim redInc As Integer = 1
        Dim blueInc As Integer = 1

        redLine = Me.DisplayRectangle.Width * 0.3
        blueLine = Me.DisplayRectangle.Width * 0.7

        If (resetScore) Then
            redScore = 0
            blueScore = 0
            tickNum = 1
            gameRunning = False
        End If

        redScoreBox.Text = redScore.ToString
        blueScoreBox.Text = blueScore.ToString

        x = Me.DisplayRectangle.Width / 2           ' Center court
        y = Me.DisplayRectangle.Height / 2          ' Center court

        For i = 0 To maxPlayers
            If (Not (allPlayers(i).obj Is Nothing)) Then
                '
                '  Everything is at rest at the beginning of the game
                '
                allPlayers(i).obj.Set_Speed(0, 0)

                If (allPlayers(i).obj.type = playerOrObject.playerType.ball) Then
                    '
                    '  Put the ball(s) "almost" in the center
                    '
                    allPlayers(i).obj.X = x - 5 + (10 * Rnd())
                    allPlayers(i).obj.Y = y - 5 + (10 * Rnd())

                    '
                    '  Set the ball at rest
                    '
                ElseIf (allPlayers(i).obj.type = playerOrObject.playerType.player) Then
                    If (allPlayers(i).team = RED_TEAM) Then
                        allPlayers(i).obj.X = redLine
                        allPlayers(i).obj.Y = redInc * (Me.DisplayRectangle.Height / (redPlayers + 1))
                        redInc = redInc + 1
                    End If

                    If (allPlayers(i).team = BLUE_TEAM) Then
                        allPlayers(i).obj.X = blueLine
                        allPlayers(i).obj.Y = blueInc * (Me.DisplayRectangle.Height / (bluePlayers + 1))
                        blueInc = blueInc + 1
                    End If
                End If

                move_one_piece(i)
            End If
        Next

    End Sub


    Private Sub Send_Add(ByVal who As Integer, _
                             Optional ByVal toall As Boolean = True, _
                             Optional ByVal conn As Integer = 0)


        Dim str As String

        '
        '   ADD <id> <type> <team#> <xloc> <yloc> <size> "<name>"
        '
        '       type = ball|wall|goal|player|other
        '

        str = "ADD" & vbTab & who.ToString & vbTab

        Select Case allPlayers(who).obj.type
            Case playerOrObject.playerType.ball
                str = str & "ball"
            Case playerOrObject.playerType.goal
                str = str & "goal"
            Case playerOrObject.playerType.player
                str = str & "player"
            Case playerOrObject.playerType.wall
                str = str & "wall"
            Case Else
                str = str & "other"
        End Select

        '
        '
        '  Add team next
        str = str & vbTab & allPlayers(who).team.ToString & vbTab

        '
        '  Add the current location
        '
        str = str & allPlayers(who).obj.X & vbTab & allPlayers(who).obj.Y & vbTab

        '
        '  Add the current size
        '
        str = str & allPlayers(who).obj.size & vbTab

        '
        '   Add the name of the object (only if it exists)
        '
        If (allPlayers(who).lbl IsNot Nothing) Then
            str = str & """" & allPlayers(who).lbl.Text & """"
        Else
            str = str & """"""
        End If


        'Public obj As playerOrObject
        'Public pic As PictureBox
        'Public lbl As Label
        'Public team As Integer

        If (toall) Then
            SimpleTCPNetwork.SendMessageToAll(str)
        Else
            SimpleTCPNetwork.SendMessageTo(str, conn)
        End If
    End Sub

    Private Sub Send_Loc(ByVal who As Integer, _
                         Optional ByVal toall As Boolean = True, _
                         Optional ByVal conn As Integer = 0)


        Dim str As String

        '
        '   LOC <id> <xloc> <yloc>
        '

        '

        str = "LOC" & vbTab & who.ToString & vbTab

        '
        '  Add the current location
        '
        str = str & allPlayers(who).obj.X & vbTab & allPlayers(who).obj.Y

        If (toall) Then
            SimpleTCPNetwork.SendMessageToAll(str)
        Else
            SimpleTCPNetwork.SendMessageTo(str, conn)
        End If
    End Sub

    Private Sub send_scores(Optional ByVal toall As Boolean = True, _
                            Optional ByVal conn As Integer = 0)

        If (toall) Then
            SimpleTCPNetwork.SendMessageToAll("SCORE" & vbTab & "red" & vbTab & redScoreBox.Text)
            SimpleTCPNetwork.SendMessageToAll("SCORE" & vbTab & "blue" & vbTab & blueScoreBox.Text)
        Else
            SimpleTCPNetwork.SendMessageTo("SCORE" & vbTab & "red" & vbTab & redScoreBox.Text, conn)
            SimpleTCPNetwork.SendMessageTo("SCORE" & vbTab & "blue" & vbTab & blueScoreBox.Text, conn)
        End If

    End Sub

    Delegate Sub delete_all_from_connection_delegate(ByVal conn As Integer)
    Private Sub delete_all_from_connection(ByVal conn As Integer)
        Dim i As Integer

        If (Me.InvokeRequired) Then
            Me.Invoke(New delete_all_from_connection_delegate(AddressOf delete_all_from_connection), _
                      New Object() {conn})
        Else
            '
            '   Remove all players related to this connection
            '
            For i = 0 To maxPlayers
                If (is_active(i)) Then
                    If (allPlayers(i).remoteId = conn) Then
                        allPlayers(i).deletePending = True
                    End If
                End If
            Next i

            invalidatorNeedsToRun = True
        End If

    End Sub
    Private Sub tcpNet_closedTCPConnection(ByVal conn As Integer) Handles tcpNet.closedTCPConnection

        delete_all_from_connection(conn)

        controlForm.UpdateStatus()
    End Sub



    Private Sub tcpNet_newTCPConnection(ByVal conn As Integer) Handles tcpNet.newTCPConnection
        Dim i As Integer

        If (mymode = playingMode.server) Then
            '
            '   Tell the new client hello
            '
            SimpleTCPNetwork.SendMessageTo("HELLO" & vbTab & "RobotSoccer" & vbTab & PROTO_VERSION, conn)
            SimpleTCPNetwork.SendMessageTo("FIELD" & vbTab & Me.DisplayRectangle.Width.ToString & _
                                      vbTab & Me.DisplayRectangle.Height.ToString, conn)
            send_scores(False, conn)
            '
            '  Tell the new client about everything
            '
            For i = 0 To maxPlayers
                If is_active(i) Then
                    Send_Add(i, False, conn)
                End If
            Next
        End If


        controlForm.UpdateStatus()
    End Sub

    Private Sub tcpNet_newTCPMessage(ByVal id As Integer) Handles tcpNet.newTCPMessage
        Dim str As String

        str = SimpleTCPNetwork.GetMessage(id)

        decode_message(id, str)
    End Sub

    Delegate Sub decode_message_delegate(ByVal conn As Integer, ByVal str As String)

    Private Sub decode_message(ByVal conn As Integer, ByVal str As String)
        Dim splitChars() As Char = {vbTab}


        If (Me.InvokeRequired()) Then
            Try
                Me.Invoke(New decode_message_delegate(AddressOf decode_message), _
                          New Object() {conn, str})
            Catch ex As Exception

            End Try
        Else
            If (mymode = playingMode.server) Then
                decode_from_client(conn, str.Split(splitChars))
            Else
                decode_from_server(conn, str.Split(splitChars))
            End If
        End If
    End Sub

    Private Sub decode_from_server(ByVal conn As Integer, ByRef str() As String)
        Select Case str(0)
            Case "HELLO"
                Handle_Hello(str)
            Case "FIELD"
                Handle_Field(str)
            Case "ADD"
                Handle_Add(conn, str)
            Case "LOC"
                Handle_Loc(str)
            Case "DISPLAY"
                Handle_Display(str)
            Case "SCORE"
                Handle_Score(str)
            Case "DEL"
                Handle_Del(str)
            Case Else
                MsgBox("Unknown command " & str(0) & " received")
        End Select
    End Sub

    Private Sub Handle_Hello(ByRef str() As String)
        If (str(1) <> "RobotSoccer") Then
            MsgBox("Failed - Server (" & str(1) & _
                   ")is not running RobotSoccer")

            SimpleTCPNetwork.CloseConnection()

        Else
            If (str(2) <> PROTO_VERSION) Then
                MsgBox("Failed - Server (" & str(2) & _
                       ")is running a different version than me (" & _
                        PROTO_VERSION & ")")

                SimpleTCPNetwork.CloseConnection()
            End If
        End If
    End Sub

    Private Sub Handle_Display(ByRef str() As String)
        ' DISPLAY <how> "Message"
        Dim ok = True
        Dim how As Integer
        Dim msg As String = "Score!"
        Dim trimChars() As Char = {"""", " "}

        If (str.Length = 3) Then
            ok = Integer.TryParse(str(1), how)
            msg = str(2)
            msg.Trim(trimChars)
        Else
            ok = False
        End If

        If (ok) Then
            scoreLogo.Text = msg.Trim(trimChars)

            If (how > 0) Then
                scoreLogo.Visible = True
            Else
                scoreLogo.Visible = False
            End If

            scoreCounter = how

        Else
            MsgBox("Invalid DISPLAY command")
        End If
    End Sub

    Private Sub Handle_Score(ByRef str() As String)
        ' SCORE <red|blue> <num>
        Dim ok = True
        Dim num As Integer

        If (str.Length() = 3) Then
            ok = Integer.TryParse(str(2), num)
        Else
            ok = False
        End If

        If (ok) Then
            Select Case str(1)
                Case "red"
                    redScoreBox.Text = num.ToString
                Case "blue"
                    blueScoreBox.Text = num.ToString
            End Select
        Else
            MsgBox("Invalid SCORE command received")
        End If
    End Sub

    Private Sub Handle_Field(ByRef str() As String)
        Dim X As Integer
        Dim Y As Integer

        If (Integer.TryParse(str(1), X) And Integer.TryParse(str(2), Y)) Then
            X = X + Me.Size.Width - Me.DisplayRectangle.Width
            Y = Y + Me.Size.Height - Me.DisplayRectangle.Height

            Me.Size = New Size(X, Y)
            fix_field()
        Else
            MsgBox("Invalid FIELD command")
        End If

    End Sub

    Private Sub Handle_Add(ByVal conn As Integer, ByRef str() As String)
        '   ADD <id> <type> <team#> <xloc> <yloc> <size> "<name>"
        Dim ok = True
        Dim id As Integer
        Dim ptype As playerOrObject.playerType = playerOrObject.playerType.player
        Dim team As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Size As Integer
        Dim name As String = String.Empty
        Dim trimChars() As Char = {"""", " "}

        If (str.Length = 8) Then
            ok = Integer.TryParse(str(1), id)

            ok = ok And Integer.TryParse(str(3), team)

            Select Case str(2)
                Case "ball"
                    ptype = playerOrObject.playerType.ball
                    team = NO_TEAM      ' Force no team
                Case "wall"
                    ptype = playerOrObject.playerType.wall
                    team = NO_TEAM      ' Force no team
                Case "goal"
                    ptype = playerOrObject.playerType.goal
            End Select

            ok = ok And Integer.TryParse(str(4), X)
            ok = ok And Integer.TryParse(str(5), Y)
            ok = ok And Integer.TryParse(str(6), Size)
            name = str(7)
            name = name.Trim(trimChars)
        Else
            ok = False
        End If

        If (ok) Then
            create_player(conn, X, Y, team, name, ptype)
        Else
            MsgBox("Invalid ADD command received")
        End If
    End Sub

    Private Sub Handle_Loc(ByRef str() As String)
        '   LOC <id> <xloc> <yloc>
        Dim ok = True
        Dim id As Integer
        Dim X As Integer
        Dim Y As Integer

        If (str.Length = 4) Then
            ok = Integer.TryParse(str(1), id)
            ok = ok And Integer.TryParse(str(2), X)
            ok = ok And Integer.TryParse(str(3), Y)
        Else
            ok = False
        End If

        If (ok) Then
            If (is_active(id)) Then
                allPlayers(id).obj.X = X
                allPlayers(id).obj.Y = Y
                move_one_piece(id)
            End If
        Else
            MsgBox("Invalid LOC command received")
        End If
    End Sub

    Private Sub Handle_Del(ByRef str() As String)
        '   DEL <id>
        Dim id As Integer

        If (Integer.TryParse(str(1), id)) Then
            If (is_active(id)) Then
                Delete_Player(id)
            End If
        End If
    End Sub

    Private Sub decode_from_client(ByVal conn As Integer, ByRef str() As String)
        Select Case str(0)
            Case "CREATE"
                Handle_Create(conn, str)
            Case "SPEED"
            Case "REMOVE"
                Handle_Remove(conn, str)
            Case Else
                MsgBox("Unknown command " & str(0) & " received")
        End Select
    End Sub

    Private Sub Handle_Create(ByVal conn As Integer, ByRef str() As String)
        ' CREATE <team> "<name>"
        '   team = red|blue|any
        Dim team As Integer = RED_TEAM
        Dim name As String = String.Empty
        Dim X As Integer
        Dim Y As Integer
        Dim newid As Integer
        Dim trimChars() As Char = {"""", " "}

        If (redPlayers > bluePlayers) Then team = BLUE_TEAM

        If (str.Length = 3) Then name = str(2).Trim(trimChars)

        If (str.Length >= 2) Then
            Select Case str(1)
                Case "red"
                    team = RED_TEAM
                Case "blue"
                    team = BLUE_TEAM
            End Select
        End If

        Y = Rnd() * Me.DisplayRectangle.Height
        X = Rnd() * Me.DisplayRectangle.Width

        '
        '  Place the player away from the ball at first
        '   (realistically rule only comes into play if game has already started,
        '    otherwise Reset_Game will put the person in the correct spot)
        '
        If (ballid >= 0) Then
            '
            '  Which side of the court is the ball on?
            '
            If (allPlayers(ballid).obj.X > Me.DisplayRectangle.Width / 2) Then
                X = Me.DisplayRectangle.Width * 0.2
            Else
                X = Me.DisplayRectangle.Width * 0.8
            End If
        End If

        newid = create_player(conn, X, Y, team, name, playerOrObject.playerType.player)
        SimpleTCPNetwork.SendMessageTo("YOU" & vbTab & newid.ToString, conn)
    End Sub


    Private Sub Handle_Remove(ByVal conn As Integer, ByRef str() As String)
        Dim who As Integer
        '
        '  Right number of arguments?
        '
        If (str.Length = 2) Then
            '
            ' Valid integer?   Owned by me?
            '
            If (Integer.TryParse(str(1), who)) Then
                If (is_active(who) And (allPlayers(who).remoteId = conn)) Then
                    Delete_Player(who)
                End If
            End If
        End If

    End Sub
End Class
