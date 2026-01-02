' Imports System.IO
' Imports Salesforce.Force

' Usage:
'
' Dim service = New SalesforceCompositeService(forceClient)
'
' Dim result = Await service.CreateContactWithPdfAsync(
'     "John",
'     "Smith",
'     "john.smith@example.com",
'     "C:\docs\id.pdf",
'     "Customer ID Document"
' )

' Public Class SalesforceCompositeService

'     Private ReadOnly _client As ForceClient
'     Private ReadOnly _apiVersion As String = "v65.0"

'     Public Sub New(client As ForceClient)
'         _client = client
'     End Sub

'     Public Async Function CreateContactWithPdfAsync(
'         firstName As String,
'         lastName As String,
'         email As String,
'         pdfPath As String,
'         pdfTitle As String
'     ) As Task(Of Object)

'         Dim pdfBytes = File.ReadAllBytes(pdfPath)
'         Dim pdfBase64 = Convert.ToBase64String(pdfBytes)

'         Dim payload = New With {
'             .compositeRequest = New Object() {
'                 New With {
'                     .method = "POST",
'                     .url = $"/services/data/{_apiVersion}/sobjects/Contact",
'                     .referenceId = "contactRef",
'                     .body = New With {
'                         .FirstName = firstName,
'                         .LastName = lastName,
'                         .Email = email
'                     }
'                 },
'                 New With {
'                     .method = "POST",
'                     .url = $"/services/data/{_apiVersion}/sobjects/ContentVersion",
'                     .referenceId = "fileRef",
'                     .body = New With {
'                         .Title = pdfTitle,
'                         .PathOnClient = Path.GetFileName(pdfPath),
'                         .VersionData = pdfBase64
'                     }
'                 },
'                 New With {
'                     .method = "POST",
'                     .url = $"/services/data/{_apiVersion}/sobjects/ContentDocumentLink",
'                     .referenceId = "linkRef",
'                     .body = New With {
'                         .ContentDocumentId = "@{fileRef.id}",
'                         .LinkedEntityId = "@{contactRef.id}",
'                         .ShareType = "V",
'                         .Visibility = "AllUsers"
'                     }
'                 }
'             }
'         }

'         Return Await _client.ExecuteCompositeAsync(Of Object)(payload)

'     End Function

' End Class