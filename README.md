# Simple VB.NET Salesforce API proof of concept

This POSTs using SF's composite API to create a new contact and attach a PDF to it

- No private key is present. This needs creating new and placed in Resources/
  `openssl genrsa ...` can do this
- Targetting .NET foundation, not the newer .NET Core
  Also needed to use Visual Basic, not C#
- Using Newtonsoft.JSON and BouncyCastle (encryption), which wouldn't be
  needed if .NET Core was used
- Not all the code is related to the POC. Left some debug there

## Features
- This code needs to run without any user interaction except the initial
  private key upload.
- Get a access token from Salesforce using the private key. Using JWT with RS256.
  SF doesn't support PS256, RS256 will do though.
- Using the received token from JSON to POST a JSON composite request to SF
  The composite is from SF's own API, it allows bundling of requests
- The PDF has restrictions. It can't be larger than 30MB, this is fine since the
  PDFs generated are sized around 200kb. The PDF is converted to Base64 and
  included into the composite request.