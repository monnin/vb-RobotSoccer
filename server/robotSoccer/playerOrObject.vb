'           v0.1    - Initial Version
'           v0.2    - Conform to Option Explict/Option Strict


Option Explicit On
Option Strict On


Public Class playerOrObject

    Enum playerType
        player = 1
        ball = 2
        goal = 3
    End Enum

    Private ptype As playerType

    Private realx As Double
    Private realy As Double

    Private oldRealX As Double                   ' loc before last SelfMove
    Private oldRealY As Double                   ' loc before last SelfMove

    Private radius As Double                     ' Act as if all objects are circles

    Private currentYspeed As Double
    Private currentXspeed As Double

    Private newXspeed As Double
    Private newYspeed As Double

    Private oldXSpeed As Double                 ' speed prior to last SelfMove
    Private oldYSpeed As Double                 ' speed prior to last SelfMove

    Private mymomentum As Double                  ' The amount of momentum an object continues to have
    Private mymass As Double

    Private Shared maxX As Integer = 300
    Private Shared maxY As Integer = 300

    Public Const CORNER_SIZE As Integer = 10


    '
    '   Property to control the location in the X axis
    '
    Public Property X() As Double
        Get
            Return realx
        End Get

        Set(ByVal value As Double)
            '
            '   Don't let the item go off of the screen
            '
            If ((ptype = playerType.ball) Or (ptype = playerType.player)) Then
                If (value < radius) Then value = radius
                If (value > (maxX - radius)) Then value = maxX - radius
            End If

            realx = value
            oldRealX = value
        End Set
    End Property


    Public Property Mass() As Double
        Get
            Return mymass
        End Get

        Set(ByVal value As Double)
            If (value > 0) Then
                mymass = value
            End If
        End Set
    End Property

    Public Property Momentum() As Double
        Get
            Return mymomentum
        End Get

        Set(ByVal value As Double)
            If ((value >= 0) And (value <= 1)) Then
                mymomentum = value
            End If
        End Set
    End Property

    '
    '   Property to return the current speed
    '
    Public ReadOnly Property XSpeed() As Double
        Get
            Return currentXspeed
        End Get
    End Property

    Public ReadOnly Property YSpeed() As Double
        Get
            Return currentYspeed
        End Get
    End Property

    '
    '   Property to control the location in Y axis
    '
    Public Property Y() As Double

        Get
            Return realy
        End Get

        Set(ByVal value As Double)
            '
            '   Don't let the item go off of the screen
            '            
            If ((ptype = playerType.ball) Or (ptype = playerType.player)) Then
                If (value < radius) Then value = radius
                If (value > (maxY - radius)) Then value = maxY - radius
            End If

            realy = value
            oldRealY = value
        End Set
    End Property


    Public ReadOnly Property MinScreenX() As Integer
        Get
            Return CInt(realx - radius)
        End Get
    End Property


    Public ReadOnly Property MinScreenY() As Integer
        Get
            Return CInt(realy - radius)
        End Get
    End Property

    Public ReadOnly Property type() As playerType
        Get
            Return ptype
        End Get
    End Property
    '
    '   Property to control the size
    '
    Public Property size() As Integer
        Get
            Return CInt(radius * 2)
        End Get


        Set(ByVal invalue As Integer)
            Dim value As Double = invalue       ' Promote to a DOuble

            If (value <= 0) Then value = 20
            If (value >= (maxX / 2)) Then value = maxX / 2

            radius = value / 2.0
        End Set
    End Property


    Public Sub New(Optional ByVal X As Double = -1, Optional ByVal Y As Double = -1, _
                Optional ByVal mytype As playerType = playerType.player, _
                Optional ByVal mysize As Double = 0.0)

        ptype = mytype

        Select Case ptype
            Case playerType.ball
                mymomentum = Parameters.ball_momentum
                mymass = Parameters.ball_mass
                If (mysize <= 0) Then mysize = Parameters.ball_size

            Case playerType.goal
                mymomentum = Parameters.goal_momentum
                mymass = Parameters.goal_mass
                If (mysize <= 0) Then mysize = Parameters.goal_size

            Case playerType.player
                mymomentum = Parameters.robot_momentum
                mymass = Parameters.robot_mass
                If (mysize <= 0) Then mysize = Parameters.robot_size
        End Select

        radius = mysize / 2.0

        '
        '   Set the location
        '
        '   TODO: Do we want to check for an immediate collision here?
        '
        If (X < 0) Then
            realx = Rnd() * maxX
        Else
            realx = X
        End If

        If (Y < 0) Then
            realy = Rnd() * maxY
        Else
            realy = Y
        End If

        oldRealX = realx
        oldRealY = realy

        '
        ' Assume stationary
        '
        newXspeed = 0
        newYspeed = 0

        oldXSpeed = 0
        oldYSpeed = 0

        currentXspeed = 0
        currentYspeed = 0
    End Sub

    Private Function mySpeed(ByVal Xspeed As Double, ByVal Yspeed As Double) As Double
        Return Math.Sqrt((Xspeed * Xspeed) + (Yspeed * Yspeed))
    End Function

    Private Sub AdjustSpeed(ByRef XSpeed As Double, ByRef YSpeed As Double, _
                            ByRef NewXSpeed As Double, ByRef NewYSpeed As Double, _
                            ByVal maxSpeed As Double)
        Dim speed As Double
        Dim adjustment As Double = 1

        If ((XSpeed <> NewXSpeed) Or (YSpeed <> NewYSpeed)) Then
            speed = mySpeed(NewXSpeed, NewYSpeed)

            '
            '  Lower both X and Y speed appropriately if over the new max
            '
            If (speed > maxSpeed) Then
                adjustment = maxSpeed / speed

                NewXSpeed = adjustment * NewXSpeed
                NewYSpeed = adjustment * NewYSpeed
            End If

            XSpeed = NewXSpeed
            YSpeed = NewYSpeed

        End If

    End Sub


    Function isInCorner(ByVal x As Double, y As Double) As Boolean
        Dim cpr As Double = CORNER_SIZE + radius
        Dim bigx As Double = maxX - x
        Dim bigy As Double = maxY - y

        Return ((x + y < cpr) Or (bigx + bigy < cpr) Or (bigx + y < cpr) Or (x + bigy < cpr))
    End Function


    Function isInNegativeCorner(ByVal x As Double, y As Double) As Boolean
        Dim cpr As Double = CORNER_SIZE + radius
        Dim bigx As Double = maxX - x
        Dim bigy As Double = maxY - y

        Return ((bigx + y < cpr) Or (x + bigy < cpr))
    End Function

    Public Sub selfMove()
        Dim minSpeed As Double = 0
        Dim maxSpeed As Double = 0
        Dim wallbounce As Double = 0
        Dim cornerbounce As Double = 0
        Dim oldXspeed As Double
        Dim nextX As Double
        Dim nextY As Double
        Dim i As Integer

        If (ptype = playerType.ball) Then
            maxSpeed = Parameters.ball_max_speed
            minSpeed = Parameters.ball_min_speed
            wallbounce = Parameters.ball_wallbounce
            cornerbounce = Parameters.ball_cornerbounce
        End If

        If (ptype = playerType.player) Then
            maxSpeed = Parameters.robot_max_speed
            minSpeed = Parameters.robot_min_speed
            wallbounce = Parameters.robot_wallbounce
            cornerbounce = Parameters.robot_cornerbounce
        End If

        If (ptype = playerType.goal) Then
            maxSpeed = Parameters.goal_max_speed
            minSpeed = Parameters.goal_min_speed
            'wallbounce = Parameters.goal_wallbounce             ' Not actually used
            'cornerbounce = Parameters.goal_cornerbounce         ' Not actually used
        End If

        AdjustSpeed(currentXspeed, currentYspeed, newXspeed, newYspeed, maxSpeed)

        oldRealX = realx
        oldRealY = realy

        realx = realx + currentXspeed
        realy = realy + currentYspeed

        oldXSpeed = currentXspeed
        oldYSpeed = currentYspeed

        '
        '   Constrain to the playing field
        '

        If ((type = playerType.ball) Or (type = playerType.player)) Then
            '
            '   Handle the corners first, then the flat walls
            '
            If (isInCorner(realx, realy)) Then
                '
                '   Try again, and do it in 10 moves to find the bounce
                '
                realx = oldRealX
                realy = oldRealY

                nextX = realx
                nextY = realy

                i = 0
                '
                '   Go up to (but not into) the corner
                '
                While ((i < 10) And (Not isInCorner(nextX, nextY)))
                    realx = nextX
                    realy = nextY

                    nextX = nextX + currentXspeed * 0.1
                    nextX = nextX + currentYspeed * 0.1
                    i = i + 1
                End While

                '
                '  Ok, we now have "hit" the corner wall, so swap the velocities and 
                '  change the signs
                '
                oldXspeed = currentXspeed
            
                If (isInNegativeCorner(nextX, nextY)) Then
                    currentXspeed = currentYspeed * cornerbounce
                    currentYspeed = oldXspeed * cornerbounce
                Else
                    currentXspeed = -currentYspeed * cornerbounce
                    currentYspeed = -oldXspeed * cornerbounce
                End If


                i = i - 1       ' Resstart the failed move that would have made the object go into the corner
                '
                '  Now finish the move
                While (i < 10)
                    realx = realx + currentXspeed * 0.1
                    realy = realy + currentYspeed * 0.1
                    i = i + 1
                End While


            ElseIf ((realx - radius) <= 0) Then
                '
                '  Handle the left wall
                '

                realx = radius
                currentXspeed = -currentXspeed * wallbounce

            ElseIf ((realy - radius) <= 0) Then
                realy = radius
                currentYspeed = -currentYspeed * wallbounce

            ElseIf ((realx + radius) >= maxX) Then
                realx = maxX - radius
                currentXspeed = -currentXspeed * wallbounce

            ElseIf ((realy + radius) >= maxY) Then
                realy = maxY - radius
                currentYspeed = -currentYspeed * wallbounce
            End If


        End If

        currentYspeed = currentYspeed * mymomentum
        currentXspeed = currentXspeed * mymomentum

        '
        '   Allow things to come to a complete rest
        '
        If (Math.Abs(currentXspeed) < minSpeed) Then currentXspeed = 0
        If (Math.Abs(currentYspeed) < minSpeed) Then currentYspeed = 0


        newXspeed = currentXspeed
        newYspeed = currentYspeed

    End Sub


    Public Sub Set_Speed(ByVal xspeed As Double, ByVal yspeed As Double)
        newXspeed = xspeed
        newYspeed = yspeed
    End Sub

    Public Sub Change_Speed(ByVal deltaXspeed As Double, ByVal deltaYspeed As Double)
        newXspeed = currentXspeed + deltaXspeed
        newYspeed = currentYspeed + deltaYspeed
    End Sub

    Public Sub Multiply_Speed(ByVal xfactor As Double, ByVal yfactor As Double)
        newXspeed = currentXspeed * xfactor
        newYspeed = currentYspeed * yfactor
    End Sub

    '
    '   See if we have collided with another object on the board
    '
    Public Function Collided_With(ByRef otherPlayer As playerOrObject) As Boolean
        Dim result As Boolean = False
        Dim distance As Double

        distance = Math.Sqrt((realx - otherPlayer.realx) ^ 2 + (realy - otherPlayer.realy) ^ 2)

        If (distance <= (radius + otherPlayer.radius)) Then
            result = True
        End If

        Return result
    End Function


    '
    '   See if we have collided with another object on the board
    '
    Public Function Will_Collide_With(ByRef otherPlayer As playerOrObject) As Boolean
        Dim result As Boolean = False
        Dim distance As Double

        If (otherPlayer IsNot Nothing) Then
            distance = Math.Sqrt((realx - otherPlayer.realx + currentXspeed) ^ 2 + _
                                 (realy - otherPlayer.realy + currentYspeed) ^ 2)

            If (distance <= (radius + otherPlayer.radius)) Then
                result = True
            End If
        End If

        Return result
    End Function

    Public Sub Handle_Collision(ByRef otherPlayer As playerOrObject)
        Elastic_Collisions.collision2Ds(mymass, otherPlayer.mymass, _
                                       realx, realy, otherPlayer.realx, otherPlayer.realy, _
                                      currentXspeed, currentYspeed, otherPlayer.currentXspeed, otherPlayer.currentYspeed)

        'Elastic_Collisions.collision2Dfull("o", 0, mass, otherplayer.mass, _
        '                                 radius, otherplayer.radius, _
        '                                realx, realy, _
        '                               otherplayer.realx, otherplayer.realy, _
        '                              currentXspeed, currentYspeed, _
        '                             otherplayer.currentXspeed, otherplayer.currentYspeed, _
        '                            haderror)

        If ((currentXspeed = 0) And (currentYspeed = 0)) Then
            If ((otherPlayer.currentXspeed = 0) And (otherPlayer.currentYspeed = 0)) Then
                '
                '  Force some motion in the case of two stationary items colliding
                '
                currentXspeed = 1
                otherPlayer.currentXspeed = -1

                currentYspeed = 1
                otherPlayer.currentYspeed = -1
            End If
        End If

        newXspeed = currentXspeed
        newYspeed = currentYspeed

        otherPlayer.newXspeed = otherPlayer.currentXspeed
        otherPlayer.newYspeed = otherPlayer.currentYspeed
    End Sub

    Public Function Check_Collision(ByRef otherplayer As playerOrObject) As Boolean
        Dim Result As Boolean = False
        ' Dim haderror As Integer

        If (Collided_With(otherplayer)) Then
            Result = True

            '
            '   Undo last move that made the two collide and then
            '     compute the effect of the collision
            '

            realx = oldRealX
            realy = oldRealY

            otherplayer.realx = otherplayer.oldRealX
            otherplayer.realy = otherplayer.oldRealY

            currentXspeed = oldXSpeed
            currentYspeed = oldYSpeed

            otherplayer.currentXspeed = otherplayer.oldXSpeed
            otherplayer.currentYspeed = otherplayer.oldYSpeed

            Handle_Collision(otherplayer)
        End If

        Return Result
    End Function

    '   Method to allow the playing field to adjust the size of the board
    '   (for the bouncing and other physics to work)
    '
    Public Shared Sub Set_Boundries(ByVal X As Integer, ByVal Y As Integer)
        If (X > 0) Then maxX = X
        If (Y > 0) Then maxY = Y
    End Sub


    '   Method to allow the playing field to adjust the size of the board
    '   (for the bouncing and other physics to work)
    '
    Public Shared Sub Get_Boundries(ByRef X As Integer, ByRef Y As Integer)
        X = maxX
        Y = maxY
    End Sub

End Class
