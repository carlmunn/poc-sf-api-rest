'
' 1. Salesforce.Force (most used .NET client)
' NuGet: Salesforce.Force
' Handles REST calls, auth flows (including JWT).
' Written in C# but straightforward to consume from VB.NET.
' Well-documented and maintained.
'
' https://github.com/wadewegner/Force.com-Toolkit-for-NET
' Newer but for .NET Core https://github.com/anthonyreilly/NetCoreForce
'
' Imports Salesforce.Common
' Imports Salesforce.Force

' Namespace Salesforce
'   Class SalesforceAPI
'     Sub Run()
'       Dim clientId = "YOUR_CLIENT_ID"
'       Dim privateKey = File.ReadAllText("Resources/private.key")
'       Dim username = "YOUR_SF_USERNAME"
'       Dim tokenReq = New JwtBearerTokenRequest(clientId, username, privateKey, "https://login.salesforce.com")
'       Dim auth = Await New AuthenticationClient().JwtLoginAsync(tokenReq)
'       Dim client = New ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion)
'     End Sub
'   End Class
' End Namespace

' Raw Composite request with PDF from FS
'
' Dim pdfBytes = File.ReadAllBytes("example.pdf")
' Dim pdfBase64 = Convert.ToBase64String(pdfBytes)
'
' Dim compositeRequest = New With {
'     .compositeRequest = New Object() {
'         New With {
'             .method = "POST",
'             .url = "/services/data/v60.0/sobjects/Contact",
'             .referenceId = "newContact",
'             .body = New With {
'                 .FirstName = "John",
'                 .LastName = "Smith",
'                 .Email = "john.smith@example.com"
'             }
'         },
'         New With {
'             .method = "POST",
'             .url = "/services/data/v60.0/sobjects/ContentVersion",
'             .referenceId = "newFile",
'             .body = New With {
'                 .Title = "ID Document",
'                 .PathOnClient = "id.pdf",
'                 .VersionData = pdfBase64
'             }
'         },
'         New With {
'             .method = "POST",
'             .url = "/services/data/v60.0/sobjects/ContentDocumentLink",
'             .referenceId = "fileLink",
'             .body = New With {
'                 .ContentDocumentId = "@{newFile.id}",
'                 .LinkedEntityId = "@{newContact.id}",
'                 .ShareType = "V"
'             }
'         }
'     }
' }
'
' Dim result = Await client.ExecuteCompositeAsync(Of Object)(compositeRequest)