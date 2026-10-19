# CERSAI CKYCRR 2.0 – ASP.NET Core Client API

ASP.NET Core 9 Web API that integrates with **Central KYC Record Registry (CKYCRR 2.0)** provided by CERSAI.

- **mTLS** (client certificate) on every outbound call  
- **OAuth 2.0** client-credentials token management with in-memory cache  
- **Separate controllers / routes** for **Individual** and **Legal** entities  
- Encryption / decryption of PI data (stub included – replace with real JWE)

---

## Project structure

```
CersaiCkycApi/
├── Configuration/CersaiOptions.cs
├── Controllers/
│   ├── IndividualCkycController.cs   → /api/individual/*
│   ├── LegalCkycController.cs        → /api/legal/*
│   └── CommonCkycController.cs       → /api/ckyc/*
├── Models/
│   ├── Common/
│   ├── Search/
│   ├── Download/
│   └── Create/
├── Services/
│   ├── CkycClient.cs                 # Main HTTP client
│   ├── TokenService.cs
│   └── StubEncryptionService.cs      # REPLACE before production
└── Program.cs                        # mTLS HttpClient registration
```

---

## Configuration (`appsettings.json`)

```json
"Cersai": {
  "BaseUrl": "https://testbed.secureapi.ckycregistry.co.in",
  "ClientId": "...",
  "ClientSecret": "...",
  "ReId": "IN0106",
  "ClientCertificatePath": "certs/client.pfx",
  "ClientCertificatePassword": "",
  "EncryptionPublicKeyPath": "certs/cersai-public.pem",
  "DecryptionPrivateKeyPath": "certs/client-private.pem"
}
```

| Environment | Base URL |
|-------------|----------|
| Production  | `https://secureapi.ckycregistry.co.in` |
| Testbed     | `https://testbed.secureapi.ckycregistry.co.in` |

Place the client certificate (`.pfx`) supplied by CERSAI under `certs/`.

---

## API Endpoints

### Individual (`/api/individual`)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/search/{searchOption}` | Search (OVD / CKYC / DEMOGRAPHIC / PHOTO / MOBILE / VC) |
| POST | `/download/initiate` | Start download journey |
| POST | `/download/validate` | Consent (OTP / PHYSICAL / FACEAUTH) |
| POST | `/download/validate-otp` | Verify OTP |
| POST | `/download/resend-otp` | Resend OTP |
| POST | `/download/get` | Fetch full KYC record |
| POST | `/create` | Create new Individual record |
| POST | `/update` | Update existing record |

### Legal (`/api/legal`)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/search/{searchOption}` | Search (OVD / MOBILE / DOI / CKYC) |
| POST | `/download/initiate` | Start download (OTP auto-triggered) |
| POST | `/download/validate-otp` | Verify OTP |
| POST | `/download/resend-otp` | Resend OTP |
| POST | `/download/get` | Fetch full KYC record |
| POST | `/create` | Create new Legal record |
| POST | `/update` | Update existing record |

### Common (`/api/ckyc`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/update/status?ackNo=` | Check Create/Update status |
| GET | `/update/fetch?lastUpdatedAfter=` | List updated CKYC numbers |

---

## Example – Individual OVD Search

```http
POST /api/individual/search/OVD
Content-Type: application/json

{
  "ovdDetails": [
    {
      "ovdType": "C",
      "ovdNo": "ABCDE1234A"
    }
  ]
}
```

## Example – Legal GSTIN Search

```http
POST /api/legal/search/OVD
Content-Type: application/json

{
  "documentIds": [
    {
      "id_type": "01",
      "id_number": "27ABCDE1234A1Z5"
    }
  ]
}
```

---

## Security notes

1. **mTLS** – The `HttpClient` named `CersaiMtls` loads the client certificate and presents it on every request.
2. **OAuth** – `TokenService` caches the bearer token and refreshes ~30 s before expiry.
3. **Encryption** – `StubEncryptionService` only Base64-encodes data. **Replace it** with a proper JWE (RSA-OAEP + AES-GCM) implementation using the public key CERSAI provides during onboarding.
4. **Headers** – `Authorization`, `x-correlation-id` and (for mutations) `x-idempotency-key` are added automatically.

---

## Running

```bash
dotnet restore
dotnet run --project CersaiCkycApi
```

Swagger UI is available at `/swagger` in Development.

---

## Next steps for production readiness

1. Implement real JWE encryption/decryption (e.g. with `jose-jwt` or `Microsoft.IdentityModel.Tokens`).
2. Add certificate rotation / health checks.
3. Expand the Create/Update models with the full field set from the CERSAI guide (personalDetails, identityProofs, addressDetails, relatedPartyDetails, etc.).
4. Add retry / circuit-breaker (Polly) around the CERSAI HttpClient.
5. Secure your own API (API keys, JWT, IP allow-list) before exposing it.

---

## API Key Authentication

All endpoints (except `/` and `/swagger`) require an API key.

**Header:** `X-Api-Key: <your-key>`

Configure valid keys in `appsettings.json`:

```json
"ApiKeyAuth": {
  "Enabled": true,
  "HeaderName": "X-Api-Key",
  "ValidKeys": [ "dev-api-key-change-me-001" ]
}
```

In **Swagger UI**:
1. Click **Authorize**
2. Enter a valid key (e.g. `dev-api-key-change-me-001`)
3. Click **Authorize** → **Close**
4. Use **Try it out** on any endpoint

Set `"Enabled": false` only for local debugging without a key.

---

## Dummy mTLS certificate (development only)

A self-signed client certificate is included for local testing of the mTLS HttpClient pipeline:

| File | Purpose |
|------|---------|
| `certs/client.pfx` | Client certificate + private key (password: `changeit`) |
| `certs/client-cert.pem` / `client-key.pem` | PEM form of the same cert |
| `certs/cersai-public.pem` | Placeholder public key for encryption stub |
| `certs/client-private.pem` | Placeholder private key for decryption stub |

**Subject:** `CN=cersai-dummy-client, O=Dummy RE, C=IN`  
**Password:** `changeit`  
**Validity:** 10 years from generation date  

> **Important:** CERSAI will reject this certificate on the real testbed/production hosts.  
> Replace `certs/client.pfx` with the official certificate issued during RE onboarding before any live integration testing.
