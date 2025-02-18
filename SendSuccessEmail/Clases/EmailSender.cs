using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace SendSuccessEmail.Clases
{
    internal class EmailSender
    {
        private string smtpServer;
        private int smtpPort;
        private string senderEmail;
        private string senderPassword;

        public EmailSender()
        {
            this.smtpServer = SendSuccessEmail.Configuracion.Default.smtpServer;
            this.smtpPort = SendSuccessEmail.Configuracion.Default.port;
            this.senderEmail = SendSuccessEmail.Configuracion.Default.senderEmail;
            this.senderPassword = SendSuccessEmail.Configuracion.Default.senderPassword;
        }
        // --------------------------------- funciones para enviar mail de backups -------------------------------------------------------------
        public string GenerateEmailBody_backup(List<string> failedDB, Dictionary<string, string> successDB)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("<html><body>");
            body.AppendLine("<h2>Reporte de backups:</h2>");

            // Bases de datos con error
            if (failedDB.Count > 0)
            {
                body.AppendLine("<h3>Bases de datos con error:</h3>");
                body.AppendLine("<ul>");
                foreach (var db in failedDB)
                {
                    body.AppendLine($"<li>{db}</li>");
                }
                body.AppendLine("</ul>");
            }
            else
            {
                body.AppendLine("<p>No hay bases de datos con error.</p>");
            }

            // Bases de datos exitosas
            if (successDB.Count > 0)
            {
                body.AppendLine("<h3>Bases de datos exitosas:</h3>");
                body.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
                body.AppendLine("<tr><th>Base de Datos</th><th>Tiempo de Respaldo (segundos)</th></tr>");
                foreach (var db in successDB)
                {
                    body.AppendLine($"<tr><td>{db.Key}</td><td>{db.Value}</td></tr>");
                }
                body.AppendLine("</table>");
            }
            else
            {
                body.AppendLine("<p>No hay bases de datos exitosas.</p>");
            }

            body.AppendLine("</body></html>");

            return body.ToString();
        }

        public string generateSubject_backup(List<string> failedDB, Dictionary<String, String> successDB)
        {
            if (failedDB.Count > 0 && successDB.Count == 0)
            {
                return "Proceso backup SOFTLAND no se pudo realizar ";
            }
            else if (failedDB.Count == 0 && successDB.Count > 0){
                return "Proceso Backup SOFTLAND completado exitosamente";
            }
            else
            {
                return " Proceso Backup SOFTLAND completado con algunos errores";
            }
        }
        // Método para enviar el correo
        public void SendEmail_backup(List<string> failedDB, Dictionary<string, string> successDB)
        {
            try
            {
                // Crea el mensaje de correo
                string subject = generateSubject_backup(failedDB, successDB);
                string body = GenerateEmailBody_backup(failedDB, successDB);

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(SendSuccessEmail.Configuracion.Default.recipientEmail);  // Dirección de correo del destinatario
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // Establece si el cuerpo del correo es HTML o no

                // Configuración del servidor SMTP
                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true  // Activar SSL si es necesario
                };

                // Envía el correo
                smtp.Send(mail);
                Console.WriteLine("Correo enviado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
        }

        // -------------------------------------------- funciones para enviar mail de mantenimiento -----------------------------------------------
        public string GenerateEmailBody_maintenance(Dictionary<string, string> failedDB, Dictionary<String, String> successDB)
        {
            StringBuilder htmlBody = new StringBuilder();
            htmlBody.AppendLine("<html><body>");

            htmlBody.AppendLine("<h2>Reporte de Mantenimiento de Bases de Datos:</h2>");

            // Procesar las bases de datos exitosas
            if (successDB.Count > 0)
            {
                htmlBody.AppendLine("<h3>Bases de Datos Exitosas:</h3>");
                htmlBody.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>");
                htmlBody.AppendLine("<tr><th>Base de Datos</th><th>Plan de Mantenimiento</th><th>Tiempo(en Segundos)</th></tr>");
                foreach (var entry in successDB)
                {
                    string[] keyParts = entry.Key.Split('_');
                    string dbName = keyParts[0];
                    string maintenancePlan = keyParts[1];
                    string tiempo = entry.Value;

                    htmlBody.AppendLine($"<tr><td>{dbName}</td><td>{maintenancePlan}</td><td>{tiempo}</td></tr>");
                }
                htmlBody.AppendLine("</table>");
            }
            else
            {
                htmlBody.AppendLine("<p>No hay bases de datos exitosas.</p>");
            }

            // Procesar las bases de datos con errores
            if (failedDB.Count > 0)
            {
                htmlBody.AppendLine("<br/><br/>");
                htmlBody.AppendLine("<h3>Bases de Datos con Errores:</h3>");
                htmlBody.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>");
                htmlBody.AppendLine("<tr><th>Base de Datos</th><th>Subplan</th><th>Tipo de Error</th></tr>");

                foreach (var entry in failedDB)
                {
                    string[] keyParts = entry.Key.Split('_');
                    string dbName = keyParts[0];
                    string errorType = keyParts[1];
                    string errorDetails = entry.Value;

                    htmlBody.AppendLine($"<tr><td>{dbName}</td><td>{errorType}</td><td>{errorDetails}</td></tr>");
                }
                htmlBody.AppendLine("</table>");
            }
            else
            {
                htmlBody.AppendLine("<p>No hay bases de datos con errores.</p>");
            }

            htmlBody.AppendLine("</body></html>");

            return htmlBody.ToString();
        }



        public string generateSubject_maintenance(Dictionary<string,string> failedDB, Dictionary<String, String> successDB)
        {
            if (failedDB.Count > 0 && successDB.Count == 0)
            {
                return "Proceso Mantenimiento SOFTLAND no se pudo realizar ";
            }
            else if (failedDB.Count == 0 && successDB.Count > 0)
            {
                return "Proceso Mantenimiento SOFTLAND completado exitosamente";
            }
            else
            {
                return " Proceso Mantenimiento SOFTLAND completado con algunos errores";
            }
        }
        public void SendEmail_maintenance(Dictionary<string, string> failedDB, Dictionary<string, string> successDB)
        {
            try
            {
                // Crea el mensaje de correo
                string subject = generateSubject_maintenance(failedDB, successDB);
                string body = GenerateEmailBody_maintenance(failedDB, successDB);

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(SendSuccessEmail.Configuracion.Default.recipientEmail);  // Dirección de correo del destinatario
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // Establece si el cuerpo del correo es HTML o no

                // Configuración del servidor SMTP
                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true  // Activar SSL si es necesario
                };

                // Envía el correo
                smtp.Send(mail);
                Console.WriteLine("Correo enviado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
        }
    }
}
