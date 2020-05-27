'
'   MoreMath
'
'   Some additional mathematical routines (like dealing with geometry)
'
'


Module MoreMath
    '
    '   isInRectangle(...) as boolean
    '
    '   Given a point, and a rectangle determine if the point is in the rectangle
    '
    Public Function isInRectangle(ByVal myx As Integer, ByVal myy As Integer, _
                                    ByVal xmin As Integer, ByVal xmax As Integer, ByVal ymin As Integer, ByVal ymax As Integer) As Boolean
        Return (myx >= xmin) And (myx <= xmax) And (myy >= ymin) And (myy <= ymax)
    End Function

    '
    '  rectanglesOverlap(...) as boolean
    '
    '   See if two rectangle overlap at all
    '   (using inverse logic)
    ' http://stackoverflow.com/questions/306316/determine-if-two-rectangles-overlap-each-other
    '
    Public Function rectanglesOverlap(ByVal r1xmin As Integer, r1xmax As Integer, r1ymin As Integer, r1ymax As Integer, _
                                       ByVal r2xmin As Integer, r2xmax As Integer, r2ymin As Integer, r2ymax As Integer) As Boolean
        Dim notOverlapping As Boolean = False

        notOverlapping = (r2xmin > r1xmax) OrElse (r1xmin > r2xmax) OrElse (r2ymin > r1ymax) OrElse (r1ymin > r2ymax)

        Return (Not notOverlapping)
    End Function
End Module
