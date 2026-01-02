Imports System
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Text.Json
Imports System.IO

Public Class HttpExample
  ' Todo item class to deserialize JSON
  Public Class TodoItem
  Public Property id As Integer
  Public Property title As String
  Public Property completed As Boolean
  Public Property userId As Integer
  End Class

  Sub Start()
    ' Run the async method and wait for completion
    FetchTodos().Wait()

    Console.WriteLine(vbCrLf & "Press any key to exit...")
    Console.ReadKey()
  End Sub

  Async Function FetchTodos() As Task
    Try
      ' Create HTTP client
      Using client As New HttpClient()
        ' Set base address and headers
        client.BaseAddress = New Uri("https://jsonplaceholder.typicode.com/")
        client.DefaultRequestHeaders.Accept.Clear()
        client.DefaultRequestHeaders.Accept.Add(
          New System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))

        Console.WriteLine("Fetching todos from API...")
        Console.WriteLine("URL: https://jsonplaceholder.typicode.com/todos")
        Console.WriteLine()

        ' Make GET request
        Dim response As HttpResponseMessage = Await client.GetAsync("todos")

        If response.IsSuccessStatusCode Then
          ' Read response content
          Dim jsonContent As String = Await response.Content.ReadAsStringAsync()

          ' Deserialize JSON using System.Text.Json
          Dim options As New JsonSerializerOptions With {
              .PropertyNameCaseInsensitive = True
          }
          Dim todos As TodoItem() = JsonSerializer.Deserialize(Of TodoItem())(jsonContent, options)

          Console.WriteLine($"Successfully retrieved {todos.Length} todos")
          Console.WriteLine(New String("-"c, 60))

          ' Display first 5 todos as example
          For i As Integer = 0 To Math.Min(4, todos.Length - 1)
            Dim todo As TodoItem = todos(i)
            Console.WriteLine($"Todo #{todo.id}")
            Console.WriteLine($"  Title: {todo.title}")
            Console.WriteLine($"  Completed: {todo.completed}")
            Console.WriteLine($"  User ID: {todo.userId}")
            Console.WriteLine()
          Next

          If todos.Length > 5 Then
            Console.WriteLine($"... and {todos.Length - 5} more todos")
          End If
        Else
          Console.WriteLine($"Error: {response.StatusCode}")
          Console.WriteLine($"Reason: {response.ReasonPhrase}")
        End If
      End Using

    Catch ex As Exception
        Console.WriteLine($"An error occurred: {ex.Message}")
    End Try
  End Function

  Async Function PostPDFExampleAsync(args As String()) As Task
    Console.WriteLine("Starting...")

    Dim fileBytes() as Byte = File.ReadAllBytes("../Resources/small.pdf")
    Console.WriteLine($"DEBUG: fileBytes.Length: {fileBytes.Length}")

    Using httpClient As new HttpClient(),
      httpContent As new ByteArrayContent(fileBytes)
      httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf")

      Try
        Using response As HttpResponseMessage = Await httpClient.PostAsync("http://localhost:8000", httpContent)
          Console.WriteLine($"=> {response.StatusCode}")
        End Using
      Catch exp As Exception
        Console.WriteLine($"ERRORED!:{exp.Message}")
      End Try
    End Using
    ' TodoListClient.Start()
  End Function
End Class
