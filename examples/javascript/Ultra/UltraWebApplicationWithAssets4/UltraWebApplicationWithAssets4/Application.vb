Imports ScriptCoreLib.JavaScript.DOM.HTML
Imports ScriptCoreLib.JavaScript.Extensions
Imports ScriptCoreLib.JavaScript
Imports ScriptCoreLib.Ultra.Library
Imports ScriptCoreLib.Ultra.Library.Delegates
Imports System.Runtime.CompilerServices

Imports UltraWebApplicationWithAssets4.HTML.Pages

Public NotInheritable Class Application

    ' Note: This code will run as javascript

    Public Sub New(ByVal a As IHTMLPage1)

        Native.Document.title = "UltraWebApplicationWithAssets4"

        a.Button1.innerText = "Click to show time"

        AddHandler a.Button1.onclick,
            Sub()

                Dim x = <div>
                            <code style='color: blue;'>
                                <span>Client time:</span>
                                <span><%= "" & (DateTime.Now) %></span>
                            </code>
                        </div>.GetTime(
                            Sub(y As XElement)

                                a.Content.Add(a.PageShadowContainer.cloneNode(True))
                                a.Content.Add(y)

                            End Sub
                        )

            End Sub

        With a.Button2
            .disabled = True
            .innerText = "This button was disabled"
        End With


    End Sub

End Class

Public NotInheritable Class ApplicationWebService
    ' Methods
    Public Sub GetTime(ByVal x As XElement, ByVal y As XElementAction)
        ' every time client calls here 
        ' 1. we append to database 
        Using ctx As DataClasses1DataContext = New DataClasses1DataContext
            ctx.Table1s.InsertOnSubmit(New Table1 With { _
            .Text = x.Value _
        })
            ctx.SubmitChanges()
        End Using

        ' 2. and echo back.
        Dim history = <div></div>

        Using ctx As DataClasses1DataContext = New DataClasses1DataContext
            Dim item As Table1
            For Each item In ctx.Table1s
                history.Add(<div><code style='color: blue;'>history: </code><code><%= item.Text %></code></div>)
            Next
        End Using
  
        y(
            <div style='margin-left: 1em; border: 1px solid gray; margin: 1em;'>
                <div style='padding-left: 1em; border-left: 2em solid gray;'>
                    <code>
                        <span>Server time:</span>
                        <span><%= "" & (DateTime.Now) %></span>
                    </code>
                    <hr/>
                    <%= history %>
                </div>
            </div>
        )

    End Sub

End Class


Public Module ApplicationWebServiceModule
    'http://msdn.microsoft.com/en-us/library/bb384936.aspx
    Dim ws As New ApplicationWebService

    ''' <summary>
    ''' Client asks server for time.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <remarks></remarks>
    <Extension()>
    Public Function GetTime(ByVal x As XElement, ByVal y As XElementAction) As XElement

        ws.GetTime(x, y)

        Return x
    End Function

End Module