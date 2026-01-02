Imports System
Imports System.IO
Imports System.Security.Cryptography

Namespace SigningExample
  Public Class SigningClass

    Function BuildRsaObject() As RSA
      Dim keyPath As String = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "Resources/private.key"
      )

      If Not File.Exists(keyPath)
        Throw new Exception($"private.key does not exist ({keyPath})")
      End If

      Dim pemContent = File.ReadAllText(keyPath)

      Dim base64 = pemContent _
        .Replace("-----BEGIN PRIVATE KEY-----", "") _
        .Replace("-----END PRIVATE KEY-----", "") _
        .Replace("-----BEGIN RSA PRIVATE KEY-----", "") _
        .Replace("-----END RSA PRIVATE KEY-----", "") _
        .Replace(vbCr, "").Replace(vbLf, "").Trim()

      Dim privateKeyBytes = Convert.FromBase64String(base64)

      Dim rsa As RSA = RSA.Create()

      Dim bytesRead As Integer

      Console.WriteLine("Importing...")
      Try
        ' Try importing as PKCS#8 first
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, bytesRead)
      Catch ex As Exception
        ' Fall back to PKCS#1 for RSA keys
        Console.WriteLine("PKCS8 Failed, trying RSA instead")
        rsa.ImportRSAPrivateKey(privateKeyBytes, bytesRead)
      End Try

      Return rsa
    End Function

    Function SignData(data As String, rsa As RSA) As Byte()
      Dim dataBytes = System.Text.Encoding.UTF8.GetBytes(data)
      Return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
    End Function

    Function GetFileContent(Optional pemFilePath As String = "Resources/private.key") As Byte()
      Dim pemContent As String = File.ReadAllText(pemFilePath)

      ' Find the base64 content between headers
      Dim base64Content As String
      If pemContent.Contains("-----BEGIN PRIVATE KEY-----") Then
          ' PKCS#8 format
          base64Content = pemContent _
              .Replace("-----BEGIN PRIVATE KEY-----", "") _
              .Replace("-----END PRIVATE KEY-----", "") _
              .Replace(vbCr, "").Replace(vbLf, "").Trim()
      ElseIf pemContent.Contains("-----BEGIN RSA PRIVATE KEY-----") Then
          ' PKCS#1 format
          base64Content = pemContent _
              .Replace("-----BEGIN RSA PRIVATE KEY-----", "") _
              .Replace("-----END RSA PRIVATE KEY-----", "") _
              .Replace(vbCr, "").Replace(vbLf, "").Trim()
      ElseIf pemContent.Contains("-----BEGIN CERTIFICATE-----") Then
          ' Certificate format
          base64Content = pemContent _
              .Replace("-----BEGIN CERTIFICATE-----", "") _
              .Replace("-----END CERTIFICATE-----", "") _
              .Replace(vbCr, "").Replace(vbLf, "").Trim()
      Else
          Throw New ArgumentException("Invalid PEM format")
      End If
      Console.WriteLine($"{vbLf}Base64 Key: {base64Content}{vbLf}")
      Dim certBytes As Byte() = Convert.FromBase64String(base64Content)

      Return certBytes
    End Function
  End Class
End Namespace