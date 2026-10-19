namespace CersaiCkycApi.Models.Common;

public static class CkycType
{
    public const string Individual = "INDIVIDUAL";
    public const string Legal = "LEGAL";
}

public static class SearchOptionIndividual
{
    public const string Ovd = "OVD";
    public const string Ckyc = "CKYC";
    public const string Demographic = "DEMOGRAPHIC";
    public const string Photo = "PHOTO";
    public const string Mobile = "MOBILE";
    public const string Vc = "VC";
}

public static class SearchOptionLegal
{
    public const string Ovd = "OVD";
    public const string Mobile = "MOBILE";
    public const string Doi = "DOI";
    public const string Ckyc = "CKYC";
}

public static class ConsentType
{
    public const string Otp = "OTP";
    public const string Physical = "PHYSICAL";
    public const string FaceAuth = "FACEAUTH";
}

public static class OvdType
{
    public const string Passport = "A";
    public const string VoterId = "B";
    public const string Pan = "C";
    public const string DrivingLicense = "D";
    public const string Aadhaar = "E";
    public const string Nrega = "F";
    public const string Npr = "G";
    public const string Gstin = "01";
    public const string Cin = "02";
    public const string Llpin = "04";
}
