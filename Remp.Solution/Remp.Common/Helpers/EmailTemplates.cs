namespace Remp.Common.Helpers;

public static class EmailTemplates
{
    public static string CreateAccountEmail(string password, string email, string loginUrl)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Agent Account Created</title>
</head>
<body style=""margin:0;padding:0;background-color:#f4f4f5;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;"">
    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f4f5;padding:24px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px;background-color:#ffffff;border-radius:8px;overflow:hidden;border:1px solid #e5e7eb;"">
                    <!-- Header -->
                    <tr>
                        <td style=""background-color:#111827;padding:16px 24px;color:#f9fafb;font-size:18px;font-weight:600;"">
                            Recam
                        </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                        <td style=""padding:24px 24px 8px 24px;color:#111827;font-size:16px;"">
                            <p style=""margin:0 0 12px 0;"">Hi,</p>
                            <p style=""margin:0 0 12px 0;"">
                                Your <strong>agent account</strong> has been created. You can now log in to the Recam website using the credentials below.
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style=""padding:0 24px 16px 24px;"">
                            <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;border-collapse:collapse;margin-top:8px;"">
                                <tr>
                                    <td style=""padding:8px 0;font-size:14px;color:#6b7280;width:120px;"">Login email:</td>
                                    <td style=""padding:8px 0;font-size:14px;color:#111827;"">{email}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:8px 0;font-size:14px;color:#6b7280;"">Temporary password:</td>
                                    <td style=""padding:8px 0;font-size:14px;color:#111827;font-family:Consolas,Menlo,Monaco,monospace;"">
                                        {password}
                                    </td>
                                </tr>
                            </table>
                            <p style=""margin:16px 0 8px 0;font-size:13px;color:#6b7280;"">
                                For security, please change this password immediately after your first login.
                            </p>
                        </td>
                    </tr>

                    <!-- Button -->
                    <tr>
                        <td style=""padding:0 24px 24px 24px;"" align=""left"">
                            <a href=""{loginUrl}""
                               style=""display:inline-block;padding:10px 18px;border-radius:6px;background-color:#2563eb;color:#ffffff;
                                      text-decoration:none;font-size:14px;font-weight:500;""
                               target=""_blank"">
                                Go to Recam
                            </a>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""padding:16px 24px 20px 24px;border-top:1px solid #e5e7eb;font-size:12px;color:#9ca3af;"">
                            <p style=""margin:0 0 4px 0;"">
                                If you did not expect this email, please contact the Recam team.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
