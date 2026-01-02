Imports System
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports System.Text.Encoding
Imports System.IO
Imports System.Collections.Generic

Namespace JsonExample

  Public Class JsonExample
    Public Class PersonObject
      Public Property Name As String = "Unknown"
      Public Property Age As Integer = 1
      Public Property Enabled As Boolean = True
    End Class

    Public Class CategoryObject
      Public Property Type As String = "Default"

      Public Property Id As Integer
      Public Property Records() As List(Of PersonObject) = New List(Of PersonObject)()

      Public Sub New()
        Records.Add(New PersonObject())
      End Sub

      Private Property CategoryName As String
        Get
          Return "CategoryName"
        End Get
        Set(val As String)

        End Set

      End Property
    End Class

    Public Function Generate() As String
      Dim jsonObject = new CategoryObject With {.Id = 10}
      Return JsonSerializer.Serialize(jsonObject)
    End Function

    Public Sub JsonToBase64Example()
      'Dim strBytes() as Byte = UTF8.GetBytes("AAA")
      'Dim strObject = UTF8.GetString(strBytes)
      'Dim data = New ByteArrayContent(strBytes)
      'Dim val = 123.10!
      Dim strObject = new JsonExample().Generate()
      Console.WriteLine($"Hello World, testing [{strObject}]")


      Dim base64ObjectBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(strObject)
      Dim base64Object As String = System.Convert.ToBase64String(base64ObjectBytes)
      Console.WriteLine($"Base64:{base64Object}")
    End Sub
  End Class
End Namespace
