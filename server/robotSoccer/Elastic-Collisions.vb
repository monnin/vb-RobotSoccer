Option Strict On
Option Explicit On


'******************************************************************************
'  Simplified Version
'  The advantage of the 'remote' collision detection in the program above is 
'  that one does not have to continuously track the balls to detect a collision. 
'  The program needs only to be called once for any two balls unless their 
'  velocity changes. However, if somebody wants to use a separate collision 
'  detection routine for whatever reason, below is a simplified version of the 
'  code which just calculates the new velocities, assuming the balls are already 
'  touching (this condition is important as otherwise the results will be incorrect)
'
'   Converted from http:'www.plasmaphysics.org.uk/programs/coll2d_for.htm
'
'****************************************************************************
Module Elastic_Collisions

    Public Sub collision2Ds(ByVal m1 As Double, ByVal m2 As Double, _
                            ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double, _
                            ByRef vx1 As Double, ByRef vy1 As Double, ByRef vx2 As Double, ByRef vy2 As Double)


        'CHARACTER*4 mode
        'INTEGER*2 error
        Dim m21 As Double       ' Mass of 2nd over first
        Dim x21 As Double       ' Diff of X locations
        Dim y21 As Double       ' Diff of Y locations

        Dim vx21 As Double      ' Diff of X speeds
        Dim vy21 As Double      ' Diff of Y speeds

        Dim fy21 As Double
        Dim sign As Double      ' 1 or -1

        Dim a As Double      ' temp spot
        Dim dvx2 As Double

        m21 = m2 / m1
        x21 = x2 - x1
        y21 = y2 - y1
        vx21 = vx2 - vx1
        vy21 = vy2 - vy1

        ' *** return old velocities if balls are not approaching ***
        If ((vx21 * x21 + vy21 * y21) > 0) Then
            Return
        End If

        If (x21 = 0) Then x21 = 0.000000000001
        If (y21 = 0) Then y21 = 0.000000000001

        If (Math.Abs(x21 + y21) <= 0.000000000002) Then
            '
            '  Basically at the same location - so make the collision "bad"
            vx21 = vx21 * 100
            vy21 = vy21 * 100
        End If

        ' *** I have inserted the following statements to avoid a zero divide; 
        ' *** (for single precision calculations, 
        ' ***         1.0E-12 should be replaced by a larger value). **********  

        fy21 = 0.000000000001 * Math.Abs(y21)

        If (Math.Abs(x21) < fy21) Then
            If (x21 < 0) Then
                sign = -1.0
            Else
                sign = 1.0
            End If
            x21 = fy21 * sign
        End If

        ' ***  update velocities ***

        a = y21 / x21

        dvx2 = -2 * (vx21 + a * vy21) / ((1 + a * a) * (1 + m21))

        vx2 = vx2 + dvx2
        vy2 = vy2 + a * dvx2
        vx1 = vx1 - m21 * dvx2
        vy1 = vy1 - a * m21 * dvx2

        Return
    End Sub



    '******************************************************************************
    '   This program is a 'remote' 2D-collision detector for two balls on linear
    '   trajectories and returns, if applicable, the location of the collision for 
    '   both balls as well as the new velocity vectors (assuming a fully elastic
    '   collision).
    '   In  'f' (free) mode no positions but only the initial velocities
    '   and an impact angle are required.
    '   All variables apart from 'mode' and 'error' are of Double Precision
    '   Floating Point type.
    '
    '   The Parameters are:
    '
    '    mode  (char) (if='f' alpha must be supplied; otherwise arbitrary)
    '    alpha (impact angle) only required in mode='f'; 
    '                     should be between -PI/2 and PI/2 (0 = head-on collision))
    '    m1   (mass of ball 1)
    '    m2   (mass of ball 2)
    '    r1   (radius of ball 1)        not needed for 'f' mode
    '    r2   (radius of ball 2)                "
    '  & x1   (x-coordinate of ball 1)          "
    '  & y1   (y-coordinate of ball 1)          "
    '  & x2   (x-coordinate of ball 2)          "
    '  & y2   (y-coordinate of ball 2)          "
    '  & vx1  (velocity x-component of ball 1) 
    '  & vy1  (velocity y-component of ball 1)         
    '  & vx2  (velocity x-component of ball 2)         
    '  & vy2  (velocity y-component of ball 2)
    '  & error (int)  (0: no error
    '                  1: balls do not collide
    '                  2: initial positions impossible (balls overlap))
    '
    '   Note that the parameters with an ampersand (&) are passed by reference,
    '   i.e. the corresponding arguments in the calling program will be updated;
    '   however, the coordinates and velocities will only be updated if 'error'=0.
    '
    '   All variables should have the same data types in the calling program
    '   and all should be initialized before calling the function even if
    '   not required in the particular mode.
    '
    '   This program is free to use for everybody. However, you use it at your own
    '   risk and I do not accept any liability resulting from incorrect behaviour.
    '   I have tested the program for numerous cases and I could not see anything 
    '   wrong with it but I can not guarantee that it is bug-free under any 
    '   circumstances.
    '
    '   I would appreciate if you could report any problems to me
    '   (for contact details see  http:'www.plasmaphysics.org.uk/feedback.htm ).
    '
    '   Thomas Smid, January  2004
    '                December 2005 (corrected faulty collision detection; 
    '                               a few minor changes to improve speed;
    '                               added simplified code without collision detection)
    '*********************************************************************************

    Public Sub collision2Dfull(ByVal mode As Char, ByVal alpha As Double, _
            ByVal m1 As Double, ByVal m2 As Double, _
            ByVal r1 As Double, ByVal r2 As Double, _
            ByRef x1 As Double, ByRef y1 As Double, ByRef x2 As Double, ByRef y2 As Double, _
            ByRef vx1 As Double, ByRef vy1 As Double, _
            ByRef vx2 As Double, ByRef vy2 As Double, _
            ByRef haderror As Integer)


        Dim r12, m21, d, gammav, gammaxy, dgamma, dr, dc, sqs, t As Double
        Dim dvx2, a, x21, y21, vx21, vy21, pi2 As Double

        '     ***initialize some variables ****
        pi2 = 2 * Math.Acos(-1.0)
        haderror = 0
        r12 = r1 + r2
        m21 = m2 / m1
        x21 = x2 - x1
        y21 = y2 - y1
        vx21 = vx2 - vx1
        vy21 = vy2 - vy1


        '     ****  return old positions and velocities if relative velocity =0 ****
        If ((vx21 = 0) And (vy21 = 0)) Then
            haderror = 1
            Return
        End If


        '     *** calculate relative velocity angle             
        gammav = Math.Atan2(-vy21, -vx21)

        '******** this block only if initial positions are given *********

        If (mode <> "f") Then

            d = Math.Sqrt(x21 * x21 + y21 * y21)

            '     **** return if distance between balls smaller than sum of radii ***
            If (d < r12) Then
                'MsgBox("Bogus")
                'haderror = 2
                'Return
                d = r12
            End If


            '     *** calculate relative position angle and normalized impact parameter ***
            gammaxy = Math.Atan2(y21, x21)
            dgamma = gammaxy - gammav

            If (dgamma > pi2) Then
                dgamma = dgamma - pi2
            ElseIf (dgamma < -pi2) Then
                dgamma = dgamma + pi2
            End If

            dr = d * Math.Sin(dgamma) / r12

            '     **** return old positions and velocities if balls do not collide ***
            If (((Math.Abs(dgamma) > pi2 / 4) And (Math.Abs(dgamma) < 0.75 * pi2)) Or (Math.Abs(dr) > 1)) Then
                haderror = 1
                Return
            End If

            '     **** calculate impact angle if balls do collide ***
            alpha = Math.Asin(dr)


            '     **** calculate time to collision ***
            dc = d * Math.Cos(dgamma)

            If (dc > 0) Then
                sqs = 1.0
            Else
                sqs = -1.0
            End If

            t = (dc - sqs * r12 * Math.Sqrt(1 - dr * dr)) / Math.Sqrt(vx21 * vx21 + vy21 * vy21)
            '    **** update positions ***
            x1 = x1 + vx1 * t
            y1 = y1 + vy1 * t
            x2 = x2 + vx2 * t
            y2 = y2 + vy2 * t
        End If

        '******** END 'this block only if initial positions are given' *********

        '     ***  update velocities ***

        a = Math.Tan(gammav + alpha)

        dvx2 = -2 * (vx21 + a * vy21) / ((1 + a * a) * (1 + m21))

        vx2 = vx2 + dvx2
        vy2 = vy2 + a * dvx2
        vx1 = vx1 - m21 * dvx2
        vy1 = vy1 - a * m21 * dvx2

        Return
    End Sub



End Module
