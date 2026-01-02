Imports System
Imports System.IO
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Text

Module PostPdfExample
    Sub Main()
        ' Run the async method and wait for completion
        PostPdfExamples().Wait()

        Console.WriteLine(vbCrLf & "Press any key to exit...")
        Console.ReadKey()
    End Sub

    Async Function PostPdfExamples() As Task
        Console.WriteLine("=== PDF Posting Examples ===")
        Console.WriteLine()

        ' Example 1: Post PDF as raw binary data
        Await Example1_PostRawBinary()
        Console.WriteLine()

        ' Example 2: Post PDF as multipart/form-data (like a form upload)
        Await Example2_PostMultipartFormData()
        Console.WriteLine()

        ' Example 3: Post PDF as Base64 JSON payload
        Await Example3_PostAsBase64Json()
    End Function

    ' Example 1: POST PDF as raw binary content
    Async Function Example1_PostRawBinary() As Task
        Console.WriteLine("--- Example 1: Raw Binary POST ---")

        Try
            ' Read PDF file into memory (or create PDF bytes in memory)
            Dim pdfBytes As Byte() = File.ReadAllBytes("example.pdf")
            ' Or generate PDF in memory: Dim pdfBytes As Byte() = GeneratePdfInMemory()

            Using client As New HttpClient()
                ' Create binary content from bytes in memory
                Using content As New ByteArrayContent(pdfBytes)
                    ' Set content type for PDF
                    content.Headers.ContentType = New System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf")

                    ' POST to endpoint
                    Dim response As HttpResponseMessage = Await client.PostAsync(
                        "https://httpbin.org/post", content)

                    Console.WriteLine($"Status: {response.StatusCode}")
                    Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                    Console.WriteLine($"Response preview: {responseBody.Substring(0, Math.Min(200, responseBody.Length))}...")
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
        End Try
    End Function

    ' Example 2: POST PDF as multipart/form-data (like HTML form file upload)
    Async Function Example2_PostMultipartFormData() As Task
        Console.WriteLine("--- Example 2: Multipart Form Data POST ---")

        Try
            ' PDF bytes in memory
            Dim pdfBytes As Byte() = CreateSamplePdfBytes()

            Using client As New HttpClient()
                Using formData As New MultipartFormDataContent()
                    ' Add PDF file from memory
                    Dim fileContent As New ByteArrayContent(pdfBytes)
                    fileContent.Headers.ContentType = New System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf")
                    formData.Add(fileContent, "file", "document.pdf")

                    ' Add additional form fields
                    formData.Add(New StringContent("John Doe"), "username")
                    formData.Add(New StringContent("Important document"), "description")

                    ' POST to endpoint
                    Dim response As HttpResponseMessage = Await client.PostAsync(
                        "https://httpbin.org/post", formData)

                    Console.WriteLine($"Status: {response.StatusCode}")
                    Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                    Console.WriteLine($"Response preview: {responseBody.Substring(0, Math.Min(200, responseBody.Length))}...")
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
        End Try
    End Function

    ' Example 3: POST PDF as Base64-encoded JSON payload
    Async Function Example3_PostAsBase64Json() As Task
        Console.WriteLine("--- Example 3: Base64 JSON POST ---")

        Try
            ' PDF bytes in memory
            Dim pdfBytes As Byte() = CreateSamplePdfBytes()

            ' Convert to Base64
            Dim base64Pdf As String = Convert.ToBase64String(pdfBytes)

            ' Create JSON payload
            Dim jsonPayload As String = $"{{""filename"":""document.pdf"",""content"":""{base64Pdf}"",""contentType"":""application/pdf""}}"

            Using client As New HttpClient()
                ' Create JSON content
                Using content As New StringContent(jsonPayload, Encoding.UTF8, "application/json")
                    ' POST to endpoint
                    Dim response As HttpResponseMessage = Await client.PostAsync(
                        "https://httpbin.org/post", content)

                    Console.WriteLine($"Status: {response.StatusCode}")
                    Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                    Console.WriteLine($"Response preview: {responseBody.Substring(0, Math.Min(200, responseBody.Length))}...")
                End Using
            End Using

        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
        End Try
    End Function

    ' Helper function to create sample PDF bytes in memory
    Function CreateSamplePdfBytes() As Byte()
        ' This creates a minimal valid PDF structure in memory
        ' In real scenarios, you'd use a PDF library or read from file
        Dim pdfContent As String = "%PDF-1.4" & vbLf &
            "1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj" & vbLf &
            "2 0 obj<</Type/Pages/Count 1/Kids[3 0 R]>>endobj" & vbLf &
            "3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]>>endobj" & vbLf &
            "xref" & vbLf & "0 4" & vbLf &
            "0000000000 65535 f" & vbLf &
            "0000000009 00000 n" & vbLf &
            "0000000056 00000 n" & vbLf &
            "0000000110 00000 n" & vbLf &
            "trailer<</Size 4/Root 1 0 R>>" & vbLf &
            "startxref" & vbLf & "185" & vbLf & "%%EOF"

        Return Encoding.ASCII.GetBytes(pdfContent)
    End Function
End Module