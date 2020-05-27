Option Explicit On
Option Strict On


Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class playingField
    Enum playingMode
        server = 1
        viewer = 2
    End Enum

    Const PROTO_VERSION As String = "1.0"

    Public Enum teamSide As Integer
        NO_TEAM = 0
        RED_TEAM = 1
        BLUE_TEAM = 2
    End Enum

    Private maxArray As Integer = 100
    Public WithEvents tcpNet As New SimpleTCPNetwork
    Private mymode As playingMode = playingMode.server

    Private TimerRunning As Boolean = False             ' Allow only one timer at a time

    Private ClockLogo As New Bitmap(My.Resources.Clock_Logo)  ' Use a bitmap so we can set the transparency
    Private robotLabelFont As Font = SystemFonts.SmallCaptionFont
    Private robotLabelFontHeight As Integer = robotLabelFont.Height

    Const khakiPenWidth As Integer = 4
    Private khakiPen As New Pen(Color.DarkKhaki, khakiPenWidth)

    Private redLabelBrush As SolidBrush = New SolidBrush(Color.Pink)
    Private blueLabelBrush As SolidBrush = New SolidBrush(Color.LightBlue)

    Private ballBitmap As Bitmap = My.Resources.SoccerBall.ToBitmap
    Private backgroundBitmap As Bitmap = Nothing

    Private baseGraphic As Graphics = Me.CreateGraphics

    Private xmin As Integer = -1            ' These four variables define what area
    Private xmax As Integer = -1            '  needs repainted when objects move
    Private ymin As Integer = -1
    Private ymax As Integer = -1

    Private tickCounts As Integer = 0
    Public numTicksPerSecond As Decimal = 0.0

    Public numTicks(9) As Integer
    Public tickSlot As Integer = 0

    Public numPlayersTotal As Integer = 0


    '
    '   Structure to be used to hold a single player
    '   and the relevant graphics parts
    '
    Private Structure onePlayer
        Public obj As playerOrObject        ' The location and physics of the thing
        'Public pic As PictureBox            ' The image of your robot (a square, actually)
        Public name As String                 ' A label with your name on it
        Public numStr As String
        Public team As teamSide             ' Whose side are you on

        Public baseBrush As SolidBrush ' If the item is a single color, what is it?
        Public commonPen As Pen             ' What Pen should I use to draw the item?

        'Public displayWidth As Integer
        'Public displayHeight As Integer

        Public topOffset As Integer         ' Given the x & y location of this object, what is the
        Public bottomOffset As Integer      ' Bounding Box that the object is displayed in
        Public leftOffset As Integer        ' (for round objects it will just be +/- the radius)
        Public rightOffset As Integer       ' left and top are often negative

        Public jerseyFont As Font           ' What font to draw the INTERNAL jersey number
        Public jerseyFontHeight As Integer  ' Height of the jerseyFont

        Public labelWidth As Integer        ' Width of the label that accompanies the robots
        Public labelHeight As Integer       ' Height of the label that accompanies the robots

        Public jerseyOffsetX As Integer        ' Offset in x direction for jersey # (in pixels)
        Public remoteId As Integer          ' Who created the player
        Public animX As Integer
        Public animY As Integer
        Public origXstring As String       ' How to place someone during a reset or resize
        Public origYstring As String       ' How to place someone during a reset or resize
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

    Private tenPtFont As New Font("Microsoft Sans Serif", 10, FontStyle.Bold)
    Private fourteenPtFont As New Font("Microsoft Sans Serif", 14, FontStyle.Bold)

    '
    '   Select between the different modes we can operate in
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
                        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
                        Me.Text = "Robot Soccer Playing Field - Server"
                    End If
                Case playingMode.viewer
                    mymode = value
                    Delete_All_Players()
                    ' actionTimer.Enabled = True  ' To handle the messages  (it's always on)

                    Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Fixed3D
                    Me.Text = "Robot Soccer Playing Field - Viewer"
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

        Dim oldRed As Integer
        Dim oldBlue As Integer

        oldRed = redPlayers
        oldBlue = bluePlayers

        redPlayers = 0
        bluePlayers = 0

        For i = 0 To maxPlayers
            '
            '  Find only real players
            '
            If (is_active(i) And allPlayers(i).obj IsNot Nothing) Then
                If (allPlayers(i).obj.type = playerOrObject.playerType.player) Then
                    Select Case allPlayers(i).team
                        Case teamSide.RED_TEAM
                            redPlayers = redPlayers + 1

                        Case teamSide.BLUE_TEAM
                            bluePlayers = bluePlayers + 1
                    End Select
                End If
            End If
        Next i

        If ((oldRed <> redPlayers) Or (oldBlue <> bluePlayers)) Then

        End If

    End Sub


    '
    '   Simple routine to count the number of players on the 
    '   playing field and return that count
    '
    Public Sub Get_Player_Count(ByRef red As Integer, ByRef blue As Integer)
        Count_Players()

        red = redPlayers
        blue = bluePlayers
    End Sub


    Private Function is_ball_and_goal(ByRef one As onePlayer, ByRef two As onePlayer) As Boolean
        Dim result As Boolean = False

        If ((one.obj.type = playerOrObject.playerType.ball) And (two.obj.type = playerOrObject.playerType.goal)) Then
            If (two.team = teamSide.RED_TEAM) Then
                blueScore = blueScore + 1
            ElseIf (two.team = teamSide.BLUE_TEAM) Then
                redScore = redScore + 1
            End If

            result = True
        End If

        If ((two.obj.type = playerOrObject.playerType.ball) And (one.obj.type = playerOrObject.playerType.goal)) Then
            If (one.team = teamSide.RED_TEAM) Then
                blueScore = blueScore + 1
            ElseIf (one.team = teamSide.BLUE_TEAM) Then
                redScore = redScore + 1
            End If

            result = True
        End If

        Return result
    End Function

    Public Sub Set_Real_Loc(ByVal who As Integer, ByVal x As Double, ByVal y As Double)
        If (is_active(who)) Then
            allPlayers(who).obj.Set_Speed(0, 0)
            allPlayers(who).obj.X = x
            allPlayers(who).obj.Y = y

            move_one_piece(who, False, True)
        End If
    End Sub

    '
    '
    '       Set the location (allow % for percentages of the screen)
    '

    Public Sub Set_Loc(ByVal who As Integer, ByVal x As String, ByVal y As String)
        Dim xloc As Double
        Dim yloc As Double
        Dim xmax As Integer
        Dim ymax As Integer
        Dim ok As Boolean = True

        '
        '  Only change "real" objects
        '
        If (is_active(who)) Then

            playerOrObject.Get_Boundries(xmax, ymax)


            If (x.EndsWith("%"c)) Then
                x = x.TrimEnd("%"c)
                ok = ok And Double.TryParse(x, xloc)

                If (ok) Then
                    xloc = xloc * xmax / 100.0
                End If
            Else
                ok = ok And Double.TryParse(x, xloc)
            End If

            If (y.EndsWith("%")) Then
                y = y.TrimEnd("%"c)
                ok = ok And Double.TryParse(y, yloc)

                If (ok) Then
                    yloc = yloc * ymax / 100.0
                End If
            Else
                ok = ok And Double.TryParse(y, yloc)
            End If

            '
            '  Valid place to set
            '
            If (ok) Then
                allPlayers(who).origXstring = x
                allPlayers(who).origYstring = y

                Set_Real_Loc(who, xloc, yloc)
            End If
        End If

    End Sub


    Public Sub Set_All_Loc(ByVal x As String, ByVal y As String, _
                           ByVal type As playerOrObject.playerType, ByVal team As teamSide)
        Dim i As Integer

        For i = 0 To maxPlayers
            If (is_active(i)) Then
                If ((allPlayers(i).obj.type = type) And (allPlayers(i).team = team)) Then
                    Set_Loc(i, x, y)
                End If
            End If

        Next i
    End Sub

    Private Function IntMax(ByVal a As Integer, ByVal b As Integer) As Integer
        If (a < b) Then
            a = b
        End If

        Return a
    End Function


    Private Function IntMin(ByVal a As Integer, ByVal b As Integer) As Integer
        If (a > b) Then
            a = b
        End If

        Return a
    End Function

    Private Function IntMax(ByVal a As Double, ByVal b As Double) As Integer
        If (a < b) Then
            a = b
        End If

        Return CInt(a)
    End Function

    Private Function IntMin(ByVal a As Double, ByVal b As Double) As Integer
        If (a > b) Then
            a = b
        End If

        Return CInt(a)
    End Function


    Public Sub move_one_piece_on_screen(ByVal piece As Integer,
                                             ByVal force_move As Boolean,
                                             ByVal oldx As Double,
                                             ByVal oldy As Double)
        Dim mymaxx, mymaxy, myminx, myminy As Integer

        '
        '  Did the piece move?
        '
        If (force_move Or _
            (oldx <> allPlayers(piece).obj.X) Or (oldy <> allPlayers(piece).obj.Y)) Then

            '
            '  Expand the clipping region - by adding a +/- 1 for "rounding errors" 
            '

            mymaxx = IntMax(oldx, allPlayers(piece).obj.X) + allPlayers(piece).rightOffset + 1
            mymaxy = IntMax(oldy, allPlayers(piece).obj.Y) + allPlayers(piece).bottomOffset + 1
            myminx = IntMin(oldx, allPlayers(piece).obj.X) + allPlayers(piece).leftOffset - 1
            myminy = IntMin(oldy, allPlayers(piece).obj.Y) + allPlayers(piece).topOffset - 1
            '
            update_repaint_rect(myminx, mymaxx, myminy, mymaxy)

            If (mymode = playingMode.server) Then
                Send_Loc(piece, True)
            End If ' If (mymode = playingMode.server)

        End If ' If ((oldX <> allPlayers...
    End Sub

    Public Function move_one_piece(ByVal piece As Integer, _
                                   ByVal force_move As Boolean, _
                                   Optional ByVal no_collision As Boolean = False) As Boolean

        Dim oldX As Double
        Dim oldY As Double
        Dim j As Integer = 0
        Dim other As Integer = -1
        Dim pieceScored As Boolean = False
        Dim distMoved As Double


        '
        '  Don't move imaginary pieces
        '
        If is_active(piece) Then
            If (mymode = playingMode.server) Then

                oldX = allPlayers(piece).obj.X
                oldY = allPlayers(piece).obj.Y

                ' Move the piece
                If (no_collision) Then
                    other = -1
                Else

                    While ((j <= maxPlayers) And (other = -1))
                        ' You can't collide with something that is not there
                        If (allPlayers(piece).obj IsNot Nothing) Then
                            ' You can't collide with yourself
                            If (j <> piece) Then
                                If (allPlayers(piece).obj.Will_Collide_With(allPlayers(j).obj)) Then other = j
                            End If
                        End If

                        j = j + 1
                    End While
                End If

                If (other <> -1) Then
                    ' Collision!!!
                    '
                    '   See if the ball went into the goal
                    '
                    If (is_ball_and_goal(allPlayers(piece), allPlayers(other))) Then
                        pieceScored = True
                    Else
                        allPlayers(piece).obj.Handle_Collision(allPlayers(other).obj)
                    End If ' If (is_ball_and_goal...)
                End If ' If (other <> -1)

                allPlayers(piece).obj.selfMove()

                '
                '   Did the piece move?
                '
                distMoved = allPlayers(piece).obj.X - oldX
                distMoved = Math.Min(distMoved, 1)
                distMoved = Math.Max(distMoved, -1)

                allPlayers(piece).animX = allPlayers(piece).animX + CInt(distMoved)

                distMoved = allPlayers(piece).obj.Y - oldY
                distMoved = Math.Min(distMoved, 1)
                distMoved = Math.Max(distMoved, -1)

                allPlayers(piece).animY = allPlayers(piece).animY + CInt(distMoved)

                If (allPlayers(piece).animX < 0) Then
                    allPlayers(piece).animX = allPlayers(piece).animX + 32
                End If

                If (allPlayers(piece).animY < 0) Then
                    allPlayers(piece).animY = allPlayers(piece).animY + 32
                End If
            End If

            '
            '  Move the onscreen version of the player (if necessary)?
            '

            move_one_piece_on_screen(piece, force_move, oldX, oldY)
        End If

        Return pieceScored
    End Function


    '
    '-------------------------------------------------------------------------------




    Private Function has_valid_string_locs(ByVal who As Integer) As Boolean
        Dim xok As Boolean = False
        Dim yok As Boolean = False

        If (is_active(who)) Then

            If (allPlayers(who).origXstring IsNot Nothing) Then
                If (allPlayers(who).origXstring <> "") Then
                    xok = True
                End If
            End If

            If (allPlayers(who).origYstring IsNot Nothing) Then
                If (allPlayers(who).origYstring <> "") Then
                    yok = True
                End If
            End If
        End If


        Return (xok And yok)
    End Function


    '
    '   fix_field
    '
    '       Called whenever the field size changes 
    '       (or when the field is first created)
    '       (or when the field is minimized or un-minimized)
    '
    '   Draws the background image and then puts the goals in the proper place
    '


    Private Sub fix_field()

        playerOrObject.Set_Boundries(Me.DisplayRectangle.Width, Me.DisplayRectangle.Height)

        ' draw_field()

        '
        '  Don't mess with the goals unless acting as the server
        '
        If (mymode = playingMode.server) Then

            If (redgoal >= 0) Then
                allPlayers(redgoal).obj.Set_Speed(0, 0)

                If (has_valid_string_locs(redgoal)) Then
                    Set_Loc(redgoal, allPlayers(redgoal).origXstring, allPlayers(redgoal).origYstring)
                Else
                    allPlayers(redgoal).obj.X = 0
                    allPlayers(redgoal).obj.Y = Me.DisplayRectangle.Height / 2
                End If

                move_one_piece(redgoal, True)
            End If

            If (bluegoal >= 0) Then
                allPlayers(bluegoal).obj.Set_Speed(0, 0)

                If (has_valid_string_locs(bluegoal)) Then
                    Set_Loc(bluegoal, allPlayers(bluegoal).origXstring, allPlayers(bluegoal).origYstring)
                Else
                    allPlayers(bluegoal).obj.X = Me.DisplayRectangle.Width
                    allPlayers(bluegoal).obj.Y = Me.DisplayRectangle.Height / 2
                End If

                move_one_piece(bluegoal, True)
            End If


            tcpNet.SendMessageToAll("FIELD" & vbTab & _
                                                  Me.DisplayRectangle.Width.ToString & _
                                                  vbTab & Me.DisplayRectangle.Height.ToString)
        End If

    End Sub

    Private Sub playingField_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '
        '   Hide the window if they close the window manually
        '
        If (e.CloseReason = CloseReason.UserClosing) Then
            e.Cancel = True
            Me.Visible = False
        End If
    End Sub


    '
    '   Once the form is created, then adjust it's location
    '   (happens only once due to a boolean variable in the timer)
    '
    Private Sub playingField_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        controlForm.startupTimer.Start()
    End Sub

    Private Sub playingField_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim backColor As Color

        backColor = ClockLogo.GetPixel(1, 1)
        ClockLogo.MakeTransparent(backColor)

        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)

        Initialize_Server()
    End Sub

    Private Sub Initialize_Server()
        Delete_All_Players()

        redgoal = create_player(-1, 0, 0, teamSide.RED_TEAM, "", playerOrObject.playerType.goal)
        bluegoal = create_player(-1, 0, 0, teamSide.BLUE_TEAM, "", playerOrObject.playerType.goal)

        fix_field()

        ballid = create_player(-1, 0, 0, teamSide.NO_TEAM, "", playerOrObject.playerType.ball)

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
        '
        '   Don't change a minimized window
        '
        If ((Me.DisplayRectangle.Width <> 0) And (Me.DisplayRectangle.Height <> 0)) Then
            fix_field()
        End If

    End Sub



    '   
    '    CountPlayersById(id)
    '
    '   Returns the number of robots controlled by a single person or program
    '
    Public Function CountPlayersById(ByVal id As Integer) As Integer
        Dim I As Integer
        Dim num As Integer = 0
        For I = 0 To maxPlayers
            If (is_active(I) And allPlayers(I).obj IsNot Nothing) Then

                If (allPlayers(I).remoteId = id) Then
                    num = num + 1
                End If
            End If

        Next I

        Return num

    End Function


    Private Sub compute_jersey_num_size(ByRef thisPlayer As onePlayer, ByVal mysize As Integer)
        Dim myEmSize As Single = 12.0
        Dim sf As SizeF

        '
        '  Compute the jersey label size
        '
        '


        thisPlayer.jerseyFont = New Font(Me.Font.FontFamily.ToString, myEmSize, FontStyle.Bold, GraphicsUnit.Pixel)
        thisPlayer.jerseyFontHeight = thisPlayer.jerseyFont.Height

        sf = baseGraphic.MeasureString(thisPlayer.numStr, thisPlayer.jerseyFont, New PointF(0, 0), StringFormat.GenericTypographic)

        '
        '  Make it fit
        '
        While ((myEmSize > 3) And (sf.ToSize.Width > mysize))
            myEmSize = myEmSize - 0.25!         ' 0.25 is forced to type single with a "!"

            thisPlayer.jerseyFont = New Font(Me.Font.FontFamily.ToString, myEmSize, FontStyle.Bold, GraphicsUnit.Pixel)
            thisPlayer.jerseyFontHeight = thisPlayer.jerseyFont.Height

            sf = baseGraphic.MeasureString(thisPlayer.numStr, thisPlayer.jerseyFont, New PointF(0, 0), StringFormat.GenericTypographic)
        End While

        thisPlayer.jerseyOffsetX = CInt((mysize - sf.Width - 2) / 2)

        If (thisPlayer.jerseyOffsetX < 1) Then
            thisPlayer.jerseyOffsetX = 1
        End If
    End Sub


    Private Sub compute_label_size(ByRef thisplayer As onePlayer)
        Dim sf As SizeF

        sf = baseGraphic.MeasureString(thisplayer.name, robotLabelFont)

        thisplayer.labelWidth = CInt(Math.Ceiling(sf.Width))
        thisplayer.labelHeight = CInt(Math.Ceiling(sf.Height))

    End Sub

    Private Sub compute_display_size(ByRef thisPlayer As onePlayer)
        Dim mySize As Integer = thisPlayer.obj.size()
        Dim halfsize As Integer = mySize \ 2

        '
        '  Assume a base square (which is true for all but the robots)
        '
        thisPlayer.leftOffset = -halfsize - 3
        thisPlayer.rightOffset = halfsize + 3
        thisPlayer.topOffset = -halfsize - 3
        thisPlayer.bottomOffset = halfsize + 3


        If (thisPlayer.obj.type = playerOrObject.playerType.player) Then
            compute_jersey_num_size(thisPlayer, mySize)
            compute_label_size(thisPlayer)

            thisPlayer.topOffset = -halfsize - thisPlayer.labelHeight
            thisPlayer.rightOffset = halfsize + thisPlayer.labelWidth
        End If
    End Sub


    Private Sub update_repaint_rect(ByVal myminx As Integer, ByVal mymaxx As Integer, _
                                    ByVal myminy As Integer, ByVal mymaxy As Integer)
        '  Enlarge the "clipping rectangle" if necessary
        '
        If ((xmin = -1) Or (xmin > myminx)) Then
            xmin = myminx
        End If

        If (xmax < mymaxx) Then
            xmax = mymaxx
        End If

        If ((ymin = -1) Or (ymin > myminy)) Then
            ymin = myminy
        End If

        If (ymax < mymaxy) Then
            ymax = mymaxy
        End If
    End Sub

    Private Sub update_repaint_rect(ByRef thisPlayer As onePlayer)
        Dim myminx, mymaxx, myminy, mymaxy As Integer

        myminx = CInt(thisPlayer.obj.X) + thisPlayer.leftOffset
        mymaxx = CInt(thisPlayer.obj.X) + thisPlayer.rightOffset
        myminy = CInt(thisPlayer.obj.Y) + thisPlayer.topOffset
        mymaxy = CInt(thisPlayer.obj.Y) + thisPlayer.bottomOffset

        update_repaint_rect(myminx, mymaxx, myminy, mymaxy)
    End Sub

    Public Function create_player(ByVal creator As Integer, _
                                  ByVal X As Double, ByVal Y As Double, ByVal myteam As teamSide, _
                                  Optional ByVal Name As String = "", Optional ByVal ptype As playerOrObject.playerType = playerOrObject.playerType.player, _
                                  Optional ByVal desiredSize As Integer = -1) As Integer

        Dim result As Integer = -1
        '
        '   Put a limit on the number of robots any one player can create
        '
        If ((creator < 0) OrElse _
            (mymode = playingMode.viewer) _
            OrElse (CountPlayersById(creator) < Parameters.numrobots)) Then

            result = create_player_helper(creator, X, Y, myteam, Name, _
                                          ptype, desiredSize)

        End If

        Return result
    End Function


    '
    '-------------------------------------------------------------------------------
    '

    Public Function create_player_helper(ByVal creator As Integer, _
                                  ByVal X As Double, ByVal Y As Double, ByVal myteam As teamSide, _
                                  Optional ByVal Name As String = "", Optional ByVal ptype As playerOrObject.playerType = playerOrObject.playerType.player, _
                                  Optional ByVal desiredSize As Integer = -1) As Integer
        Dim i As Integer = 0
        'Dim g As Graphics
        'Dim j As Integer
        ' Dim bmap As Bitmap
        'Dim mySolidBrush As SolidBrush
        'Dim mypen As Pen
        Dim numStr As String
        Dim jerseycolor As Color
        Dim ch As Char
        ' Dim mybrush As New TextureBrush(My.Resources.SoccerBall.ToBitmap)
        ' Dim myFont As Font

        'Dim origX As Single
        ' Dim origY As Single


        Select Case myteam
            Case teamSide.NO_TEAM
                jerseycolor = Color.DarkOrange
            Case teamSide.RED_TEAM
                jerseycolor = Color.Red
            Case teamSide.BLUE_TEAM
                jerseycolor = Color.Blue
            Case Else
                jerseycolor = Color.Yellow
        End Select


        '
        '  Find the first empty slot
        '
        While ((i < maxArray) And (allPlayers(i).obj IsNot Nothing))
            i = i + 1
        End While


        If (i >= maxArray) Then
            maxArray = maxArray * 2
            ReDim Preserve allPlayers(maxArray)
        End If

        allPlayers(i).animX = 0
        allPlayers(i).animY = 0

        allPlayers(i).origYstring = Nothing
        allPlayers(i).origXstring = Nothing

        allPlayers(i).team = myteam
        allPlayers(i).baseBrush = New SolidBrush(jerseycolor)
        allPlayers(i).obj = New playerOrObject(X, Y, ptype)

        '
        '  Allow for overriding of size
        '
        If (desiredSize > 0) Then
            allPlayers(i).obj.size() = desiredSize
        End If

        allPlayers(i).remoteId = creator

        'allPlayers(i).pic = New PictureBox()
        'allPlayers(i).pic.Size = New Size(allPlayers(i).obj.size(), allPlayers(i).obj.size())

        ' allPlayers(i).lbl = Nothing   ' Assume blank for now

        Select Case ptype
            Case playerOrObject.playerType.player

                ' allPlayers(i).pic.BackColor = jerseycolor
                'allPlayers(i).pic.BorderStyle = BorderStyle.FixedSingle

                If (Name = "") Then
                    Name = "Robot #%d"
                End If

                '
                '   Allow for player numbers to be imbedded into name
                '
                Name = Name.Replace("%d", i.ToString)
                allPlayers(i).name = Name

                'g = allPlayers(i).lbl.CreateGraphics()

                'sf = g.MeasureString(Name, allPlayers(i).lbl.Font)

                'allPlayers(i).lbl.AutoSize = True
                ' allPlayers(i).lbl.Width = CInt(sf.Width) ' + 2
                'allPlayers(i).lbl.Height = CInt(sf.Height) ' + 2

                'allPlayers(i).lbl.Parent = Me

                '
                '   Take all numbers in Name and use as jersey number
                '

                '
                '   Find all of the numbers in the name
                '
                numStr = ""
                For Each ch In Name
                    If (Char.IsDigit(ch)) Then
                        numStr &= ch
                    End If
                Next

                '
                '  Default to my location in the array
                '
                If (numStr = "") Then
                    numStr = i.ToString
                End If

                allPlayers(i).numStr = numStr

                'bmap = New Bitmap(allPlayers(i).pic.DisplayRectangle.Width, allPlayers(i).pic.DisplayRectangle.Height)

                'g = Graphics.FromImage(bmap)
                'mySolidBrush = New SolidBrush(jerseycolor)

                'g.FillRectangle(mySolidBrush, 0, 0, bmap.Size.Width, bmap.Size.Height)


                'origY = (bmap.Height - sf.Height - 4) / 2

                'If (origY < 1) Then
                'origY = 1
                'End If

                'g.DrawString(numStr, myFont, Brushes.White, origX, origY, StringFormat.GenericTypographic)

                ' g.DrawRectangle(Pens.Black, origY, origY, origX + sf.Width, origY + sf.Height)
                ' g.DrawString("*", Me.Font, Brushes.White, bmap.Width / 2, bmap.Height / 2)

                'allPlayers(i).pic.Image = bmap


            Case playerOrObject.playerType.goal

                allPlayers(i).commonPen = New Pen(jerseycolor, 3)

                'allPlayers(i).pic.BackColor = Color.Transparent
                'bmap = New Bitmap(allPlayers(i).obj.size(), allPlayers(i).obj.size())
                'g = Graphics.FromImage(bmap)

                'g.FillEllipse(Brushes.Black, 1, 1, allPlayers(i).obj.size() - 3, allPlayers(i).obj.size() - 3)
                'g.DrawEllipse(mypen, 1, 1, allPlayers(i).obj.size() - 3, allPlayers(i).obj.size() - 3)

                'allPlayers(i).pic.Image = bmap
                ' allPlayers(i).pic.Invalidate()

            Case playerOrObject.playerType.ball

                allPlayers(i).commonPen = New Pen(Color.Black, 1)

                'allPlayers(i).pic.BackColor = Color.Transparent
                'bmap = New Bitmap(allPlayers(i).obj.size(), allPlayers(i).obj.size())
                'g = Graphics.FromImage(bmap)
                'g.FillEllipse(mybrush, 0, 0, allPlayers(i).obj.size() - 2, allPlayers(i).obj.size() - 2)
                ' g.FillEllipse(Brushes.White, 0, 0, allPlayers(i).obj.size() - 2, allPlayers(i).obj.size() - 2)
                'g.DrawEllipse(mypen, 0, 0, allPlayers(i).obj.size() - 2, allPlayers(i).obj.size() - 2)

                'allPlayers(i).pic.Image = bmap
                ' allPlayers(i).pic.Invalidate()

        End Select

        'allPlayers(i).pic.Parent = Me



        '
        ' Choose a team
        '
        Select Case jerseycolor
            Case Color.Red : allPlayers(i).team = teamSide.RED_TEAM
            Case Color.Blue : allPlayers(i).team = teamSide.BLUE_TEAM
            Case Else : allPlayers(i).team = teamSide.NO_TEAM
        End Select

        compute_display_size(allPlayers(i))

        If (mymode = playingMode.server) Then
            Send_Add(i, True)
        End If
        '
        '   Put the piece on the correct location on the board
        '
        move_one_piece(i, True)

        If (i > maxPlayers) Then maxPlayers = i

        '
        '   Send all labels to the back of the field
        '
        'For j = 0 To maxPlayers
        'If (Not (allPlayers(j).lbl Is Nothing)) Then
        'allPlayers(j).lbl.SendToBack()
        'End If
        'Next

        If (mymode = playingMode.server) Then
            Count_Players()

            If (Not gameRunning) Then
                Reset_Game()            ' Move everyone around until the game begins
            End If
        End If


        redScoreBox.SendToBack()
        blueScoreBox.SendToBack()
        redLabel.SendToBack()
        blueLabel.SendToBack()

        controlForm.UpdateStatus()

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

    Public Sub Delete_Player(ByVal player As Integer)
        Dim i As Integer

        'If (Not (allPlayers(player).lbl Is Nothing)) Then allPlayers(player).lbl.Parent = Nothing
        'allPlayers(player).lbl = Nothing

        'If (Not (allPlayers(player).pic Is Nothing)) Then allPlayers(player).pic.Parent = Nothing
        'allPlayers(player).pic = Nothing

        'allPlayers(player).origXstring = Nothing
        'allPlayers(player).origYstring = Nothing

        '
        '   See if we have to update the maxPlayers
        '
        If (allPlayers(player).obj IsNot Nothing) Then
            '
            '  Make sure that the screen gets updated
            '
            update_repaint_rect(allPlayers(player))

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
            tcpNet.SendMessageToAll("DEL" & vbTab & player.ToString)
            Count_Players()
            '
            '  If the control form goes away first - don't blow up
            '
            Try
                controlForm.UpdateStatus()
            Catch ex As Exception
                ' do nothing
            End Try

        End If

    End Sub

    '
    '   Score
    '
    '       Called when the a goal is made
    '
    '       (causes all of the clients to display the "SCORE" string)
    '
    Private Sub Score()
        scoreCounter = 100

        redScoreBox.Text = redScore.ToString
        blueScoreBox.Text = blueScore.ToString

        send_scores(True)

        scoreLogo.Visible = True

        tcpNet.SendMessageToAll("DISPLAY" & vbTab & scoreCounter.ToString & vbTab & _
                                          """" & scoreLogo.Text & """")

    End Sub

    Private Sub Handle_Tick()
        Dim i As Integer
        Dim didScore As Boolean = False


        tickNum = tickNum + 1
        tcpNet.SendMessageToAll("TICK" & vbTab & tickNum & vbTab & secondsRemain)

        gameRunning = True

        For i = 0 To maxArray
            '
            '   Once the ball is in a goal, don't move any more pieces
            '
            If (Not didScore) Then
                didScore = move_one_piece(i, False)
            End If

        Next

        '
        '  If ball is in goal, then run score routine
        '
        If (didScore) Then Score()

    End Sub


    Private Sub actionTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles actionTimer.Tick
        Dim offset As Integer
        Dim x As Double
        Dim r As Rectangle

        '
        '   Change the timer tick if necessary
        '
        If (actionTimer.Interval <> Parameters.interval) Then
            actionTimer.Interval = Parameters.interval
        End If

        If (Not TimerRunning) Then
            TimerRunning = True


            If (scoreCounter = 0) Then
                If (mymode = playingMode.server) Then
                    Reset_Game()
                End If

                scoreCounter = -1
                scoreLogo.Visible = False

            ElseIf (scoreCounter > 0) Then

                offset = scoreCounter \ 5
                offset = offset Mod 2
                x = Me.DisplayRectangle.Width / 2 - scoreLogo.Width / 2
                x = x - 20 + (40 * offset)
                scoreLogo.Left = CInt(x)
                scoreCounter = scoreCounter - 1

            ElseIf (actionEnabled) Then

                tickCounts = tickCounts + 1             ' Allow the overall system to determine the speed

                If (mymode = playingMode.server) Then
                    Handle_Tick()
                End If

            End If

            '
            '  If necessary, repaint the field
            '
            If ((xmax >= 0) Or (ymax >= 0)) Then
                r = New Rectangle(xmin, ymin, xmax - xmin, ymax - ymin)

                Me.Invalidate(r)
            End If


            TimerRunning = False
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

        redLine = CInt(Me.DisplayRectangle.Width * 0.3)
        blueLine = CInt(Me.DisplayRectangle.Width * 0.7)

        If (resetScore) Then
            redScore = 0
            blueScore = 0
            tickNum = 1
            gameRunning = False

            send_scores(True)
        End If

        redScoreBox.Text = redScore.ToString
        blueScoreBox.Text = blueScore.ToString

        x = Me.DisplayRectangle.Width \ 2           ' Center court
        y = Me.DisplayRectangle.Height \ 2          ' Center court

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
                    If (allPlayers(i).team = teamSide.RED_TEAM) Then
                        allPlayers(i).obj.X = redLine
                        allPlayers(i).obj.Y = redInc * (Me.DisplayRectangle.Height / (redPlayers + 1))
                        redInc = redInc + 1
                    End If

                    If (allPlayers(i).team = teamSide.BLUE_TEAM) Then
                        allPlayers(i).obj.X = blueLine
                        allPlayers(i).obj.Y = blueInc * (Me.DisplayRectangle.Height / (bluePlayers + 1))
                        blueInc = blueInc + 1
                    End If
                End If

                move_one_piece(i, False, True)

                Send_Loc(i, True)
            End If
        Next

        '
        '  Now that we moved everything "by teleportation" we need to  get the screen to match.
        '
        '  It's probably easiest (vs. having the individual items refreshed seperately) just to repaint
        '  the entire screen
        '
        update_repaint_rect(0, Me.DisplayRectangle.Width, 0, Me.DisplayRectangle.Height)
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
            Case Else
                str = str & "other"
        End Select

        '
        '
        '  Add team next

        str = str & vbTab

        Select Case allPlayers(who).team
            Case teamSide.BLUE_TEAM
                str = str & "2"
            Case teamSide.RED_TEAM
                str = str & "1"
            Case Else
                str = str & "0"
        End Select

        str = str & vbTab

        '
        '  Add the current location
        '
        str = str & CInt(allPlayers(who).obj.X).ToString & vbTab & CInt(allPlayers(who).obj.Y).ToString & vbTab

        '
        '  Add the current size
        '
        str = str & allPlayers(who).obj.size & vbTab

        '
        '   Add the name of the object (only if it exists)
        '
        If (allPlayers(who).name <> "") Then
            str = str & """" & allPlayers(who).name & """"
        Else
            str = str & """"""
        End If

        If (toall) Then
            tcpNet.SendMessageToAll(str)
        Else
            tcpNet.SendMessageTo(str, conn)
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
        str = str & CInt(allPlayers(who).obj.X).ToString & vbTab & CInt(allPlayers(who).obj.Y).ToString

        If (toall) Then
            tcpNet.SendMessageToAll(str)
        Else
            tcpNet.SendMessageTo(str, conn)
        End If
    End Sub

    Private Sub send_scores(Optional ByVal toall As Boolean = True, _
                            Optional ByVal conn As Integer = 0)

        If (toall) Then
            tcpNet.SendMessageToAll("SCORE" & vbTab & "red" & vbTab & redScore.ToString)
            tcpNet.SendMessageToAll("SCORE" & vbTab & "blue" & vbTab & blueScore.ToString)
        Else
            tcpNet.SendMessageTo("SCORE" & vbTab & "red" & vbTab & redScore.ToString, conn)
            tcpNet.SendMessageTo("SCORE" & vbTab & "blue" & vbTab & blueScore.ToString, conn)
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
                        Delete_Player(i)
                    End If
                End If
            Next i
        End If

    End Sub

    Private Sub tcpNet_closedTCPConnection(ByVal conn As Integer) Handles tcpNet.closedTCPConnection

        delete_all_from_connection(conn)

        '
        '   Handle the case when the main form goes away (aka an exit)
        '
        Try
            controlForm.UpdateStatus()
        Catch ex As Exception
            ' do nothing
        End Try

    End Sub



    Private Sub tcpNet_newTCPConnection(ByVal conn As Integer) Handles tcpNet.newTCPConnection
        Dim i As Integer
        Dim x As Integer
        Dim y As Integer

        If (mymode = playingMode.server) Then
            '
            '   Tell the new client hello
            '
            tcpNet.SendMessageTo("HELLO" & vbTab & "RobotSoccer" & vbTab & PROTO_VERSION, conn)
            playerOrObject.Get_Boundries(x, y)

            tcpNet.SendMessageTo("FIELD" & vbTab & x.ToString & vbTab & y.ToString, conn)
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

        tcpNet.SendMessageTo("TICK" & vbTab & tickNum & vbTab & secondsRemain, conn)


        controlForm.UpdateStatus()
    End Sub

    Private Sub tcpNet_newTCPMessage(ByVal id As Integer) Handles tcpNet.newTCPMessage
        Dim str As String

        str = tcpNet.GetMessage(id)

        decode_message(id, str)
    End Sub

    Delegate Sub decode_message_delegate(ByVal conn As Integer, ByVal str As String)

    Private Sub decode_message(ByVal conn As Integer, ByVal str As String)
        Dim splitChars() As Char = {ControlChars.Tab}


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
        'Dim needsInvalidate As Boolean = True           ' Assume the screen needs updated

        Select Case str(0).ToUpper
            Case "HELLO"
                Handle_Hello(str)
                'needsInvalidate = False
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
                'needsInvalidate = False
            Case "DEL"
                Handle_Del(str)
            Case "TICK"
                'needsInvalidate = False
                ' Do nothing
            Case ""
                'needsInvalidate = False
                ' Do nothing
            Case "MESG"
                'needsInvalidate = False
                Handle_Mesg(str)
            Case "YOU"
                ' Do nothing
                'needsInvalidate = False
            Case Else
                MsgBox("Unknown command " & str(0) & " received")
                'needsInvalidate = False
        End Select

        'If (needsInvalidate) Then
        '    Me.Invalidate()
        'End If
    End Sub

    Private Sub Handle_Hello(ByRef str() As String)
        If (str(1) <> "RobotSoccer") Then
            MsgBox("Failed - Server (" & str(1) & _
                   ") is not running RobotSoccer")

            tcpNet.CloseConnection()

        Else
            If (str(2) <> PROTO_VERSION) Then
                MsgBox("Failed - Server (" & str(2) & _
                       ")is running a different version than me (" & _
                        PROTO_VERSION & ")")

                tcpNet.CloseConnection()
            End If
        End If
    End Sub

    Private Sub Handle_Display(ByRef str() As String)
        ' DISPLAY <how> "Message"
        Dim ok As Boolean = False
        Dim how As Integer
        Dim msg As String
        Dim trimChars() As Char = {ControlChars.Quote, " "c}

        If (str.Length = 3) Then
            ok = Integer.TryParse(str(1), how)
        End If

        If (ok) Then
            msg = str(2)
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


    Private Sub Handle_Mesg(ByRef str() As String)
        Dim ok As Boolean = True
        Dim fromId As Integer
        Dim toId As Integer

        If (str.Length() = 4) Then
            ok = Integer.TryParse(str(1), fromId)
            ok = ok And Integer.TryParse(str(2), toId)

            If (ok) Then
                MessageLog.addMessage(fromId, toId, str(3))
            End If
        End If
    End Sub

    Private Sub Handle_Score(ByRef str() As String)
        ' SCORE <red|blue> <num>
        Dim ok As Boolean = True
        Dim num As Integer

        If (str.Length() = 3) Then
            ok = Integer.TryParse(str(2), num)
        Else
            ok = False
        End If

        If (ok) Then
            Select Case str(1).ToLower
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
        Dim ok As Boolean = True
        Dim id As Integer
        Dim ptype As playerOrObject.playerType = playerOrObject.playerType.player
        Dim team As teamSide
        Dim X As Integer
        Dim Y As Integer
        Dim Size As Integer
        Dim name As String = String.Empty
        Dim trimChars() As Char = {ControlChars.Quote, " "c}

        If (str.Length = 8) Then
            ok = Integer.TryParse(str(1), id)

            Select Case str(3).Trim
                Case teamSide.BLUE_TEAM.ToString("D")
                    team = teamSide.BLUE_TEAM
                Case teamSide.RED_TEAM.ToString("D")
                    team = teamSide.RED_TEAM
                Case teamSide.NO_TEAM.ToString("D")
                    team = teamSide.NO_TEAM
                Case Else
                    ok = False                      ' All other answers are "wrong"
            End Select


            Select Case str(2).ToLower
                Case "ball"
                    ptype = playerOrObject.playerType.ball
                    team = teamSide.NO_TEAM      ' Force no team
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
        Dim ok As Boolean = True
        Dim id As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim oldx, oldy As Double

        If (str.Length = 4) Then
            ok = Integer.TryParse(str(1), id)
            ok = ok And Integer.TryParse(str(2), X)
            ok = ok And Integer.TryParse(str(3), Y)
        Else
            ok = False
        End If

        If (ok) Then
            If (is_active(id)) Then
                oldx = allPlayers(id).obj.X
                oldy = allPlayers(id).obj.Y

                allPlayers(id).obj.X = X
                allPlayers(id).obj.Y = Y

                move_one_piece_on_screen(id, False, oldx, oldy)
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
        Select Case str(0).ToUpper
            Case "CREATE"
                Handle_Create(conn, str)
            Case "SPEED"
                Handle_Speed(conn, str)
            Case "CHANGESPEED"
                Handle_ChangeSpeed(conn, str)
            Case "REMOVE"
                Handle_Remove(conn, str)
            Case "TALK"
                Handle_Talk(conn, str)
            Case Else
                ' MsgBox("Unknown command " & str(0) & " received")
        End Select
    End Sub

    Private Sub Handle_Talk(ByVal conn As Integer, ByRef str() As String)
        ' TALK <from-who> <to-who> "<mesg>"
        '
        '   Creates outgoing event of
        '       MESG <from-who> <to-who> "<mesg>"
        '
        Dim fromwho As Integer
        Dim towho As Integer

        '
        '  Right # of arguments?
        '
        If (str.Length = 4) Then
            '
            ' Valid integer?   Owned by me?
            '
            If ((Integer.TryParse(str(1), fromwho)) And (Integer.TryParse(str(2), towho))) Then
                '
                '  Validate the from address.  Must be...
                '
                '       1) An active player    and
                '       2) A player controlled by this connection
                '
                If (is_active(fromwho) And (allPlayers(fromwho).remoteId = conn)) Then
                    '
                    '   From is ok, now check the to
                    '
                    MessageLog.addMessage(fromwho, towho, str(3))

                    If (towho = -1) Then
                        tcpNet.SendMessageToAllExcept("MESG" & vbTab & fromwho.ToString & _
                                                                vbTab & "-1" & vbTab & str(3), conn)
                    ElseIf (is_active(towho)) Then

                        tcpNet.SendMessageTo("MESG" & vbTab & fromwho.ToString & _
                                                       vbTab & towho.ToString & vbTab & str(3), _
                                                       allPlayers(towho).remoteId)
                    End If
                End If


            End If
        End If

    End Sub

    Private Sub Handle_Create(ByVal conn As Integer, ByRef str() As String)
        ' CREATE <team> "<name>"
        '   team = red|blue|any
        Dim team As playingField.teamSide = teamSide.RED_TEAM
        Dim name As String = String.Empty
        Dim X As Double
        Dim Y As Double
        Dim newid As Integer
        Dim trimChars() As Char = {ControlChars.Quote, " "c}

        If (redPlayers > bluePlayers) Then team = teamSide.BLUE_TEAM

        If (str.Length = 3) Then name = str(2).Trim(trimChars)

        If (str.Length >= 2) Then
            Select Case str(1)
                Case "red"
                    team = teamSide.RED_TEAM
                Case "blue"
                    team = teamSide.BLUE_TEAM
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

        tcpNet.SendMessageTo("YOU" & vbTab & newid.ToString, conn)
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

    Private Sub Handle_Speed(ByVal conn As Integer, ByRef str() As String)
        ' SPEED <tick#|*> <who> <newXspeed> <newYspeed>

        Dim ok As Boolean = True
        Dim who As Integer
        Dim xSpeed As Double
        Dim ySpeed As Double
        Dim desiredTickNum As Integer

        '
        '  Right number of arguments?
        '
        If (str.Length = 5) Then
            '
            ' Valid integer?   Owned by me?
            '
            If (Integer.TryParse(str(2), who)) Then
                If (is_active(who) And (allPlayers(who).remoteId = conn)) Then
                    ok = Double.TryParse(str(3), xSpeed)
                    ok = ok And Double.TryParse(str(4), ySpeed)
                    ok = ok And (Not Double.IsInfinity(ySpeed))
                    ok = ok And (Not Double.IsInfinity(xSpeed))
                    ok = ok And (Not Double.IsNaN(xSpeed))
                    ok = ok And (Not Double.IsNaN(ySpeed))

                    '
                    '   Numbers valid, and the tick is a number or *?
                    '
                    If (ok And ((str(1) = "*") Or (Integer.TryParse(str(1), desiredTickNum)))) Then
                        '
                        ' Is this for the correct time?
                        '
                        If ((str(1) = "*") Or desiredTickNum = tickNum) Then
                            allPlayers(who).obj.Set_Speed(xSpeed, ySpeed)
                        End If
                    End If
                End If
            End If
        End If
    End Sub


    Private Sub Handle_ChangeSpeed(ByVal conn As Integer, ByRef str() As String)
        ' CHANGESPEED <tick#|*> <who> <newXspeed> <newYspeed>

        Dim ok As Boolean = True
        Dim who As Integer
        Dim xSpeed As Double
        Dim ySpeed As Double
        Dim desiredTickNum As Integer

        '
        '  Right number of arguments?
        '
        If (str.Length = 5) Then
            '
            ' Valid integer?   Owned by me?
            '
            If (Integer.TryParse(str(2), who)) Then
                If (is_active(who) And (allPlayers(who).remoteId = conn)) Then
                    ok = Double.TryParse(str(3), xSpeed)
                    ok = ok And Double.TryParse(str(4), ySpeed)
                    ok = ok And (Not Double.IsInfinity(ySpeed))
                    ok = ok And (Not Double.IsInfinity(xSpeed))
                    ok = ok And (Not Double.IsNaN(xSpeed))
                    ok = ok And (Not Double.IsNaN(ySpeed))
                    '
                    '   Numbers valid, and the tick is a number or *?
                    '
                    If (ok And ((str(1) = "*") Or (Integer.TryParse(str(1), desiredTickNum)))) Then
                        '
                        ' Is this for the correct time?
                        '
                        If ((str(1) = "*") Or desiredTickNum = tickNum) Then
                            allPlayers(who).obj.Change_Speed(xSpeed, ySpeed)
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    '
    '   randomly_place_balls
    '
    '   Move all soccer balls to a (different) random location on the playing field
    '   (used mostly to test robot logic)
    '

    Public Sub randomly_place_balls()
        Dim i As Integer
        Dim maxX As Integer
        Dim maxY As Integer

        playerOrObject.Get_Boundries(maxX, maxY)

        For i = 0 To maxPlayers
            '
            '   Move only real objects
            '
            If (allPlayers(i).obj IsNot Nothing) Then
                '
                '   Move only real objects that are balls
                '
                If (allPlayers(i).obj.type = playerOrObject.playerType.ball) Then
                    ' Make ball stationary
                    allPlayers(i).obj.Set_Speed(0, 0)

                    '
                    '   Move to a random location
                    '
                    allPlayers(i).obj.X = Rnd() * maxX
                    allPlayers(i).obj.Y = Rnd() * maxY
                    move_one_piece(i, False, True)

                    Send_Loc(i, True)
                End If
            End If

        Next
    End Sub

    Public Sub Apply_Parameters()
        Dim i As Integer
        Dim size As Single
        Dim mass As Single
        Dim momentum As Single
        Dim size_changed As Boolean

        For i = 0 To maxPlayers
            If (allPlayers(i).obj IsNot Nothing) Then
                Select Case allPlayers(i).obj.type

                    Case playerOrObject.playerType.ball

                        size = Parameters.ball_size
                        mass = Parameters.ball_mass
                        momentum = Parameters.ball_momentum

                    Case playerOrObject.playerType.goal

                        size = Parameters.goal_size
                        mass = Parameters.goal_mass
                        momentum = Parameters.goal_momentum

                    Case playerOrObject.playerType.player

                        size = Parameters.robot_size
                        mass = Parameters.robot_mass
                        momentum = Parameters.robot_momentum

                End Select

                allPlayers(i).obj.Mass() = mass
                allPlayers(i).obj.Momentum = momentum

                size_changed = (allPlayers(i).obj.size <> size)

                allPlayers(i).obj.size = CInt(size)

                '
                '  Recompute the displayWidth, displayHeight, etc...
                '
                compute_display_size(allPlayers(i))


                '
                '   If the object changed size, then make all clients recreate the object
                '   (so that they will have the correct size)
                '
                If (size_changed) Then
                    tcpNet.SendMessageToAll("DEL" & vbTab & i.ToString)
                    Send_Add(i)
                End If
            End If

        Next
    End Sub

    '   describe_object
    '
    '
    Public Function describe_object(ByVal num As Integer) As String
        Dim s As String = ""

        '
        '  Do we have a good index into the array?
        '
        If ((num >= 0) And (num <= maxPlayers)) Then
            '
            '  Does the object exist
            '
            If (allPlayers(num).obj IsNot Nothing) Then

                Select Case allPlayers(num).team
                    Case teamSide.BLUE_TEAM
                        s = "blue "
                    Case teamSide.RED_TEAM
                        s = "red "
                End Select

                Select Case allPlayers(num).obj.type
                    Case playerOrObject.playerType.ball
                        s = s & "ball"
                    Case playerOrObject.playerType.goal
                        s = s & "goal"
                    Case playerOrObject.playerType.player
                        s = s & "player"
                End Select

                '    If (allPlayers(num).lbl IsNot Nothing) Then
                's = s & " " & allPlayers(num).lbl.Text
                'End If

            End If
        End If

        Return s
    End Function

    Private Sub playingField_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
        controlForm.PlayfieldToolStripMenuItem.Checked = Me.Visible
    End Sub



    '
    '------------------------------------------------------------------------------------------------------------
    '




    '
    '   draw_goal(g,i)
    '
    '   Draw a single goal using the information in the array
    '
    Private Sub draw_goal(ByRef g As Graphics, ByRef thisPlayer As onePlayer)
        Dim mysize As Integer = thisPlayer.obj.size()
        Dim myX As Integer = CInt(thisPlayer.obj.X) - (mysize \ 2)
        Dim myY As Integer = CInt(thisPlayer.obj.Y) - (mysize \ 2)

        g.FillEllipse(Brushes.Black, myX, myY, mysize - 3, mysize - 3)
        g.DrawEllipse(thisPlayer.commonPen, myX + 1, myY + 1, mysize - 3, mysize - 3)
    End Sub

    '
    '   draw_ball(g, i)
    '
    '   Draw a single ball (yes, there can be more than one) using the information in the array
    '
    Private Sub draw_ball(ByRef g As Graphics, ByRef thisPlayer As onePlayer)
        Dim animX As Integer
        Dim animY As Integer
        Dim mybrush As TextureBrush
        Dim mysize As Integer = thisPlayer.obj.size()
        Dim myX As Integer = CInt(thisPlayer.obj.X) - (mysize \ 2)
        Dim myY As Integer = CInt(thisPlayer.obj.Y) - (mysize \ 2)

        animX = thisPlayer.animX
        animY = thisPlayer.animY

        mybrush = New TextureBrush(ballBitmap, _
                                   New Rectangle(15 - (animX Mod 16), 15 - (animY Mod 16), 16, 16))

        g.FillEllipse(mybrush, myX, myY, mysize - 2, mysize - 2)
        g.DrawEllipse(thisPlayer.commonPen, myX, myY, mysize - 2, mysize - 2)
    End Sub

    Private Sub draw_player(ByRef g As Graphics, ByRef thisPlayer As onePlayer)
        Dim mysize As Integer = thisPlayer.obj.size()
        Dim myX As Integer = CInt(thisPlayer.obj.X) - (mysize \ 2)
        Dim myY As Integer = CInt(thisPlayer.obj.Y) - (mysize \ 2)
        Dim myLabelSolidBrush As SolidBrush
        Dim origY As Integer
        'Dim myFont As Font

        If (thisPlayer.team = teamSide.RED_TEAM) Then
            myLabelSolidBrush = redLabelBrush
        Else
            myLabelSolidBrush = blueLabelBrush
        End If

        g.FillRectangle(thisPlayer.baseBrush, myX, myY, mysize, mysize)

        origY = (mysize - thisPlayer.jerseyFontHeight - 4) \ 2

        If (origY < 1) Then
            origY = 1
        End If

        g.DrawString(thisPlayer.numStr, thisPlayer.jerseyFont, Brushes.White, _
                     thisPlayer.jerseyOffsetX + myX, myY + origY, StringFormat.GenericTypographic)

        ' g.DrawRectangle(Pens.Black, origY, origY, origX + sf.Width, origY + sf.Height)
        ' g.DrawString("*", Me.Font, Brushes.White, bmap.Width / 2, bmap.Height / 2)


        g.DrawString(thisPlayer.name, robotLabelFont, myLabelSolidBrush, myX + (mysize \ 2), _
                     myY - robotLabelFontHeight)

        'allPlayers(i).pic.Image = bmap
    End Sub

    Private Sub draw_one_item(ByRef g As Graphics, ByRef thisPlayer As onePlayer)

        Select Case thisPlayer.obj.type
            Case playerOrObject.playerType.ball
                draw_ball(g, thisPlayer)
            Case playerOrObject.playerType.goal
                draw_goal(g, thisPlayer)
            Case playerOrObject.playerType.player
                draw_player(g, thisPlayer)

        End Select


    End Sub


    Private Sub draw_all_items(ByRef g As Graphics, _
                               ByVal xmin As Integer, ByVal xmax As Integer, ByVal ymin As Integer, ByVal ymax As Integer)
        Dim i As Integer
        Dim nplayers As Integer = 0
        Dim myX As Integer
        Dim myY As Integer

        For i = 0 To maxPlayers
            If is_active(i) Then
                myX = CInt(allPlayers(i).obj.X)
                myY = CInt(allPlayers(i).obj.Y)

                If (allPlayers(i).obj.type = playerOrObject.playerType.player) Then
                    nplayers = nplayers + 1
                End If

                '
                '  Redraw only if necessary
                '
                If (rectanglesOverlap(xmin, xmax, ymin, ymax, _
                                      myX + allPlayers(i).leftOffset, myX + allPlayers(i).rightOffset, _
                                      myY + allPlayers(i).topOffset, myY + allPlayers(i).bottomOffset)) Then
                    draw_one_item(g, allPlayers(i))
                End If

            End If
        Next

        numPlayersTotal = nplayers
    End Sub

    '
    '   Draw_Field
    '
    '    Redraw the background image of the soccer field
    '    (centering the lines and circle in the middle)
    '
    '
    '

    Private Sub draw_field(ByRef graph As Graphics, _
                           ByVal xmin As Integer, ByVal xmax As Integer, ByVal ymin As Integer, ByVal ymax As Integer)
        'Dim bmp As Bitmap
        'Dim graph As Graphics

        ' Dim tbrush As TextureBrush

        Dim x As Integer = Me.DisplayRectangle.Width
        Dim y As Integer = Me.DisplayRectangle.Height

        Dim centerx As Integer
        Dim centery As Integer
        Dim quartery As Integer
        Dim boxwidth As Integer = 100
        Dim darkdarkgreen As New SolidBrush(Color.FromArgb(0, 50, 0))
        Const halfpen As Integer = khakiPenWidth \ 2
        Dim triangle(3) As Point

        If ((Me.DisplayRectangle.Width <> 0) And (Me.DisplayRectangle.Height <> 0)) Then


            centerx = x \ 2
            centery = y \ 2
            quartery = y \ 4


            graph.FillRectangle(Brushes.DarkGreen, xmin, ymin, xmax - xmin, ymax - ymin)

            If (rectanglesOverlap(xmin, xmax, ymin, ymax, centerx - 40 - halfpen, centerx + 40 + halfpen, _
                                  centery - 40 - halfpen, centery + 40 + halfpen)) Then

                ' Center circle
                graph.FillEllipse(darkdarkgreen, centerx - 40, centery - 40, 80, 80)

                graph.DrawImageUnscaled(ClockLogo, _
                    centerx - (ClockLogo.Width \ 2), _
                    centery - (ClockLogo.Height \ 2))

                graph.DrawEllipse(khakiPen, centerx - 40, centery - 40, 80, 80)

            End If

            If (rectanglesOverlap(xmin, xmax, ymin, ymax, centerx - halfpen, centerx + halfpen, 0, y)) Then
                ' Center line
                graph.DrawLine(khakiPen, centerx, 0, centerx, centery - 40)
                graph.DrawLine(khakiPen, centerx, centery + 40, centerx, y)
            End If

            ' Around the goal
            graph.DrawLine(khakiPen, 0, quartery, boxwidth, quartery)
            graph.DrawLine(khakiPen, boxwidth, quartery - halfpen, boxwidth, halfpen + 3 * quartery)
            graph.DrawLine(khakiPen, boxwidth, 3 * quartery, 0, 3 * quartery)

            graph.DrawLine(khakiPen, x, quartery, x - boxwidth, quartery)
            graph.DrawLine(khakiPen, x - boxwidth, quartery - halfpen, x - boxwidth, halfpen + 3 * quartery)
            graph.DrawLine(khakiPen, x - boxwidth, 3 * quartery, x, 3 * quartery)

            '
            '  Red Score
            '
            If (rectanglesOverlap(xmin, xmax, ymin, ymax, 24, 24 + 40, 0, 50)) Then
                '
                '  TODO:  Use the two Font variables to draw the score for the red team
                '
            End If


            '
            '  top left
            '
            triangle(0).X = 0
            triangle(0).Y = 0

            triangle(1).X = playerOrObject.CORNER_SIZE
            triangle(1).Y = 0

            triangle(2).Y = playerOrObject.CORNER_SIZE
            triangle(2).X = 0

            triangle(3).X = 0
            triangle(3).Y = 0

            graph.FillPolygon(Brushes.DarkKhaki, triangle)


            '
            '  top right
            '
            triangle(0).X = x
            triangle(0).Y = 0

            triangle(1).X = x - playerOrObject.CORNER_SIZE
            triangle(1).Y = 0

            triangle(2).X = x
            triangle(2).Y = playerOrObject.CORNER_SIZE

            triangle(3).X = x
            triangle(3).Y = 0

            graph.FillPolygon(Brushes.DarkKhaki, triangle)

            '
            '   bottom left
            '
            triangle(0).X = 0
            triangle(0).Y = y

            triangle(1).X = 0
            triangle(1).Y = y - playerOrObject.CORNER_SIZE

            triangle(2).X = playerOrObject.CORNER_SIZE
            triangle(2).Y = y

            triangle(3).X = 0
            triangle(3).Y = y
            graph.FillPolygon(Brushes.DarkKhaki, triangle)

            '
            '   bottom right
            '
            triangle(0).X = x
            triangle(0).Y = y

            triangle(1).X = x - playerOrObject.CORNER_SIZE
            triangle(1).Y = y

            triangle(2).X = x
            triangle(2).Y = y - playerOrObject.CORNER_SIZE

            triangle(3).X = x
            triangle(3).Y = y
            graph.FillPolygon(Brushes.DarkKhaki, triangle)

        End If

    End Sub


    Private Sub repaint_field(ByRef g As Graphics, _
                              ByVal xmin As Integer, ByVal xmax As Integer, ByVal ymin As Integer, ByVal ymax As Integer)


        '
        '  Greedily handle the larger of the two areas that are requested
        '  (although they often are the same)
        '
        '
        If (Me.xmin < xmin) Then xmin = Me.xmin
        If (Me.xmax > xmax) Then xmax = Me.xmax
        If (Me.ymin < ymin) Then ymin = Me.ymin
        If (Me.ymax > ymax) Then ymax = Me.ymax



        '
        '  Only update if a valid update area is given
        '
        If ((xmax >= 0) Or (ymax >= 0)) Then
            draw_field(g, xmin, xmax, ymin, ymax)
            draw_all_items(g, xmin, xmax, ymin, ymax)
        End If


        Me.xmin = -1
        Me.xmax = -1
        Me.ymin = -1
        Me.ymax = -1

    End Sub

    Private Sub playingField_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        repaint_field(e.Graphics, e.ClipRectangle.Left, e.ClipRectangle.Right, e.ClipRectangle.Top, e.ClipRectangle.Bottom)
    End Sub


    Private Sub oneSecondTimer_Tick(sender As System.Object, e As System.EventArgs) Handles oneSecondTimer.Tick
        Dim i As Integer

        numTicks(tickSlot) = tickCounts
        tickCounts = 0

        '
        '  Set up for the next tick slot
        '
        tickSlot = tickSlot + 1

        If (tickSlot >= numTicks.Length) Then
            tickSlot = 0
        End If

        numTicksPerSecond = 0D
        For i = 0 To numTicks.Length - 1
            numTicksPerSecond = numTicksPerSecond + numTicks(i)
        Next

        numTicksPerSecond = numTicksPerSecond / numTicks.Length
    End Sub


    Public Sub splotchField()
        Dim g As Graphics

        g = Me.CreateGraphics()

        g.FillRectangle(Brushes.OrangeRed, 0, 0, Me.Width, Me.Height)
    End Sub




End Class
