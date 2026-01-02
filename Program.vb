Imports System
Imports System.IO
Imports System.Net.Http
Imports Newtonsoft.Json

' Local classes
Imports SigningExample
Imports JsonExample
Imports HttpExample
Imports Salesforce

Module Program
  Sub Main(args As String())
    Console.WriteLine("Starting...")
    SalesforceCompositeExample()

    ' Console.WriteLine($"PdfToBase64(): {PdfToBase64()}")
    ' BuildCompositeJson
    ' SalesforceQueryExample()
  End Sub

  Private Function PdfToBase64()
    Console.WriteLine("Calling ProcessPdf")
    Dim filePath = Path.Combine("Resources", "sample.pdf")

    If Not File.Exists(filePath)
      Throw New Exception("No PDF file")
    End If

    Dim pdfSample = File.ReadAllBytes(filePath)
    Dim base64 = Convert.ToBase64String(pdfSample)

    ' Console.WriteLine($"pdfSample: {pdfSample.Length}: {base64}")
    Return base64
  End Function

  Private Sub BuildCompositeJson
    Console.WriteLine("Calling BuildCompositeJson")

    Dim json = GenerateJson

    Console.WriteLine($"json: {json}")
  End Sub

  Private Function GenerateJson As String
    Dim obj = New With {
      .allOrNone = false,
      .compositeRequest = New Object() {
        New With {
          .method = "POST",
          .url = "/services/data/v65.0/sobjects/Contact",
          .referenceId = "newContact",
          .body = New With {
            .LastName = "LastName",
            .FirstName = "FirstName",
            .Email = "Email@none.com"
          }
        },
        New With {
          .method = "POST",
          .url = "/services/data/v65.0/sobjects/ContentVersion",
          .referenceId = "newFile",
          .body = New With {
            .Title = "Title of file PDF sample file",
            .PathOnClient = "test-sample.pdf",
            .VersionData = PdfToBase64()
          }
        },
        New With {
          .method = "GET",
          .url = "/services/data/v65.0/sobjects/ContentVersion/@{newFile.id}?fields=ContentDocumentId",
          .referenceId = "getContentDoc"
        },
        New With {
          .method = "POST",
          .url = "/services/data/v65.0/sobjects/ContentDocumentLink",
          .referenceId = "linkFile",
          .body = New With {
            .ContentDocumentId = "@{getContentDoc.ContentDocumentId}",
            .LinkedEntityId = "@{newContact.id}",
            .ShareType = "V",
            .Visibility = "ALlUsers"
          }
        }
      }
    }

    Return JsonConvert.SerializeObject(obj)
  End Function

  ' Private Function SalesforceClientSetup As SalesforceClient
  '   Dim signClass = New SigningExample.SigningClass()

  '   Using rsa = signClass.BuildRsaObject()
  '     Dim byteData = signClass.SignData("testData", rsa)
  '     Dim signatureBase64 = Convert.ToBase64String(byteData)
  '     Console.WriteLine($"Signature: {signatureBase64}")
  '   End Using

  '   Return New SalesforceClient()
  ' End Function

  Private Sub SalesforceCompositeExample
    Dim sfClient = New Salesforce.SalesforceClient()
    Dim authResponse = sfClient.CallHttpJwtAccessTokenRequest().Result
    Dim authTask = sfClient.PostComposite(GenerateJson, authResponse.access_token)
    Dim compositeResponse = authTask.Result
    Console.WriteLine($"OUTPUT: {compositeResponse}")
  End Sub

  ' Private Sub SalesforceQueryExample()
  '   Dim sfClient = SalesforceClientSetup()
  '   Dim authTask = sfClient.CallHttpJwtAccessTokenRequest()
  '   Dim authResponse = authTask.Result
  '   Console.WriteLine($"OUTPUT:")
  '   Console.WriteLine($"Access Token: {authResponse.access_token}")
  '   Console.WriteLine($"Instance URL: {authResponse.instance_url}")

  '   Dim jsonResponse = sfClient.ApiQuery("SELECT Id, Name FROM Account", authResponse.access_token).Result
  '   Console.WriteLine($"OUTPUT Accounts: {jsonResponse}")
  '   ' PostPDFExampleAsync(args).GetAwaiter().GetResult()
  ' End Sub
End Module
