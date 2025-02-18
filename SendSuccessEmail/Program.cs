// See https://aka.ms/new-console-template for more information
using SendSuccessEmail.Clases;


// Verifica si el argumento está presente
if (args.Length != 1)
{
    Console.WriteLine("Debe especificar un parámetro: 'backup' o 'maintenance'.");
    return;
}
string mode = args[0].ToLower();

LookUpInformation l = new LookUpInformation();
EmailSender e = new EmailSender();

if (mode == "backup")
{
    l.ProcessLatestLogFile_backup();
    foreach (var s in l.successDB)
    {
        Console.WriteLine($"bd:{s.Key} , se demoro : {s.Value}");
    }
    foreach (var s in l.failedDB)
    {
        Console.WriteLine($"fallaron :{s}");
    }
    e.SendEmail_backup(l.failedDB, l.successDB);
}

else if (mode == "maintenance")
{
    l.ProcessLatestLogFile_mantenimiento();
    foreach (var s in l.successDB)
    {
        Console.WriteLine($"clave:{s.Key} , valor : {s.Value}");
    }
    foreach (var s in l.failedDBMaintenance)
    {
        Console.WriteLine($"clave:{s.Key} , valor : {s.Value}");
    }
    e.SendEmail_maintenance(l.failedDBMaintenance, l.successDB);

}
else
{
    Console.WriteLine("Parámetro inválido. Ejecutable debe ser llamado con 'backup' o 'maintenance'.");
}




