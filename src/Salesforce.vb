Imports System
Imports System.IO
Imports System.Net.Http
Imports System.Collections.Generic
Imports System.Text.Json
Imports System.Threading.Tasks
Imports System.IdentityModel.Tokens.Jwt
Imports Microsoft.IdentityModel.Tokens
Imports System.Security.Claims

Namespace Salesforce

  Public Class SalesforceTokenResponse
    Public Property access_token As String
    Public Property instance_url As String
    Public Property id As String
    Public Property token_type As String
    Public Property issued_at As String
    Public Property signature As String
  End Class

  Public Class SalesforceClient
    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
      _httpClient = New HttpClient()
    End Sub

    Public Sub New(httpClient As HttpClient)
      _httpClient = httpClient
    End Sub

    Public Const SalesforceHost As String = "https://orgfarm-8c12c2c9c7-dev-ed.develop.my.salesforce-setup.com"

    Public Async Function CallHttpJwtAccessTokenRequest() As Task(Of SalesforceTokenResponse)
      Dim responseBody = New Dictionary(Of String, String) From {
        {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
        {"assertion", GenerateJWTToken()}
      }

      Dim formResponseBody = New FormUrlEncodedContent(responseBody)
      Dim oauthUrl = $"{SalesforceHost}/services/oauth2/token"

      Dim response As HttpResponseMessage = Await _httpClient.PostAsync(oauthUrl, formResponseBody)
      Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()

        If response.IsSuccessStatusCode Then
          Dim options As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
          Return JsonSerializer.Deserialize(Of SalesforceTokenResponse)(jsonResponse, options)
        Else
          Throw New Exception($"Salesforce JWT authentication failed: {response.StatusCode} - {jsonResponse}")
        End If
    End Function

    Public Async Function ApiQuery(query As String, accessToken As String) As Task(Of String)
      Dim encodedSoql = Uri.EscapeDataString(query)

      Dim oauthUrl = $"{SalesforceHost}/services/data/v65.0/query?q={encodedSoql}"

      _httpClient.DefaultRequestHeaders.Authorization = New System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken)
      Dim response As HttpResponseMessage = Await _httpClient.GetAsync(oauthUrl)
      Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()

        If response.IsSuccessStatusCode Then
          Dim options = New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
          ' Return JsonSerializer.Deserialize(Of SalesforceAccountList)(jsonResponse, options)
          Return jsonResponse
        Else
          Throw New Exception($"Salesforce request failed: {response.StatusCode} - {jsonResponse}")
        End If
    End Function

    Public Async Function PostComposite(body As String, accessToken As String) As Task(Of String)
      Dim oauthUrl = $"{SalesforceHost}/services/data/v65.0/composite"
      _httpClient.DefaultRequestHeaders.Authorization = New System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken)
      Dim content = New StringContent(body, System.Text.Encoding.UTF8, "application/json")
      Dim response As HttpResponseMessage = Await _httpClient.PostAsync(oauthUrl, content)
      Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()

      If response.IsSuccessStatusCode Then
        Dim options = New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
        ' Return JsonSerializer.Deserialize(Of SalesforceAccountList)(jsonResponse, options)
        Return jsonResponse
      Else
        Throw New Exception($"Salesforce request failed: {response.StatusCode} - {jsonResponse}")
      End If
    End Function

    Public Function GenerateJWTToken() As String
      Dim tokenHandler = New JwtSecurityTokenHandler()

      Dim signing = New SigningExample.SigningClass()
      Dim rsa     = signing.BuildRsaObject()

      Dim tokenDescriptor = New SecurityTokenDescriptor With {
        .Issuer   = "3MVG97L7PWbPq6UxO4S6IDmhmax8RDM8MQ_bELeZTNhnAX2iE_ZmWxAZuc_oM6yypNrGMmEWa9zHpmEp.9nJq",
        .Audience = "https://test.salesforce.com",
        .Subject            = New ClaimsIdentity(New Claim() {
          New Claim("sub", "agent+salesforce.devloper547@agentforce.com")
        }),
        .Expires            = DateTime.UtcNow.AddHours(1),
        .SigningCredentials = New SigningCredentials(New RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
      }

      Dim token     = tokenHandler.CreateToken(tokenDescriptor)
      Dim jwtString = tokenHandler.WriteToken(token)

      ' Use https://www.jwt.io/ to validate
      Console.WriteLine($"JWT: {jwtString}")

      Return jwtString
    End Function

  End Class

End Namespace