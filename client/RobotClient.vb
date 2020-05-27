Option Strict On
Option Explicit On


Public Class RobotClient
    '
    '   RoboClient
    '
    '   In interface to control robots on the soccer field
    '
    '           v0.7 - Mark Monnin
    '
    '   Notes:
    '       * Needs SimpleTCPNetwork to run
    '       * Is full of bugs, I'm sure of it
    '
    '   History:
    '           v0.1    - Initial release
    '           v0.2    - Add ChangeSpeed and make myPlayerNum public
    '           v0.3    - Allow allPlayers() to grow (never shrinks)
    '           v0.4    - Add TALK/MESG and allow NULL commands to silently be allowed
    '           v0.5    - Add Option Strict/Option Explicit, add get_id_by_name
    '           v0.6    - Allow it to work with newer versions of SimpleTCPNetwork
    '           v0.7    - Small fix to get_id_by_name
    '
    '

    '=================Declaration=======================================
    '
    '
    '   PROTO_VERSION
    '
    '       Version of the protocol.  Must agree with Server
    '
    Const PROTO_VERSION As String = "1.0"

    '
    '       The three choices of teams (NO_TEAM = any when sent)
    '       (Used in the procol only)
    '
    Const NO_TEAM As Integer = 0
    Const RED_TEAM As Integer = 1
    Const BLUE_TEAM As Integer = 2


    '
    '       objType
    '
    '       What type of thing is each entry in the array?
    '
    Public Enum objType
        Empty = 0
        RedPlayer = 1
        BluePlayer = 2
        Ball = 3
        RedGoal = 4
        BlueGoal = 5
        Other = 6
    End Enum

    Public Enum teamSides
        thedefault = 0          ' The default is either side at first, then keep the same side
        red = 1
        blue = 2
    End Enum

    Private maxArray As Integer = 100
    Private WithEvents tcpNet As New SimpleTCPNetwork(False)

    Public Structure oneMessage
        Public toWho As Integer
        Public fromWho As Integer
        Public mesg As String
    End Structure

    '
    '   Structure to be used to hold a single player
    '   and the relevant graphics parts
    '
    Public Structure onePlayer
        Public X As Integer             ' Location on board (X)
        Public Y As Integer             ' Location on board (Y)
        Public Size As Integer          ' Size of object
        Public name As String           ' Description (given by person controlling it)
        Public type As objType          ' What is it?
        Public isMine As Boolean        ' Can I control it?
    End Structure

    Public maxNumMessages As Integer = 100
    Public mesgQueue As New Queue(Of oneMessage)

    Public allPlayers(maxArray) As onePlayer        ' All of the things on the playing field
    Public maxPlayers As Integer = 0                ' What is the max # for the array?

    Public myPlayerNum As Integer = -1              ' Which one of these is mine?

    Private myType As objType = objType.Other       ' What am I?  (Hopefully a RedPlayer or BluePlayer)

    Public theBall As Integer = -1                  ' Shortcut to finding the ball
    Public ourGoal As Integer = -1                  ' Where I don't want the ball to go
    Public theirGoal As Integer = -1                ' Where I want the ball to go

    Public redScore As Integer                      ' Current score Red side
    Public blueScore As Integer                     ' Current score Blue side

    Public fieldX As Integer                        ' Size of the field (X/width)
    Public fieldY As Integer                        ' Size of the field (Y/height)
    Public tickNum As Integer                       ' What tick # are we on?
    Public secondsLeft As Integer                   ' Time left in the game (currently not active)
    Public newTick As Boolean = False               ' Is this a tick since last SetSpeed op?

    '
    '       canMove()
    '
    '       Is it time for a new move?   (and do I exist?)
    '
    ReadOnly Property canMove() As Boolean
        Get
            Return (newTick) And (myPlayerNum >= 0)
        End Get
    End Property

    '
    '       HavePlayer()
    '
    '       Do I exist?
    '
    ReadOnly Property havePlayer() As Boolean
        Get
            Return (myPlayerNum >= 0)
        End Get
    End Property

    '
    '   newMessage
    '
    '   Indicate if there is a new "talk" message received from the server
    '
    ReadOnly Property newMessage() As Boolean
        Get
            Return (mesgQueue.Count > 0)
        End Get
    End Property

    '
    '   myTeam()
    '
    '       Look or change the team that I am on
    '       (does not send change to the server, though)
    '

    Property myTeam() As teamSides
        Get
            Dim out As teamSides = teamSides.thedefault

            If (myType = objType.BluePlayer) Then out = teamSides.blue
            If (myType = objType.RedPlayer) Then out = teamSides.red

            Return out
        End Get


        Set(ByVal value As teamSides)
            If (value = teamSides.blue) Then
                myType = objType.BluePlayer
            ElseIf (value = teamSides.red) Then
                myType = objType.RedPlayer
            Else
                myType = objType.Other
            End If
        End Set
    End Property


    '
    '   IsConnected() = Are we connected to the server right now?
    '
    ReadOnly Property IsConnected() As Boolean
        Get
            Return (tcpNet.numConnections > 0)
        End Get
    End Property


    '
    '   Get_Score(red,blue)
    '
    '   Return the scores of the two sides
    '
    Public Sub Get_Score(ByRef red As Integer, ByRef blue As Integer)
        red = redScore
        blue = blueScore
    End Sub

    '
    '   Connect = start the connection up
    '
    Public Sub Connect(ByVal server As String, Optional ByVal Port As Integer = 8080)
        tcpNet.StartClient(server, Port)
        Reset_Array()
    End Sub

    '
    '   Disconnect = stop playing
    '
    Public Sub Disconnect()
        tcpNet.CloseConnection()
        Reset_Array()
    End Sub

    '
    '   get_new_message
    '
    '   Get the next message from a list of new messages received from
    '   the server.   Return FALSE if no message exists, or TRUE otherwise
    '
    '   who will return the id of the robot who is receiving the message (or -1 if to all)
    '
    '
    '
    Public Function Get_Message(ByRef fromwho As Integer, ByRef towho As Integer, ByRef mesg As String) As Boolean
        Dim result As Boolean = False
        Dim topMesg As oneMessage

        '
        '   Has a message waiting?  Then grab it from the top of the queue
        '
        If (mesgQueue.Count > 0) Then
            topMesg = mesgQueue.Dequeue
            mesg = topMesg.mesg
            towho = topMesg.toWho
            fromwho = topMesg.fromWho

            result = True   ' Indicate success
        End If

        Return result
    End Function

    '
    '   CreatePlayer
    '
    '   Called to create a new player on the playing field
    '
    '   Once a team side is decided, defaults to "same" side for subsequent players
    '   (still can be requested to server to have players on both teams)
    '
    Public Sub CreatePlayer(Optional ByVal name As String = "", Optional ByVal desiredTeam As teamSides = teamSides.thedefault)
        Dim str As String

        If (desiredTeam = teamSides.blue) Then
            str = "blue"
        ElseIf (desiredTeam = teamSides.red) Then
            str = "red"
        Else
            If (myType = objType.BluePlayer) Then
                str = "blue"
            ElseIf (myType = objType.RedPlayer) Then
                str = "red"
            Else
                str = "any"
            End If
        End If

        tcpNet.SendMessageToAll("CREATE" & vbTab & str & vbTab & """" & name & """")

    End Sub


    '
    '   RemovePlayer()
    '
    '   Removes one of my players from the field. If no id specified, then removes
    '   the "known" player
    '
    Public Sub RemovePlayer(Optional ByVal who As Integer = -1)
        '
        '  Grab our number if necessary
        '
        If (who < 0) Then
            who = myPlayerNum
        End If

        If (who >= 0) Then

            tcpNet.SendMessageToAll("REMOVE" & vbTab & who.ToString)

            If (who = myPlayerNum) Then
                myPlayerNum = -1
            End If
        End If

    End Sub

    '
    '   Send_Message()
    '
    '   Send a line of text to either one specific player (by using the twowho [array entry #] of the
    '   player), or to all (by using -1)
    '
    Public Sub Send_Message(ByVal message As String, Optional ByVal towho As Integer = -1, Optional ByVal fromwho As Integer = -1)
        message = message.Replace(vbTab, " ") ' Replace all tabs with spaces
        message = message.Replace(vbCr, " ")  ' Replace CR with space
        message = message.Replace(vbLf, " ")  ' Replace LF with space
        message = """" & message & """"         ' Surround with quotes

        If (fromwho = -1) Then
            fromwho = myPlayerNum
        End If

        tcpNet.SendMessageToAll("TALK" & vbTab & fromwho.ToString & vbTab & towho.ToString & vbTab & message)

    End Sub


    '
    '       SetSpeed
    '
    '       Used to control the robot.   Speed will deteriorate if no new calls
    '       to SetSpeed are sent.  Also, only one SetSpeed is observed for a time tick
    '       (the last one).
    '

    Public Sub SetSpeed(ByVal Xspeed As Double, ByVal Yspeed As Double, Optional ByVal who As Integer = -1)
        '
        '  Grab our number if necessary
        '
        If (who < 0) Then
            who = myPlayerNum
        End If

        If (who >= 0) Then
            tcpNet.SendMessageToAll("SPEED" & vbTab & "*" & vbTab & _
                                              who.ToString & vbTab & _
                                              Xspeed.ToString & vbTab & Yspeed.ToString)
            newTick = False         ' Need to wait to next tick to move this piece again
        End If

    End Sub



    '
    '       SetSpeed
    '
    '       Used to control the robot.   Speed will deteriorate if no new calls
    '       to SetSpeed are sent.  Also, only one SetSpeed is observed for a time tick
    '       (the last one).
    '

    Public Sub ChangeSpeed(ByVal Xspeed As Double, ByVal Yspeed As Double, Optional ByVal who As Integer = -1)
        '
        '  Grab our number if necessary
        '
        If (who < 0) Then
            who = myPlayerNum
        End If

        If (who >= 0) Then
            tcpNet.SendMessageToAll("CHANGESPEED" & vbTab & "*" & vbTab & _
                                              who.ToString & vbTab & _
                                              Xspeed.ToString & vbTab & Yspeed.ToString)
            newTick = False         ' Need to wait to next tick to move this piece again
        End If

    End Sub

    '
    '       Get_Obj_Loc
    '
    '       Given an id, return the X and Y location of that object
    '
    Public Function Get_Obj_Loc(ByVal who As Integer, ByRef X As Integer, ByRef Y As Integer) As Boolean
        Dim ok As Boolean = True

        If ((who >= 0) And (who <= maxPlayers)) Then
            If (allPlayers(who).type = objType.Empty) Then
                ok = False
            Else
                X = allPlayers(who).X
                Y = allPlayers(who).Y
            End If
        Else
            ok = False
        End If

        If (Not ok) Then
            '
            '   No object, then lead them to the middle, I guess
            '
            X = fieldX \ 2              ' Integer division
            Y = fieldY \ 2              ' Integer division
        End If

        Return ok
    End Function

    '
    '       Get_Ball_Loc
    '
    '       Return the X & Y location of the ball
    '
    Public Function Get_Ball_Loc(ByRef X As Integer, ByRef Y As Integer) As Boolean
        Return Get_Obj_Loc(theBall, X, Y)
    End Function


    '
    '       Get_Our_Goal_Loc
    '
    '       Return the X & Y location of our goal (where we want to defend)
    '
    Public Function Get_Our_Goal_Loc(ByRef X As Integer, ByRef Y As Integer) As Boolean
        Return Get_Obj_Loc(ourGoal, X, Y)
    End Function


    '
    '       Get_Their_Goal_Loc
    '
    '       Return the X & Y location of their goal (where we want to send the ball)
    '
    Public Function Get_Their_Goal_Loc(ByRef X As Integer, ByRef Y As Integer) As Boolean
        Return Get_Obj_Loc(theirGoal, X, Y)
    End Function


    '
    '       Get_Ball_Loc
    '
    '       Return the X & Y location of me
    '
    Public Function Get_My_Loc(ByRef X As Integer, ByRef Y As Integer) As Boolean
        Return Get_Obj_Loc(myPlayerNum, X, Y)
    End Function


    '
    '   Given a string name, return the ID of a specific player
    '   (returns -1 if the name is not found)
    '
    Public Function Get_Id_By_Name(ByVal theirName As String) As Integer
        Dim who As Integer = -1     ' Assume not found
        Dim i As Integer = 0

        While ((i <= maxPlayers) And (who = -1))
            If (allPlayers(i).name = theirName) Then
                who = i
            End If

            i = i + 1
        End While

        Return who
    End Function


    '
    '
    '--------------------------------------------------------------------------------
    '
    '

    '
    '       New() - Called on creation of the object
    '
    '       Just make sure the main player array is sane
    '   
    Public Sub New()
        Reset_Array()
    End Sub


    '
    '       Reset_Array
    '
    '       Reset everything back to the case at starting point
    '
    '           (aka no players, and every slot in the array "empty")
    '
    Private Sub Reset_Array()
        Dim i As Integer

        maxPlayers = 0                  ' No players
        '
        '  Make all player slots empty
        '
        For i = 0 To maxArray - 1
            allPlayers(i).type = objType.Empty
            allPlayers(i).isMine = False
        Next

        myType = objType.Other

    End Sub


    '
    '       Recount_Players
    '
    '       Used when a player is removed to find the new upper limit on the array
    '
    Private Sub Recount_Players()
        Dim i As Integer
        Dim newMax As Integer = 0
        For i = 0 To maxPlayers
            If (allPlayers(i).type <> objType.Empty) Then
                newMax = i
            End If
        Next i

        maxPlayers = i
    End Sub

    '
    '       tcpNet_newTCPMessage(id)
    '
    '       Called when SimpleTCPNetwork gets a new message from the server
    '
    Private Sub tcpNet_newTCPMessage(ByVal id As Integer) Handles tcpNet.newTCPMessage
        Dim str As String
        Dim splitChars() As Char = {ControlChars.Tab}

        str = tcpNet.GetMessage(id)

        decode_from_server(str.Split(splitChars))
    End Sub


    '
    '       decode_from_server
    '
    '       Given a set of strings (aka the line broken by TABs), determine the
    '       verb and use the correct subroutine to decode the rest of the command)
    '

    Private Sub decode_from_server(ByRef str() As String)
        Select Case str(0).ToUpper
            Case ""
                ' Do nothing
            Case "HELLO"
                Handle_Hello(str)
            Case "FIELD"
                Handle_Field(str)
            Case "ADD"
                Handle_Add(str)
            Case "LOC"
                Handle_Loc(str)
            Case "DISPLAY"
                ' Handle_Display(str)
            Case "YOU"
                Handle_You(str)
            Case "SCORE"
                Handle_Score(str)
            Case "DEL"
                Handle_Del(str)
            Case "TICK"
                Handle_Tick(str)
            Case "MESG"
                Handle_Mesg(str)
            Case Else
                MsgBox("Unknown command """ & str(0) & """ received")
        End Select
    End Sub

    '
    '       Handle_Hello
    '
    '       Decodes the "HELLO" command
    '
    '       Verify that the two sides are running the same protocol version
    '       (if not, don't let them talk)
    '
    Private Sub Handle_Hello(ByRef str() As String)
        If (str(1) <> "RobotSoccer") Then
            MsgBox("Failed - Server (" & str(1) & _
                   ")is not running RobotSoccer")

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

    '
    '   Handle_Mesg
    '
    '   Take an incoming "MESG" command and place the message into a queue
    '   for the client to use
    '
    Private Sub Handle_Mesg(ByRef str() As String)
        Dim fromwho As Integer
        Dim towho As Integer
        Dim newMesg As oneMessage

        If (Integer.TryParse(str(1), fromwho) And Integer.TryParse(str(2), towho)) Then
            newMesg.fromWho = fromwho
            newMesg.toWho = towho

            newMesg.mesg = str(3).Trim(ControlChars.Quote)            ' Remove the leading and trailing double-quote

            '
            '  Make sure that the queue doesn't get out of hand
            '  (if the client decides not to implement messages)
            '
            While (mesgQueue.Count > maxNumMessages)
                mesgQueue.Dequeue()
            End While

            mesgQueue.Enqueue(newMesg)
        End If

    End Sub


    '
    '       Handle_Field
    '
    '       Decodes the "FIELD" command
    '
    '       Retrieves the size of the playing field
    '       (may change if the server window is resized)
    '
    Private Sub Handle_Field(ByRef str() As String)
        Dim X As Integer
        Dim Y As Integer

        If (Integer.TryParse(str(1), X) And Integer.TryParse(str(2), Y)) Then
            fieldX = X
            fieldY = Y
        Else
            MsgBox("Invalid FIELD command")
        End If
    End Sub


    '
    '       Handle_Add
    '
    '       Decodes the "ADD" command
    '
    '       Happens everytime something is added to the playing field
    '       (either by this client or some other client)
    '
    Private Sub Handle_Add(ByRef str() As String)
        '   ADD <id> <type> <team#> <xloc> <yloc> <size> "<name>"
        Dim ok As Boolean = True
        Dim id As Integer
        Dim ptype As objType = objType.Other
        Dim team As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Size As Integer
        Dim oldMax As Integer
        Dim i As Integer
        Dim name As String = String.Empty
        Dim trimChars() As Char = {ControlChars.Quote, " "c}

        If (str.Length = 8) Then
            ok = Integer.TryParse(str(1), id)

            ok = ok And Integer.TryParse(str(3), team)

            Select Case str(2).ToLower
                Case "ball"
                    ptype = objType.Ball

                Case "goal"
                    Select Case team
                        Case RED_TEAM
                            ptype = objType.RedGoal
                        Case BLUE_TEAM
                            ptype = objType.BlueGoal
                    End Select

                Case "player"
                    Select Case team
                        Case RED_TEAM
                            ptype = objType.RedPlayer
                        Case BLUE_TEAM
                            ptype = objType.BluePlayer
                    End Select

                Case Else
                    ok = False
            End Select

            ok = ok And Integer.TryParse(str(4), X)
            ok = ok And Integer.TryParse(str(5), Y)
            ok = ok And Integer.TryParse(str(6), Size)
            name = str(7)
            name = name.Trim(trimChars)
        Else
            ok = False
        End If

        '
        '   Array out of bounds?
        '
        If (id < 0) Then
            ok = False
        ElseIf (id >= maxArray) Then
            '
            '   Make the array larger
            '
            oldMax = maxArray
            maxArray = maxArray * 2

            ReDim Preserve allPlayers(maxArray)

            For i = oldMax To maxArray - 1
                allPlayers(i).type = objType.Empty
                allPlayers(i).isMine = False
            Next
        End If

        If (ok) Then
            allPlayers(id).X = X
            allPlayers(id).Y = Y
            allPlayers(id).Size = Size
            allPlayers(id).name = name
            allPlayers(id).type = ptype
            '
            '  Record the location of the goal
            '
            If (ptype = objType.Ball) Then
                theBall = id
            End If

            allPlayers(id).isMine = False   ' all start out as not mine

            '
            '  Do we have more players now?
            '
            If (id > maxPlayers) Then
                maxPlayers = id
            End If
        Else
            MsgBox("Invalid ADD command received")
        End If
    End Sub

    '
    '       Handle_Loc
    '
    '       Decodes the "LOC" command
    '
    '       Updates the client of some object changing position on the field
    '
    Private Sub Handle_Loc(ByRef str() As String)
        '   LOC <id> <xloc> <yloc>
        Dim ok As Boolean = True
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
            If ((id >= 0) And (id < maxArray)) Then
                allPlayers(id).X = X
                allPlayers(id).Y = Y
            End If
        Else
            MsgBox("Invalid LOC command received")
        End If
    End Sub


    '
    '       Handle_You
    '
    '       Decodes the "YOU" command
    '
    '       Tells this client that this object is controllable
    '       (sent after the client adds 
    '   
    Private Sub Handle_You(ByRef str() As String)
        '   YOU <id>
        Dim id As Integer
        Dim ok As Boolean
        Dim i As Integer


        If (str.Length = 2) Then
            ok = Integer.TryParse(str(1), id)
        Else
            ok = False
        End If

        If (ok) Then
            If ((id >= 0) And (id < maxArray)) Then
                allPlayers(id).isMine = True


                Select Case allPlayers(id).type
                    Case objType.RedGoal, objType.RedPlayer
                        myTeam = teamSides.red
                    Case objType.BlueGoal, objType.BluePlayer
                        myTeam = teamSides.blue
                    Case Else
                        myTeam = teamSides.thedefault
                End Select

                myPlayerNum = id

                '
                '  Find the goals
                '
                For i = 0 To maxPlayers
                    If (allPlayers(i).type = objType.BlueGoal) Then
                        If (myType = objType.BluePlayer) Then
                            ourGoal = i
                        Else
                            theirGoal = i
                        End If

                    ElseIf (allPlayers(i).type = objType.RedGoal) Then
                        If (myType = objType.RedPlayer) Then
                            ourGoal = i
                        Else
                            theirGoal = i
                        End If
                    End If
                Next i
            End If
        Else
            MsgBox("Invalid YOU command received")
        End If
    End Sub


    '
    '       Handle_Score
    '
    '       Decode the "SCORE" command
    '
    '       Updates either the red side or blue side's score
    '
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
                    redScore = num
                Case "blue"
                    blueScore = num
            End Select
        Else
            MsgBox("Invalid SCORE command received")
        End If
    End Sub


    '
    '       Handle_Del
    '
    '       Decode the "DEL" command
    '
    '       Tells the client that a player has been removed from the field
    '
    Private Sub Handle_Del(ByRef str() As String)
        '   DEL <id>
        Dim id As Integer

        If (Integer.TryParse(str(1), id)) Then
            If ((id >= 0) And (id < maxArray)) Then
                allPlayers(id).type = objType.Empty
                allPlayers(id).isMine = False           ' Empties are not mine
            End If

            Recount_Players()
        End If
    End Sub


    '
    '       Handle_Tick
    '
    '       Decode the "TICK" command
    '
    '       Tells the client that it is ok to send another SPEED command
    '       (or that soccer time is passing)
    '
    Private Sub Handle_Tick(ByRef str() As String)
        Dim newtickNum As Integer
        Dim newSecondsLeft As Integer

        If (Integer.TryParse(str(1), newtickNum) And Integer.TryParse(str(2), newSecondsLeft)) Then
            tickNum = newtickNum
            secondsLeft = newSecondsLeft
            newTick = True
        Else
            MsgBox("Invalid TICK command")
        End If
    End Sub

End Class
