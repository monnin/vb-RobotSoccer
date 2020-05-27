Option Explicit On
Option Strict On

Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text

'
'   SimpleTCPNetwork
'
'   An (over) simplistic class to communicate over TCP
'
'           v0.8 - Mark Monnin
'
'   Notes:
'       * Is full of bugs, I'm sure of it
'
'   History:
'           v0.1    - Initial release
'           v0.2    - Rewrite code to use array instead of collections
'           v0.3    - Fix a stupid bug with HasMessages()
'           v0.4    - Major changes - make it use instances instead of a shared object
'                   - also allow it to be more "thread safe" in its events
'           v0.5    - Add "SendMessage" as an alias to "SendMessageToAll"
'           v0.6    - Enable Option Strict
'           v0.7    - Handle cases where the holding class is not of type Control 
'                   - (aka allow RaiseEvent by allowing an option to the constructor)
'           v0.8    - Make StopClient an alias for CloseConnection
'                     Add GetMessageWithReturn()
'
'
'=================Declaration=======================================
'
Public Class SimpleTCPNetwork

    '
    '   The following four events can be seen by the program and indicates
    '   when specific events occur
    '
    '       newTCPConnection -  when a client connects to a server 
    '                           (contains an id that can be used to identify this client)
    '
    '       closeTCPConnection - when a client (or server) shuts down the connection 
    '                           (contains id that is now gone)
    '
    '       tcpConnectionFailed - if client cannot connect to the remote system
    '                             (no connection was made, hence no remote id)
    '
    '       newTCPMessage - when any connection now has new data ready for it
    '                           (contains an id that indicates who now has new data)
    '
    '
    Public Event newTCPConnection(ByVal conn As Integer)
    Public Event closedTCPConnection(ByVal conn As Integer)
    Public Event tcpConnectionFailed()
    Public Event newTCPMessage(ByVal conn As Integer)


    Private Const origArraySize As Integer = 100
    Private currentArraySize As Integer = origArraySize
    Private connectionCount As Integer = 0

    Private nextClientID As Integer = 1

    Private useInvoke As Boolean = True

    '
    '   oneConnection
    '
    '   Defines a single connection to another system
    '
    '   For programs running in "client" mode, there will only be one
    '   instance of this.  For programs running in "server" mode, there will
    '   be one for each connected system.
    '
    Private Class oneConnection


        Private Shared ASCII As Encoding = Encoding.ASCII
        Public Shared parent As SimpleTCPNetwork

        Private socket As TcpClient
        Private partialLine As String
        Private fullLines As Queue(Of String)
        Private stream As NetworkStream
        Private buff(512) As Byte
        Private myid As Integer
        Private remoteAddr As String

        '
        '   Return an externally usable id for this connection
        '
        ReadOnly Property id() As Integer
            Get
                Return myid
            End Get
        End Property

        '
        '   Return a name that defines where the other endpoint is
        '
        ReadOnly Property name() As String
            Get
                Return remoteAddr
            End Get
        End Property


        '
        '   New
        '
        '       Given a TcpClient (aka something already established), set up
        '       oneConnection with some reasonable defaults
        '
        Public Sub New(ByRef client As TcpClient, ByVal clientId As Integer)
            Dim ipEnd As Net.IPEndPoint

            socket = client
            myid = clientId

            stream = client.GetStream()

            '
            '  Enable TCP Keepalives
            '
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1)


            '
            '  Diable Nagle (aka enable "nodelay")
            '
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1)

            '
            '  Convert the Net.EndPoint to Net.IPEndpoint in a way
            '  that makes "strict" happy
            '
            ipEnd = CType(client.Client.RemoteEndPoint, Net.IPEndPoint)

            remoteAddr = ipEnd.Address.ToString & ":" & ipEnd.Port.ToString

            fullLines = New Queue(Of String)
            fullLines.Clear()

            partialLine = ""

            stream.BeginRead(buff, 0, buff.Length, AddressOf newDataReady, Me)
        End Sub

        '
        '   Shutdown - remove a connection and tell the program that this happened
        '
        Public Sub Shutdown()
            If (stream IsNot Nothing) Then

                stream = Nothing

                If (socket IsNot Nothing) Then
                    socket.Close()
                    socket = Nothing

                    parent.SendClosedEvent(myid)
                End If

            End If

        End Sub

        Protected Overrides Sub Finalize()
            Shutdown()
            MyBase.Finalize()
        End Sub

        '
        '   newDataReady
        '
        '       Handle when data arrives (asynchronously) from remote system
        '
        Private Sub newDataReady(ByVal ar As IAsyncResult)
            Dim numBytes As Integer
            Dim wholeLine As String
            Dim loc As Integer
            Dim trimChars() As Char = {ControlChars.Cr}

            Try
                numBytes = stream.EndRead(ar)
            Catch ex As Exception
                numBytes = 0
            End Try

            If (numBytes > 0) Then

                partialLine = partialLine & ASCII.GetString(buff, 0, numBytes)
                ' SimpleTCPNetwork.myStatus = "New data! " & partialLine

                '
                '  Did we get an end of line (LF)?
                '
                While (partialLine.Contains(vbLf))
                    loc = partialLine.IndexOf(vbLf)
                    '
                    '  Ignore empty lines
                    '
                    If (loc > 0) Then
                        '
                        '  Get the first string
                        '
                        wholeLine = partialLine.Substring(0, loc)

                        ' Remove the optional CR at beginning and end of message
                        wholeLine = wholeLine.Trim(trimChars)

                        '
                        '   Add to the queue
                        '
                        fullLines.Enqueue(wholeLine)

                        '
                        '   Inform the program that data is ready
                        'raiseEvent newTCPMessage(myid)
                        parent.SendNewMessageEvent(myid)
                    End If

                    '
                    '   See what's left
                    '
                    partialLine = partialLine.Substring(loc + 1)

                End While

                '
                '  Set up for the next read
                '
                Try
                    stream.BeginRead(buff, 0, buff.Length, AddressOf newDataReady, Me)
                Catch ex As Exception
                    '
                    ' Stream closed, so close up shop
                    '
                    parent.CloseConnection(myid)
                End Try

            Else
                '
                ' Stream closed, so close up shop
                '
                parent.CloseConnection(myid)
            End If
        End Sub


        '
        '   SendMessage
        '
        '   Send a string to the remote system
        '
        Public Sub SendMessage(ByVal str As String)

            If (str.LastIndexOf(vbLf) <> str.Length - 1) Then
                str = str & vbCrLf
            End If

            If (stream IsNot Nothing) Then
                Try
                    stream.BeginWrite(ASCII.GetBytes(str), 0, str.Length, AddressOf sendMessageDone, Me)
                Catch ex As Exception

                End Try
            End If

        End Sub

        '
        '   sendMessageDone - called when the Async send finishes
        '   (does nothing right now)
        '
        Private Sub sendMessageDone(ByVal ar As IAsyncResult)

        End Sub


        '
        '   HasMessage
        '
        '   Function to tell how many (incoming) messages are waiting for us
        '
        Public Function HasMessage() As Boolean
            Return (fullLines.Count() > 0)
        End Function

        '
        '   GetMessage
        '
        '   Remove the first message in the queue.  Returns an empty string if there are
        '   no more messages
        '
        Public Function GetMessage() As String
            Dim str As String = String.Empty

            If (fullLines.Count() > 0) Then
                str = fullLines.Dequeue()
            End If

            Return str
        End Function
    End Class

    '
    '   myListener - (used only in server mode) Contains the port ready for clients
    '
    Private myListener As TcpListener = Nothing

    '
    '   connections - All of the "oneConnections" in use
    '
    Private connections(origArraySize) As oneConnection

    '
    '   myStatus - an internal/external debug message for this module
    '               (use foo.status() to retrieve the status)
    '
    Private myStatus As String = "Idle"

    '
    '   serverRunning - True/False
    '
    '       Indicate if the server is running and ready for connections
    '
    ReadOnly Property serverRunning() As Boolean
        Get
            Return (myListener IsNot Nothing)
        End Get
    End Property


    '
    '   numConnections
    '
    '   Return the number of connections to other systems
    '   (most useful for server applications)
    '
    ReadOnly Property numConnections() As Integer
        Get
            Return connectionCount
        End Get
    End Property

    '
    '   SendNewMessageEvent
    '
    '       Just a small "helper" function (because oneConnection can't do it alone)
    '
    Private Sub SendNewMessageEvent(ByVal conn As Integer)
        If (useInvoke) Then
            Raise(newTCPMessageEvent, conn)
        Else
            RaiseEvent newTCPMessage(conn)
        End If

    End Sub

    '
    '   Class constructor...
    '
    '       Use Invokes by default (for cross-thread uses)
    '       (or use the more simple RaiseEvent otherwise)
    '
    Public Sub New(Optional ByVal invokeable As Boolean = True)
        useInvoke = invokeable

        oneConnection.parent = Me
    End Sub

    Private Function MakeClientID() As Integer
        Dim oldMax As Integer
        '
        '   Do we need to make room?  If so, then double the amount of space
        '
        If (connectionCount >= currentArraySize) Then
            oldMax = currentArraySize
            currentArraySize = currentArraySize * 2

            ReDim Preserve connections(currentArraySize)

            '
            '   All is great, except now the rollover aka "2 mod 2" is in the wrong place so fix it
            '
            connections(oldMax) = connections(0)
            connections(0) = Nothing

        End If

        While (connections(nextClientID Mod currentArraySize) IsNot Nothing)
            nextClientID = nextClientID + 1
        End While

        Return nextClientID
    End Function


    Private Sub AddClient(ByVal clientid As Integer, ByRef newOne As oneConnection)
        Dim loc As Integer

        loc = clientid Mod currentArraySize
        If (connections(loc) Is Nothing) Then
            connections(loc) = newOne

            connectionCount = connectionCount + 1
        Else
            MsgBox("SimpleTCPNetwork - AddClient!  Already someone there!")
        End If

    End Sub


    Private Sub DelClient(ByVal clientid As Integer)
        Dim loc As Integer

        loc = clientid Mod currentArraySize

        If (connections(loc) IsNot Nothing) Then
            connections(loc) = Nothing
            connectionCount = connectionCount - 1
        End If

        If (connectionCount < 0) Then
            connectionCount = 0     ' Yes this is would be an error, but let's try to continue
            ' even if this happens
        End If

    End Sub

    Public Function listAllSystems(Optional ByVal withPort As Boolean = True) As String
        Dim s As String = ""
        Dim oneName As String
        Dim i As Integer

        For i = 0 To currentArraySize
            If (connections(i) IsNot Nothing) Then
                oneName = connections(i).name
                If (Not withPort) Then

                    '
                    '  Remove the port info if requested
                    '
                    If (oneName.Contains(":")) Then
                        oneName = oneName.Substring(0, oneName.LastIndexOf(":"))
                    End If
                End If

                s &= oneName & vbCrLf
            End If
        Next i


        Return s
    End Function

    Private Function FindClient(ByVal clientid As Integer, ByRef thisGuy As oneConnection) As Boolean
        Dim foundHim As Boolean = False
        Dim loc As Integer

        loc = clientid Mod currentArraySize
        '
        '   Check only if the slot is valid
        '
        If (connections(loc) IsNot Nothing) Then
            '
            '   Make sure that the id's are the same (could be an old connection)
            '
            If (connections(loc).id = clientid) Then
                thisGuy = connections(loc)
                foundHim = True
            End If
        End If

        Return foundHim
    End Function

    '
    '   SendClosedEvent
    '
    '       Just a small "helper" function (because oneConnection can't do it alone)
    '
    Private Sub SendClosedEvent(ByVal conn As Integer)
        If (useInvoke) Then
            Raise(closedTCPConnectionEvent, conn)
        Else
            RaiseEvent closedTCPConnection(conn)
        End If


    End Sub

    '
    '   StartServer
    '
    '       Start listening for clients to connect to a specific port
    '       (port is 8080 by default)
    '
    Public Sub StartServer(Optional ByVal port As Integer = 8080)
        If (myListener Is Nothing) Then
            myListener = New TcpListener(IPAddress.Any, port)
            Try
                myListener.Start()
                myListener.BeginAcceptTcpClient(AddressOf newConnection, myListener)
                myStatus = "Ready"
            Catch ex As Exception
                myListener = Nothing
                myStatus = "Not Ready - Could not start"
            End Try
        End If
    End Sub

    '
    '       StopServer
    '
    '       Stop listening for new connections

    Public Sub StopServer()
        If (myListener IsNot Nothing) Then
            myListener.Stop()
            myListener = Nothing
            myStatus = "Listener is now shutdown"
        End If
    End Sub

    '
    '
    '       StartClient
    '
    '       Connect to a server, given it's hostname and port
    '       (defaults to connecting on the server on the same system)
    '
    '       Technically, the system can be a client and a server at the same time
    '
    '
    Public Function StartClient(Optional ByVal host As String = "localhost", _
                                  Optional ByVal port As Integer = 8080) As Boolean
        Dim ok As Boolean = True
        Dim newConn As New TcpClient
        Dim newClient As oneConnection
        Dim clientId As Integer

        Try
            newConn.Connect(host, port)
        Catch ex As Exception
            ok = False
        End Try

        If (ok) Then
            ok = newConn.Connected()
        End If

        If (ok) Then
            clientId = MakeClientID()
            newClient = New oneConnection(newConn, clientId)

            AddClient(clientId, newClient)

            If (useInvoke) Then
                Raise(newTCPConnectionEvent, newClient.id)
            Else
                RaiseEvent newTCPConnection(newClient.id)
            End If

            myStatus = "Connection completed, now have " & connectionCount.ToString
        Else
            myStatus = "Connection attempt failed"
        End If

        Return ok
    End Function


    '
    '   newConnection
    '
    '       This routine is called when when a new connection completes
    '       (either from StartClient or when a connection arrives from the server port)
    '
    Private Sub newConnection(ByVal ar As IAsyncResult)
        Dim newClient As oneConnection
        Dim clientID As Integer

        If (myListener IsNot Nothing) Then

            clientID = MakeClientID()
            Try
                newClient = New oneConnection(myListener.EndAcceptTcpClient(ar), clientID)

                '
                '   Reset for the next connection
                '
                myListener.BeginAcceptTcpClient(AddressOf newConnection, myListener)

                '
                '   Ready for incoming data
                '
                AddClient(clientID, newClient)

                If (useInvoke) Then
                    Raise(newTCPConnectionEvent, newClient.id)
                Else
                    RaiseEvent newTCPConnection(newClient.id)
                End If

                myStatus = "Got connection, now have " & connectionCount.ToString
            Catch ex As Exception
                myStatus = "NewConnection failed"
            End Try

        End If
    End Sub

    '
    '      SendMessageToAll
    '
    '       Send a single string to all remote systems
    '
    Public Sub SendMessageToAll(ByVal str As String)
        Dim i As Integer

        For i = 0 To currentArraySize - 1
            If (connections(i) IsNot Nothing) Then
                connections(i).SendMessage(str)
            End If
        Next i

    End Sub

    '
    '   SendMessage
    '
    '   Send a single string to all remote systems
    '
    Public Sub SendMessage(ByVal str As String)
        SendMessageToAll(str)
    End Sub

    '
    '     SendMessageToAllExcept
    '
    '       Send a message to all other systems except one specific one
    '       (say like for a repeater application)
    '
    '
    Public Sub SendMessageToAllExcept(ByVal str As String, ByVal who As Integer)
        Dim i As Integer

        For i = 0 To currentArraySize - 1
            If (connections(i) IsNot Nothing) Then
                If (who Mod currentArraySize <> i) Then
                    connections(i).SendMessage(str)
                End If
            End If
        Next i

    End Sub

    '
    '       SendMessageTo
    '
    '       Send a string to a specific connection (given by an id)
    '
    Public Sub SendMessageTo(ByVal str As String, ByVal who As Integer)
        Dim conn As oneConnection = Nothing

        If (FindClient(who, conn)) Then
            conn.SendMessage(str)
        End If

        conn = Nothing
    End Sub


    '
    '       CloseConnection(id)
    '
    '       Given an id, close that specific connection
    '
    '
    Public Sub CloseConnection(ByVal who As Integer)
        Dim conn As oneConnection = Nothing

        If (FindClient(who, conn)) Then
            conn.Shutdown()
            conn = Nothing
            DelClient(who)
            myStatus = "Connection closed, now have " & connectionCount.ToString
        End If

    End Sub

    '
    '       CloseConnection()
    '
    '       If passed with no arguments, close ALL of the connections
    '       (name is "closeConnection" to make sense for client mode)
    '
    Public Sub CloseConnection()
        Dim i As Integer

        For i = 0 To currentArraySize
            If (connections(i) IsNot Nothing) Then
                connections(i).Shutdown()
                connections(i) = Nothing
            End If
        Next i

        connectionCount = 0

        myStatus = "Closed all connections"

    End Sub

    '
    '   StopClient
    '
    '       Just a simple "alias" to CloseConnection 
    '       (since StartClient is used at the beginning)
    '
    Public Sub StopClient()
        CloseConnection()
    End Sub

    '
    '       GetMessage(id)
    '
    '       Retreive the next message in the queue for a specific connection

    Public Function GetMessage(ByVal who As Integer) As String
        Dim conn As oneConnection = Nothing
        Dim str As String = String.Empty

        If (FindClient(who, conn)) Then
            str = conn.GetMessage()
        End If

        conn = Nothing

        Return str
    End Function


    '
    '       GetMessage()
    '
    '       Get the next message from any client
    '       (note: always searches from the first client, so you could starve the later ones)
    '
    '       (this function is more for client mode than server
    '
    Public Function GetMessage() As String
        Dim i As Integer = 0
        Dim str As String = String.Empty

        While ((i < currentArraySize) And (str = String.Empty))
            If (connections(i) IsNot Nothing) Then
                str = connections(i).GetMessage()
            End If

            i = i + 1
        End While

        Return str
    End Function

    '
    '       GetMessage(name)
    '
    '       Get the next message from any client and return the name of client
    '       at the same time
    '
    '       (note: always searches from the first client, so you could starve the later ones)
    '
    Public Function GetMessage(ByRef name As String) As String
        Dim i As Integer = 0
        Dim str As String = String.Empty

        While ((i < currentArraySize) And (str = String.Empty))
            If (connections(i) IsNot Nothing) Then
                str = connections(i).GetMessage()

                ' Did we get something from this person?
                If (str <> String.Empty) Then
                    name = connections(i).name
                End If

            End If

            i = i + 1
        End While

        Return str
    End Function

    '
    '       GetMessage(str,id)
    '
    '       Get the next message from any client and return the id of client
    '       at the same time
    '
    '       (note: always searches from the first client, so you could starve the later ones)
    '
    Public Sub GetMessage(ByRef str As String, ByRef who As Integer)
        Dim i As Integer = 0

        str = String.Empty

        While ((i < currentArraySize) And (str = String.Empty))
            If (connections(i) IsNot Nothing) Then
                str = connections(i).GetMessage()

                If (str <> String.Empty) Then
                    who = connections(i).id
                End If
            End If

            i = i + 1
        End While
    End Sub


    '
    '   GetMessageWithReturn
    '
    '   Get the next message in the queue.   If one exists (and it is not blank),
    '   the return it with a Carriage Return (CR) and a Line Feed (LF) appended 
    '   to it.
    '

    Public Function GetMessageWithReturn() As String
        Dim s As String

        s = GetMessage()

        '
        '   Not empty?  Then tack on a CR and a LF
        '
        If (s <> "") Then
            s = s & vbCrLf
        End If

        Return s
    End Function

    '
    '       Name(id)
    '
    '       Return a string representing the name of the client
    '       (Returns an empty string if the id is invalid)
    '
    Public Function Name(ByVal who As Integer) As String
        Dim conn As oneConnection = Nothing
        Dim str As String = String.Empty

        If (FindClient(who, conn)) Then
            str = conn.name()
        End If

        conn = Nothing

        Return str
    End Function

    '
    '       HasMessages(id)
    '
    '       Returns true if there are incoming messages waiting to be retreived for
    '       the connection with "id"
    '
    '       (Returns false if id is not valid)
    '
    Public Function HasMessages(ByVal who As Integer) As Boolean
        Dim res As Boolean = False
        Dim conn As oneConnection = Nothing

        If (FindClient(who, conn)) Then
            res = conn.HasMessage()
        End If

        conn = Nothing

        Return res
    End Function


    '
    '       HasMessages()
    '
    '       Indicate if ANY connetion has a message that needs read
    '
    '
    Public Function HasMessages() As Boolean
        Dim res As Boolean = False
        Dim i As Integer = 0


        While ((i < currentArraySize) And (Not res))
            If (connections(i) IsNot Nothing) Then
                res = connections(i).HasMessage()
            End If

            i = i + 1
        End While

        Return res
    End Function



    '
    '       myipaddr(#)
    '
    '       Given a number, return a string representing the corresponding ip address
    '
    Public Shared Function myipaddr(ByVal entrynum As Integer) As String
        Dim addr As String = ""
        Dim ihe As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName())

        If ((entrynum >= 0) And (entrynum < ihe.AddressList.Length)) Then
            addr = ihe.AddressList(entrynum).ToString()
        End If

        Return addr
    End Function

    '
    '       myipaddr()
    '
    '       Return a string representing (one of) my ip address
    '
    '       Make sure that it is an IP address (and not IPv6 or some other proto)
    '
    Public Shared Function myipaddr() As String
        Dim ihe As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName())
        Dim str As String = String.Empty
        Dim i As Integer

        i = 0

        While ((i < ihe.AddressList.Length) And (str = String.Empty))
            If (ihe.AddressList(i).AddressFamily = AddressFamily.InterNetwork) Then
                str = ihe.AddressList(i).ToString()
            End If

            i = i + 1
        End While

        Return str
    End Function


    '
    '       allmyipaddrs()
    '
    '       Return a string representing (one of) my ip address
    '
    '       Make sure that it is an IP address (and not IPv6 or some other proto)
    '
    Public Shared Function allmyipaddrs() As String
        Dim ihe As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName())
        Dim str As String = ""
        Dim i As Integer
        i = 0

        While ((i < ihe.AddressList.Length) And (str = String.Empty))
            If (ihe.AddressList(i).AddressFamily = AddressFamily.InterNetwork) Then
                str &= ihe.AddressList(i).ToString() & vbCrLf
            End If

            i = i + 1
        End While

        Return str
    End Function


    '
    '       mydnsname
    '
    '       Return a string representing the name of my system by using DNS
    '
    Public Shared Function mydnsname() As String
        Return Dns.GetHostName()
    End Function

    '
    '  Code to remove the requirement of handling cross-thread events within the main app
    '  Taken from http://www.dreamincode.net/code/snippet5016.htm
    '
    Private Shared Sub Raise(ByVal evt As System.Delegate, ByVal num As Integer)
        Dim data(0) As Object

        If evt IsNot Nothing Then

            data(0) = num

            For Each C As System.Delegate In evt.GetInvocationList

                Try
                    Dim T As System.ComponentModel.ISynchronizeInvoke = CType(C.Target, System.ComponentModel.ISynchronizeInvoke)

                    If T IsNot Nothing AndAlso T.InvokeRequired Then
                        T.BeginInvoke(C, data)
                    Else
                        C.DynamicInvoke(data)
                    End If
                Catch Ex As Exception

                    'TODO : Should report the error to someone, somehow...

                End Try

            Next
        End If
    End Sub


End Class

